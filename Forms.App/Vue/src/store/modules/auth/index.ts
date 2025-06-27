import { computed, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';
import { defineStore } from 'pinia';
import { useLoading } from '@sa/hooks';
import { SetupStoreId } from '@/enum';
import { useRouterPush } from '@/hooks/common/router';
import { fetchGetUserInfo, fetchLogin } from '@/service/api';
import { localStg } from '@/utils/storage';
import { useRouteStore } from '../route';
import { useTabStore } from '../tab';
import { clearAuthStorage, getToken } from './shared';

export const useAuthStore = defineStore(SetupStoreId.Auth, () => {
  const route = useRoute();
  const routeStore = useRouteStore();
  const tabStore = useTabStore();
  const { toLogin, redirectFromLogin } = useRouterPush(false);
  const { loading: loginLoading, startLoading, endLoading } = useLoading();

  const token = ref(getToken());

  const userInfo = reactive<Api.Auth.Profile>({
    id: '',
    email: '',
    nickName: '',
    avatar: '',
    gender: 0,
    roleId: '0',
    isSuperAdmin: false,
    isLogin: false,
    isSettlement: false,
    isTenant: false,
    isTenantUser: true,
    tenantType: 0
  });

  const userPermission = ref<Array<string>>([]);

  const isStaticSuper = computed(() => {
    const { VITE_AUTH_ROUTE_MODE, VITE_STATIC_SUPER_ROLE } = import.meta.env;
    return VITE_AUTH_ROUTE_MODE === 'static' && userInfo.roleId === VITE_STATIC_SUPER_ROLE;
  });

  const isLogin = computed(() => Boolean(token.value && userInfo.isLogin));

  const isInternalTenant = computed(() => {
    return userInfo.tenantType === 10;
  });

  async function resetStore() {
    const authStore = useAuthStore();

    clearAuthStorage();

    authStore.$reset();

    if (!route.meta.constant) {
      await toLogin();
    }

    tabStore.clearTabs([], true);
    routeStore.resetStore();
  }

  async function login(
    { email, password, remember }: { email: string; password: string; remember: boolean },
    redirect = true
  ) {
    startLoading();

    try {
      const { data: res, error } = await fetchLogin(email, password, remember);

      if (!error && res && res.code === 0) {
        const pass = await loginByToken(res.data);

        if (pass) {
          await redirectFromLogin(redirect);

          if (routeStore.isInitAuthRoute) {
            window.$notification?.success({
              message: '登录成功',
              description: `欢迎回来，${userInfo.userName} ！`
            });
          }
        }
      } else {
        resetStore();
      }
    } finally {
      endLoading();
    }
  }

  async function loginByToken(loginToken: Api.Auth.LoginToken) {
    // 1. stored in the localStorage, the later requests need it in headers
    localStg.set('token', loginToken.token);
    localStg.set('refreshToken', loginToken.refreshToken);

    // 2. get user info
    const pass = await getUserInfo();

    if (pass) {
      token.value = loginToken.token;

      return true;
    }

    return false;
  }

  async function getUserInfo() {
    const { data: res, error } = await fetchGetUserInfo();

    if (!error && res && res.code === 0) {
      Object.assign(userInfo, res.data);
      userInfo.isTenant = userInfo.roleId === 1002 || userInfo.roleId === 1003;
      userInfo.isTenantUser = userInfo.roleId === 1003;
      return true;
    }

    return false;
  }

  async function initUserInfo() {
    const hasToken = getToken();

    if (hasToken) {
      const pass = await getUserInfo();

      if (!pass) {
        resetStore();
      }
    }
  }

  function setUserPermission(input: Array<string>): void {
    userPermission.value = [...input];
  }

  return {
    token,
    userInfo,
    userPermission,
    isStaticSuper,
    isLogin,
    loginLoading,
    isInternalTenant,
    resetStore,
    login,
    initUserInfo,
    setUserPermission
  };
});
