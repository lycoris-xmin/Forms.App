<script setup lang="ts">
import { computed, nextTick, onMounted, reactive, ref, watch } from 'vue';
import { fetchGetDeviceTaobaoDetail, fetchUpdateDeviceTaobao } from '@/service/api';
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
  name: 'DeviceTaobaoEditDrawer'
});

const visible = defineModel<boolean>('visible', { default: false });

type Props = {
  rowData?: object;
  payType: Array<{ value: number; label: string }>;
  authStore: {
    isInternalTenant: boolean;
    userInfo: { isTenantUser: boolean };
  };
};

type Model = {
  id: string | number;
  taoQi?: number;
  age?: string;
  allowPair: boolean;
  buyPayType: number;
  payPassword?: string;
  payPhone?: string;
  loading: boolean;
  submitLoading: boolean;
  oldPayPhone: string;
  province: string;
  city: string;
  address: string;
  name: string;
  phone: string;
  tbAddressText: string;
};

type Emits = {
  (e: 'submitted'): void;
};

const payPhoneRef = ref<HTMLElement | null>(null);
const payPasswordRef = ref<HTMLElement | null>(null);

const { formRef, validate, resetFields } = useAntdForm();

const model = reactive<Model>({
  id: '',
  taoQi: 0,
  age: '1天',
  allowPair: 0,
  buyPayType: 0,
  payPassword: '',
  payPhone: '',
  loading: true,
  submitLoading: false,
  oldPayPhone: '',
  province: '',
  city: '',
  address: '',
  name: '',
  phone: '',
  tbAddressText: ''
});
const labelText = computed(() => {
  return model.allowPair
    ? '任务分配(允许任务分配后，将自动分配TB联盟任务)'
    : '任务分配(停止任务分配后，TB联盟任务将不再分配)';
});

const props = defineProps<Props>();

const { defaultRequiredRule } = useFormRules();

const rules = {
  taoQi: defaultRequiredRule,
  age: defaultRequiredRule,
  allowPair: defaultRequiredRule,
  buyPayType: defaultRequiredRule,
  payPassword: [
    {
      validator: (_, value) => {
        if (!value) {
          return Promise.resolve();
        }

        if (model.buyPayType === 2) {
          return Promise.resolve();
        }

        if (value.length !== 6 || !/^\d+$/.test(value)) {
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
        if (model.buyPayType !== 2) {
          return Promise.resolve();
        }

        if (!value) {
          return Promise.resolve();
        }

        if (!REG_PHONE.test(value)) {
          return Promise.reject(new Error('代付手机号格式错误'));
        }

        return Promise.resolve();
      },
      trigger: 'blur'
    }
  ],
  name: [
    {
      validator: (_, value) => {
        if (!value) {
          return Promise.resolve();
        }

        if (value.length > 30) {
          return Promise.reject(new Error('收货人姓名不能超过30个字'));
        }

        return Promise.resolve();
      },
      trigger: 'blur'
    }
  ]
};

const emit = defineEmits<Emits>();

const areaOptions = ref<any[]>([]);
const selectedArea = ref<string[]>([]);

watch(() => model.tbAddressText, handleAddressTextChange);

watch(visible, async value => {
  if (value) {
    resetForm();
    await loadDeviceDetail();
  }
});

async function loadDeviceDetail() {
  model.loading = true;
  try {
    const { data: res, error } = await fetchGetDeviceTaobaoDetail(props.rowData?.id);

    if (error || res?.code !== 0) {
      window.$message?.error('获取详情失败');
      return;
    }

    Object.assign(model, {
      id: res.data.id,
      taoQi: props.rowData.taoQi,
      age: props.rowData.age,
      allowPair: props.rowData.allowPair ? 1 : 0,
      buyPayType: res.data.buyPayType,
      payPassword: res.data.payPassword,
      payPhone: res.data.payPhone,
      oldPayPhone: res.data.payPhone,
      name: res.data.name,
      phone: res.data.phone,
      province: res.data.province,
      city: res.data.city,
      address: res.data.address
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
  model.oldPayPhone = '';
  selectedArea.value = [];
  Object.assign(model, {
    taoQi: 0,
    age: '1天',
    buyPayType: 0,
    payPassword: '',
    payPhone: '',
    name: '',
    phone: '',
    province: '',
    city: '',
    address: '',
    tbAddressText: ''
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

const ADDRESS_PLACEHOLDER = `收件人: 张三
手机号码: 13800138000
所在地区: 浙江省杭州市西湖区
详细地址: xxx路xxx号`;

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
      buyPayType: model.buyPayType,
      taoQi: model.taoQi,
      age: model.age,
      allowPair: model.allowPair === 1,
      payPhone: model.payPhone,
      payPassword: model.payPassword,
      name: model.name,
      phone: model.phone,
      province: model.province,
      city: model.city,
      address: model.address
    };

    const { data: res, error } = await fetchUpdateDeviceTaobao(submitData);

    if (!error && res?.code === 0) {
      window.$message?.success('保存成功');
      emit('submitted');
      closeDrawer();
    }
  } finally {
    model.submitLoading = false;
  }
}

async function payTypeChangeHandler() {
  model.payPhone = model.oldPayPhone;
  formRef.value?.clearValidate();

  await nextTick();

  if (model.buyPayType === 2) {
    payPhoneRef.value?.focus();
  } else {
    payPasswordRef.value?.focus();
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
  await formRef.value?.validateFields(['tbAddressText']);
}
</script>

<template>
  <ADrawer v-model:open="visible" title="编辑淘宝设备" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem
          v-if="props.authStore.isInternalTenant && !props.authStore.userInfo.isTenantUser"
          label="淘气值"
          name="taoQi"
        >
          <AInput ref="payPasswordRef" v-model:value="model.taoQi" placeholder="请输入淘气值" />
        </AFormItem>
        <AFormItem
          v-if="props.authStore.isInternalTenant && !props.authStore.userInfo.isTenantUser"
          label="淘龄"
          name="age"
        >
          <AInput ref="payPasswordRef" v-model:value="model.age" placeholder="请输入淘龄" />
        </AFormItem>
        <AFormItem :label="labelText" name="allowPair">
          <ASelect v-model:value="model.allowPair">
            <ASelectOption :value="1">允许任务分配</ASelectOption>
            <ASelectOption :value="0">停止任务分配</ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem label="支付方式" name="buyPayType">
          <ASelect v-model:value="model.buyPayType" @change="payTypeChangeHandler">
            <ASelectOption v-for="item in props.payType" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem v-if="model.buyPayType === 2" label="代付手机" name="payPhone">
          <AInput ref="payPhoneRef" v-model:value="model.payPhone" placeholder="请输入代付手机号" />
        </AFormItem>
        <AFormItem v-else label="支付密码" name="payPassword">
          <AInput ref="payPasswordRef" v-model:value="model.payPassword" placeholder="请输入支付密码" />
        </AFormItem>

        <AFormItem label="收货人" name="name">
          <AInput v-model:value="model.name" placeholder="请输入收货人姓名（选填）" :maxlength="30" show-count />
        </AFormItem>

        <AFormItem label="联系电话" name="phone">
          <AInput v-model:value="model.phone" placeholder="请输入联系电话（选填）" />
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
          <ATextarea v-model:value="model.address" placeholder="请输入详细地址" :rows="5" :maxlength="300" show-count />
        </AFormItem>

        <AFormItem
          label="地址文本解析"
          name="tbAddressText"
          :validate-status="getAddressValidateStatus(model.tbAddressText)"
          :help="getAddressValidateHelp(model.tbAddressText)"
        >
          <ATextarea
            v-model:value="model.tbAddressText"
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
