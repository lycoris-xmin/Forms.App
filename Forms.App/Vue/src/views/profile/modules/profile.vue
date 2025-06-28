<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { useAntdForm } from '@/hooks/common/form';
import { useAuthStore } from '@/store/modules/auth';
import { saveUserProfileApi } from '@/service/api';

defineOptions({
  name: 'UserCenterProfile'
});

type Profile = {
  email: string;
  nickName: string;
  wechat: string;
};

const authStore = useAuthStore();

const { formRef, validate } = useAntdForm();

const profile = reactive<Profile>({
  email: '',
  nickName: '',
  wechat: ''
});

const rules = {
  nickName: [{ required: true, message: '用户昵称不能为空', trigger: 'blur' }]
};

onMounted(() => {
  profile.nickName = authStore.userInfo.nickName;
  profile.email = authStore.userInfo.email;
  profile.wechat = authStore.userInfo.wechat;
});

const submitHandler = async () => {
  await validate();
  const { data: res, error } = saveUserProfileApi({ ...profile });
  if (!error && res && res.code === 0) {
    window.$message?.success('保存成功');
  }
};
</script>

<template>
  <div class="profile">
    <AForm ref="formRef" :model="profile" :rules="rules" layout="vertical">
      <AFormItem label="登录邮箱">
        <AInput v-model:value="profile.email" disabled />
      </AFormItem>
      <AFormItem label="用户昵称" name="nickName">
        <AInput v-model:value="profile.nickName" />
      </AFormItem>
      <AFormItem label="联系微信" name="wechat">
        <AInput v-model:value="profile.wechat" />
      </AFormItem>
    </AForm>

    <AButton type="primary" @click="submitHandler">保存</AButton>
  </div>
</template>

<style lang="scss" scoped>
.profile {
  min-width: 300px;
  max-width: 600px;
}
</style>
