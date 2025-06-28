<script setup lang="ts">
  import { computed } from 'vue';
  import LazyImage from '@/components/lazy-image/index.vue';
  import { APP } from '@/data/app.json';
  import { imgFallback } from '@/data/empty.json';

  interface Props {
    src?: string;
    errorPlaceholder?: string;
    placeholder?: string;
    lazy?: boolean;
    // eslint-disable-next-line vue/require-default-prop, vue/no-unused-properties
    alt?: string;
    // eslint-disable-next-line vue/require-default-prop, vue/no-unused-properties
    width?: number | string;
    // eslint-disable-next-line vue/require-default-prop, vue/no-unused-properties
    height?: number | string;
    // eslint-disable-next-line vue/no-unused-properties
    preview?: boolean | object;
    // eslint-disable-next-line vue/require-default-prop, vue/no-unused-properties
    fallback?: string;
    // 允许更多透传属性
    [key: string]: any;
  }

  const props = withDefaults(defineProps<Props>(), {
    src: '',
    errorPlaceholder: '',
    placeholder: '',
    lazy: false,
    preview: false
  });

  // 构造用于传给 a-image 的 props
  const imageProps = computed(() => {
    const rest: Props = { ...props };

    // 拼接 src（去除 APP 后的 / 和 props.src 前的 /）
    rest.src = `//${APP.QINIU_DOMAIN.replace(/\/+$/, '')}/${props.src.replace(/^\/+/, '')}`;

    // 默认错误图
    if (!props.errorPlaceholder) {
      rest.fallback = imgFallback;
    }

    // placeholder（antd-image 使用 base64、URL 或 node）
    if (props.placeholder) {
      rest.placeholder = props.placeholder;
    }

    // loading 属性仅懒加载时需要（用于 <a-image>）
    if (props.lazy) {
      rest.loading = 'lazy';
    }

    return rest;
  });
</script>

<template>
  <div class="img-body">
    <LazyImage v-if="props.lazy" v-bind="imageProps" />
    <AImage v-else v-bind="imageProps" />
  </div>
</template>

<style scoped lang="scss">
  .img-body {
    display: inline-block;
  }
</style>
