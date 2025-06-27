<script setup lang="ts">
import { defineEmits, defineProps } from 'vue';

defineProps({
  open: {
    type: Boolean,
    required: true
  },
  title: {
    type: String,
    default: '确认修改组网配对等级？'
  },
  content: {
    type: String,
    required: true
  },
  okText: {
    type: String,
    default: '确认'
  },
  cancelText: {
    type: String,
    default: '取消'
  }
});

const emit = defineEmits(['update:open', 'confirm', 'cancel']);

const handleOk = () => {
  emit('confirm');
  emit('update:open', false);
};

const handleCancel = () => {
  emit('cancel');
  emit('update:open', false);
};
</script>

<template>
  <AModal :open="open" :title="title" :ok-text="okText" :cancel-text="cancelText" @ok="handleOk" @cancel="handleCancel">
    <div class="confirm-content">
      <div class="confirm-message">{{ content }}</div>
      <div class="confirm-notice">
        <div class="notice-title">注意事项：</div>
        <ul class="notice-list">
          <li>首次配置将在次日 00:00 生效</li>
          <li>后续变更将在次日 00:00 起计算，5天后生效</li>
          <li>请确保理解层级变更对业务的影响</li>
          <li>变更生效后将影响平台的订单匹配规则</li>
        </ul>
      </div>
    </div>
  </AModal>
</template>

<style scoped>
.confirm-content {
  padding: 8px 0;
}

.confirm-message {
  margin-bottom: 16px;
  color: #1f1f1f;
  font-size: 14px;
  line-height: 1.5;
}

.confirm-notice {
  background-color: #fffbe6;
  border: 1px solid #ffe58f;
  padding: 12px 16px;
  border-radius: 2px;
}

.notice-title {
  color: #1f1f1f;
  font-weight: 500;
  margin-bottom: 8px;
}

.notice-list {
  margin: 0;
  padding-left: 20px;
}

.notice-list li {
  color: #666;
  line-height: 1.8;
  font-size: 14px;
}
</style>
