<script setup lang="ts">
import { computed, reactive } from 'vue';
import { useRouterPush } from '@/hooks/common/router';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { useCaptcha } from '@/hooks/business/captcha';
import Slider from '@/components/slider-verify/index.vue';
import { fetchRegisterEmailCode } from '@/service/api';

defineOptions({
  name: 'Register'
});

const { toggleLoginModule } = useRouterPush();
const { formRef, validate } = useAntdForm();
const { label, isCounting, loading, getCaptcha } = useCaptcha();

interface FormModel {
  email: string;
  code: string;
  password: string;
  confirmPassword: string;
  sliderVisible: boolean;
  sliderSuccess: boolean;
}

const model: FormModel = reactive({
  email: '',
  code: '',
  password: '',
  confirmPassword: '',
  sliderVisible: false,
  sliderSuccess: false
});

const rules = computed<Record<keyof FormModel, App.Global.FormRule[]>>(() => {
  const { formRules, createConfirmPwdRule } = useFormRules();

  return {
    email: formRules.email,
    code: formRules.code,
    password: formRules.pwd,
    confirmPassword: createConfirmPwdRule(model.password)
  };
});

async function captchaHandler() {
  await formRef.value.validateFields('email');
  model.sliderVisible = true;
}

async function successHandler(val) {
  model.sliderVisible = false;
  model.sliderSuccess = true;
  await getCaptcha(async () => {
    const { data: res, error } = await fetchRegisterEmailCode(model.email, val);
    return !error && res && res.code === 0;
  });
}

async function submitHandle() {
  await validate();
  // request to register
  window.$message?.success($t('page.login.common.validateSuccess'));
}
</script>

<template>
  <AForm ref="formRef" :model="model" :rules="rules" @keyup.enter="submitHandle">
    <AFormItem name="email">
      <AInput v-model:value="model.email" size="large" placeholder="请输入邮箱地址" />
    </AFormItem>
    <AFormItem name="code">
      <div class="w-full flex-y-center gap-16px">
        <AInput v-model:value="model.code" size="large" placeholder="请输入验证码" :disabled="!model.sliderSuccess" />
        <AButton class="btn" size="large" :disabled="isCounting" :loading="loading" @click="captchaHandler">
          {{ label }}
        </AButton>
      </div>
    </AFormItem>
    <AFormItem name="password">
      <AInputPassword v-model:value="model.password" size="large" placeholder="请输入密码" />
    </AFormItem>
    <AFormItem name="confirmPassword">
      <AInputPassword v-model:value="model.confirmPassword" size="large" placeholder="请再次输入密码" />
    </AFormItem>
    <ASpace direction="vertical" size="large" class="w-full">
      <AButton type="primary" block size="large" shape="round" @click="submitHandle">注册</AButton>
      <AButton block size="large" shape="round" @click="toggleLoginModule('pwd-login')">返回</AButton>
    </ASpace>

    <Slider
      v-model:visible="model.sliderVisible"
      path="/api/authorize/register/code"
      @success="successHandler"
    ></Slider>
  </AForm>
</template>

<style scoped>
.btn {
  width: 112px;
}
</style>
