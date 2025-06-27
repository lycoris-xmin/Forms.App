<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { useAntdForm } from '@/hooks/common/form';
import { useSettlementStore } from '@/store/modules/settlement';
import { apiUrl, fetchSaveSettlement } from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';
import { useAuthStore } from '@/store/modules/auth';
// import { imgFallback } from '@/data/empty.json';

defineOptions({
  name: 'UserCenterAlipay'
});

type Form = {
  alipayAccount: string;
  alipayQRCode: string;
  alipayImage: object;
  wechatPayQRCode: string;
  wechatPayImage: object;
  type: string;
  loading: boolean;
  btnLoading: boolean;
};

const authStore = useAuthStore();

const settlementStore = useSettlementStore();

const uploadRef = ref(null);

const { formRef, validate } = useAntdForm();

const form = reactive<Form>({
  alipayAccount: '',
  alipayImage: void 0,
  alipayQRCode: '',
  wechatPayQRCode: '',
  wechatPayImage: void 0,
  type: 'alipay',
  loading: true,
  btnLoading: false
});

const rules = {
  alipayAccount: [{ required: true, message: '支付宝账号不能为空', trigger: 'blur' }]
};

onMounted(async () => {
  await reload();
});

async function reload(fource = false) {
  try {
    await settlementStore.initSettlement(fource);

    form.alipayAccount = settlementStore.alipayAccount;

    if (settlementStore.alipayQRCode) {
      if (window.$isDebugger) {
        form.alipayQRCode = `${apiUrl}/nocache${settlementStore.alipayQRCode}`;
      } else {
        form.alipayQRCode = `/nocache${settlementStore.alipayQRCode}`;
      }
    }

    if (settlementStore.wechatPayQRCode) {
      if (window.$isDebugger) {
        form.wechatPayQRCode = `${apiUrl}/nocache${settlementStore.wechatPayQRCode}`;
      } else {
        form.wechatPayQRCode = `/nocache${settlementStore.wechatPayQRCode}`;
      }
    }
  } finally {
    form.loading = false;
  }
}

const selectFileHandler = type => {
  form.type = type;
  uploadRef.value.click();
};

const uploadFileChangeHandler = e => {
  if (e.target.files && e.target.files.length) {
    if (form.type === 'alipay') {
      form.alipayImage = e.target.files[0];
      form.alipayQRCode = URL.createObjectURL(form.alipayImage);
    } else {
      form.wechatPayImage = e.target.files[0];
      form.wechatPayQRCode = URL.createObjectURL(form.wechatPayImage);
    }
    e.target.value = '';
  }
};

const submitHandler = async () => {
  await validate();
  form.btnLoading = true;
  try {
    const data = {
      alipayAccount: form.alipayAccount,
      alipayQRCode: form.alipayImage,
      wechatPayQRCode: form.wechatPayImage
    };

    const { data: res, error } = await fetchSaveSettlement(data);

    if (!error && res && res.code === 0) {
      await reload(true);
      window.$message?.success('保存成功');
      form.alipayImage = void 0;
      form.wechatPayImage = void 0;
      authStore.userInfo.isSettlement = true;
    }
  } finally {
    form.btnLoading = false;
  }
};
</script>

<template>
  <div class="body">
    <div class="settlement">
      <input
        ref="uploadRef"
        type="file"
        accept="image/jpeg,image/png"
        class="upload"
        @change="uploadFileChangeHandler"
      />
      <div class="grid">
        <AForm ref="formRef" :model="form" :rules="rules" layout="vertical">
          <AFormItem label="支付宝收款码">
            <div v-if="form.alipayQRCode" class="view" @click="selectFileHandler('alipay')">
              <img :src="form.alipayQRCode" />
            </div>
            <div v-else class="view center v-center flex" @click="selectFileHandler('alipay')">
              <PlusOutlined class="icon" />
            </div>
          </AFormItem>
          <AFormItem label="支付宝账号" name="alipayAccount">
            <AInput v-model:value="form.alipayAccount" placeholder="请输入支付宝账号" />
          </AFormItem>
        </AForm>
        <AForm layout="vertical">
          <AFormItem label="微信收款码">
            <div v-if="form.wechatPayQRCode" class="view" @click="selectFileHandler('wechatPay')">
              <img :src="form.wechatPayQRCode" />
            </div>
            <div v-else class="view center v-center flex" @click="selectFileHandler('wechatPay')">
              <PlusOutlined class="icon" />
            </div>
          </AFormItem>
        </AForm>
      </div>

      <AButton type="primary" :loading="form.btnLoading" @click="submitHandler">保存</AButton>
    </div>

    <LoadingWrap v-if="form.loading" :loading="form.loading" bg-color="#fff"></LoadingWrap>
  </div>
</template>

<style lang="scss" scoped>
.body {
  position: relative;
  height: 100%;
  width: 100%;
  overflow: hidden;
}

.settlement {
  min-width: 300px;
  max-width: 600px;

  .upload {
    display: none !important;
  }

  .grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 25px;
  }

  .view {
    height: 150px;
    width: 150px;
    padding: 2px;
    border: 1px solid var(--color-secondary);
    border-radius: 5px;
    transition: 0.3 ease-in-out;
    cursor: pointer;
    overflow: hidden;

    .icon {
      font-size: 36px;
      transition: inherit;
    }

    &:hover {
      &:has(.icon) {
        border-color: var(--color-purple);
      }

      .icon {
        color: var(--color-purple);
      }
    }

    img {
      height: 146px;
      width: 146px;
      object-fit: fill;
    }
  }
}
</style>
