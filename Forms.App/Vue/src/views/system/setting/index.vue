<script setup lang="tsx">
import { reactive } from 'vue';
import EmailTab from './modules/email-tab.vue';
import ConfigTab from './modules/config-tab.vue';

type Model = {
  active: string;
};

const model = reactive<Model>({
  active: 'email'
});
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <ACard class="h-full">
      <ATabs v-model:active-key="model.active">
        <ATabPane key="email">
          <template #tab>
            <p class="tab-title">邮件设置</p>
          </template>
          <EmailTab></EmailTab>
        </ATabPane>
        <ATabPane key="config">
          <template #tab>
            <p class="tab-title">所有设置</p>
          </template>
          <ConfigTab></ConfigTab>
        </ATabPane>
      </ATabs>
    </ACard>
  </div>
</template>

<style lang="scss" scoped>
.tab-title {
  font-size: 14px;
  font-weight: 500;
  letter-spacing: 2.5px;
}
</style>
