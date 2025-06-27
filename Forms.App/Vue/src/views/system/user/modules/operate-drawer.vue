<script setup lang="ts">
import { computed, reactive, ref, watch, withDefaults } from 'vue';
import { RetweetOutlined } from '@ant-design/icons-vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateSystemUser, fetchUpdateSystemUser } from '@/service/api';
import { REG_PWD } from '@/constants/reg';
import type { EnumMap } from '../../shared/shared';

defineOptions({
  name: 'SystemUserOperateDrawer'
});

interface Props {
  operateType?: AntDesign.TableOperateType;
  rowData?: Api.Product.TaobaoData | null;
  status?: Array<EnumMap>;
  role?: Array<EnumMap>;
  refreshRole?: () => void;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  roleRefresh: boolean;
  submit: boolean;
};

type Model = Pick<Api.User.UserData, 'nickName' | 'password' | 'email' | 'roleId' | 'status'>;

type RuleKey = Extract<keyof Model, 'nickName' | 'password' | 'email' | 'roleId' | 'status'>;

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  roleRefresh: false,
  submit: false
});

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null,
  status: () => [],
  role: () => [],
  refreshRole: void 0
});

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增用户',
    edit: '编辑用户'
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

    roleId: [defaultRequiredRule],
    status: [defaultRequiredRule]
  };
});

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

function createDefaultModel(): Model {
  return {
    email: '',
    password: '',
    nickName: '',
    roleId: '',
    status: 0
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value) {
    if (props.rowData) {
      Object.assign(model.value, props.rowData);
    }
  } else if (props.role.length) {
    model.value.roleId = props.role[0].value;
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
      ? await fetchUpdateSystemUser({ ...model.value })
      : await fetchCreateSystemUser({ ...model.value });

    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}

async function refreshRoleHandler() {
  if (!props.refreshRole) {
    return;
  }

  loadingMap.roleRefresh = true;
  try {
    await props.refreshRole();
  } finally {
    loadingMap.roleRefresh = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="550">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="邮箱" name="email">
          <AInput v-model:value="model.email" autocomplete="off" placeholder="请输入邮箱" />
        </AFormItem>

        <AFormItem label="密码" name="password">
          <AInput v-model:value="model.password" autocomplete="off" placeholder="请输入密码" type="password" />
        </AFormItem>

        <AFormItem label="昵称" name="nickName">
          <AInput v-model:value="model.nickName" autocomplete="off" placeholder="请输入昵称" />
        </AFormItem>

        <AFormItem label="角色" name="roleId">
          <div class="v-center center flex gap-8px">
            <ASelect v-model:value="model.roleId" :options="props.role"></ASelect>
            <AButton class="v-center flex" :loading="loadingMap.roleRefresh" @click="refreshRoleHandler">
              <RetweetOutlined />
            </AButton>
          </div>
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

<style scoped lang="scss"></style>
