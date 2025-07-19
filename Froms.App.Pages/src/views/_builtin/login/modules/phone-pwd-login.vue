<script setup lang="ts">
import { computed, onMounted, reactive } from 'vue';
import { useRouterPush } from '@/hooks/common/router';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { useAuthStore } from '@/store/modules/auth';
import Slider from '@/components/slider-verify/default/index.vue';

defineOptions({
  name: 'PhonePwdLogin'
});

const authStore = useAuthStore();
const { toggleLoginModule } = useRouterPush();
const { formRef, validate } = useAntdForm();

interface FormModel {
  phone: string;
  password: string;
  remember: boolean;
  verified: boolean;
}

const model: FormModel = reactive({
  phone: '',
  password: '',
  remember: false,
  verified: false
});

const rules = computed<Record<keyof FormModel, App.Global.FormRule[]>>(() => {
  const { formRules } = useFormRules();

  return {
    phone: formRules.phone,
    password: formRules.pwd
  };
});

onMounted(() => {
  //
  if (window.$isDebugger) {
    model.phone = '17605078477';
    model.password = '123456789';
  }
});

async function handleSubmit() {
  await validate();

  if (!model.verified) {
    window.$message?.warning('请先完成滑块验证');
    return;
  }

  await authStore.login({ ...model });
}
</script>

<template>
  <AForm ref="formRef" :model="model" :rules="rules" @keyup.enter="handleSubmit">
    <AFormItem name="phone">
      <AInput v-model:value="model.phone" size="large" placeholder="手机号" />
    </AFormItem>
    <AFormItem name="password">
      <AInputPassword v-model:value="model.password" size="large" placeholder="密码" />
    </AFormItem>
    <div class="mb-3">
      <Slider v-model:verified="model.verified"></Slider>
    </div>
    <ASpace direction="vertical" size="large" class="w-full">
      <div class="flex-y-center justify-between">
        <ACheckbox v-model:checked="model.remember">记住我</ACheckbox>
        <AButton type="text" @click="toggleLoginModule('reset-pwd')">忘记密码</AButton>
      </div>
      <AButton type="primary" block size="large" shape="round" :loading="authStore.loginLoading" @click="handleSubmit">
        登录
      </AButton>
      <div class="flex-y-center justify-center">
        <AButton class="h-34px flex-1" block @click="toggleLoginModule('code-login')">验证码登录</AButton>
        <div class="w-12px"></div>
        <AButton class="h-34px flex-1" block @click="toggleLoginModule('bind-wechat')">微信登录</AButton>
      </div>
      <div class="flex-y-center justify-center">
        <AButton class="h-34px flex-1" block @click="toggleLoginModule('code-register')">账号注册</AButton>
      </div>
    </ASpace>
  </AForm>
</template>

<style scoped></style>
