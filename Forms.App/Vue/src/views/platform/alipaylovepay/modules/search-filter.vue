<script setup lang="ts">
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';

defineOptions({
  name: 'PlatformAlipayLovePayListSearch'
});

interface Props {
  userInfo?: object;
  alipayLovePay?: object;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

type Model = Pick<
  Api.AlipayLovePay.ListSearchFilter,
  'alipayName' | 'alipayAccount' | 'userId' | 'tenantUser' | 'shopName' | 'alipayLovePayType'
>;

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Model>('model', { required: true });

const props = withDefaults(defineProps<Props>(), {
  userInfo: () => {},
  alipayLovePay: () => {}
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
          <AFormItem label="亲情卡账号" name="alipayAccount" class="m-0">
            <AInput v-model:value="model.alipayAccount" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="亲情卡备注" name="alipayName" class="m-0">
            <AInput v-model:value="model.alipayName" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>

        <ACol v-if="props.userInfo.isTenant && !props.userInfo.isTenantUser" :span="24" :md="12" :lg="6">
          <AFormItem label="绑定店铺" name="shopName" class="m-0">
            <AInput v-model:value="model.shopName" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>

        <ACol v-if="props.userInfo.isTenant && !props.userInfo.isTenantUser" :span="24" :md="12" :lg="6">
          <AFormItem label="亲情卡类型" name="alipayLovePayType" class="m-0">
            <ASelect v-model:value="model.alipayLovePayType" placeholder="- 全部 -">
              <ASelectOption v-for="item in props.alipayLovePay" :key="item.value" :value="item.value">
                {{ item.label }}
              </ASelectOption>
            </ASelect>
          </AFormItem>
        </ACol>

        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="所属商户" name="userId" class="m-0">
            <TenantSelect v-model:value="model.userId" placeholder="- 全部 -" />
          </AFormItem>
        </ACol>

        <ACol v-else-if="!props.userInfo.isTenantUser" :span="24" :md="12" :lg="6">
          <AFormItem label="绑定员工" name="tenantUser" class="m-0">
            <TenantSelect v-model:value="model.tenantUser" placeholder="- 全部 -" :tenant-user="true" />
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
