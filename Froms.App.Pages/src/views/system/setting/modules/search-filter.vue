<script setup lang="ts">
  import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
  import { useAntdForm } from '@/hooks/common/form';

  defineOptions({
    name: 'SystemConfigurationListSearch'
  });

  interface Emits {
    (e: 'reset'): void;
    (e: 'search'): void;
  }

  const { formRef, validate, resetFields } = useAntdForm();

  const model = defineModel<Api.Configuration.ListSearchFilter>('model', { required: true });

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
          <AFormItem label="配置名称" name="configName" class="m-0">
            <AInput v-model:value="model.configName" autocomplete="off" placeholder="支持模糊查询" />
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
