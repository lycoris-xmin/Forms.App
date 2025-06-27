<script setup lang="ts">
import { computed, onMounted, reactive } from 'vue';
// import { $t } from '@/locales';
// import { loginModuleRecord } from '@/constants/app';
import { useRouterPush } from '@/hooks/common/router';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { useAuthStore } from '@/store/modules/auth';

defineOptions({
  name: 'PwdLogin'
});

const authStore = useAuthStore();
const { toggleLoginModule } = useRouterPush();
const { formRef, validate, validateFields } = useAntdForm();

interface FormModel {
  email: string;
  password: string;
  remember: boolean;
}

const model: FormModel = reactive({
  email: '',
  password: '',
  remember: false
});

const rules = computed<Record<keyof FormModel, App.Global.FormRule[]>>(() => {
  // inside computed to make locale reactive, if not apply i18n, you can define it without computed
  const { formRules } = useFormRules();

  return {
    email: formRules.email,
    password: formRules.pwd
  };
});

onMounted(() => {
  //
  if (window.$isDebugger) {
    model.email = '13000000000@163.com';
    model.password = '123456789';
  }
});

async function emailChangeHandler() {
  if (model.email) {
    model.email = model.email.toLowerCase().trim();
    await validateFields('email');
  }
}

async function handleSubmit() {
  await validate();
  await authStore.login({ ...model });
}
</script>

<template>
  <AForm ref="formRef" :model="model" :rules="rules" @keyup.enter="handleSubmit">
    <AFormItem name="email">
      <AInput v-model:value="model.email" size="large" placeholder="账号" @blur="emailChangeHandler" />
    </AFormItem>
    <AFormItem name="password">
      <AInputPassword v-model:value="model.password" size="large" placeholder="密码" />
    </AFormItem>
    <ASpace direction="vertical" size="large" class="w-full">
      <div class="flex-y-center justify-between">
        <ACheckbox v-model:checked="model.remember">记住我</ACheckbox>
        <AButton type="text" @click="toggleLoginModule('reset-pwd')">忘记密码</AButton>
      </div>
      <AButton type="primary" block size="large" shape="round" :loading="authStore.loginLoading" @click="handleSubmit">
        登录
      </AButton>
      <div class="flex-y-center justify-center">
        <!--
 <AButton class="h-34px flex-1" block @click="toggleLoginModule('code-login')">
          {{ $t(loginModuleRecord['code-login']) }}
        </AButton>
        <div class="w-12px"></div> 
-->
        <!--
 <AButton class="h-34px flex-1" block @click="toggleLoginModule('register')">
          {{ $t(loginModuleRecord.register) }}
        </AButton> 
-->
      </div>
    </ASpace>
  </AForm>
</template>

<style scoped></style>
