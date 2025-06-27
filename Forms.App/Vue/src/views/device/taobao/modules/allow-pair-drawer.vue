<script setup lang="ts">
import { computed, reactive } from 'vue';
import { fetchDeviceTaobaoAllowPair } from '@/service/api';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
const visible = defineModel<boolean>('visible', { default: false });
type Props = {
  ids: Array<string>;
};
type Emits = {
  (e: 'submitted'): void;
};
type Model = {
  allowPair: number;
};

const model = reactive<Model>({
  allowPair: 1,
  submitLoading: false
});
const emit = defineEmits<Emits>();
const props = defineProps<Props>();

const closeModel = () => {
  visible.value = false;
};

const { formRef } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const rules = {
  allowPair: defaultRequiredRule
};

const labelText = computed(() => {
  return model.allowPair
    ? '任务分配(允许任务分配后，将自动分配TB联盟任务)'
    : '任务分配(停止任务分配后，TB联盟任务将不再分配)';
});

async function handleAllowPair() {
  model.submitLoading = true;
  const data = {
    ids: props.ids,
    allowPair: model.allowPair
  };
  try {
    const { data: res, error } = await fetchDeviceTaobaoAllowPair(data);
    if (!error && res?.code === 0) {
      window.$message?.success('保存成功');
      emit('submitted');
      closeModel();
    }
  } finally {
    model.submitLoading = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="任务分配" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem :label="labelText" name="allowPair">
          <ASelect v-model:value="model.allowPair">
            <ASelectOption :value="1">允许任务分配</ASelectOption>
            <ASelectOption :value="0">停止任务分配</ASelectOption>
          </ASelect>
        </AFormItem>
      </AForm>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton type="default" @click="closeModel">取消</AButton>
        <AButton type="primary" :loading="model.submitLoading" @click="handleAllowPair">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>
