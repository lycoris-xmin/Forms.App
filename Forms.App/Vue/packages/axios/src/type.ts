import type { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios';

// 内容类型
export type ContentType = 'text/html' | 'text/plain' | 'multipart/form-data' | 'application/json' | 'application/x-www-form-urlencoded' | 'application/octet-stream';

// 请求配置选项
export interface RequestOption<ResponseData = any> {
  /**
   * 请求前的钩子函数
   *
   * 例如：你可以在这个钩子中添加 token 到请求头
   *
   * @param config Axios 配置对象
   */
  onRequest: (config: InternalAxiosRequestConfig) => InternalAxiosRequestConfig | Promise<InternalAxiosRequestConfig>;

  /**
   * 判断后端响应是否成功的钩子函数
   *
   * @param response Axios 响应对象
   */
  isBackendSuccess: (response: AxiosResponse<ResponseData>) => boolean;

  /**
   * 后端请求失败时的钩子函数
   *
   * 例如：你可以在这个钩子中处理 token 过期的情况
   *
   * @param response Axios 响应对象
   * @param instance Axios 实例
   */
  onBackendFail: (response: AxiosResponse<ResponseData>, instance: AxiosInstance) => Promise<AxiosResponse | null> | Promise<void>;

  /**
   * 当 responseType 为 json 时，转换后端响应的钩子函数
   *
   * @param response Axios 响应对象
   */
  transformBackendResponse(response: AxiosResponse<ResponseData>): any | Promise<any>;

  /**
   * 错误处理钩子函数
   *
   * 例如：你可以在这个钩子中显示错误信息
   *
   * @param error Axios 错误对象
   */
  onError: (error: AxiosError<ResponseData>) => void | Promise<void>;
}

// 响应类型映射
interface ResponseMap {
  blob: Blob;
  text: string;
  arrayBuffer: ArrayBuffer;
  stream: ReadableStream<Uint8Array>;
  document: Document;
}

// 响应类型
export type ResponseType = keyof ResponseMap | 'json';

// 根据响应类型映射对应的数据类型
export type MappedType<R extends ResponseType, JsonType = any> = R extends keyof ResponseMap ? ResponseMap[R] : JsonType;

// 自定义请求配置（支持泛型响应类型）
export type CustomAxiosRequestConfig<R extends ResponseType = 'json'> = Omit<AxiosRequestConfig, 'responseType'> & {
  responseType?: R;
};

// 请求实例公共方法定义
export interface RequestInstanceCommon<T> {
  /**
   * 通过请求 ID 取消请求
   *
   * 如果请求配置中提供了 abort controller 的标识，将不会加入到中止控制器映射中
   *
   * @param requestId 请求标识符
   */
  cancelRequest: (requestId: string) => void;

  /**
   * 取消所有请求
   *
   * 如果请求配置中提供了 abort controller 的标识，将不会加入到中止控制器映射中
   */
  cancelAllRequest: () => void;

  /** 可以在请求实例中设置自定义状态 */
  state: T;
}

/** 请求实例接口 */
export interface RequestInstance<S = Record<string, unknown>> extends RequestInstanceCommon<S> {
  <T = any, R extends ResponseType = 'json'>(config: CustomAxiosRequestConfig<R>): Promise<MappedType<R, T>>;
}

// 展平响应成功数据结构
export type FlatResponseSuccessData<T = any, ResponseData = any> = {
  data: T; // 实际数据
  error: null; // 没有错误
  response: AxiosResponse<ResponseData>; // 原始响应
};

// 展平响应失败数据结构
export type FlatResponseFailData<ResponseData = any> = {
  data: null; // 没有数据
  error: AxiosError<ResponseData>; // 错误对象
  response: AxiosResponse<ResponseData>; // 原始响应
};

// 统一的展平响应数据结构（成功或失败）
export type FlatResponseData<T = any, ResponseData = any> = FlatResponseSuccessData<T, ResponseData> | FlatResponseFailData<ResponseData>;

// 展平请求实例接口
export interface FlatRequestInstance<S = Record<string, unknown>, ResponseData = any> extends RequestInstanceCommon<S> {
  <T = any, R extends ResponseType = 'json'>(config: CustomAxiosRequestConfig<R>): Promise<FlatResponseData<MappedType<R, T>, ResponseData>>;
}
