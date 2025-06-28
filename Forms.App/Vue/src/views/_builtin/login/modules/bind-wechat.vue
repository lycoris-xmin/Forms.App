<script setup lang="ts">
import { useRouterPush } from '@/hooks/common/router';

defineOptions({
  name: 'BindWechat'
});

const { toggleLoginModule } = useRouterPush();

const backRoute = import.meta.env.VITE_LOGIN_TYPE === 'phone' ? 'phone-pwd-login' : 'email-pwd-login';
</script>

<template>
  <div>
    <div>微信登录</div>
    <div class="flex-y-center justify-center">
      <AButton class="h-34px flex-1" block @click="toggleLoginModule(backRoute)">返回</AButton>
    </div>
  </div>
</template>

<style scoped></style>
