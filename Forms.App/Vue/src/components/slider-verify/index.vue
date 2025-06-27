<script lang="ts" setup>
import { reactive, watch, withDefaults } from 'vue';
import { fetchSlider } from '@/service/api';
import LoadingWrap from '../loading/index.vue';
import Slider from './slider.vue';

type SliderPorpsBlock = {
  x: number;
  y: number;
  validate: string;
  loading: boolean;
};

type SliderPorps = {
  l?: number;
  r?: number;
  w?: number;
  h?: number;
  sliderText?: string;
  accuracy?: number;
  show?: boolean;
  imgs?: Array<string>;
  interval?: number;
  path?: string;
};

type Emits = {
  success: () => void;
};

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = withDefaults(defineProps<SliderPorps>(), {
  l: 42,
  r: 10,
  w: 310,
  h: 155,
  sliderText: '滑动验证',
  accuracy: 5,
  show: true,
  imgs: () => {
    const maps = [];
    for (let i = 1; i <= 30; i += 1) {
      maps.push(`/slider/${i}.jpg`);
    }

    return maps;
  },
  interval: 50,

  path: '/'
});

const block = reactive<SliderPorpsBlock>({
  x: 0,
  y: 0,
  validate: '',
  loading: true
});

watch(visible, async value => {
  if (value) {
    await getSlider();
  }
});

const emit = defineEmits<Emits>();

async function refreshHadnler() {
  await getSlider();
}

async function getSlider() {
  block.loading = true;
  try {
    const { data: res, error } = await fetchSlider({
      width: props.w,
      height: props.h,
      l: props.l,
      r: props.r,
      path: props.path,
      timespan: new Date().getTime()
    });

    if (!error && res && res.code === 0) {
      block.x = res.data.x;
      block.y = res.data.y;
      block.validate = res.data.validate;
    }
  } finally {
    setTimeout(() => {
      block.loading = false;
    }, 500);
  }
}

function successHandler() {
  emit('success', block.validate);
}

function againHandler() {
  setTimeout(() => {
    refreshHadnler();
  }, 1000);
}
</script>

<template>
  <AModal
    v-model:open="visible"
    :width="370"
    :footer="false"
    :mask-closable="false"
    :closable="false"
    :destroy-on-close="true"
  >
    <div class="body">
      <div class="p-5px">
        <Slider
          :l="props.l"
          :r="props.r"
          :w="props.w"
          :h="props.h"
          :slider-text="props.sliderText"
          :accuracy="props.accuracy"
          :show="props.show"
          :imgs="props.imgs"
          :interval="props.interval"
          :block="block"
          @again="againHandler"
          @fail="againHandler"
          @success="successHandler"
          @refresh="refreshHadnler"
        ></Slider>
      </div>

      <LoadingWrap :loading="block.loading" bg-color="#fff"></LoadingWrap>
    </div>
  </AModal>
</template>

<style lang="scss" scoped>
.body {
  position: relative;
}
</style>
