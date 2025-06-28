import { flatRequest } from '../../request';

export async function loginApi(account: string, password: string, remember: boolean) {
  const resp = await flatRequest({
    url: '/api/authorize/validation',
    method: 'post',
    data: {
      account,
      password
    }
  });

  const { data: res, error } = resp;

  if (error && error.status >= 300) {
    return { data: undefined, error };
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
      account,
      oathCode: res?.data.oathCode,
      remember
    }
  });
}

export function getUserInfoApi() {
  return flatRequest({
    url: '/api/authorize/user/profile'
  });
}

export function refreshTokenApi(refreshToken: string) {
  return flatRequest({
    url: '/api/authorize/token/refresh',
    method: 'post',
    data: {
      refreshToken
    }
  });
}

export function logoutApi() {
  return flatRequest({
    url: '/api/authorize/logout',
    method: 'post'
  });
}

export function registerEmailCodeApi(email: string, validate: string) {
  return flatRequest({
    url: '/api/authorize/register/code',
    params: {
      email,
      validate,
      time: new Date().getTime()
    }
  });
}
