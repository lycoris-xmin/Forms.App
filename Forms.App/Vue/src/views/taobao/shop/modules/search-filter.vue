<script setup lang="ts">
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';

defineOptions({
  name: 'TaobaoSHopListSearch'
});

type Props = {
  userInfo: Api.Auth.Profile;
};

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.Shop.BaseSearchFilter>('model', { required: true });

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
          <AFormItem label="店铺名称" name="shopName" class="m-0">
            <AInput v-model:value="model.shopName" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>
        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="创建商户" name="tenantId" class="m-0">
            <TenantSelect v-model:value="model.tenantId" placeholder="- 全部 -" />
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
