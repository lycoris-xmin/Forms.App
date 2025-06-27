<script setup lang="ts">
import { computed, reactive, ref, watch, withDefaults } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateRole, fetchUpdateRole } from '@/service/api';

defineOptions({
  name: 'RoleOperateDrawer'
});

interface Props {
  operateType?: AntDesign.TableOperateType;
  rowData?: Api.Product.TaobaoData | null;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
};

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null
});

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增角色',
    edit: '编辑角色'
  };
  return titles[props.operateType];
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const model = ref(createDefaultModel());

const { defaultRequiredRule } = useFormRules();

const rules = {
  roleName: defaultRequiredRule
};

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

function createDefaultModel(): Api.Role.UpdateRole {
  return {
    id: '',
    roleName: ''
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value) {
    if (props.rowData) {
      Object.assign(model.value, props.rowData);
    }
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();

  loadingMap.submit = true;
  try {
    const { data: res, error } = isEdit.value
      ? await fetchUpdateRole({ ...model.value })
      : await fetchCreateRole(model.value.roleName);

    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="550">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="角色名称" name="roleName">
          <AInput v-model:value="model.roleName" autocomplete="off" placeholder="角色名称" />
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
.role-select {
  gap: 10px;
}
</style>
