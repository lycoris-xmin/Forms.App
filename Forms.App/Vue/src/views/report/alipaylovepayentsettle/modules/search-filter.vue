<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import dayjs from 'dayjs';
import { UserOutlined } from '@ant-design/icons-vue';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import { fetchGetAlipayLovePayForTenantAdminEnum } from '@/service/api';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';

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
  Api.AlipayLovePayEntSettle.ListSearchFilter,
  'platform' | 'alipayLovePayId' | 'userId' | 'settlemented' | 'beginTime' | 'endTime' | 'shopName'
>;

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Model>('model', { required: true });

const enums = reactive({
  alipayLovePay: []
});

const props = defineProps<propsType>();

const emit = defineEmits<Emits>();

onMounted(() => {
  //
  getAlipayLovePayEnum();
});

async function getAlipayLovePayEnum() {
  const { data: res, error } = await fetchGetAlipayLovePayForTenantAdminEnum();
  if (!error && res && res.code === 0) {
    enums.alipayLovePay = res.data.list;
  }
}

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

function tenantFilterOption(input, option) {
  if (!option || !option.options || !option.options.length) {
    return false;
  }

  for (const item of option.options) {
    if (
      enums.alipayLovePay.filter(x => x.filter(y => y.text.includes(input) || y.data.includes(input)).length > 0).length
    ) {
      return true;
    }
    const child = item.children();
    if (child && child.length) {
      for (const tmp of child) {
        const label = tmp.children.toLowerCase();
        if (label.includes(input)) {
          return true;
        }
      }
    }
  }

  return false;
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
          <AFormItem label="支付平台" name="platform" class="m-0">
            <ASelect v-model:value="model.platform" placeholder="- 全部 -" :options="props.platform" allow-clear />
          </AFormItem>
        </ACol>

        <ACol v-if="props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="亲情卡" name="alipayLovePayId" class="m-0">
            <!-- 商户 -->
            <ASelect
              v-if="props.userInfo.isTenant && !props.userInfo.isTenantUser"
              v-model:value="model.alipayLovePayId"
              show-search
              option-filter-prop="children"
              :filter-option="tenantFilterOption"
              placeholder="- 全部 -"
              allow-clear
            >
              <ASelectOptGroup v-for="(list, index) in enums.alipayLovePay" :key="index">
                <template #label>
                  <span>
                    <UserOutlined />
                    {{ list[0].data }}
                  </span>
                </template>
                <ASelectOption v-for="item in list" :key="item.value" :value="item.value">
                  {{ item.text }}
                </ASelectOption>
              </ASelectOptGroup>
            </ASelect>
          </AFormItem>
        </ACol>

        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="所属商户" name="userId" class="m-0">
            <TenantSelect v-model:value="model.userId" placeholder="- 全部 -" />
          </AFormItem>
        </ACol>

        <ACol v-if="props.userInfo.isTenant && !props.userInfo.isTenantUser" :span="24" :md="12" :lg="6">
          <AFormItem label="店铺名称" name="shopName" class="m-0">
            <AInput v-model:value="model.shopName" autocomplete="off" placeholder="支持模糊查询" />
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
