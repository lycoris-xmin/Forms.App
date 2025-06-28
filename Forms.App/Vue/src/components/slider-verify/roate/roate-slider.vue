<script setup lang="ts">
  import { onMounted, onUnmounted, reactive, ref } from 'vue';
  import { useThemeStore } from '@/store/modules/theme';

  defineOptions({
    name: 'RoateSlider'
  });

  type Props = {
    image: string;
    correctRotation?: number | null;
    rotationThreshold?: number;
    imageSize?: number | null;
    sliderWidth?: number | null;
    failed?: () => void;
  };

  interface Emits {
    (e: 'success', timeSpent: number): void;
  }

  const themeStore = useThemeStore();

  const props = withDefaults(defineProps<Props>(), {
    correctRotation: null,
    rotationThreshold: 15,
    imageSize: 200,
    sliderWidth: 300,
    failed: void 0
  });

  const emit = defineEmits<Emits>();

  const model = reactive({
    buttonPosition: 0,
    currentRotation: 0,
    isDragging: false,
    isVerified: false,
    startTime: 0,
    buttonRoate: 0,
    timeSpent: 0
  });

  let animationFrameId: number;
  let startX = 0;

  const imageStyle = ref({
    transform: 'rotate(0deg)',
    willChange: 'transform',
    height: `${props.imageSize}px`,
    width: `${props.imageSize}px`
  });

  const onMouseMove = (e: MouseEvent) => handleMove(e.clientX);
  const onTouchMove = (e: TouchEvent) => handleMove(e.touches[0].clientX);

  onMounted(() => {
    initRotation();
    window.addEventListener('mousemove', onMouseMove);
    window.addEventListener('mouseup', endDrag);
    window.addEventListener('touchmove', onTouchMove, { passive: true });
    window.addEventListener('touchend', endDrag);
  });

  onUnmounted(() => {
    cancelAnimationFrame(animationFrameId);
    window.removeEventListener('mousemove', onMouseMove);
    window.removeEventListener('mouseup', endDrag);
    window.removeEventListener('touchmove', onTouchMove);
    window.removeEventListener('touchend', endDrag);
  });

  function initRotation() {
    model.currentRotation = props.correctRotation ?? Math.floor(Math.random() * 360); // 设置为正确的初始值
    updateImageRotation();
  }

  function resetRotation() {
    model.buttonPosition = 0; // 重置滑块位置
    model.buttonRoate = 0;
    model.currentRotation = props.correctRotation ?? Math.floor(Math.random() * 360); // 重置旋转角度
    updateImageRotation();
  }

  function handleMove(clientX: number) {
    if (!model.isDragging || model.isVerified) {
      return;
    }

    const currentX = clientX - startX;
    const maxPosition = props.sliderWidth! - 40;
    model.buttonPosition = Math.max(0, Math.min(currentX, maxPosition));

    cancelAnimationFrame(animationFrameId);
    animationFrameId = requestAnimationFrame(() => {
      // 更新当前旋转角度
      model.buttonRoate = Math.floor((model.buttonPosition / maxPosition) * 360);
      updateImageRotation();
    });
  }

  function updateImageRotation() {
    imageStyle.value.transform = `rotate(${model.currentRotation + model.buttonRoate}deg)`;
  }

  function onMouseDown(e: MouseEvent) {
    model.isDragging = true;
    startX = e.clientX;
    model.startTime = Date.now();
    model.timeSpent = 0;
    document.body.style.userSelect = 'none';
    cancelAnimationFrame(animationFrameId);
  }

  function onTouchStart(e: TouchEvent) {
    model.isDragging = true;
    startX = e.touches[0].clientX;
    model.startTime = Date.now();
    model.timeSpent = 0;
    cancelAnimationFrame(animationFrameId);
  }

  function endDrag() {
    if (!model.isDragging) {
      return;
    }

    model.isDragging = false;
    document.body.style.userSelect = '';

    cancelAnimationFrame(animationFrameId);
    checkVerification();
  }

  function checkVerification() {
    model.timeSpent = getTimeSpent();

    // 计算最终旋转角度
    const finalRotation = (model.currentRotation + model.buttonRoate) % 360;

    // 验证条件
    if (model.buttonPosition > 0 && finalRotation <= props.rotationThreshold) {
      verifySuccess(model.timeSpent);
    } else {
      verifyFail(model.timeSpent);
    }
  }

  function verifySuccess(timeSpent: number) {
    model.isVerified = true;
    emit('success', timeSpent);
  }

  async function verifyFail() {
    if (props.failed && typeof props.failed === 'function') {
      await props.failed();
    } else {
      resetRotation();
    }
  }

  function resetSlider() {
    model.isVerified = false;
    resetRotation();
  }

  function getTimeSpent() {
    return Date.now() - model.startTime;
  }

  defineExpose({
    resetSlider
  });
</script>

<template>
  <div class="slider-container" :class="{ dark: themeStore.darkMode, success: model.isVerified }">
    <div class="image-container">
      <div class="image-wrapper" :style="imageStyle">
        <img :src="image" alt="验证图片" class="rotatable-image" draggable="false" />
      </div>
      <p v-if="model.timeSpent > 0" class="verified-text" :class="{ success: model.isVerified, failed: !model.isVerified }">
        {{ model.isVerified ? `验证成功，耗时${(model.timeSpent / 1000).toFixed(1)}秒` : '验证失败' }}
      </p>
    </div>

    <div class="slider-wrapper" :style="{ width: props.sliderWidth + 'px' }">
      <div class="slider-progress" :style="{ width: model.buttonPosition + 'px' }"></div>

      <div class="slider-button" :class="{ verified: model.isVerified }" :style="{ left: model.buttonPosition + 'px' }" @mousedown="onMouseDown" @touchstart="onTouchStart">
        {{ model.isVerified ? '√' : '>>' }}
      </div>

      <p class="v-center center hit-text flex">旋转图片到正确角度</p>
    </div>
  </div>
</template>

<style scoped lang="scss">
  .slider-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 20px;
    touch-action: manipulation;

    --slider-dark-bg: #f1f3f6;
    --slider-border-color: #e4e4e7;
    --slider-success-bg: #57d188;
    --slider-slider-color: #ffffff;
    --text-color: #575757;

    &.dark {
      --slider-dark-bg: #14161a;
      --slider-border-color: #36363a;
      --slider-slider-color: #2e3033;
    }

    .image-container {
      position: relative;
      border: 2px solid #ccc;
      overflow: hidden;
      border-radius: 50%;

      .verified-text {
        position: absolute;
        left: 0;
        bottom: 10%;
        width: 100%;
        text-align: center;
        opacity: 0.8;
        color: #ffffff;
        padding: 3px;
        display: block;

        &.success {
          background-color: var(--slider-success-bg);
        }

        &.failed {
          background-color: #fb5074;
        }
      }
    }

    .image-wrapper {
      position: relative;
      overflow: hidden;
      backface-visibility: hidden;
      transform: translateZ(0);
      transition: transform 0.1s ease-in-out; /* 确保流畅性 */
      will-change: transform; /* 启用 GPU 加速 */
      transform-origin: center;
      border-radius: 50%;
    }

    .rotatable-image {
      width: 100%;
      height: 100%;
      object-fit: cover;
      pointer-events: none;
    }

    .slider-wrapper {
      position: relative;
      height: var(--slider-height);
      background-color: var(--slider-slider-color);
      border-radius: 5px;
      margin-top: 10px;
      transform: translateZ(0);
      border: 1px solid var(--slider-border-color);
      padding: 2px 0;
    }

    .slider-track {
      position: absolute;
      top: 0;
      left: 0;
      height: 100%;
      width: 100%;
      border-radius: 5px;
    }

    .slider-progress {
      position: absolute;
      top: 0;
      left: 0;
      height: 100%;
      background-color: var(--slider-success-bg);
      border-radius: 5px;
      transition: width 0.1s ease;
    }

    .slider-button {
      position: absolute;
      top: 0;
      width: var(--slider-height);
      height: 100%;
      background-color: var(--slider-slider-color);
      color: #a3a4a5;
      border-radius: 5px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: grab;
      user-select: none;
      z-index: 2;
      font-size: 20px;
      touch-action: none;
      font-weight: bold;
      border: 1px solid var(--slider-border-color);
      z-index: 6;

      &:active {
        cursor: grabbing;
      }

      &.verified {
        color: var(--slider-success-bg);
      }
    }

    .hit-text {
      position: absolute;
      inset: 0;
      color: #575757;
      z-index: 4;
      font-weight: bold;
    }

    .reset-btn {
      &:hover {
        background-color: #e0e0e0;
      }
    }
  }
</style>
