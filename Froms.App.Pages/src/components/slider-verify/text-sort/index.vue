<script setup lang="ts">
  import { ref } from 'vue';
  import Slider from './text-sort-slider.vue';

  defineOptions({
    name: 'TextSortSliderModal'
  });

  interface Word {
    text: string;
    x: number;
    y: number;
  }

  type Props = {
    words: Word[];
    image: string;
  };

  const visible = defineModel<boolean>('visible', {
    default: false
  });

  const sliderRef = ref<HTMLElement | null>(null);

  const props = defineProps<Props>();

  const loading = ref<boolean>(false);

  function submitHandler() {
    const result = sliderRef.value.confirmSelection();
    if (result) {
      window.$message?.success('验证成功');
    } else {
      window.$message?.success('验证失败');
    }
  }
</script>

<template>
  <AModal v-model:open="visible" title="请完成安全验证" :mask-closable="false" :closable="false" :destroy-on-close="true" :width="450">
    <div class="body v-center center flex">
      <Slider ref="sliderRef" :words="props.words" :sequence="props.sequence" :image="props.image"></Slider>
    </div>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="() => (visible = false)">取消</AButton>
        <AButton type="primary" :loading="loading" @click="submitHandler">确定</AButton>
      </ASpace>
    </template>
  </AModal>
</template>

<style lang="scss" scoped>
  .body {
    position: relative;
    padding-top: 24px;
  }
</style>
