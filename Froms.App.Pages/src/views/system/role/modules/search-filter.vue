<script setup lang="ts">
  import { useAntdForm } from '@/hooks/common/form';
  import TableSearchFilter from '@/components/advanced/table-search-filter.vue';

  defineOptions({
    name: 'RoleListSearch'
  });

  interface Emits {
    (e: 'reset'): void;
    (e: 'search'): void;
  }

  const { formRef, validate, resetFields } = useAntdForm();

  const model = defineModel<Api.User.ListSearchFilter>('model', { required: true });

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
          <AFormItem label="角色名称" name="roleName" class="m-0">
            <AInput v-model:value="model.roleName" autocomplete="off" placeholder="支持模糊查询" />
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
