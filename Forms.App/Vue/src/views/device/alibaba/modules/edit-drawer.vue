<script setup lang="ts">
import { onMounted, reactive, ref, watch } from 'vue';
import { fetchGetDeviceAliDetail, fetchUpdateDeviceAli } from '@/service/api/modules/device-alibaba';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import LoadingWrap from '@/components/loading/index.vue';
import { REG_PHONE } from '@/constants/reg';
import {
  Map as ChinaArea,
  addressTextChangeHandler,
  findAreaValues,
  getAddressValidateHelp,
  getAddressValidateStatus
} from './utils';

defineOptions({
  name: 'DeviceAlibabaEditDrawer'
});

const visible = defineModel<boolean>('visible', { default: false });

type Props = {
  rowData?: object;
  authStore: {
    isInternalTenant: boolean;
    userInfo: { isTenantUser: boolean };
  };
};

type Model = {
  id: string | number;
  payPassword?: string;
  payPhone?: string;
  province: string;
  city: string;
  address: string;
  name: string;
  phone: string;
  currentMonth: number;
  lastMonth: number;
  aliAddress: string;
  loading: boolean;
  submitLoading: boolean;
};

type Emits = {
  (e: 'submitted'): void;
};

const payPhoneRef = ref<HTMLElement | null>(null);
const payPasswordRef = ref<HTMLElement | null>(null);

const { formRef, validate, resetFields } = useAntdForm();

const model = reactive<Model>({
  id: '',
  payPassword: '',
  payPhone: '',
  province: '',
  city: '',
  address: '',
  name: '',
  phone: '',
  currentMonth: 0,
  lastMonth: 0,
  aliAddress: '',
  loading: true,
  submitLoading: false
});

const props = defineProps<Props>();

const { defaultRequiredRule } = useFormRules();

const rules = {
  payPassword: [
    {
      required: true,
      message: '支付密码不能为空',
      trigger: 'blur'
    },
    {
      validator: (_, value) => {
        if (value && (value.length !== 6 || !/^\d+$/.test(value))) {
          return Promise.reject(new Error('支付密码应为六位数字'));
        }

        return Promise.resolve();
      },
      trigger: 'blur'
    }
  ],
  payPhone: [
    defaultRequiredRule,
    {
      validator: (_, value) => {
        if (!value) {
          return Promise.resolve();
        }

        if (!REG_PHONE.test(value)) {
          return Promise.reject(new Error('代付手机号格式错误'));
        }

        return Promise.resolve();
      }
    }
  ],
  name: [
    {
      validator: (_, value) => {
        if (!value) {
          return Promise.resolve();
        }
        if (value.length > 30) {
          return Promise.reject(new Error('收货人姓名至少2个字符,可以是汉字或英文字母'));
        }
        return Promise.resolve();
      },
      trigger: 'blur'
    }
  ],
  phone: [
    {
      validator: (_, value) => {
        if (!value) {
          return Promise.resolve();
        }
        if (!REG_PHONE.test(value)) {
          return Promise.reject(new Error('联系电话格式错误'));
        }
        return Promise.resolve();
      },
      trigger: 'blur'
    }
  ],
  currentMonth: [
    {
      required: true,
      message: '请输入本月牛气值',
      trigger: 'blur'
    }
  ],
  lastMonth: [
    {
      required: true,
      message: '请输入上月牛气值',
      trigger: 'blur'
    }
  ]
};

const emit = defineEmits<Emits>();

const areaOptions = ref<any[]>([]);

const selectedArea = ref<string[]>([]);

watch(() => model.aliAddress, handleAddressTextChange);

watch(visible, async (value, old) => {
  if (value && !old) {
    resetForm();
    await loadDeviceDetail();
  }
});

async function loadDeviceDetail() {
  model.loading = true;
  try {
    const { data: res, error } = await fetchGetDeviceAliDetail(props.rowData?.id);

    if (error || res?.code !== 0) {
      window.$message?.error('获取详情失败');
      return;
    }

    Object.assign(model, {
      id: res.data.id,
      payPassword: res.data.payPassword,
      payPhone: res.data.payPhone,
      name: res.data.name,
      phone: res.data.phone,
      province: res.data.province,
      city: res.data.city,
      address: res.data.address,
      currentMonth: props.rowData.currentMonth,
      lastMonth: props.rowData.lastMonth
    });
    if (res.data.province && res.data.city) {
      const areaValues = findAreaValues(res.data.province, res.data.city);
      if (areaValues.length === 2) {
        selectedArea.value = areaValues;
      }
    }
  } finally {
    model.loading = false;
  }
}

function resetForm() {
  resetFields();
  model.id = '';
  model.aliAddress = '';
  selectedArea.value = [];
  Object.assign(model, {
    payPassword: '',
    payPhone: '',
    name: '',
    phone: '',
    province: '',
    city: '',
    address: '',
    currentMonth: 0,
    lastMonth: 0
  });
}

onMounted(() => {
  const provinces = ChinaArea['86'];
  areaOptions.value = Object.entries(provinces).map(([code, name]) => {
    const cities = ChinaArea[code];
    const children = cities
      ? Object.entries(cities).map(([cityCode, cityName]) => ({
          value: cityCode,
          label: cityName,
          isLeaf: true
        }))
      : [];

    return {
      value: code,
      label: name,
      children
    };
  });
});

const ADDRESS_PLACEHOLDER = `姓名: 李四
手机号码: 10000000000
所在地区: 北京市 朝阳区
详细地址: XXX街道xxx号`;

async function handleAddressTextChange(text) {
  if (!text) {
    return;
  }
  const result = await addressTextChangeHandler(text);
  Object.assign(model, result);
  selectedArea.value = result.areaValues;
}

function closeDrawer() {
  visible.value = false;
}

async function handleOk() {
  await validate();
  try {
    model.submitLoading = true;

    const submitData = {
      id: props.rowData?.id,
      payPassword: model.payPassword,
      payPhone: model.payPhone,
      province: model.province,
      city: model.city,
      address: model.address,
      name: model.name,
      phone: model.phone,
      currentMonth: model.currentMonth,
      lastMonth: model.lastMonth
    };

    const { data: res, error } = await fetchUpdateDeviceAli(submitData);

    if (!error && res?.code === 0) {
      window.$message?.success('保存成功');
      emit('submitted');
      closeDrawer();
    }
  } finally {
    model.submitLoading = false;
  }
}

function handleAreaChange(values: string[]) {
  if (values && values.length === 2) {
    const provinceOption = areaOptions.value.find(p => p.value === values[0]);
    if (provinceOption) {
      model.province = provinceOption.label;
      const cityOption = provinceOption.children?.find(c => c.value === values[1]);
      if (cityOption) {
        model.city = cityOption.label;
      }
    }
  } else {
    model.province = '';
    model.city = '';
  }
}

async function validateAddressText() {
  await formRef.value?.validateFields(['aliAddress']);
}
</script>

<template>
  <ADrawer v-model:open="visible" title="编辑1688设备" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem
          v-if="props.authStore.isInternalTenant && !props.authStore.userInfo.isTenantUser"
          label="本月牛气值"
          name="currentMonth"
        >
          <AInput ref="payPasswordRef" v-model:value="model.currentMonth" placeholder="请输入本月牛气值" />
        </AFormItem>

        <AFormItem
          v-if="props.authStore.isInternalTenant && !props.authStore.userInfo.isTenantUser"
          label="上月牛气值"
          name="lastMonth"
        >
          <AInput ref="payPasswordRef" v-model:value="model.lastMonth" placeholder="请输入上月牛气值" />
        </AFormItem>

        <AFormItem v-if="model.buyPayType === 2" label="代付手机" name="payPhone">
          <AInput ref="payPhoneRef" v-model:value="model.payPhone" placeholder="请输入代付手机号" />
        </AFormItem>
        <AFormItem v-else label="支付密码" name="payPassword">
          <AInput ref="payPasswordRef" v-model:value="model.payPassword" placeholder="请输入支付密码" type="password" />
        </AFormItem>

        <AFormItem label="收货人" name="name">
          <AInput v-model:value="model.name" placeholder="请输入收货人姓名" />
        </AFormItem>

        <AFormItem label="联系电话" name="phone">
          <AInput v-model:value="model.phone" placeholder="请输入联系电话" />
        </AFormItem>

        <AFormItem label="所在地区" name="selectedArea">
          <ACascader
            v-model:value="selectedArea"
            :options="areaOptions"
            placeholder="请选择省市"
            @change="handleAreaChange"
          />
        </AFormItem>

        <AFormItem label="详细地址" name="address">
          <ATextarea v-model:value="model.address" placeholder="请输入详细地址" :rows="3" />
        </AFormItem>

        <AFormItem
          label="地址文本解析"
          name="aliAddress"
          :validate-status="getAddressValidateStatus(model.aliAddress)"
          :help="getAddressValidateHelp(model.aliAddress)"
        >
          <ATextarea
            v-model:value="model.aliAddress"
            :placeholder="ADDRESS_PLACEHOLDER"
            :rows="4"
            :auto-size="{ minRows: 4, maxRows: 6 }"
            @input="validateAddressText"
          />
        </AFormItem>
      </AForm>

      <LoadingWrap :loading="model.loading"></LoadingWrap>
    </div>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="model.submitLoading" @click="handleOk">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style lang="scss" scoped>
.body {
  position: relative;
}

.mt-8px {
  margin-top: 8px;
}

:deep(.ant-form-item-explain-error) {
  white-space: pre-line;
}
</style>
