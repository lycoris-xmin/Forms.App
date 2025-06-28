<script setup lang="ts">
  import { nextTick, onMounted, onUnmounted, ref, watch } from 'vue';
  import { sliderDefaultApi } from '@/service/api';
  import { useThemeStore } from '@/store/modules/theme';

  type Porps = {
    path?: string;
  };

  defineOptions({
    name: 'SliderDefault'
  });

  const themeStore = useThemeStore();

  const isVerified = defineModel<boolean>('verified', {
    default: false
  });

  const isDragging = ref(false); // 是否正在拖动
  const handleLeft = ref(0); // 滑块位置
  const sliderRef = ref<HTMLElement | null>(null);
  const sliderHandleRef = ref<HTMLElement | null>(null);

  let sliderWidth = 0;
  let sliderHandleWidth = 0;

  const props = withDefaults(defineProps<Porps>(), {
    path: '/'
  });

  watch(isVerified, value => {
    if (!value) {
      if (handleLeft.value !== 0) {
        handleLeft.value = 0;
      }
    }
  });

  const startDrag = (event: MouseEvent) => {
    if (isVerified.value) {
      return;
    }

    isDragging.value = true;
    const startX = event.clientX;
    const initialLeft = handleLeft.value;

    const onMouseMove = (moveEvent: MouseEvent) => {
      if (!isDragging.value) {
        return;
      }

      requestAnimationFrame(() => {
        const deltaX = moveEvent.clientX - startX;
        handleLeft.value = Math.max(0, Math.min(initialLeft + deltaX, sliderWidth - sliderHandleWidth));
      });
    };

    // 365px
    const onMouseUp = () => {
      isDragging.value = false;

      if (handleLeft.value >= sliderWidth - sliderHandleWidth) {
        isVerified.value = true;
        handleLeft.value = sliderWidth - sliderHandleWidth;
      } else {
        handleLeft.value = 0; // 滑回原位
      }

      document.removeEventListener('mousemove', onMouseMove);
      document.removeEventListener('mouseup', onMouseUp);
    };

    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
  };

  const updateSliderWidth = () => {
    if (sliderRef.value) {
      sliderWidth = sliderRef.value.offsetWidth;
    }

    if (sliderHandleRef.value) {
      sliderHandleWidth = sliderHandleRef.value.getBoundingClientRect().width;
    }
  };

  onMounted(() => {
    getSliderDefault();

    nextTick(() => {
      updateSliderWidth();
      window.addEventListener('resize', updateSliderWidth);
    });
  });

  onUnmounted(() => {
    window.removeEventListener('resize', updateSliderWidth);
  });

  async function getSliderDefault() {
    const { data: res, error } = await sliderDefaultApi({ path: props.path });
    if (!error && res && res.code === 0) {
      //
    }
  }
</script>

<template>
  <div ref="sliderRef" class="slider-container" :class="{ dark: themeStore.darkMode, success: isVerified }">
    <div class="slider-track" :class="{ success: isVerified }">
      <!-- 滑动填充区域 -->
      <div class="slider-fill" :style="{ width: handleLeft + 'px' }"></div>

      <!-- 滑块按钮 -->
      <div ref="sliderHandleRef" class="slider-handle" :style="{ transform: `translateX(${handleLeft}px)` }" @mousedown="startDrag">
        {{ isVerified ? '√' : '>>' }}
      </div>

      <!-- 文本提示 -->
      <span class="slider-text">
        {{ isVerified ? '验证通过' : '请按住滑块拖动' }}
      </span>
    </div>
  </div>
</template>

<style scoped lang="scss">
  .slider-container {
    width: 100%; // 自适应父级宽度
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    user-select: none;
    --slider-dark-bg: #f1f3f6;
    --slider-border-color: #e4e4e7;
    --slider-success-bg: #57d188;
    --slider-slider-color: #ffffff;
    --text-color: #575757;

    &.dark {
      --slider-dark-bg: #14161a;
      --slider-border-color: #36363a;
      --slider-slider-color: #2e3033;

      .slider-handle {
        color: white;
      }
    }

    &.success {
      .slider-handle {
        color: var(--slider-success-bg);
      }
    }

    .slider-track {
      width: 100%; // 自适应父级宽度
      height: var(--slider-height);
      background: var(--slider-dark-bg);
      border-radius: 5px;
      display: flex;
      align-items: center;
      position: relative;
      overflow: hidden;
      border: 1px solid var(--slider-border-color);
      transition: background 0.3s ease;
    }

    .slider-fill {
      height: 100%;
      background: var(--slider-success-bg);
      position: absolute;
      top: 0;
      left: 0;
      transition: width 0.1s ease;
    }

    .slider-handle {
      width: var(--slider-height);
      height: 100%;
      background: var(--slider-slider-color);
      position: absolute;
      left: 0;
      cursor: grab;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: bold;
      color: #a3a4a5;
      border-radius: 5px;
      transition: transform 0.1s ease;
      z-index: 10;
      border: 1px solid var(--slider-border-color);

      &:active {
        cursor: grabbing;
      }
    }

    .slider-text {
      flex: 1;
      color: var(--text-color);
      text-align: center;
      position: absolute;
      width: 100%;
      z-index: 5;
      transition: opacity 0.3s ease;
      pointer-events: none;
      font-weight: bold;
    }
  }
</style>
