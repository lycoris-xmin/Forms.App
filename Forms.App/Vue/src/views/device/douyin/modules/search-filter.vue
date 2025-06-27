<script setup lang="ts">
import { withDefaults } from 'vue';
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';

defineOptions({
  name: 'DeviceTaobaoListSearch'
});

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

interface Props {
  init?: Array;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.DeviceTaobao.SearchFilter>('model', { required: true });

const props = withDefaults(defineProps<Props>(), {
  init: () => []
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
          <AFormItem label="系统编号" name="deviceId" class="m-0">
            <AInput
              v-model:value="model.deviceId"
              autocomplete="off"
              placeholder="支持模糊查询"
              @keyup.enter="search"
            />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="设备名称" name="deviceName" class="m-0">
            <AInput
              v-model:value="model.deviceName"
              autocomplete="off"
              placeholder="支持模糊查询"
              @keyup.enter="search"
            />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="登录账号" name="account" class="m-0">
            <AInput v-model:value="model.account" autocomplete="off" placeholder="支持模糊查询" @keyup.enter="search" />
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="状态" name="init">
            <ASelect v-model:value="model.init" placeholder="- 全部 -" allow-clear>
              <ASelectOption v-for="item in props.init" :key="item.value" :value="item.value">
                {{ item.label }}
              </ASelectOption>
            </ASelect>
          </AFormItem>
        </ACol>
        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="任务分配" name="allowPair">
            <ASelect v-model:value="model.allowPair" placeholder="- 全部 -" allow-clear>
              <ASelectOption :value="true">允许任务分配</ASelectOption>
              <ASelectOption :value="false">停止任务分配</ASelectOption>
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
