import { useAuthStore } from '@/store/modules/auth';
import { localStg } from '@/utils/storage';
import { refreshTokenApi } from '../api';
import type { RequestInstanceState } from './type';

export function getAuthorization() {
  const token = localStg.get('token');
  return token || null;
}

/** refresh token */
async function handleRefreshToken() {
  const { resetStore } = useAuthStore();

  const rToken = localStg.get('refreshToken') || '';

  const { error, data: res } = await refreshTokenApi(rToken);
  if (!error && res && res.code === 0) {
    localStg.set('token', res.data.token);
    localStg.set('refreshToken', res.data.refreshToken);
    return true;
  }

  resetStore();

  return false;
}

export async function handleExpiredRequest(state: RequestInstanceState) {
  if (!state.refreshTokenFn) {
    state.refreshTokenFn = handleRefreshToken();
  }

  const success = await state.refreshTokenFn;

  setTimeout(() => {
    state.refreshTokenFn = null;
  }, 1000);

  return success;
}

export function showErrorMsg(state: RequestInstanceState, message: string) {
  if (!state.errMsgStack?.length) {
    state.errMsgStack = [];
  }

  const isExist = state.errMsgStack.includes(message);

  if (!isExist) {
    state.errMsgStack.push(message);

    window.$message?.error(message, 1.5, () => {
      state.errMsgStack = state.errMsgStack.filter(msg => msg !== message);

      setTimeout(() => {
        state.errMsgStack = [];
      }, 5000);
    });
  }
}
