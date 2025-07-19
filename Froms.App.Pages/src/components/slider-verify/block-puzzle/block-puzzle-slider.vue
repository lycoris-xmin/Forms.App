<!-- eslint-disable no-underscore-dangle -->
<script setup lang="ts">
  import { onBeforeUnmount, onMounted, reactive, ref } from 'vue';
  import { createImg, draw, getRandomImg, getRandomNumberByRange, square, sum, throttle } from './util.ts';

  defineOptions({
    name: 'BlockPuzzleSlider'
  });

  interface BlockPosition {
    x: number;
    y: number;
  }

  interface Props {
    l?: number;
    r?: number;
    w?: number;
    h?: number;
    sliderText?: string;
    accuracy?: number;
    show?: boolean;
    imgs?: string[];
    interval?: number;
    block?: BlockPosition;
  }

  interface Emits {
    (e: 'success', duration: number): void; // 改为duration
    (e: 'again'): void;
    (e: 'fail'): void;
    (e: 'refresh'): void;
  }

  const props = withDefaults(defineProps<Props>(), {
    l: 42,
    r: 10,
    w: 310,
    h: 155,
    sliderText: '滑动验证',
    accuracy: 5,
    show: true,
    imgs: () => [],
    interval: 50,
    block: () => ({ x: 0, y: 0 })
  });

  const emit = defineEmits<Emits>();

  // Integrated useSlideAction logic
  const origin = reactive({
    x: 0,
    y: 0
  });

  const success = ref(false);
  const isMouseDown = ref(false);
  const timestamp = ref(0);
  const trail = ref<number[]>([]);

  const start = (e: MouseEvent | TouchEvent) => {
    if (success.value) {
      return;
    }

    if (e instanceof MouseEvent) {
      origin.x = e.clientX;
      origin.y = e.clientY;
    } else {
      origin.x = e.changedTouches[0].pageX;
      origin.y = e.changedTouches[0].pageY;
    }

    isMouseDown.value = true;
    timestamp.value = Date.now();
  };

  const move = (w: number, e: MouseEvent | TouchEvent, cb: (moveX: number) => void) => {
    if (!isMouseDown.value) {
      return false;
    }

    let moveX = 0;
    let moveY = 0;

    if (e instanceof MouseEvent) {
      moveX = e.clientX - origin.x;
      moveY = e.clientY - origin.y;
    } else {
      moveX = e.changedTouches[0].pageX - origin.x;
      moveY = e.changedTouches[0].pageY - origin.y;
    }

    if (moveX < 0 || moveX + 38 >= w) {
      return false;
    }

    cb(moveX);
    trail.value.push(moveY);

    return true;
  };

  const verify = (left: string, blockX: number, accuracy: number) => {
    const arr = trail.value;
    const average = arr.reduce(sum) / arr.length;
    const deviations = arr.map(x => x - average);
    const stddev = Math.sqrt(deviations.map(square).reduce(sum) / arr.length);
    const leftNum = Number.parseInt(left, 10);
    const maxAccuracy = Math.max(1, Math.min(10, accuracy));

    return {
      spliced: Math.abs(leftNum - blockX) <= maxAccuracy,
      TuringTest: average !== stddev
    };
  };

  const end = (e: MouseEvent | TouchEvent, cb: (duration: number) => void): boolean => {
    if (!isMouseDown.value) {
      return false;
    }

    isMouseDown.value = false;

    const moveX = e instanceof MouseEvent ? e.clientX : e.changedTouches[0].pageX;

    if (moveX === origin.x) {
      return false;
    }

    const duration = Date.now() - timestamp.value;
    timestamp.value = duration;

    cb(duration);

    return true;
  };

  // Component logic
  const { imgs, l, r, w, h, accuracy, interval } = props;
  const loadBlock = ref<boolean>(true);
  const blockX = ref<number>(0);
  const blockY = ref<number>(0);
  const containerCls = reactive({
    containerActive: false,
    containerSuccess: false,
    containerFail: false
  });
  const sliderBox = reactive({
    iconCls: 'arrow-right',
    width: '0',
    left: '0'
  });

  const sildeBlock = ref<HTMLCanvasElement | null>(null);
  const blockCtx = ref<CanvasRenderingContext2D | null>(null);
  const canvas = ref<HTMLCanvasElement | null>(null);
  const canvasCtx = ref<CanvasRenderingContext2D | null>(null);
  let img: HTMLImageElement;

  const reset = (): void => {
    success.value = false;
    containerCls.containerActive = false;
    containerCls.containerSuccess = false;
    containerCls.containerFail = false;
    sliderBox.iconCls = 'arrow-right';
    sliderBox.left = '0';
    sliderBox.width = '0';
    if (sildeBlock.value) {
      sildeBlock.value.style.left = '0';
    }
    canvasCtx.value?.clearRect(0, 0, w, h);
    blockCtx.value?.clearRect(0, 0, w, h);
    if (sildeBlock.value) {
      sildeBlock.value.width = w;
    }
    img.src = getRandomImg(imgs);
    trail.value = [];
  };

  const refresh = (): void => {
    reset();
    emit('refresh');
  };

  const moveCb = (moveX: number): void => {
    sliderBox.left = `${moveX}px`;
    const blockLeft = ((w - 40 - 20) / (w - 40)) * moveX;
    if (sildeBlock.value) {
      sildeBlock.value.style.left = `${blockLeft}px`;
    }
    containerCls.containerActive = true;
    sliderBox.width = `${moveX}px`;
  };

  const endCb = (duration: number): void => {
    if (!sildeBlock.value) {
      return;
    }

    const { spliced, TuringTest } = verify(sildeBlock.value.style.left, blockX.value, accuracy);
    if (spliced) {
      if (accuracy === -1) {
        containerCls.containerSuccess = true;
        sliderBox.iconCls = 'success';
        success.value = true;
        emit('success', duration); // 使用duration
        return;
      }
      if (TuringTest) {
        containerCls.containerSuccess = true;
        sliderBox.iconCls = 'success';
        success.value = true;
        emit('success', duration); // 使用duration
      } else {
        containerCls.containerFail = true;
        sliderBox.iconCls = 'fail';
        emit('again');
      }
    } else {
      containerCls.containerFail = true;
      sliderBox.iconCls = 'fail';
      emit('fail');
      setTimeout(() => {
        reset();
      }, 1000);
    }
  };

  const touchMoveEvent = throttle((e: MouseEvent | TouchEvent) => {
    move(w, e, moveCb);
  }, interval);

  const touchEndEvent = (e: MouseEvent | TouchEvent) => {
    end(e, endCb);
  };

  onMounted(() => {
    if (!canvas.value || !sildeBlock.value) return;

    const _canvasCtx = canvas.value.getContext('2d');
    const _blockCtx = sildeBlock.value.getContext('2d');
    canvasCtx.value = _canvasCtx;
    blockCtx.value = _blockCtx;

    img = createImg(imgs, () => {
      loadBlock.value = false;
      const L = l + r * 2 + 3;

      // 图片坐标位置
      if (props.block.x <= 0 && props.block.y <= 0) {
        blockX.value = getRandomNumberByRange(L + 10, w - (L + 10));
        blockY.value = getRandomNumberByRange(10 + r * 2, h - (L + 10));
      } else {
        blockX.value = props.block.x;
        blockY.value = props.block.y;
      }

      if (_canvasCtx && _blockCtx) {
        draw(_canvasCtx, blockX.value, blockY.value, l, r, 'fill');
        draw(_blockCtx, blockX.value, blockY.value, l, r, 'clip');

        _canvasCtx.drawImage(img, 0, 0, w, h);
        _blockCtx.drawImage(img, 0, 0, w, h);
        const _y = blockY.value - r * 2 - 1;
        const imgData = _blockCtx.getImageData(blockX.value, _y, L, L);
        sildeBlock.value.width = L;
        _blockCtx.putImageData(imgData, 0, _y);
      }
    });

    document.addEventListener('mousemove', touchMoveEvent);
    document.addEventListener('mouseup', touchEndEvent);
  });

  onBeforeUnmount(() => {
    document.removeEventListener('mousemove', touchMoveEvent);
    document.removeEventListener('mouseup', touchEndEvent);
  });
</script>

<template>
  <!-- Template remains exactly the same as before -->
  <div id="slideVerify" class="slide-verify" :style="{ width: w + 'px' }" onselectstart="return false;">
    <!-- 图片加载遮蔽罩 -->
    <div v-if="loadBlock" class="slide-verify-loading"></div>
    <canvas ref="canvas" :width="w" :height="h"></canvas>
    <div v-if="show" class="slide-verify-refresh-icon" @click="refresh">
      <i class="iconfont icon-refresh"></i>
    </div>
    <canvas ref="sildeBlock" :width="w" :height="h" class="slide-verify-block"></canvas>
    <!-- container -->
    <div
      class="slide-verify-slider"
      :class="{
        'container-active': containerCls.containerActive,
        'container-success': containerCls.containerSuccess,
        'container-fail': containerCls.containerFail
      }"
    >
      <div class="slide-verify-slider-mask" :style="{ width: sliderBox.width }">
        <!-- slider -->
        <div class="slide-verify-slider-mask-item" :style="{ left: sliderBox.left }" @mousedown="start" @touchstart="start" @touchmove="touchMoveEvent" @touchend="touchEndEvent">
          <i class="slide-verify-slider-mask-item-icon iconfont" :class="[`icon-${sliderBox.iconCls}`]"></i>
        </div>
      </div>
      <span class="slide-verify-slider-text">{{ sliderText }}</span>
    </div>
  </div>
</template>

<style lang="scss" scoped>
  /* Styles remain exactly the same as before */
  @use '@/assets/iconfont.css';

  @mixin position() {
    position: absolute;
    left: 0;
    top: 0;
  }

  .slide-verify {
    position: relative;

    &-loading {
      @include position();
      right: 0;
      bottom: 0;
      background: rgba(255, 255, 255, 0.9);
      z-index: 999;
      animation: loading 1.5s infinite;
    }

    &-block {
      @include position();
    }

    &-refresh-icon {
      position: absolute;
      right: 3px;
      top: -3px;
      cursor: pointer;

      .iconfont {
        font-size: 26px;
        color: #fff;
      }
    }

    &-slider {
      position: relative;
      text-align: center;
      width: 100%;
      height: var(--slider-height);
      line-height: var(--slider-height);
      margin-top: 15px;
      background: #f7f9fa;
      color: #45494c;
      border: 1px solid #e4e7eb;

      &-mask {
        @include position();
        height: var(--slider-height);
        border: 0 solid #1991fa;
        background: #d1e9fe;

        &-item {
          @include position();
          width: var(--slider-height);
          height: var(--slider-height);
          background: #fff;
          box-shadow: 0 0 3px rgba(0, 0, 0, 0.3);
          cursor: pointer;
          transition: background 0.2s linear;
          display: flex;
          align-items: center;
          justify-content: center;

          &:hover {
            background: #1991fa;

            .iconfont {
              color: #fff;
            }
          }

          &-icon {
            line-height: 1;
            font-size: 30px;
            color: #303030;
          }
        }
      }
    }

    .container-active &-slider-mask {
      height: 38px;
      border-width: 1px;

      &-item {
        height: 38px;
        top: -1px;
        border: 1px solid #1991fa;
      }
    }

    .container-success &-slider-mask {
      height: 38px;
      border: 1px solid #52ccba;
      background-color: #d2f4ef;

      &-item {
        height: 38px;
        top: -1px;
        border: 1px solid #52ccba;
        background-color: #52ccba !important;
      }

      .iconfont {
        color: #fff;
      }
    }

    .container-fail &-slider-mask {
      height: 38px;
      border: 1px solid #f57a7a;
      background-color: #fce1e1;

      &-item {
        height: 38px;
        top: -1px;
        border: 1px solid #f57a7a;
        background-color: #f57a7a !important;
      }

      .iconfont {
        color: #fff;
      }
    }

    .container-active &-slider-text,
    .container-success &-slider-text,
    .container-fail &-slider-text {
      display: none;
    }

    @keyframes loading {
      0% {
        opacity: 0.7;
      }
      100% {
        opacity: 1;
      }
    }
  }
</style>
