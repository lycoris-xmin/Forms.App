<script setup lang="ts">
import { ref, watch } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { useChinaSelect } from '@/hooks/custome/china-area-map';
import { fetchGetPlanTaskCompletion, fetchUpdatePlanTaskCompletion } from '@/service/api';

defineOptions({
  name: 'PlanTaskCompletionDrawer'
});

type Props = {
  rowData?: object;
};

type Model = {
  id: string;
  success: number;
  orderId: string;
  payPrice: number;
  province: string;
  city: string;
  address: string;
  name: string;
  phone: string;
  note: string | null;
  addressContent: string;
  loading: boolean;
};

interface Emits {
  (e: 'submitted'): void;
}

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = withDefaults(defineProps<Props>(), {
  rowData: () => {
    return {};
  }
});

const { provinceOptions, useCity } = useChinaSelect();

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const emit = defineEmits<Emits>();

const rules = {
  orderId: defaultRequiredRule,
  payPrice: defaultRequiredRule,
  province: defaultRequiredRule,
  city: defaultRequiredRule,
  address: defaultRequiredRule,
  name: defaultRequiredRule,
  phone: defaultRequiredRule
};

const model = ref<Model>(createDeflateModel());

const cityOptions = ref<Array<any>>([]);

watch(visible, async value => {
  if (value) {
    //
    resetFields();

    model.value = createDeflateModel();

    if (!props.rowData.success) {
      model.value.success = 0;
    }

    await getPlanTaskCompletion();
  }
});

function createDeflateModel(): Model {
  return {
    id: '',
    success: 1,
    orderId: '',
    payPrice: 0,
    province: '',
    city: '',
    address: '',
    name: '',
    phone: '',
    addressContent: '',
    note: '',
    loading: false
  };
}

function provinceChangeHandler() {
  cityOptions.value = useCity(model.value.province);
  model.value.city = cityOptions.value[0].value;
}

const selectFilterOption = (input: string, option: any) => {
  return option.value.toLowerCase().includes(input.toLowerCase());
};

async function getPlanTaskCompletion() {
  const { data: res, error } = await fetchGetPlanTaskCompletion(props.rowData.id);
  if (!error && res && res.code === 0) {
    model.value.id = res.data.id;

    if (res.data.success) {
      model.value.orderId = res.data.orderId;
      model.value.payPrice = res.data.payPrice;
      model.value.province = res.data.province;
      model.value.city = res.data.city;
      model.value.address = res.data.address;
      model.value.name = res.data.name;
      model.value.phone = res.data.phone;
    }

    model.value.note = res.data.note || '';
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  if (model.value.success === 0) {
    await updatePlanTaskCompletion();
    return;
  }

  window.$modal?.confirm({
    title: '订单补全信息提示',
    cancelText: '取消',
    okText: '确定',
    content: '请确认商品信息是否填写正确，提交后将无法修改，确定要提交吗？',
    async onOk() {
      await updatePlanTaskCompletion();
    }
  });
}

async function updatePlanTaskCompletion() {
  model.value.loading = true;
  try {
    const { data: res, error } = await fetchUpdatePlanTaskCompletion({ ...model.value });
    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      emit('submitted');
      closeDrawer();
    }
  } finally {
    model.value.loading = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="任务信息" :width="550">
    <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
      <AFormItem label="购买结果" name="success">
        <ASelect
          v-model:value="model.success"
          :options="[
            { value: 0, label: '购买失败' },
            { value: 1, label: '购买成功' }
          ]"
          :disabled="props.rowData.success"
        ></ASelect>
      </AFormItem>

      <AFormItem v-if="model.success === 1" label="订单号" name="orderId">
        <AInput
          v-model:value="model.orderId"
          autocomplete="off"
          placeholder="请输入订单号"
          :disabled="props.rowData.success"
        />
      </AFormItem>

      <AFormItem v-if="model.success === 1" label="支付金额" name="payPrice">
        <AInputNumber
          v-model:value="model.payPrice"
          :min="0"
          :controls="false"
          :precision="2"
          :disabled="props.rowData.success"
        >
          <template #addonBefore>
            <span>￥</span>
          </template>
          <template #addonAfter>
            <span>元</span>
          </template>
        </AInputNumber>
      </AFormItem>

      <AFormItem v-if="model.success === 1" label="省份" name="province">
        <ASelect
          v-model:value="model.province"
          placeholder="- 请选择 -"
          show-search
          :default-active-first-option="false"
          :show-arrow="false"
          :options="provinceOptions"
          :filter-option="selectFilterOption"
          :disabled="props.rowData.success"
          @change="provinceChangeHandler"
        ></ASelect>
      </AFormItem>

      <AFormItem v-if="model.success === 1" label="市区" name="city">
        <ASelect
          v-model:value="model.city"
          placeholder="- 请选择 -"
          show-search
          :default-active-first-option="false"
          :show-arrow="false"
          :options="cityOptions"
          :filter-option="selectFilterOption"
          :disabled="props.rowData.success"
        >
          <template #notFoundContent>
            <p>没有找到匹配的市区</p>
          </template>
        </ASelect>
      </AFormItem>

      <AFormItem v-if="model.success === 1" label="详细地址" name="address">
        <ATextarea
          v-model:value="model.address"
          autocomplete="off"
          placeholder="请输入详细地址"
          :auto-size="{ minRows: 5 }"
          :disabled="props.rowData.success"
        />
      </AFormItem>

      <AFormItem v-if="model.success === 1" label="收货人" name="name">
        <AInput
          v-model:value="model.name"
          autocomplete="off"
          placeholder="请输入收货人名称"
          :disabled="props.rowData.success"
        />
      </AFormItem>

      <AFormItem v-if="model.success === 1" label="联系方式" name="phone">
        <AInput
          v-model:value="model.phone"
          autocomplete="off"
          placeholder="请输入联系方式"
          :disabled="props.rowData.success"
        />
      </AFormItem>

      <AFormItem label="任务备注">
        <ATextarea v-model:value="model.note" :auto-size="{ minRows: 3 }" :maxlength="300" />
      </AFormItem>
    </AForm>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="model.loading" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.ant-input-number-group-wrapper {
  width: 100%;
}
</style>
