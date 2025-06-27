<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchEmailTest, fetchGetEmailConfiguration, fetchSaveEmailConfiguration } from '@/service/api';

type Model = Api.Configuration.EmailConfiguration & {
  loading: boolean;
  testEmail: string;
  testModal: boolean;
};

const modalFormRef = ref(null);
const { formRef, validate } = useAntdForm();

const model = reactive<Model>({
  stmpServer: '',
  stmpPort: '',
  emailAddress: '',
  emailPassword: '',
  emailUser: '',
  emailSignature: '',
  useSSL: false,
  loading: false,
  testEmail: '',
  testModal: false
});

const { defaultRequiredRule, formRules } = useFormRules();

const rules = {
  stmpServer: defaultRequiredRule,
  stmpPort: defaultRequiredRule,
  emailAddress: formRules.email,
  emailPassword: defaultRequiredRule,
  emailUser: defaultRequiredRule,
  emailSignature: defaultRequiredRule,
  testEmail: formRules.email
};

onMounted(async () => {
  await init();
});

async function init() {
  const { data: res, error } = await fetchGetEmailConfiguration();
  if (!error && res && res.code === 0) {
    Object.assign(model, res.data);
  }
}

function triggerSTMP() {
  if (!model.emailAddress || !model.emailAddress.includes('@')) {
    model.stmpServer = '';
    model.stmpPort = '';
    return;
  }

  const stmpEmums = [
    {
      extensions: '@qq.com',
      stmpServer: 'smtp.qq.com',
      stmpPort: 465
    },
    {
      extensions: '@163.com',
      stmpServer: 'smtp.163.com',
      stmpPort: 465
    },
    {
      extensions: '@gmail.com',
      stmpServer: 'smtp.gmail.com',
      stmpPort: 465
    },
    {
      extensions: '@126.com',
      stmpServer: 'smtp.126.com',
      stmpPort: 465
    },
    {
      extensions: '@139.com',
      stmpServer: 'smtp.139.com',
      stmpPort: 465
    },
    {
      extensions: '@foxmail.com',
      stmpServer: 'smtp.foxmail.com',
      stmpPort: 465
    },
    {
      extensions: '@sina.com',
      stmpServer: 'smtp.sina.com',
      stmpPort: 465
    },
    {
      extensions: '@sohu.com',
      stmpServer: 'smtp.sohu.com',
      stmpPort: 465
    },
    {
      extensions: '@outlook.com',
      stmpServer: 'smtp.office365.com',
      stmpPort: 465
    },
    {
      extensions: '@icloud.com',
      stmpServer: 'smtp.mail.me.com',
      stmpPort: 587
    }
  ];

  for (const stmp of stmpEmums) {
    if (model.emailAddress.endsWith(stmp.extensions)) {
      //
      model.stmpServer = stmp.stmpServer;
      model.stmpPort = stmp.stmpPort;
      break;
    }
  }
}

function testHandler() {
  model.testModal = true;
}

async function testSubmitHandler() {
  await modalFormRef.value.validate();

  const { data: res, error } = await fetchEmailTest(model.testEmail);
  if (!error && res && res.code === 0) {
    model.testModal = false;
    window.$message?.success('邮件发送成功');
  }
}

async function submittedHandler() {
  await validate();

  const { data: res, error } = await fetchSaveEmailConfiguration({ ...model });
  if (!error && res && res.code === 0) {
    window.$message?.success('保存成功');
  }
}
</script>

<template>
  <div class="body">
    <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
      <AFormItem label="发件箱地址" name="emailAddress">
        <AInput v-model:value="model.emailAddress" autocomplete="off" placeholder="发件箱地址" @change="triggerSTMP" />
      </AFormItem>
      <AFormItem label="发件箱授权码" name="emailPassword">
        <AInput v-model:value="model.emailPassword" autocomplete="off" placeholder="发件箱授权码" type="password" />
      </AFormItem>
      <AFormItem label="发件人" name="emailUser">
        <AInput v-model:value="model.emailUser" autocomplete="off" placeholder="发件人" />
      </AFormItem>
      <AFormItem label="邮件签名" name="emailSignature">
        <AInput v-model:value="model.emailSignature" autocomplete="off" placeholder="邮件签名" />
      </AFormItem>
      <AFormItem label="STMP服务器" name="stmpServer">
        <AInput v-model:value="model.stmpServer" autocomplete="off" placeholder="STMP服务器" />
      </AFormItem>
      <AFormItem label="STMP端口" name="stmpPort">
        <AInput v-model:value="model.stmpPort" autocomplete="off" placeholder="STMP端口" />
      </AFormItem>
      <AFormItem label="使用SSL" name="useSSL">
        <ASwitch v-model:checked="model.useSSL" />
      </AFormItem>
    </AForm>

    <div class="v-center flex gap-8px">
      <AButton @click="testHandler">邮件测试</AButton>
      <AButton type="primary" @click="submittedHandler">保存</AButton>
    </div>

    <AModal v-model:open="model.testModal" title="邮件服务测试" @ok="testSubmitHandler">
      <AForm ref="modalFormRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="测试地址" name="testEmail">
          <AInput v-model:value="model.testEmail" autocomplete="off" placeholder="测试地址" />
        </AFormItem>
      </AForm>
    </AModal>
  </div>
</template>

<style lang="scss" scoped>
.body {
  min-width: 300px;
  max-width: 400px;
}
</style>
