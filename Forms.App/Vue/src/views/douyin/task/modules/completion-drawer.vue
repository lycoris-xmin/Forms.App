<script setup lang="ts">
import { nextTick, ref, watch } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { useChinaSelect } from '@/hooks/custome/china-area-map';
import { fetchGetPlanTaskCompletion, fetchUpdatePlanTaskCompletion } from '@/service/api';

defineOptions({
  name: 'PlanTaskCompletionDrawer'
});

type Props = {
  rowData?: object;
  lovePayEnum?: Array<any>;
};

type Model = {
  id: string;
  success: number;
  orderId: string;
  payPrice: number | null;
  province: string;
  city: string;
  address: string;
  name: string;
  phone: string;
  note: string | null;
  addressContent: string;
  isLovePay: boolean;
  lovePayId: string | null;
  lovePayName: string | null;
  lovePayEnum: Array<any>;
  newLovePayName: string;
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
  },
  lovePayEnum: () => {
    return [];
  }
});

const { provinceOptions, useCity } = useChinaSelect();

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const emit = defineEmits<Emits>();

const rules = {
  orderId: defaultRequiredRule,
  payPrice: defaultRequiredRule,
  province: [],
  city: [],
  address: [],
  name: [],
  phone: [],
  lovePayId: [{ required: true, message: '请选择亲情卡' }]
};

const model = ref<Model>(createDeflateModel());

const cityOptions = ref<Array<any>>([]);

const inputRef = ref<HTMLElement | null>(null);

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
    payPrice: null,
    province: '',
    city: '',
    address: '',
    name: '',
    phone: '',
    addressContent: '',
    note: '',
    isLovePay: false,
    lovePayId: '',
    lovePayName: '',
    lovePayEnum: props.lovePayEnum ?? [],
    newLovePayName: '',
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
    const d = res.data;
    model.value.id = d.id;
    model.value.success = d.success ? 1 : 0;
    model.value.orderId = d.orderId || '';
    model.value.payPrice = typeof d.payPrice !== 'undefined' && d.payPrice !== null && d.payPrice !== '' ? Number(d.payPrice) : null;
    model.value.province = d.province || '';
    model.value.city = d.city || '';
    model.value.address = d.address || '';
    model.value.name = d.name || '';
    model.value.phone = d.phone || '';
    model.value.lovePayId = d.lovePayId || '';
    model.value.lovePayName = d.lovePayName || '';
    model.value.isLovePay = Boolean(d.lovePayId || d.lovePayName);
    model.value.note = d.note || '';

    handleProvinceCityLinkage();
  }
}

function handleProvinceCityLinkage() {
  if (model.value.province) {
    cityOptions.value = useCity(model.value.province);
    if (model.value.city && !cityOptions.value.find(x => x.value === model.value.city)) {
      model.value.city = cityOptions.value[0]?.value || '';
    }
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

function addLovePayHandler(e) {
  e.preventDefault();
  if (!model.value.newLovePayName) {
    return;
  }

  model.value.newLovePayName = model.value.newLovePayName.trim();

  const value = -1 * Date.now();
  if (!model.value.lovePayEnum.filter(x => x.value === value).length) {
    model.value.lovePayEnum.push({
      text: model.value.newLovePayName,
      value
    });
  }

  model.value.lovePayId = value;
  model.value.lovePayName = model.value.newLovePayName;

  nextTick(() => {
    model.value.newLovePayName = '';
    inputRef.value?.focus();
  });
}

function lovePayChangeHandler() {
  if (model.value.lovePayId < -1) {
    model.value.lovePayName = model.value.lovePayEnum.filter(x => x.value === model.value.lovePayId)[0].text;
  }
}

function lovePayilterOption(input: string, option: any) {
  return props.lovePayEnum.filter(x => x.text.includes(input)).filter(x => x.value === option.value).length > 0;
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

      <AFormItem v-if="model.success === 1 && model.isLovePay" label="亲情卡" name="lovePayId">
        <ASelect
          v-if="!props.rowData.success"
          v-model:value="model.lovePayId"
          placeholder="请选择亲情卡"
          show-search
          :filter-option="lovePayilterOption"
          @change="lovePayChangeHandler"
        >
          <ASelectOption v-for="item in model.lovePayEnum" :key="item.value" :value="item.value">
            {{ item.text }}
          </ASelectOption>

          <template #dropdownRender="{ menuNode: menu }">
            <VNodes :vnodes="menu" />
            <ADivider class="mb-4px mt-4px" />
            <ASpace class="select-action">
              <AInput ref="inputRef" v-model:value="model.newLovePayName" placeholder="新亲情卡" />
              <AButton ghost size="small" type="primary" @click="addLovePayHandler">
                <template #icon>
                  <PlusOutlined />
                </template>
                添加新亲情卡
              </AButton>
            </ASpace>
          </template>
        </ASelect>
        <AInput v-else v-model:value="model.lovePayName" disabled />
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
.role-select {
  gap: 10px;
}

.select-action {
  padding: 4px 8px;
  display: flex;
  align-items: center;
  width: 100%;

  /* 使用 :deep 选择器，深入修改 Ant Design Vue 组件的内部样式（针对 .ant-space-item 类） */
  :deep(.ant-space-item) {
    /* 第一个 .ant-space-item（通常是输入框） */
    &:first-child {
      /* 设置 flex: 1，使其占用剩余空间，自动扩展宽度 */
      flex: 1;
    }

    /* 最后一个 .ant-space-item（通常是按钮） */
    &:last-child {
      /* 设置 flex-shrink: 0，防止按钮被压缩，保持其固有宽度 */
      flex-shrink: 0;
    }

    /* 针对 .ant-space-item 内的 .ant-btn 元素（按钮） */
    .ant-btn {
      /* 使用 flex 布局，使按钮内容水平和垂直居中 */
      display: flex;
      /* 垂直居中对齐 */
      align-items: center;
      /* 水平居中对齐 */
      justify-content: center;
    }
  }
}
</style>
