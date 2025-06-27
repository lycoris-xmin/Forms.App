<script setup lang="ts">
import dayjs from 'dayjs';
import { ref } from 'vue';
import { CaretDownOutlined, CaretUpOutlined } from '@ant-design/icons-vue';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';
import { planFilterStatus } from '@/views/shared/enums';
import DeviceSelect from '@/views/shared/components/device-select/index.vue';
import ShopNameSelect from '@/views/shared/components/shopname-select/index.vue';

defineOptions({
  name: 'PlanTaskListSearch'
});

interface propsType {
  userInfo: Api.Auth.Profile;
  mode: Array<Any>;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const { formRef, resetFields } = useAntdForm();

const model = defineModel<Api.PlanTask.ListSearchFilter>('model', { required: true });

const props = defineProps<propsType>();

const showMore = ref<boolean>(false);

const emit = defineEmits<Emits>();

async function reset() {
  await resetFields();
  emit('reset');
}

async function search() {
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
          <AFormItem label="商品标题" name="title" class="m-0">
            <AInput v-model:value="model.title" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>

        <ACol v-if="props.userInfo.tenantType === 20 || !props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="店铺名称" name="shopName" class="m-0">
            <AInput v-model:value="model.shopName" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>

        <ACol v-else-if="props.userInfo.tenantType !== 20 || !props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="店铺名称" name="shopName" class="m-0">
            <ShopNameSelect v-model:value="model.shopName" :platform="0"></ShopNameSelect>
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="订单编号" name="orderId" class="m-0">
            <AInput v-model:value="model.orderId" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>

        <ACol v-if="showMore" :span="24" :md="12" :lg="6">
          <AFormItem label="执行设备" name="deviceId" class="m-0">
            <DeviceSelect v-model:value="model.deviceId" />
          </AFormItem>
        </ACol>

        <ACol v-if="showMore" :span="24" :md="12" :lg="6">
          <AFormItem label="购买状态" name="success" class="m-0">
            <ASelect
              v-model:value="model.success"
              placeholder="- 全部 -"
              :options="[
                { value: true, label: '成功' },
                { value: false, label: '失败' }
              ]"
              allow-clear
            />
          </AFormItem>
        </ACol>

        <ACol v-if="showMore" :span="24" :md="12" :lg="6">
          <AFormItem label="任务状态" name="status" class="m-0">
            <ASelect
              v-model:value="model.status"
              placeholder="- 全部 -"
              :options="planFilterStatus"
              allow-clear
              mode="multiple"
              max-tag-count="responsive"
            />
          </AFormItem>
        </ACol>

        <ACol v-if="showMore" :span="24" :md="12" :lg="6">
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
        <ACol v-if="showMore && !props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="创建商户" name="userId" class="m-0">
            <TenantSelect v-model:value="model.userId" placeholder="- 全部 -" />
          </AFormItem>
        </ACol>

        <ACol v-else-if="showMore && !props.userInfo.isTenantUser" :span="24" :md="12" :lg="6">
          <AFormItem label="创建员工" name="userId" class="m-0">
            <TenantSelect v-model:value="model.userId" placeholder="- 全部 -" :tenant-user="true" />
          </AFormItem>
        </ACol>
        <div class="flex-1">
          <AFormItem class="m-0">
            <div class="w-full flex-y-center justify-end gap-12px">
              <AButton @click="() => (showMore = !showMore)">
                <div class="v-center center flex">
                  <CaretUpOutlined v-if="showMore" />
                  <CaretDownOutlined v-else />
                  <span class="ml-8px">{{ !showMore ? '展开更多' : '收起筛选' }}</span>
                </div>
              </AButton>
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
