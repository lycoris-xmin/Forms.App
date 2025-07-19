<script setup lang="ts">
  import Slider from './roate-slider.vue';

  defineOptions({
    name: 'RoateSliderModal'
  });

  type Props = {
    image: string;
    correctRotation?: number | null;
    rotationThreshold?: number;
    imageSize?: number | null;
    sliderWidth?: number | null;
  };

  const visible = defineModel<boolean>('visible', {
    default: false
  });

  const props = withDefaults(defineProps<Props>(), {
    correctRotation: Math.floor(Math.random() * 360),
    rotationThreshold: 15,
    imageSize: 200,
    sliderWidth: 300
  });
</script>

<template>
  <AModal v-model:open="visible" :footer="false" :mask-closable="false" :closable="false" :destroy-on-close="true" :width="450">
    <div class="body">
      <Slider :image="props.image" :correct-rotation="props.correctRotation" :rotation-threshold="props.rotationThreshold" :image-size="props.imageSize" :slider-width="props.sliderWidth"></Slider>
    </div>
  </AModal>
</template>

<style lang="scss" scoped>
  .body {
    position: relative;
    padding-top: 24px;
  }
</style>
