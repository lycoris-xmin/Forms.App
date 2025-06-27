<script setup lang="ts">
import dayjs from 'dayjs';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';
import TenantUserSelect from './components/tenuser-select.vue';

defineOptions({
  name: 'ReportAlipayLovePayRecordListSearch'
});

interface propsType {
  platform: Array<any>;
  userInfo: Api.Auth.Profile;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

type Model = Pick<
  Api.AlipayLovePayPerSettle.ListSearchFilter,
  'platform' | 'userId' | 'settlemented' | 'beginTime' | 'endTime' | 'usedUserId'
>;

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Model>('model', { required: true });

const props = defineProps<propsType>();

const emit = defineEmits<Emits>();

// onMounted(() => {
//   //
//   getAlipayLovePayEnum();
// });

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
      name="filer"
      :model="model"
      :label-col="{
        span: 5,
        md: 7
      }"
    >
      <ARow :gutter="[16, 16]" wrap>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="支付平台" name="platform" class="m-0">
            <ASelect v-model:value="model.platform" placeholder="- 全部 -" :options="props.platform" allow-clear />
          </AFormItem>
        </ACol>

        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="所属商户" name="userId" class="m-0">
            <TenantSelect v-model:value="model.userId" placeholder="- 全部 -" />
          </AFormItem>
        </ACol>

        <ACol v-else :span="24" :md="12" :lg="6">
          <AFormItem label="持有者" name="userId" class="m-0">
            <TenantUserSelect v-model:value="model.userId" placeholder="- 全部 -" />
          </AFormItem>
        </ACol>

        <ACol v-if="props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="使用人" name="usedUserId" class="m-0">
            <TenantUserSelect v-model:value="model.usedUserId" placeholder="- 全部 -" />
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

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="结算状态" name="settlemented" class="m-0">
            <ASelect v-model:value="model.settlemented" placeholder="- 全部 -" allow-clear>
              <ASelectOption :value="1">已结算</ASelectOption>
              <ASelectOption :value="0">未结算</ASelectOption>
            </ASelect>
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
