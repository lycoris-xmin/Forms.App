<script setup lang="ts">
  import { withDefaults } from 'vue';
  import { useAntdForm, useFormRules } from '@/hooks/common/form';
  import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
  import type { EnumMap } from '@/views/shared/types';

  defineOptions({
    name: 'UserListSearch'
  });

  interface Emits {
    (e: 'reset'): void;
    (e: 'search'): void;
  }

  interface Props {
    status?: Array<EnumMap>;
    role?: Array<EnumMap>;
  }

  const { formRef, validate, resetFields } = useAntdForm();
  const { patternRules } = useFormRules();
  const model = defineModel<Api.User.ListSearchFilter>('model', { required: true });

  const props = withDefaults(defineProps<Props>(), {
    status: () => [],
    role: () => []
  });

  const rules = {
    phone: [
      {
        validator(_, value) {
          if (!value) {
            return Promise.resolve();
          }

          if (!patternRules.phone.pattern.test(value)) {
            return Promise.reject(new Error(patternRules.phone.message));
          }

          return Promise.resolve();
        },
        trigger: 'change'
      }
    ],
    email: [
      {
        validator(_, value) {
          if (!value) {
            return Promise.resolve();
          }

          if (!patternRules.email.pattern.test(value)) {
            return Promise.reject(new Error(patternRules.email.message));
          }

          return Promise.resolve();
        },
        trigger: 'change'
      }
    ]
  };

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
      :rules="rules"
      :label-col="{
        span: 5,
        md: 7
      }"
    >
      <ARow :gutter="[16, 16]" wrap>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="手机号" name="phone" class="m-0">
            <AInput v-model:value="model.phone" autocomplete="off" placeholder="精确查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="邮箱" name="email" class="m-0">
            <AInput v-model:value="model.email" autocomplete="off" placeholder="精确查询" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="角色" name="roleId">
            <ASelect v-model:value="model.roleId" placeholder="- 全部 -" :options="props.role" allow-clear></ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="状态" name="status">
            <ASelect v-model:value="model.status" placeholder="- 全部 -" :options="props.status" allow-clear></ASelect>
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
