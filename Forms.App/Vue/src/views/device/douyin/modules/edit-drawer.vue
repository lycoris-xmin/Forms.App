<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue';
import { fetchGetDeviceDouyinDetail, fetchUpdateDeviceDouyin } from '@/service/api/modules/device-douyin';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import LoadingWrap from '@/components/loading/index.vue';
import { REG_PHONE } from '@/constants/reg';
import {
  createAddressHandler,
  findAreaValues,
  getAddressValidateHelp,
  getAddressValidateStatus,
  getAreaList,
  handleAreaChange as handleAreaChangeUtil
} from './utils';
import type { Model } from './utils';

defineOptions({
  name: 'DeviceDouyinEditDrawer'
});

const visible = defineModel<boolean>('visible', { default: false });

type Props = {
  rowData?: object;
  payType: Array;
};

type Emits = {
  (e: 'submitted'): void;
};

const payPhoneRef = ref<HTMLElement | null>(null);
const payPasswordRef = ref<HTMLElement | null>(null);
const selectedArea = ref<string[]>([]);
const areaList = ref(getAreaList());

const { formRef, validate, resetFields } = useAntdForm();

const model = reactive<Model>({
  id: '',
  buyPayType: 0,
  payPassword: '',
  payPhone: '',
  loading: true,
  submitLoading: false,
  oldPayPhone: '',
  name: '',
  phone: '',
  province: '',
  city: '',
  address: '',
  addressText: '',
  tbAddressText: '',
  allowPair: false
});

const labelText = computed(() => {
  return model.allowPair
    ? '任务分配(允许任务分配后，将自动分配DY联盟任务)'
    : '任务分配(停止任务分配后，DY联盟任务将不再分配)';
});

const props = defineProps<Props>();

const { defaultRequiredRule } = useFormRules();

const rules = {
  taoQi: defaultRequiredRule,
  age: defaultRequiredRule,
  buyPayType: defaultRequiredRule,
  payPassword: [
    {
      required: model.buyPayType !== 2,
      message: '请输入支付密码',
      trigger: ['change', 'blur']
    }
  ],
  payPhone: [
    {
      required: model.buyPayType === 2,
      message: '请输入代付手机号',
      trigger: ['change', 'blur']
    },
    {
      pattern: REG_PHONE,
      message: '请输入正确的手机号',
      trigger: ['change', 'blur']
    }
  ],
  phone: [
    {
      validator: (_, value) => {
        if (!value) {
          return Promise.resolve();
        }
        if (!REG_PHONE.test(value)) {
          return Promise.reject(new Error('请输入正确的手机号'));
        }
        return Promise.resolve();
      },
      trigger: ['change', 'blur']
    }
  ]
};

const debounceHandleAddressTextChange = createAddressHandler(model, selectedArea);

watch(
  () => model.tbAddressText,
  async text => {
    await debounceHandleAddressTextChange(text);
  }
);

function handleDeviceDetail(res: any) {
  model.buyPayType = res.data.buyPayType;
  model.payPassword = res.data.payPassword;
  model.payPhone = res.data.payPhone;
  model.name = res.data.name || '';
  model.phone = res.data.phone || '';
  model.province = res.data.province || '';
  model.city = res.data.city || '';
  model.address = res.data.address || '';
  model.allowPair = res.data.allowPair === true || res.data.allowPair === 1 || res.data.allowPair === '1';
  if (res.data.province && res.data.city) {
    const areaValues = findAreaValues(res.data.province, res.data.city);
    if (areaValues && areaValues.length === 2) {
      selectedArea.value = areaValues;
    }
  }
}

watch(visible, async (value, old) => {
  resetFields();
  if (value && value !== old) {
    model.loading = true;
    try {
      // 先设置基础数据
      Object.keys(model).forEach(key => {
        if (Object.keys(props.rowData).includes(key)) {
          model[key] = props.rowData[key];
        }
      });

      // 然后获取详细数据并覆盖
      const { data: res, error } = await fetchGetDeviceDouyinDetail(props.rowData.id);
      if (!error && res && res.code === 0) {
        handleDeviceDetail(res);
      }
    } finally {
      model.loading = false;
    }
  }
});

const emit = defineEmits<Emits>();

const ADDRESS_PLACEHOLDER = `收货人: 张三, 手机号码: 13356568989, 所在地区: 广西壮族自治区北海市合浦县白沙镇, 详细地址: 广西送变电建设有限责任公司施工项目部`;

async function validateAddressText() {
  await formRef.value?.validateFields(['tbAddressText']);
}

function handleAreaChange(value: string[]) {
  handleAreaChangeUtil(value, model, areaList.value);
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();
  model.submitLoading = true;
  try {
    const { data: res, error } = await fetchUpdateDeviceDouyin({ ...model });

    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
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
</script>

<template>
  <ADrawer v-model:open="visible" title="编辑抖音设备" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem :label="labelText" name="allowPair" required>
          <ASelect v-model:value="model.allowPair">
            <ASelectOption :value="true">允许任务分配</ASelectOption>
            <ASelectOption :value="false">停止任务分配</ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem label="支付方式" name="buyPayType">
          <ASelect v-model:value="model.buyPayType" @change="payTypeChangeHandler">
            <ASelectOption v-for="item in props.payType" :key="item.value" :value="item.value">
              {{ item.label }}
            </ASelectOption>
          </ASelect>
        </AFormItem>

        <AFormItem
          v-if="model.buyPayType === 2"
          label="代付手机"
          name="payPhone"
          :required="model.buyPayType === 2"
          class="required-field"
        >
          <AInput ref="payPhoneRef" v-model:value="model.payPhone" placeholder="请输入代付手机号" />
        </AFormItem>
        <AFormItem v-else label="支付密码" name="payPassword" :required="model.buyPayType !== 2" class="required-field">
          <AInput ref="payPasswordRef" v-model:value="model.payPassword" placeholder="请输入支付密码" type="password" />
        </AFormItem>

        <AFormItem label="收货人" name="name">
          <AInput v-model:value="model.name" placeholder="请输入收货人姓名(选填)" :maxlength="30" show-count />
        </AFormItem>

        <AFormItem label="联系电话" name="phone">
          <AInput v-model:value="model.phone" placeholder="请输入联系电话(选填)" />
        </AFormItem>

        <AFormItem label="所在地区">
          <ACascader
            v-model:value="selectedArea"
            :options="areaList"
            placeholder="请选择省市"
            @change="handleAreaChange"
          />
        </AFormItem>
        <AFormItem label="详细地址" name="address">
          <ATextarea v-model:value="model.address" placeholder="请输入详细地址" :rows="3" />
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
        <AButton type="primary" :loading="model.submitLoading" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style lang="scss" scoped>
.body {
  position: relative;
}
.required-field .ant-form-item-label > label::before {
  color: #ff4d4f !important;
  margin-right: 4px !important;
}
:deep(.ant-form-item-explain-error) {
  white-space: pre-line;
}
</style>
