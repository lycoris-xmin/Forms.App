<script setup lang="ts">
import { reactive, watch } from 'vue';
import { fetchBatchDeviceRecharge, fetchGetTenantSubscriptionEnum } from '@/service/api';
import { useAntdForm, useFormRules } from '@/hooks/common/form';

defineOptions({
  name: 'DeviceBatchRechargeDrawer'
});

const visible = defineModel<boolean>('visible', { default: false });

type Props = {
  ids: Array<string>;
  userInfo: Api.Auth.Profile;
};

type Model = {
  days: number;
  unUseDays: number;
  loading: boolean;
  submitLoading: boolean;
};

type Emits = {
  (e: 'submitted'): void;
};

const { formRef, validate, resetFields } = useAntdForm();

const model = reactive<Model>({
  days: 0,
  unUseDays: 0,
  loading: true,
  submitLoading: false
});

const props = defineProps<Props>();

const { defaultRequiredRule } = useFormRules();

const rules = {
  days: defaultRequiredRule
};

const emit = defineEmits<Emits>();

watch(visible, async (value, old) => {
  if (value && value !== old) {
    if (props.userInfo.isTenant) {
      await getTenanSubscription();
    } else {
      model.days = 7;
      model.unUseDays = 9999999;
    }
  }

  resetFields();
});

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();
  model.submitLoading = true;
  try {
    const { data: res, error } = await fetchBatchDeviceRecharge(props.ids, model.days);

    if (!error && res && res.code === 0) {
      window.$message?.success('批量使用成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    model.submitLoading = false;
  }
}

async function getTenanSubscription() {
  const { data: res, error } = await fetchGetTenantSubscriptionEnum();
  if (!error && res && res.code === 0) {
    model.unUseDays = res.data.days;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="设备使用" :width="500">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem>
          <div class="space-between v-center flex">
            <p>已选择 {{ props.ids.length }} 个设备</p>
            <AButton v-if="props.userInfo.isTenant" @click="getTenanSubscription">刷新使用数</AButton>
          </div>
        </AFormItem>
        <AFormItem label="使用数">
          <AInputNumber
            v-model:value="model.days"
            :max="model.unUseDays"
            :min="props.userInfo.isTenant ? (model.unUseDays > 30 ? 30 : 0) : 1"
            :controls="false"
            :precision="0"
          >
            <template v-if="props.userInfo.isTenant" #addonAfter>
              剩余 {{ model.unUseDays - model.days * props.ids.length }} 天
            </template>
            <template v-else #addonAfter>天</template>
          </AInputNumber>
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
</style>
