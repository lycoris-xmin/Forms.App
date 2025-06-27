<script setup lang="ts">
import { computed, reactive, ref, watch, withDefaults } from 'vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateTenantUser, fetchUpdateTenantUser } from '@/service/api';
import { REG_PWD } from '@/constants/reg';

defineOptions({
  name: 'TenantUserOperateDrawer'
});

interface Props {
  operateType?: AntDesign.TableOperateType;
  rowData?: Api.Product.TaobaoData | null;
  status?: Array;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  roleRefresh: boolean;
  submit: boolean;
};

type Model = Pick<Api.Tenant.TenantData, 'nickName' | 'password' | 'email' | 'status'> & {
  isTenantAdmin: number;
};

type RuleKey = Extract<keyof Model, 'nickName' | 'password' | 'email' | 'status'>;

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null,
  status: () => []
});

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增员工',
    edit: '编辑员工'
  };
  return titles[props.operateType];
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const model = ref(createDefaultModel());

const rules = computed<Record<RuleKey, App.Global.FormRule[]>>(() => {
  const { formRules, defaultRequiredRule } = useFormRules();

  return {
    email: formRules.email,
    nickName: formRules.userName,
    password: isEdit.value
      ? [
          {
            validator(_, value) {
              if (value && value.trim() && !REG_PWD.test(value)) {
                return Promise.reject(new Error('密码格式不正确，6-18位字符，英文字母、数字及@#_!'));
              }

              return Promise.resolve();
            },
            trigger: 'blur'
          }
        ]
      : formRules.pwd,
    status: defaultRequiredRule,
    settingAmount: defaultRequiredRule,
    isTenantAdmin: defaultRequiredRule
  };
});

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

const roleOptions = [
  {
    value: 1,
    label: '管理员'
  },
  {
    value: 0,
    label: '员工'
  }
];

function createDefaultModel(): Model {
  return {
    email: '',
    password: '',
    nickName: '',
    status: 0,
    settingAmount: 0,
    isTenantAdmin: 0
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value) {
    if (props.rowData) {
      Object.assign(model.value, props.rowData);
      model.value.isTenantAdmin = props.rowData.isTenantAdmin ? 1 : 0;
    }
  }
}

function emailChangeHandler() {
  if (model.value.email) {
    model.value.email = model.value.email?.toLowerCase();
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
      ? await fetchUpdateTenantUser({ ...model.value })
      : await fetchCreateTenantUser({ ...model.value });

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
        <AFormItem label="邮箱" name="email">
          <AInput v-model:value="model.email" autocomplete="off" placeholder="请输入邮箱" @blur="emailChangeHandler" />
        </AFormItem>

        <AFormItem label="密码" name="password">
          <AInput v-model:value="model.password" autocomplete="off" placeholder="请输入密码" type="password" />
        </AFormItem>

        <AFormItem label="昵称" name="nickName">
          <AInput v-model:value="model.nickName" autocomplete="off" placeholder="请输入昵称" />
        </AFormItem>

        <AFormItem label="权限" name="isTenantAdmin">
          <ASelect v-model:value="model.isTenantAdmin" placeholder="请选择" :options="roleOptions"></ASelect>
        </AFormItem>

        <AFormItem v-if="isEdit" label="状态" name="status">
          <ASelect v-model:value="model.status" :options="props.status"></ASelect>
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
.input-number {
  width: 100%;
}
</style>
