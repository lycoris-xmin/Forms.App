<script setup lang="ts">
import { nextTick, reactive, watch } from 'vue';
import { useAntdForm } from '@/hooks/common/form';
import { fetchUpdateFriend } from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';

defineOptions({
  name: 'FriendSettingDrawer'
});

type Model = Api.Friend.UpdateFriend & {
  id: string;
  friendAccount?: string | null;
  friendNikeName?: string | null;
  status: number;
  loading: boolean;
  submitLoading: boolean;
};

type Props = {
  rowData: Api.Friend.FriendData;
};

type Emits = {
  (e: 'submitted'): void;
};

const emit = defineEmits<Emits>();

const visible = defineModel<boolean>('visible', { default: false });

const props = defineProps<Props>();

const model = reactive<Model>({
  id: '',
  friendAccount: '',
  friendNickName: '',
  loading: true,
  submitLoading: false
});

const { formRef, validate, resetFields } = useAntdForm();

watch(visible, () => {
  if (visible.value) {
    resetFields();
    nextTick(async () => {
      initModelHandler();
    });
  }
});

function initModelHandler() {
  try {
    Object.keys(model).forEach(key => {
      if (Object.keys(props.rowData).includes(key)) {
        model[key] = props.rowData[key];
      }
    });
  } finally {
    model.loading = false;
  }
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  await validate();
  model.submitLoading = true;
  try {
    const { data: res, error } = await fetchUpdateFriend({ ...model });
    if (!error && res && res.code === 0) {
      window.$message?.success('更新成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    model.submitLoading = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="信息修改" :width="500">
    <div class="body" :class="{ loading: model.loading }">
      <AForm
        ref="formRef"
        layout="vertical"
        :model="model"
        :label-col="{
          span: 5,
          md: 7
        }"
      >
        <AFormItem label="好友备注" name="friendNickName">
          <AInput v-model:value="model.friendNickName" placeholder="请输入好友备注" :maxlength="30" show-count />
        </AFormItem>
      </AForm>

      <LoadingWrap :loading="model.loading"></LoadingWrap>
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

.input-number {
  width: 100%;
}

.body {
  position: relative;

  &.loading {
    max-height: 70vh;
    overflow: hidden;
  }
}
</style>
