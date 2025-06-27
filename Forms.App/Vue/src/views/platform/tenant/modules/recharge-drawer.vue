<script setup lang="ts">
import { reactive, ref, watch, withDefaults } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchTenantRecharge } from '@/service/api';

defineOptions({
  name: 'SystemUserOperateDrawer'
});

interface Props {
  tenantId?: string | null;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
};

type Model = Pick<Api.Tenant.TenantRecharge> & {
  id: number;
};

const { formRef, validate, resetFields } = useAntdForm();
const { defaultRequiredRule } = useFormRules();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const props = withDefaults(defineProps<Props>(), {
  tenantId: ''
});

const emit = defineEmits<Emits>();

const model = ref(createDefaultModel());

const rules = {
  days: defaultRequiredRule
};

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

function createDefaultModel(): Model {
  return {
    tenantId: '',
    days: 0
  };
}

function initModelHandler() {
  model.value = createDefaultModel();
  model.value.id = props.tenantId;
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  if (model.value.days <= 0) {
    closeDrawer();
    return;
  }

  loadingMap.submit = true;
  try {
    const { data: res, error } = await fetchTenantRecharge({ ...model.value });
    if (!error && res && res.code === 0) {
      window.$message?.success('使用成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="会员使用" :width="450">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem name="days" label="使用数">
          <AInputNumber
            v-model:value="model.days"
            class="input-number"
            addon-after="天"
            :controls="false"
            :min="0"
            :max="999999999"
            :step="1"
            :precision="0"
          ></AInputNumber>
        </AFormItem>
      </AForm>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingMap.submit" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.body {
  .input-number {
    width: 100%;
  }

  .flex {
    gap: 15px;

    > div {
      flex: 1;
      flex-shrink: 0;
    }
  }
}
</style>
