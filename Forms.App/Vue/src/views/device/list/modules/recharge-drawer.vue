<script setup lang="ts">
import { reactive, watch } from 'vue';
import { fetchDeviceRecharge, fetchGetTenantSubscriptionEnum } from '@/service/api';
import { useAntdForm, useFormRules } from '@/hooks/common/form';

defineOptions({
  name: 'DeviceRechargeDrawer'
});

const visible = defineModel<boolean>('visible', { default: false });

type Props = {
  id: string;
  userInfo: Api.Auth.Profile;
};

type Model = {
  type: number;
  days: number;
  unUseDays: number;
  loading: boolean;
  submitLoading: boolean;
};

type Emits = {
  (e: 'submitted'): void;
};

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const model = reactive<Model>({
  type: 0,
  days: 0,
  unUseDays: 0,
  loading: true,
  submitLoading: false
});

const props = defineProps<Props>();

const emit = defineEmits<Emits>();

const rules = {
  days: defaultRequiredRule
};

watch(visible, async value => {
  resetFields();

  if (value) {
    if (props.userInfo.isTenant) {
      await getTenantSubscription();
    } else {
      model.days = 7;
      model.unUseDays = 9999999;
    }
  }
});

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  model.submitLoading = true;
  try {
    const { data: res, error } =
      model.type === 0
        ? await fetchDeviceRecharge({ id: props.id, ...model })
        : await fetchDeviceRecharge({ id: props.id, ...model });

    if (!error && res && res.code === 0) {
      window.$message?.success('更新成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    model.submitLoading = false;
  }
}

async function getTenantSubscription() {
  const { data: res, error } = await fetchGetTenantSubscriptionEnum();
  if (!error && res && res.code === 0) {
    model.unUseDays = res.data.days;
    if (model.unUseDays > 30) {
      model.days = 30;
    }
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="设备使用" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem v-if="props.userInfo.isTenant">
          <div class="text-right">
            <AButton @click="getTenantSubscription">刷新使用数</AButton>
          </div>
        </AFormItem>
        <AFormItem v-if="model.type === 0" label="添加使用数" name="days">
          <AInputNumber
            v-model:value="model.days"
            :max="model.unUseDays"
            :min="props.userInfo.isTenant ? (model.unUseDays > 30 ? 30 : 0) : 1"
            :controls="false"
            :precision="0"
          >
            <template v-if="props.userInfo.isTenant" #addonAfter>剩余 {{ model.unUseDays - model.days }} 天</template>
            <template v-else #addonAfter>天</template>
          </AInputNumber>
          <p v-if="props.userInfo.isTenant && model.unUseDays > 30" class="text-danger">注意：最小使用天数为30天</p>
          <p v-if="props.userInfo.isTenant && model.unUseDays < 30">提示：最小使用天数为30天，账号剩余天数不足</p>
        </AFormItem>
      </AForm>
    </div>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="model.submitLoading" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style lang="scss" scoped>
.form-label {
  gap: 15px;
}

.ant-input-number-group-wrapper {
  width: 100%;
}

.ant-input-number {
  width: 100%;
}
</style>
