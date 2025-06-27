<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateTaobaoShop, fetchUpdateTaobaoShop } from '@/service/api';

defineOptions({
  name: 'TaobaoShopOperateDrawer'
});

interface Props {
  /** the type of operation */
  operateType: AntDesign.TableOperateType;
  /** the edit row data */
  rowData?: Api.Product.TaobaoData | null;
}

interface Emits {
  (e: 'submitted'): void;
}

type Model = Pick<Api.Shop.BaseData, 'id' | 'shopName'> & {
  loading: boolean;
};

type RuleKey = Extract<keyof Model, 'shopName'>;

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = defineProps<Props>();

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增店铺',
    edit: '编辑店铺'
  };
  return titles[props.operateType];
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const model = ref(createDefaultModel());

const rules: Record<RuleKey, App.Global.FormRule> = {
  shopName: defaultRequiredRule
};

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

function createDefaultModel(): Model {
  return {
    id: '',
    shopName: '',
    loading: false
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value && props.rowData) {
    Object.assign(model.value, props.rowData);
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  model.value.loading = true;
  try {
    const { data: res, error } = isEdit.value
      ? await fetchUpdateTaobaoShop({ ...model.value })
      : await fetchCreateTaobaoShop(model.value.shopName);

    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    model.value.loading = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="550">
    <AForm ref="formRef" layout="vertical" :model="model" :rules="rules">
      <AFormItem label="店铺名称" name="shopName">
        <AInput v-model:value="model.shopName" placeholder="请输入店铺名称" :maxlength="100" />
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

<style scoped></style>
