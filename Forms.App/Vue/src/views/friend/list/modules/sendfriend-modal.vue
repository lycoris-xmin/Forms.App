<script setup lang="ts">
import { ref } from 'vue';
import { SearchOutlined } from '@ant-design/icons-vue';
import { useAntdForm } from '@/hooks/common/form';
import { fetchGetCheckFriend, fetchSendFriend } from '@/service/api';

type Model = {
  friendId: string;
  friendAccount: string;
  reasons: string;
};

type Emits = {
  (e: 'submitted'): void;
};

const inputRef = ref<HTMLElement | null>(null);

const visible = defineModel<boolean>('visible', {
  default: false
});

const model = ref(createDefaultModel());

const { formRef, validate } = useAntdForm();

const emit = defineEmits<Emits>();

const rules = {
  friendAccount: [{ required: true, message: '请输入好友账号' }]
};

function createDefaultModel(): Model {
  return {
    friendId: '',
    friendAccount: '',
    reasons: '',
    searchedFriend: null, // 存储查找结果
    alert: false
  };
}

const next = async () => {
  await validate();

  try {
    const { data: res, error } = await fetchGetCheckFriend(model.value.friendAccount);

    if (!error && res && res.code === 0 && res.data) {
      model.value.searchedFriend = res.data;
      model.value.alert = false;
    } else {
      model.value.searchedFriend = null;
      model.value.alert = true;
    }
  } catch {
    model.value.searchedFriend = null;
    model.value.alert = true;
  }
};

const submittedHandler = async () => {
  const json: Api.Friend.createRequest = {
    friendId: model.value.searchedFriend.id,
    friendAccount: model.value.searchedFriend.email,
    reasons: model.value.reasons
  };

  const { data: res, error } = await fetchSendFriend(json);
  if (!error && res && res.code === 0) {
    window.$message?.success('申请成功');
    emit('submitted');
    visible.value = false;
    model.value = createDefaultModel();
  }
};
</script>

<template>
  <AModal v-model:open="visible" title="搜索好友" :mask-closable="false" width="450px" :footer="null">
    <div>
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <div class="steps-content">
          <AFormItem label="好友账号" name="friendAccount">
            <ASpace class="select-action">
              <AInput ref="inputRef" v-model:value="model.friendAccount" placeholder="好友账号" />
              <AButton ghost size="small" type="primary" @click="next">
                <template #icon>
                  <SearchOutlined />
                </template>
                搜索
              </AButton>
            </ASpace>
          </AFormItem>
        </div>
        <div v-if="model.searchedFriend" class="steps-content">
          <AFormItem label="搜索结果" name="groupName">
            <div class="friend-info">
              <p>账号：{{ model.searchedFriend.email }}</p>
              <p>昵称：{{ model.searchedFriend.nickName }}</p>
              <p class="mb-6px">备注：{{ model.searchedFriend.remark || '无' }}</p>
              <AInput ref="inputRef" v-model:value="model.reasons" placeholder="申请理由" />
            </div>
          </AFormItem>
        </div>
        <AAlert v-if="model.alert" type="warning" message="未找到好友信息" />
      </AForm>

      <div class="mt-12px text-right">
        <AButton type="primary" :disabled="!model.searchedFriend" @click="submittedHandler">添加好友</AButton>
      </div>
    </div>
  </AModal>
</template>

<style scoped lang="scss">
.count {
  width: 100%;
}

.select-action {
  padding: 4px 8px;
  display: flex;
  align-items: center;
  width: 100%;

  :deep(.ant-space-item) {
    &:first-child {
      flex: 1;
    }

    &:last-child {
      flex-shrink: 0;
    }

    .ant-btn {
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }
}
</style>
