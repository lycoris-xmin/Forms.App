import axios, { AxiosError } from 'axios';
import type { AxiosResponse, CreateAxiosDefaults, InternalAxiosRequestConfig } from 'axios';
import axiosRetry from 'axios-retry';
import { nanoid } from '@sa/utils';
import { createAxiosConfig, createDefaultOptions, createRetryOptions } from './options';
import { BACKEND_ERROR_CODE, REQUEST_ID_KEY } from './constant';
import type { CustomAxiosRequestConfig, FlatRequestInstance, MappedType, RequestInstance, RequestOption, ResponseType } from './type';

/**
 * 创建通用请求方法
 *
 * @param axiosConfig Axios 初始化配置
 * @param options 请求处理钩子函数配置
 */
function createCommonRequest<ResponseData = any>(axiosConfig?: CreateAxiosDefaults, options?: Partial<RequestOption<ResponseData>>) {
  const opts = createDefaultOptions<ResponseData>(options);

  const axiosConf = createAxiosConfig(axiosConfig);
  const instance = axios.create(axiosConf);

  const abortControllerMap = new Map<string, AbortController>();

  // 配置 axios 重试机制
  const retryOptions = createRetryOptions(axiosConf);
  axiosRetry(instance, retryOptions);

  // 请求拦截器
  instance.interceptors.request.use(conf => {
    const config: InternalAxiosRequestConfig = { ...conf };

    // 设置请求 ID
    const requestId = nanoid();
    config.headers.set(REQUEST_ID_KEY, requestId);

    // 配置 AbortController
    if (!config.signal) {
      const abortController = new AbortController();
      config.signal = abortController.signal;
      abortControllerMap.set(requestId, abortController);
    }

    // 执行请求前钩子处理
    const handledConfig = opts.onRequest?.(config) || config;

    return handledConfig;
  });

  // 响应拦截器
  instance.interceptors.response.use(
    async response => {
      const responseType: ResponseType = (response.config?.responseType as ResponseType) || 'json';

      // 非 JSON 响应或后端判断为成功，直接返回
      if (responseType !== 'json' || opts.isBackendSuccess(response)) {
        return Promise.resolve(response);
      }

      // 后端失败处理钩子
      const fail = await opts.onBackendFail(response, instance);
      if (fail) {
        return fail;
      }

      // 构造并抛出后端错误
      const backendError = new AxiosError<ResponseData>('请求异常', BACKEND_ERROR_CODE, response.config, response.request, response);

      await opts.onError(backendError);

      return Promise.reject(backendError);
    },
    async (error: AxiosError<ResponseData>) => {
      // 请求异常错误处理
      await opts.onError(error);
      return Promise.reject(error);
    }
  );

  // 取消单个请求
  function cancelRequest(requestId: string) {
    const abortController = abortControllerMap.get(requestId);
    if (abortController) {
      abortController.abort();
      abortControllerMap.delete(requestId);
    }
  }

  // 取消所有请求
  function cancelAllRequest() {
    abortControllerMap.forEach(abortController => {
      abortController.abort();
    });
    abortControllerMap.clear();
  }

  return {
    instance,
    opts,
    cancelRequest,
    cancelAllRequest
  };
}

/**
 * 创建标准请求实例
 *
 * @param axiosConfig Axios 配置
 * @param options 请求处理钩子选项
 */
export function createRequest<ResponseData = any, State = Record<string, unknown>>(axiosConfig?: CreateAxiosDefaults, options?: Partial<RequestOption<ResponseData>>) {
  const { instance, opts, cancelRequest, cancelAllRequest } = createCommonRequest<ResponseData>(axiosConfig, options);

  const request: RequestInstance<State> = async function request<T = any, R extends ResponseType = 'json'>(config: CustomAxiosRequestConfig) {
    const response: AxiosResponse<ResponseData> = await instance(config);

    const responseType = response.config?.responseType || 'json';

    if (responseType === 'json') {
      return opts.transformBackendResponse(response);
    }

    return response.data as MappedType<R, T>;
  } as RequestInstance<State>;

  request.cancelRequest = cancelRequest;
  request.cancelAllRequest = cancelAllRequest;
  request.state = {} as State;

  return request;
}

/**
 * 创建展平响应的请求实例
 *
 * 返回数据结构为：{ data: any, error: AxiosError, response: AxiosResponse }
 *
 * @param axiosConfig Axios 配置
 * @param options 请求处理钩子选项
 */
export function createFlatRequest<ResponseData = any, State = Record<string, unknown>>(axiosConfig?: CreateAxiosDefaults, options?: Partial<RequestOption<ResponseData>>) {
  const { instance, opts, cancelRequest, cancelAllRequest } = createCommonRequest<ResponseData>(axiosConfig, options);

  const flatRequest: FlatRequestInstance<State, ResponseData> = async function flatRequest<T = any, R extends ResponseType = 'json'>(config: CustomAxiosRequestConfig) {
    try {
      const response: AxiosResponse<ResponseData> = await instance(config);

      const responseType = response.config?.responseType || 'json';

      if (responseType === 'json') {
        const data = opts.transformBackendResponse(response);
        return { data, error: null, response };
      }

      return { data: response.data as MappedType<R, T>, error: null, response };
    } catch (error) {
      return {
        data: null,
        error: error as AxiosError<ResponseData>,
        response: (error as AxiosError<ResponseData>).response
      };
    }
  } as FlatRequestInstance<State, ResponseData>;

  flatRequest.cancelRequest = cancelRequest;
  flatRequest.cancelAllRequest = cancelAllRequest;
  flatRequest.state = {} as State;

  return flatRequest;
}

export { BACKEND_ERROR_CODE, REQUEST_ID_KEY };
export type * from './type';
export type { CreateAxiosDefaults, AxiosError };
