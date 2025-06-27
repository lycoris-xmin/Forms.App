<script setup lang="ts">
import { reactive, ref, withDefaults } from 'vue';
import { UpOutlined } from '@ant-design/icons-vue';

defineOptions({
  name: 'DeviceCommandListSearch'
});

type Props = {
  showHeader?: boolean;
  title?: stirng;
  bordered?: boolean;
};

type Model = {
  collapse: boolean;
  height: number;
  padding: number;
};

const cardRef = ref<HTMLElement | null>(null);
const model = reactive<Model>({
  collapse: false,
  height: 0,
  padding: 0
});

const props = withDefaults(defineProps<Props>(), {
  showHeader: false,
  title: '搜索',
  bordered: false
});

function collapseHandler() {
  const el = cardRef.value?.$el?.querySelector('.ant-card-body');
  if (el) {
    if (!model.collapse) {
      if (model.height === 0) {
        model.height = el?.getBoundingClientRect().height;
        model.padding = Number.parseInt(window.getComputedStyle(el).paddingTop.replace('px', ''), 10);
        el.style.height = `${model.height}px`;
      }

      el.style.height = '0px';
      el.style.padding = '0px';
    } else {
      el.style.height = `${model.height}px`;
      el.style.padding = `${model.padding}px`;
    }

    model.collapse = !model.collapse;
  }
}
</script>

<template>
  <ACard
    ref="cardRef"
    :title="props.showHeader ? props.title : ''"
    :bordered="props.bordered"
    class="card-filer card-wrapper"
  >
    <template v-if="props.showHeader" #extra>
      <span class="collapse" :class="{ rotate: model.collapse }" @click="collapseHandler">
        <UpOutlined />
      </span>
    </template>

    <slot></slot>
  </ACard>
</template>

<style lang="scss" scoped>
.collapse {
  cursor: pointer;

  > span {
    transition: 0.3s ease-in-out;
  }

  &.rotate {
    > span {
      transform: rotate(180deg);
    }
  }
}

.card-filer {
  :deep(.ant-card-body) {
    transition: all 0.3s ease-in-out;
    overflow: hidden;
  }
}
</style>
