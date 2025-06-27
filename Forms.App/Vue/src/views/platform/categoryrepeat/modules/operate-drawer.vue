<script setup lang="ts">
import { computed, reactive, ref, watch, withDefaults } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateCategoryRepeatPurchase, fetchUpdateCategoryRepeatPurchase } from '@/service/api';

defineOptions({
  name: 'SystemUserOperateDrawer'
});

interface Props {
  operateType?: AntDesign.TableOperateType;
  rowData?: Api.Product.TaobaoData | null;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  roleRefresh: boolean;
  submit: boolean;
};

type Model = Pick<Api.CategoryRepeatPurchase.CategoryRepeatPurchaseData, 'keyword' | 'limitDays'>;

type RuleKey = Extract<keyof Model, 'keyword' | 'limitDays'>;

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null
});

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增类目复购配置',
    edit: '编辑类目复购配置'
  };
  return titles[props.operateType];
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const model = ref(createDefaultModel());

const rules = computed<Record<RuleKey, App.Global.FormRule[]>>(() => {
  const { defaultRequiredRule } = useFormRules();

  return {
    keyword: defaultRequiredRule,
    limitDays: defaultRequiredRule
  };
});

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

function createDefaultModel(): Model {
  return {
    keyword: '',
    limitDays: 30
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value) {
    if (props.rowData) {
      Object.assign(model.value, props.rowData);

      model.value.keyword = '';
      for (const item of props.rowData.keyword) {
        model.value.keyword += `${item}\n`;
      }
    }
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  loadingMap.submit = true;
  try {
    const { data: res, error } = isEdit.value
      ? await fetchUpdateCategoryRepeatPurchase({ ...model.value })
      : await fetchCreateCategoryRepeatPurchase({ ...model.value });

    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="类目名称" name="name">
          <AInput v-model:value="model.name" autocomplete="off" placeholder="请输入类目名称" />
        </AFormItem>
        <AFormItem label="复购限制天数" name="limitDays">
          <AInputNumber v-model:value="model.limitDays" :controls="false" :min="1" :step="1">
            <template #addonAfter>
              <span>天</span>
            </template>
          </AInputNumber>
        </AFormItem>
        <AFormItem name="keyword">
          <template #label>
            <p>
              类目关键词
              <small class="p-l-8px">*如果有多个关键词请每行一个</small>
            </p>
          </template>
          <ATextarea v-model:value="model.keyword" placeholder="请输入类目关键词" :auto-size="{ minRows: 6 }" />
        </AFormItem>
      </AForm>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingMap.submit" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.body {
  .ant-input-number-group-wrapper {
    width: 100%;
  }

  .p-l-8px {
    color: var(--color-danger);
  }
}
</style>
