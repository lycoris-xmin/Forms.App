<script setup lang="ts">
import { withDefaults } from 'vue';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';
import type { EnumMap } from '../shared/shared';

defineOptions({
  name: 'PlatformVipListSearch'
});

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

interface Props {
  status?: Array<EnumMap>;
  userInfo?: object;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.Tenant.ListSearchFilter>('model', { required: true });

const props = withDefaults(defineProps<Props>(), {
  status: () => [],
  userInfo: () => {}
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
          <AFormItem label="邮箱" name="email" class="m-0">
            <AInput v-model:value="model.email" autocomplete="off" placeholder="支持模糊查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="状态" name="status">
            <ASelect v-model:value="model.status" placeholder="- 全部 -" :options="props.status" allow-clear></ASelect>
          </AFormItem>
        </ACol>
        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="所属商户" name="tenantId" class="m-0">
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
