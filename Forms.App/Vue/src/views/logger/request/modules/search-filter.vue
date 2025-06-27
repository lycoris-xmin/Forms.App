<script setup lang="ts">
import { ref } from 'vue';
import dayjs from 'dayjs';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';

defineOptions({
  name: 'LoggerRequestListSearch'
});

interface propsType {
  httpMethod: Array<Any>;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

type Model = Api.Logger.RequestSearchFilter & {
  timeRange: Array<string>;
};

const { formRef, resetFields } = useAntdForm();

const model = defineModel<Model>('model', { required: true });

const props = defineProps<propsType>();

const emit = defineEmits<Emits>();

const enums = ref({
  success: [
    {
      value: 1,
      label: '成功'
    },
    {
      value: 0,
      label: '失败'
    }
  ]
});

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
          <AFormItem label="请求方式" name="httpMethod" class="m-0">
            <ASelect
              v-model:value="model.httpMethod"
              placeholder="- 全部 -"
              :options="props.httpMethod"
              allow-clear
            ></ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="请求地址" name="url" class="m-0">
            <AInput v-model:value="model.url" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="请求标识" name="traceId" class="m-0">
            <AInput v-model:value="model.traceId" autocomplete="off" placeholder="精确查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="请求状态" name="success" class="m-0">
            <ASelect v-model:value="model.success" placeholder="- 全部 -" :options="enums.success" allow-clear />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="请求时间" name="createTimeRange" class="m-0">
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
