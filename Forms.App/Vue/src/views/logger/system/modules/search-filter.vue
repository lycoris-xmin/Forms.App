<script setup lang="ts">
import dayjs from 'dayjs';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';

defineOptions({
  name: 'LoggerSystemListSearch'
});

interface propsType {
  level: Array<Any>;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

type Model = Api.Logger.SystemSearchFilter & {
  timeRange: Array<string>;
};

const { formRef, resetFields } = useAntdForm();

const model = defineModel<Model>('model', { required: true });

const props = defineProps<propsType>();

const emit = defineEmits<Emits>();

async function reset() {
  await resetFields();
  emit('reset');
}

async function search() {
  emit('search');
}

function timeChange(_, dateString: string[]) {
  model.value.beginTime = dateString[0];
  model.value.endTime = dateString[1];
}
</script>

<template>
  <TableSearchFilter>
    <AForm
      ref="formRef"
      :model="model"
      :label-col="{
        span: 5,
        md: 7
      }"
    >
      <ARow :gutter="[16, 16]" wrap>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="日志类型" name="level" class="m-0">
            <ASelect v-model:value="model.level" placeholder="- 全部 -" :options="props.level" allow-clear></ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="日志时间" name="createTimeRange" class="m-0">
            <ARangePicker
              v-model:value="model.timeRange"
              :show-time="{
                defaultValue: [dayjs('00:00:00', 'HH:mm:ss'), dayjs('23:59:59', 'HH:mm:ss')]
              }"
              format="YYYY-MM-DD HH:mm:ss"
              :placeholder="['开始时间', '结束时间']"
              @change="timeChange"
            />
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

<style lang="scss" scoped></style>
