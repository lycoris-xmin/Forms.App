<script setup lang="ts">
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';
import ShopNameSelect from '@/views/shared/components/shopname-select/index.vue';

defineOptions({
  name: 'ALIProductListSearch'
});

interface Props {
  userInfo: Api.Auth.Profile;
  productType: Array;
  shopNameEnum: Array;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.Product.SearchFilter>('model', { required: true });

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
      :model="model"
      :label-col="{
        span: 5,
        md: 7
      }"
    >
      <ARow :gutter="[16, 16]" wrap>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="商品编号" name="itemId" class="m-0">
            <AInput v-model:value="model.itemId" placeholder="" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="商品名称" name="title" class="m-0">
            <AInput v-model:value="model.title" placeholder="" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="所属店铺" name="shopId" class="m-0">
            <ShopNameSelect
              v-model:value="model.shopId"
              :platform="1"
              :allow-clear="true"
              :show-search="true"
              :options="props.shopNameEnum"
            ></ShopNameSelect>
          </AFormItem>
        </ACol>
        <ACol v-if="false" :span="24" :md="12" :lg="6">
          <AFormItem label="商品类型" name="type" class="m-0">
            <ASelect v-model:value="model.type">
              <ASelectOption v-for="item in props.productType" :key="item.value" :value="item.value">
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

<style lang="scss" scoped></style>
