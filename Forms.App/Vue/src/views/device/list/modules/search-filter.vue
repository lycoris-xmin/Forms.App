<script setup lang="ts">
import { useAntdForm } from '@/hooks/common/form';
import TableSearchFilter from '@/components/advanced/table-search-filter.vue';
import { deviceStatus as DEVICE_STATUS } from '@/views/shared/enums';
import TenantSelect from '@/views/shared/components/tenant-select/index.vue';

defineOptions({
  name: 'DeviceListSearch'
});

interface propsType {
  userInfo: Api.Auth.Profile;
  devieType: Array;
  groupName: Array;
}

interface Emits {
  (e: 'reset'): void;
  (e: 'search'): void;
}

const { formRef, validate, resetFields } = useAntdForm();

const model = defineModel<Api.Device.SearchFilter>('model', { required: true });

const props = defineProps<propsType>();

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
          <AFormItem label="系统编号" name="id" class="m-0">
            <AInput v-model:value="model.id" placeholder="支持模糊查询" @keyup.enter="search" />
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="设备名称" name="name" class="m-0">
            <AInput v-model:value="model.name" placeholder="设备名称(支持迷糊查询)" @keyup.enter="search" />
          </AFormItem>
        </ACol>

        <ACol
          v-if="!props.userInfo.isTenant || (props.devieType && props.devieType.length > 1)"
          :span="24"
          :md="12"
          :lg="6"
        >
          <AFormItem label="设备类型" name="type">
            <ASelect v-model:value="model.type" placeholder="- 全部 -" allow-clear>
              <ASelectOption v-for="item in props.devieType" :key="item.value" :value="item.value">
                {{ item.label }}
              </ASelectOption>
            </ASelect>
          </AFormItem>
        </ACol>

        <ACol v-if="props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="设备分组" name="groupName" class="m-0">
            <ASelect v-model:value="model.groupName" placeholder="- 全部 -" allow-clear>
              <ASelectOption v-for="item in props.groupName" :key="item.value" :value="item.value">
                {{ item.label }}
              </ASelectOption>
            </ASelect>
          </AFormItem>
        </ACol>

        <ACol :span="24" :md="12" :lg="6">
          <AFormItem label="状态" name="status" class="m-0">
            <ASelect v-model:value="model.status" placeholder="- 全部 -" :options="DEVICE_STATUS" allow-clear></ASelect>
          </AFormItem>
        </ACol>

        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="脚本数量" name="runCount" class="m-0">
            <AInputNumber
              v-model:value="model.runCount"
              addon-before=">="
              :step="1"
              :min="0"
              :precision="0"
              :controls="false"
            ></AInputNumber>
          </AFormItem>
        </ACol>

        <ACol v-if="!props.userInfo.isTenant" :span="24" :md="12" :lg="6">
          <AFormItem label="核心数量" name="coreRunCount" class="m-0">
            <AInputNumber
              v-model:value="model.coreRunCount"
              addon-before=">="
              :step="1"
              :min="0"
              :precision="0"
              :controls="false"
            ></AInputNumber>
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

<style lang="scss" scoped>
.ant-input-number-group-wrapper {
  width: 100%;
}
</style>
