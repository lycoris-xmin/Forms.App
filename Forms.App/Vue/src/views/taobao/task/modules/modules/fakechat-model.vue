<script setup lang="ts">
import { ref, watch } from 'vue';
import { DeleteOutlined, PlusOutlined, QuestionCircleOutlined } from '@ant-design/icons-vue';
import { Popover as APopover } from 'ant-design-vue';

const props = defineProps<{
  chatMessages: string[];
}>();

const emit = defineEmits<{
  (e: 'update:chat-messages', val: string[]): void;
}>();

const localMessages = ref([...props.chatMessages]);

watch(
  () => props.chatMessages,
  newVal => {
    localMessages.value = [...newVal];
  }
);

function handleStrictInput(e: Event, index: number) {
  const target = e.target as HTMLInputElement;
  const newVal = target.value;

  if (newVal.length > 80) {
    target.value = localMessages.value[index];

    target.dispatchEvent(new Event('input'));

    window.$message?.error('每条聊天信息最多不能超过80个字');
    return;
  }

  localMessages.value[index] = newVal;
  updateMessages();
}

function updateMessages() {
  emit('update:chat-messages', [...localMessages.value]);
}

function addChatMessage() {
  if (localMessages.value.length >= 10) {
    window.$message?.error('假聊语句最多允许添加十句');
    return;
  }
  localMessages.value.push('');
  updateMessages();
}

function deleteChatMessage(index: number) {
  localMessages.value.splice(index, 1);
  updateMessages();
}
</script>

<template>
  <div class="chat-panel">
    <div class="top-bar">
      <p class="section-title">
        聊天互动
        <APopover placement="top" trigger="hover">
          <template #content>
            <p class="popover-text">假聊语句最多允许添加十句每句最多不超过80个字。</p>
          </template>
          <QuestionCircleOutlined class="info-icon" @click.stop.prevent />
        </APopover>
      </p>
      <AButton size="small" type="primary" ghost class="add-btn" @click="addChatMessage">
        <template #icon><PlusOutlined /></template>
        添加聊天信息
      </AButton>
    </div>

    <div class="message-list">
      <div v-for="(message, index) in localMessages" :key="index" class="message-item">
        <div class="message-content">
          <AInput
            :value="localMessages[index]"
            placeholder="请输入聊天信息"
            class="wide-input"
            @change="updateMessages"
            @input="e => handleStrictInput(e, index)"
          />
          <AButton danger size="small" type="text" class="delete-btn" @click="deleteChatMessage(index)">
            <DeleteOutlined />
          </AButton>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.chat-panel {
  margin-bottom: 24px;
  border-radius: 6px;
}

.top-bar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}

.info-icon {
  font-size: 14px;
  cursor: pointer;
}

.add-btn {
  display: inline-flex;
  align-items: center;
}

.message-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.message-content {
  display: flex;
  align-items: center;
  width: 100%;
  gap: 8px;
}

.popover-text {
  max-width: 240px;
  margin: 0;
}
</style>
