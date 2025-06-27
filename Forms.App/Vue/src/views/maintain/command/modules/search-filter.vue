<script setup lang="ts">
import { withDefaults } from 'vue';
import dayjs from 'dayjs';
import { UpOutlined } from '@ant-design/icons-vue';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';

defineOptions({
  name: 'DeviceCommandListSearch'
});

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

interface Props {
  command?: Array<EnumAntd>;
  status?: Array<EnumAntd>;
}

type Model = Api.DeviceCommand.SearchFilte & {
  createTimeRange: Array<string>;
};

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Model>('model', { required: true });

const props = withDefaults(defineProps<Props>(), {
  status: () => [],
  command: () => []
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

function createTimeChange(_, dateString: string[]) {
  model.value.beginTime = dateString[0];
  model.value.endTime = dateString[1];
}

function collapseHandler() {
  //
}
</script>

<template>
  <TableSearchFilter>
    <template #extra>
      <span class="collapse" @click="collapseHandler"><UpOutlined /></span>
    </template>

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
          <AFormItem label="指令编号" name="id" class="m-0">
            <AInput v-model:value="model.id" autocomplete="off" placeholder="精准查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="设备编号" name="deviceId" class="m-0">
            <AInput v-model:value="model.deviceId" autocomplete="off" placeholder="精准查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务编号" name="taskId" class="m-0">
            <AInput v-model:value="model.taskId" autocomplete="off" placeholder="精准查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="执行指令" name="command">
            <ASelect
              v-model:value="model.command"
              placeholder="- 全部 -"
              :options="props.command"
              allow-clear
            ></ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="执行状态" name="status">
            <ASelect v-model:value="model.status" placeholder="- 全部 -" :options="props.status" allow-clear></ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="执行时间" name="createTimeRange" class="m-0">
            <ARangePicker
              v-model:value="model.createTimeRange"
              :show-time="{
                defaultValue: [dayjs('00:00:00', 'HH:mm:ss'), dayjs('23:59:59', 'HH:mm:ss')]
              }"
              format="YYYY-MM-DD HH:mm:ss"
              :placeholder="['开始时间', '结束时间']"
              @change="createTimeChange"
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

<style lang="scss" scoped>
.collapse {
  cursor: pointer;
}
</style>
