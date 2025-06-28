<script setup lang="ts">
  import { createTextVNode, defineComponent } from 'vue';
  import { App } from 'ant-design-vue';

  defineOptions({
    name: 'AppProvider'
  });

  const ContextHolder = defineComponent({
    name: 'ContextHolder',
    setup() {
      const { message, modal, notification } = App.useApp();

      function register() {
        window.$message = message;
        window.$modal = modal;
        window.$notification = notification;
        window.$confirm = confirm;
      }

      register();

      return () => createTextVNode();
    }
  });

  function confirm({ title, message }: { title?: string; message: string }) {
    return new Promise(resolve => {
      window.$modal?.confirm({
        title: title || '提示',
        cancelText: '取消',
        okText: '确定',
        content: message,
        onOk() {
          resolve(true);
        },
        onCancel() {
          resolve(false);
        }
      });
    });
  }
</script>

<template>
  <App class="h-full">
    <ContextHolder />
    <slot></slot>
  </App>
</template>

<style scoped></style>
