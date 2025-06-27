<script setup lang="ts">
import { computed, reactive } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchChangePassword } from '@/service/api';

defineOptions({
  name: 'UserCenterPassword'
});

type Form = {
  oldPassword: string;
  password: string;
  confirmPassword: string;
};

const { formRef, validate } = useAntdForm();

const form = reactive<Form>({
  oldPassword: '',
  password: '',
  confirmPassword: ''
});

type RuleRecord = Partial<Record<keyof FormModel, App.Global.FormRule[]>>;

const rules = computed<RuleRecord>(() => {
  const { formRules, createConfirmPwdRule } = useFormRules();

  return {
    oldPassword: formRules.pwd,
    password: formRules.pwd,
    confirmPassword: createConfirmPwdRule(form.password)
  };
});

const submitHandler = async () => {
  await validate();
  const { data: res, error } = await fetchChangePassword({ ...form });
  if (!error && res && res.code === 0) {
    window.$message?.success('修改成功');
  }
};
</script>

<template>
  <div class="pwd">
    <AForm ref="formRef" :model="form" :rules="rules" layout="vertical">
      <AFormItem label="原密码" name="oldPassword" :validate-first="true">
        <AInput v-model:value="form.oldPassword" type="password" />
      </AFormItem>
      <AFormItem label="新密码" name="password" :validate-first="true">
        <AInput v-model:value="form.password" type="password" />
      </AFormItem>
      <AFormItem label="新密码" name="confirmPassword" :validate-first="true">
        <AInput v-model:value="form.confirmPassword" type="password" />
      </AFormItem>
    </AForm>

    <AButton type="primary" @click="submitHandler">保存</AButton>
  </div>
</template>

<style lang="scss" scoped>
.pwd {
  min-width: 300px;
  max-width: 600px;
}
</style>
