import type { AxiosResponse } from 'axios';
import { BACKEND_ERROR_CODE, createFlatRequest, createRequest } from '@sa/axios';
import { localStg } from '@/utils/storage';
import { getServiceBaseURL } from '@/utils/service';
import { useAuthStore } from '@/store/modules/auth';
import { getAuthorization, handleExpiredRequest, showErrorMsg } from './shared';
import type { RequestInstanceState } from './type';

const isHttpProxy = import.meta.env.DEV && import.meta.env.VITE_HTTP_PROXY === 'Y';
const { baseURL, otherBaseURL } = getServiceBaseURL(import.meta.env, isHttpProxy);

const conf = {
  baseURL,
  headers: {},
  withCredentials: true
};

function getApiUrl() {
  let url = baseURL === '/' ? `${location.protocol}//${location.host}` : baseURL;

  // 如果尾部有斜杠 '/', 则移除
  if (url.endsWith('/')) {
    url = url.slice(0, -1);
  }

  return url;
}

export const apiUrl = getApiUrl();

export function baseResp() {
  return Promise.resolve({
    data: {
      code: 0,
      msg: ''
    },
    error: null
  });
}

export const flatRequest = createFlatRequest<any, RequestInstanceState>(conf, {
  async onRequest(config) {
    const Authorization = getAuthorization();

    Object.assign(config.headers, { Authorization });

    Object.assign(config.headers, { 'X-Request': new Date(new Date().toISOString()).getTime() });

    if (window.$fingerprint) {
      Object.assign(config.headers, { 'X-Api-Token': window.$fingerprint });
    }

    return config;
  },
  isBackendSuccess(response) {
    // 当后端响应代码为 0（默认）时，表示请求成功
    // 如果要自己更改此逻辑，可以修改`.env`文件中的`VITE_SERVICE_SUCCESS_CODE`
    return String(response.data.code) === import.meta.env.VITE_SERVICE_SUCCESS_CODE;
  },
  async onBackendFail(response, instance) {
    const responseCode = String(response.data.code);

    // 当后端响应码在 `expiredTokenCodes` 中时，表示 token 已过期，刷新 token
    // api `refreshToken` 不能返回 `expiredTokenCodes` 中的错误码，否则会死循环，应该返回 `logoutCodes` 或 `modalLogoutCodes`
    const expiredTokenCodes = import.meta.env.VITE_SERVICE_EXPIRED_TOKEN_CODES?.split(',') || [];
    if (expiredTokenCodes.includes(responseCode)) {
      const success = await handleExpiredRequest(flatRequest.state);

      if (success) {
        const Authorization = getAuthorization();

        Object.assign(response.config.headers, { Authorization });

        return instance.request(response.config) as Promise<AxiosResponse>;
      }
    }

    if (responseCode === '-99') {
      window.$message?.warning(response.data.msg || '系统繁忙，请稍后再试');
    }

    return null;
  },
  transformBackendResponse(response) {
    return response.data;
  },
  onError(error) {
    // when the request is fail, you can show error message
    function handleLogout() {
      const authStore = useAuthStore();
      authStore.resetStore();
    }

    function logoutAndCleanup() {
      handleLogout();

      window.removeEventListener('beforeunload', handleLogout);

      flatRequest.state.errMsgStack = flatRequest.state.errMsgStack.filter(msg => msg !== response.data.msg);
    }

    if (error.status === 200) {
      return;
    }

    if (error.status === 401) {
      logoutAndCleanup(this.instance);
      return;
    }

    if (error.status === 403) {
      showErrorMsg(flatRequest.state, '您暂时没有权限这么做');
      return;
    }

    let message = error.message;
    let backendErrorCode = '';

    // get backend error message and code
    if (error.code === BACKEND_ERROR_CODE) {
      message = error.response?.data?.msg || message;
      backendErrorCode = String(error.response?.data?.code) || '';
    }

    // the error message is displayed in the modal
    const modalLogoutCodes = import.meta.env.VITE_SERVICE_MODAL_LOGOUT_CODES?.split(',') || [];
    if (modalLogoutCodes.includes(backendErrorCode)) {
      return;
    }

    // when the token is expired, refresh token and retry request, so no need to show error message
    const expiredTokenCodes = import.meta.env.VITE_SERVICE_EXPIRED_TOKEN_CODES?.split(',') || [];
    if (expiredTokenCodes.includes(backendErrorCode)) {
      return;
    }

    showErrorMsg(flatRequest.state, message);
  }
});

export const demoRequest = createRequest<App.Service.DemoResponse>(
  {
    baseURL: otherBaseURL.demo
  },
  {
    async onRequest(config) {
      const { headers } = config;

      // set token
      const token = localStg.get('token');
      const Authorization = token ? `Bearer ${token}` : null;
      Object.assign(headers, { Authorization });

      return config;
    },
    isBackendSuccess(response) {
      // when the backend response code is "200", it means the request is success
      // you can change this logic by yourself
      return response.data.status === '200';
    },
    async onBackendFail(_response) {
      // when the backend response code is not "200", it means the request is fail
      // for example: the token is expired, refresh token and retry request
    },
    transformBackendResponse(response) {
      return response.data;
    },
    onError(error) {
      // when the request is fail, you can show error message

      let message = error.message;

      // show backend error message
      if (error.code === BACKEND_ERROR_CODE) {
        message = error.response?.data?.message || message;
      }

      window.$message?.error(message);
    }
  }
);

export function objectToFormData(obj, formData = new FormData(), parentKey = '') {
  if (obj === null || obj === undefined) return formData;

  if (typeof obj === 'object' && !(obj instanceof File)) {
    if (Array.isArray(obj)) {
      // 处理数组，使用 `${parentKey}[]` 形式
      obj.forEach((item, index) => {
        objectToFormData(item, formData, `${parentKey}[${index}]`);
      });
    } else {
      // 处理对象
      Object.keys(obj).forEach(key => {
        const newKey = parentKey ? `${parentKey}[${key}]` : key;
        objectToFormData(obj[key], formData, newKey);
      });
    }
  } else {
    // 处理基本类型和 File
    formData.append(parentKey, obj);
  }

  return formData;
}
