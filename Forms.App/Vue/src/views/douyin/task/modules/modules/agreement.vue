<script setup lang="ts">
import { reactive } from 'vue';

defineOptions({
  name: 'PlanTaskAgreement'
});

const model = reactive({
  dayLimit: 2,
  dayLimitChecked: true,
  freeTimeBehaviorChecked: true,
  aiScrollChecked: true,
  common: true
});

const enableAlways = key => {
  model[key] = true;
};
</script>

<template>
  <AFormItem label="友情提示">
    <div>
      <div class="line">
        <p>
          <ACheckbox v-model:checked="model.dayLimitChecked" @change="() => enableAlways('dayLimitChecked')">
            组网限制日均{{ model.dayLimit }}单
          </ACheckbox>
        </p>

        <p>
          <ACheckbox
            v-model:checked="model.freeTimeBehaviorChecked"
            @change="() => enableAlways('freeTimeBehaviorChecked')"
          >
            闲暇时光做动态浏览
          </ACheckbox>
        </p>
      </div>

      <div class="line">
        <p>
          <ACheckbox v-model:checked="model.aiScrollChecked" @change="() => enableAlways('aiScrollChecked')">
            AI 判断手指不同滑动
          </ACheckbox>
        </p>

        <p>
          <ACheckbox v-model:checked="model.common" @change="() => enableAlways('common')">
            请保持手机常亮，它要每天学习不同使用手机的习惯
          </ACheckbox>
        </p>
      </div>
    </div>
  </AFormItem>
</template>

<style scoped lang="scss">
.line {
  width: 100%;
  margin-bottom: 14px;
  display: grid;
  grid-template-columns: 1fr 2fr;

  &.flex {
    p {
      flex: 1;
    }
  }

  :deep(label) {
    font-size: 16px !important;

    span {
      &:last-child {
        letter-spacing: 1.5px;
      }
    }
  }
}
</style>
