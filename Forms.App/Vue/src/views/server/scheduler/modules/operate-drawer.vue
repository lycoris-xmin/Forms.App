<script setup lang="ts">
  import { computed, reactive, ref, watch, withDefaults } from 'vue';
  import { useAntdForm, useFormRules } from '@/hooks/common/form';
  import type { EnumMap } from '@/views/shared/types';

  defineOptions({
    name: 'SystemUserOperateDrawer'
  });

  interface Props {
    operateType?: AntDesign.TableOperateType;
    rowData?: Api.User.Data | null;
    jogGroup?: Array<string>;
    triggerType?: Array<EnumMap>;
  }

  interface Emits {
    (e: 'submitted'): void;
  }

  type LoadingMap = {
    submit: boolean;
  };

  type Model = Pick<Api.Scheduler.Data, 'jobName', 'jobGroup' | 'triggerType' | 'cron' | 'intervalSecond' | 'runTimes' | 'beginTime' | 'endTime'>;

  type RuleKey = Extract<keyof Model, 'jobName' | 'jobGroup' | 'triggerType' | 'cron' | 'intervalSecond' | 'runTimes' | 'beginTime' | 'endTime'>;

  const { formRef, validate, resetFields } = useAntdForm();

  const visible = defineModel<boolean>('visible', {
    default: false
  });

  const loadingMap = reactive<LoadingMap>({
    submit: false
  });

  const props = withDefaults(defineProps<Props>(), {
    operateType: 'add',
    rowData: null,
    jogGroup: () => [],
    triggerType: () => []
  });

  const emit = defineEmits<Emits>();

  const title = computed(() => {
    const titles: Record<AntDesign.TableOperateType, string> = {
      add: '新增用户',
      edit: '编辑用户'
    };
    return titles[props.operateType];
  });

  const isEdit = computed<boolean>(() => {
    return props.operateType === 'edit';
  });

  const model: Model = ref(createDefaultModel());

  watch(visible, () => {
    if (visible.value) {
      initModelHandler();
      resetFields();
    }
  });

  const rules = computed<Record<RuleKey, App.Global.FormRule[]>>(() => {
    const { defaultRequiredRule } = useFormRules();

    return {
      jobName: [defaultRequiredRule],
      jobGroup: [defaultRequiredRule]
    };
  });

  function createDefaultModel(): Model {
    return {
      jobName: ''
    };
  }

  function initModelHandler() {
    model.value = createDefaultModel();

    if (isEdit.value) {
      if (props.rowData) {
        Object.assign(model.value, props.rowData);
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
      //
      emit('submitted');
    } finally {
      loadingMap.submit = false;
    }
  }
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="550">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="任务名称" name="jobName" class="m-0">
          <AInput v-model:value="model.jobName"></AInput>
        </AFormItem>
        <AFormItem label="分组名称" name="jogGroup" class="m-0">
          <ASelect v-model:value="model.jogGroup" :options="props.jogGroup"></ASelect>
        </AFormItem>
        <AFormItem label="定时类型" name="triggerType" class="m-0">
          <ASelect v-model:value="model.triggerType" :options="props.triggerType"></ASelect>
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
  .role-select {
    gap: 10px;
  }

  .refresh-btn {
    width: 32px;
    flex-shrink: 0;
  }
</style>
