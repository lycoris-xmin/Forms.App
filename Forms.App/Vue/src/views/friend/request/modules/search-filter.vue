<script setup lang="ts">
import { useAntdForm } from '@/hooks/common/form';
import { friendRequestStatus as REQUEST_STATUS } from '@/views/shared/enums';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';

defineOptions({
  name: 'FriendListSearch'
});

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.Friend.SearchFilter>('model', { required: true });

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
          <AFormItem label="好友账号" name="Account" class="m-0">
            <AInput v-model:value="model.Account" placeholder="好友账号" />
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="好友昵称" name="NickName" class="m-0">
            <AInput v-model:value="model.NickName" placeholder="好友昵称" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="状态" name="status" class="m-0">
            <ASelect
              v-model:value="model.status"
              placeholder="- 全部 -"
              :options="REQUEST_STATUS"
              allow-clear
            ></ASelect>
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

<style lang="scss" scoped>
.ant-input-number-group-wrapper {
  width: 100%;
}
</style>
