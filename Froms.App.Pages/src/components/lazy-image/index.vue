<script setup lang="tsx">
  import { computed, ref } from 'vue';
  import { LoadingOutlined } from '@ant-design/icons-vue';
  import { imgFallback } from '@/data/empty.json';

  interface Props {
    src: string;
    errorPlaceholder?: string;
    placeholder?: string;
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

  const props = defineProps<Props>();

  const hasError = ref(false);
  const isLoaded = ref(false);

  // 兜底错误图地址
  const finalErrorPlaceholder = computed(() => props.errorPlaceholder || imgFallback);

  // 剔除已知字段后传给 a-image 的其他属性
  const imageProps = computed(() => {
    const rest = { ...props };
    delete rest.errorPlaceholder;
    delete rest.placeholder;

    return {
      ...rest,
      loading: 'lazy'
    };
  });

  const onError = () => {
    hasError.value = true;
  };

  const onLoad = () => {
    isLoaded.value = true;
  };

  // 若未传入 placeholder 图片地址则使用，否则为默认 loading UI
  const placeholderNode = computed(() => {
    if (props.placeholder) {
      const img = document.createElement('img');
      img.src = props.placeholder;
      img.style.width = '100%';
      img.style.opacity = '0.5';
      return () => img;
    }
    return undefined;
  });
</script>

<template>
  <AImage v-bind="imageProps" :src="hasError ? finalErrorPlaceholder : props.src" :placeholder="placeholderNode" @error="onError" @load="onLoad">
    <template #placeholder>
      <slot name="placeholder">
        <div class="lazy-image-placeholder">
          <LoadingOutlined spin />
        </div>
      </slot>
    </template>
  </AImage>
</template>

<style scoped>
  .lazy-image-placeholder {
    display: flex;
    align-items: center;
    justify-content: center;
    min-height: 80px;
    color: #999;
  }
</style>
