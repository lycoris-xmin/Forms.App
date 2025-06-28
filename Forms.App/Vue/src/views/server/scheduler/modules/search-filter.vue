<script setup lang="ts">
  import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
  import { useAntdForm } from '@/hooks/common/form';
  import type { EnumMap } from '@/views/shared/types';

  defineOptions({
    name: 'SchedulerListSearch'
  });

  interface Props {
    jogGroup?: Array<string>;
    triggerType?: Array<EnumMap>;
    status?: Array<EnumMap>;
  }

  interface Emits {
    (e: 'reset'): void;
    (e: 'search'): void;
  }

  const { formRef, validate, resetFields } = useAntdForm();

  const model = defineModel<Api.Scheduler.SearchFilter>('model', { required: true });

  const props = withDefaults(defineProps<Props>(), {
    jogGroup: () => [],
    triggerType: () => [],
    status: () => []
  });

  const emit = defineEmits<Emits>();

  async function reset() {
    await resetFields();
    emit('reset');
  }

  async function search() {
    await validate();
    emit('search');
  }
</script>

<template>
  <TableSearchFilter>
    <AForm
      ref="formRef"
      name="filer"
      :model="model"
      :label-col="{
        span: 5,
        md: 7
      }"
    >
      <ARow :gutter="[16, 16]" wrap>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="分组名称" name="jogGroup" class="m-0">
            <ASelect v-model:value="model.jogGroup" :options="props.jogGroup"></ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="定时类型" name="triggerType" class="m-0">
            <ASelect v-model:value="model.triggerType" :options="props.triggerType"></ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务状态" name="status" class="m-0">
            <ASelect v-model:value="model.status" :options="props.status"></ASelect>
          </AFormItem>
        </ACol>
        <div class="flex-1">
          <AFormItem class="m-0">
            <div class="w-full flex-y-center justify-end gap-12px">
              <AButton @click="reset">
                <template #icon>
                  <icon-ic-round-refresh class="align-sub text-icon" />
                </template>
                <span class="ml-8px">重置</span>
              </AButton>
              <AButton type="primary" ghost @click="search">
                <template #icon>
                  <icon-ic-round-search class="align-sub text-icon" />
                </template>
                <span class="ml-8px">搜索</span>
              </AButton>
            </div>
          </AFormItem>
        </div>
      </ARow>
    </AForm>
  </TableSearchFilter>
</template>

<style scoped></style>
