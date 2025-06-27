<script setup lang="ts">
import dayjs from 'dayjs';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';
import ShopNameSelect from '@/views/shared/components/shopname-select/index.vue';

defineOptions({
  name: 'AliPlanTaskListSearch'
});

interface propsType {
  userInfo: Api.Auth.Profile;
  mode: Array<Any>;
  status: Array<Any>;
  shopNameEnum?: Array<Any>;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.PlanTask.ListSearchFilter>('model', { required: true });

const props = defineProps<propsType>();

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

const disabledDate = (current: Dayjs) => {
  // Can not select days before today and today
  return current && current >= dayjs().endOf('day');
};
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
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务模式" name="mode" class="m-0">
            <ASelect v-model:value="model.mode" placeholder="- 全部 -" :options="props.mode" allow-clear />
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务状态" name="status" class="m-0">
            <ASelect v-model:value="model.status" placeholder="- 全部 -" :options="props.status" allow-clear />
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="商品标题" name="title" class="m-0">
            <AInput v-model:value="model.title" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="所属店铺" name="shopId" class="m-0">
            <ShopNameSelect
              v-model:value="model.shopId"
              :platform="1"
              :show-search="!props.userInfo.isTenant"
              :remote="!props.userInfo.isTenant"
              :options="props.userInfo.isTenant ? props.shopNameEnum : void 0"
            ></ShopNameSelect>
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="创建时间" name="createTimeRange" class="m-0">
            <ARangePicker
              v-model:value="model.createTimeRange"
              :show-time="{
                hideDisabledOptions: true,
                defaultValue: [dayjs('00:00:00', 'HH:mm:ss'), dayjs('23:59:59', 'HH:mm:ss')]
              }"
              format="YYYY-MM-DD HH:mm:ss"
              :placeholder="['开始时间', '结束时间']"
              :disabled-date="disabledDate"
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

    .ant-picker {
      width: 100%;
    }
  }
}
</style>
