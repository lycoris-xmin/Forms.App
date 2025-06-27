<script setup lang="ts">
import { onMounted } from 'vue';
import dayjs from 'dayjs';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';

defineOptions({
  name: 'PlanTaskCommentListSearch'
});

type Props = {
  // platform: Array;
  status: Array;
  userInfo: Api.Auth.Profile;
};

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.PlanTaskComment.SearchFilter>('model', { required: true });

onMounted(() => {
  model.value.createTimeRange = [];
});

const props = defineProps<Props>();

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
</script>

<template>
  <TableSearchFilter>
    <AForm
      ref="formRef"
      class="search-filter"
      :model="model"
      :label-col="{
        span: 5,
        md: 7
      }"
    >
      <ARow :gutter="[16, 16]" wrap>
        <!--
 <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务平台" name="platform" class="m-0">
            <ASelect v-model:value="model.platform" placeholder="- 全部 -" :options="props.platform" allow-clear />
          </AFormItem>
        </ACol> 
-->
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务编号" name="taskId" class="m-0">
            <AInput v-model:value="model.taskId" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务状态" name="status" class="m-0">
            <ASelect v-model:value="model.status" placeholder="- 全部 -" :options="props.status" allow-clear />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="创建时间" name="createTimeRange" class="m-0">
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
        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="创建商户" name="userId" class="m-0">
            <TenantSelect v-model:value="model.userId" placeholder="- 全部 -" />
          </AFormItem>
        </ACol>
        <ACol v-else-if="!props.userInfo.isTenantUser" :span="24" :md="12" :lg="6">
          <AFormItem label="创建员工" name="userId" class="m-0">
            <TenantSelect v-model:value="model.userId" placeholder="- 全部 -" :tenant-user="true" />
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
.search-filter {
  :deep(.ant-form-item-control-input-content) {
    width: 100%;
  }
}
</style>
