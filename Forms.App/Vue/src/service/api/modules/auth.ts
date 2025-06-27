import { flatRequest } from '../../request';

export async function fetchLogin(email: string, password: string, remember: boolean) {
  const resp = await flatRequest({
    url: '/api/authorize/validation',
    method: 'post',
    data: {
      email,
      password
    }
  });

  const { data: res, error } = resp;

  if (error && error.status >= 300) {
    return { data: void 0, error };
  }

  if (!res || res.code !== 0) {
    if (res.code !== -99) {
      return { data: res, error };
    }
  }

  return await flatRequest({
    url: '/api/authorize/login',
    method: 'post',
    data: {
      email,
      oathCode: res?.data.oathCode,
      remember
    }
  });
}

export function fetchGetUserInfo() {
  return flatRequest({
    url: '/api/authorize/userprofile'
  });
}

export function fetchRefreshToken(refreshToken: string) {
  return flatRequest({
    url: '/api/authorize/token/refresh',
    method: 'post',
    data: {
      refreshToken
    }
  });
}

export function fetchLogout() {
  return flatRequest({
    url: '/api/authorize/logout',
    method: 'post'
  });
}

export function fetchCustomBackendError(code: string, msg: string) {
  return flatRequest({
    url: '/auth/error',
    params: { code, msg }
  });
}

export function fetchRegisterEmailCode(email: string, validate: string) {
  return flatRequest({
    url: '/api/authorize/register/code',
    params: {
      email,
      validate,
      time: new Date().getTime()
    }
  });
}
