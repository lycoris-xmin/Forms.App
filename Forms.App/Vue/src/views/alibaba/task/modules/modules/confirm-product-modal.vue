<script setup lang="ts">
import { reactive, watch, withDefaults } from 'vue';
import { fetchGetPlanTaskSkuContinue, fetchPlanTaskContinue } from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';

type Props = {
  id?: string;
  step?: number;
  images?: Array<string>;
};

type Model = {
  pageLoading: boolean;
  loading: boolean;
  image: string;
  continue: boolean;
  cancel: boolean;
  skuList: Array<Array<string>>;
  count: number;
};

interface Emits {
  (e: 'submitted'): void;
}

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = withDefaults(defineProps<Props>(), {
  id: '',
  step: 0,
  images: () => []
});

const model = reactive<Model>({
  pageLoading: false,
  loading: false,
  image: '',
  continue: false,
  cancel: false,
  skuList: [],
  count: 1
});

const emit = defineEmits<Emits>();

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

watch(visible, value => {
  resetFields();
  if (value) {
    if (props.images && props.images.length) {
      model.image = props.images[0];
    }

    model.continue = false;
    model.cancel = false;
    model.skuList = [];
    model.count = 1;

    if (props.step === 3) {
      getProductSkuList();
    }
  }
});

function cancelPay() {
  model.cancel = true;
  window.$modal?.confirm({
    title: '取消购买确认',
    cancelText: '取消',
    okText: '确定',
    content: '确定要取消购买任务吗？',
    async onOk() {
      model.loading = true;
      try {
        await apiHandler();
      } finally {
        model.loading = false;
      }
    },
    onCancel() {
      model.cancel = false;
    }
  });
}

function skuClickHandler(index, skuIndex) {
  const old = model.skuList[index][skuIndex].checked;
  model.skuList[index].forEach(x => (x.checked = false));
  model.skuList[index][skuIndex].checked = !old;
}

async function getProductSkuList() {
  model.pageLoading = true;
  try {
    const { data: res, error } = await fetchGetPlanTaskSkuContinue(props.id);
    if (!error && res && res.code === 0) {
      model.skuList = res.data.list.map(item => {
        return item.map(x => {
          return {
            value: x,
            checked: false
          };
        });
      });
    }
  } finally {
    model.pageLoading = false;
  }
}

async function submitHandler() {
  const skuList = [];
  if (model.skuList && model.skuList.length) {
    // 检查每个SKU中是否至少有一个选中
    for (let i = 0; i < model.skuList.length; i += 1) {
      const checkedSku = model.skuList[i].filter(x => x.checked);
      if (checkedSku.length > 0) {
        // window.$message?.warning(`请选择规格${i + 1}的SKU属性`);
        // return;
        skuList.push(checkedSku[0].value);
      }
    }

    if (model.skuList.length !== skuList.length) {
      const confirmed = await skuSelectedConfirmHandler();
      if (!confirmed) {
        return;
      }
    }
  }

  model.continue = true;

  window.$modal?.confirm({
    title: '商品购买确认',
    cancelText: '取消',
    okText: '确定',
    content: '确定核验好商品准备购买吗？',
    async onOk() {
      model.loading = true;
      try {
        await apiHandler(skuList);
      } finally {
        model.loading = false;
      }
    },
    onCancel() {
      model.continue = false;
    }
  });
}

async function apiHandler(skuList) {
  await validate();

  const { data: res, error } = await fetchPlanTaskContinue({
    ...model,
    id: props.id,
    skuList,
    count: model.count
  });
  if (!error && res && res.code === 0) {
    window.$message?.success(model.cancel ? '取消指令已下发' : '购买指令已下发');
    visible.value = false;
    emit('submitted');
  }
}

function skuSelectedConfirmHandler() {
  return new Promise(resolve => {
    window.$modal?.confirm({
      title: 'SKU选择确认',
      cancelText: '取消',
      okText: '确定',
      content: '您尚未选择所有规格，确定只选择这些规格吗？',
      onOk() {
        resolve(true);
      },
      onCancel() {
        resolve(false);
      }
    });
  });
}
</script>

<template>
  <AModal v-model:open="visible" title="商品确认" :width="550">
    <div class="body">
      <p class="mb-3">请确认商品识别正确，如需拍照核实请拍照，点击继续购买进行后续购买流程！</p>

      <AForm ref="formRef" layout="vertical" :model="model" :rules="{ count: defaultRequiredRule }">
        <AFormItem v-if="props.step === 3 && model.skuList.length" label="请选择要购买的SKU属性">
          <div class="sku">
            <div v-for="(item, index) in model.skuList" :key="index">
              <p class="title">规格{{ index + 1 }}</p>
              <div class="wrap flex gap-8px">
                <ATag
                  v-for="(sku, skuIndex) in item"
                  :key="skuIndex"
                  :color="sku.checked ? 'purple' : ''"
                  @click="skuClickHandler(index, skuIndex)"
                >
                  {{ sku.value }}
                </ATag>
              </div>
            </div>
          </div>
        </AFormItem>

        <AFormItem v-if="props.step === 3" label="购买件数" name="count">
          <AInputNumber v-model:value="model.count" :min="1" :max="99999" :controls="false" :precision="0">
            <template #addonAfter>
              <span>件</span>
            </template>
          </AInputNumber>
        </AFormItem>
      </AForm>

      <LoadingWrap :loading="model.pageLoading"></LoadingWrap>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="() => (visible = false)">取消</AButton>
        <AButton danger :loading="model.loading" @click="cancelPay">取消购买</AButton>
        <AButton type="primary" :loading="model.loading" @click="submitHandler">继续购买</AButton>
      </ASpace>
    </template>
  </AModal>
</template>

<style scoped lang="scss">
.body {
  position: relative;

  .ant-input-number-group-wrapper {
    width: 100%;
  }
}

.sku {
  .title {
    font-weight: 600;
    letter-spacing: 1.5px;
    margin-bottom: 14px;
  }

  .ant-tag {
    font-size: 14px;
    cursor: pointer;
    transition: all 0.3s ease-in-out;

    white-space: normal;

    &:hover {
      border-color: #d3adf7;
      color: #531dab;
      background-color: #f9f0ff;
    }
  }

  > div {
    margin-bottom: 26px;

    &:last-child {
      margin-bottom: 0;
    }
  }
}
</style>
