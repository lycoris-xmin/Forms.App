<script setup lang="ts">
import { computed, reactive } from 'vue';
import { useRouterPush } from '@/hooks/common/router';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { useCaptcha } from '@/hooks/business/captcha';
import Slider from '@/components/slider-verify/block-puzzle/index.vue';
import { registerEmailCodeApi } from '@/service/api';

defineOptions({
  name: 'ResetPwd'
});

const isPhoneLogin = import.meta.env.VITE_LOGIN_TYPE === 'phone';

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

type RuleRecord = Partial<Record<keyof FormModel, App.Global.FormRule[]>>;

const rules = computed<RuleRecord>(() => {
  const { formRules, createConfirmPwdRule } = useFormRules();

  return {
    email: formRules.email,
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
    const { data: res, error } = await registerEmailCodeApi(model.email, val);
    return !error && res && res.code === 0;
  });
}

async function handleSubmit() {
  await validate();
  // request to reset password
  window.$message?.success('修改成功');
}
</script>

<template>
  <AForm ref="formRef" :model="model" :rules="rules" @keyup.enter="handleSubmit">
    <AFormItem name="email">
      <AInput v-model:value="model.email" size="large" placeholder="请输入邮箱" />
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
      <AButton type="primary" block size="large" shape="round" @click="handleSubmit">确认</AButton>
      <AButton
        block
        size="large"
        shape="round"
        @click="toggleLoginModule(isPhoneLogin ? 'phone-pwd-login' : 'email-pwd-login')"
      >
        返回
      </AButton>
    </ASpace>
  </AForm>

  <Slider v-model:visible="model.sliderVisible" path="/api/authorize/reset/code" @success="successHandler"></Slider>
</template>

<style scoped></style>
