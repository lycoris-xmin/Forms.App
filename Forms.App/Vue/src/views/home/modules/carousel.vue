<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { imgFallback } from '@/data/empty.json';
// import { LeftCircleOutlined, RightCircleOutlined } from '@ant-design/icons-vue';

defineOptions({
  name: 'HomeCarousel'
});

const images = ref<Array<string>>([]);

// 模拟从后端获取数据
onMounted(async () => {
  images.value = ['/home/carousel-1.png', '/home/carousel-2.png'];
});
</script>

<script setup lang="ts"></script>

<template>
  <div class="center flex">
    <ACarousel class="carousel" arrows>
      <!--
 <template #prevArrow>
        <div class="custom-slick-arrow left">
          <LeftCircleOutlined />
        </div>
      </template>
      <template #nextArrow>
        <div class="custom-slick-arrow right">
          <RightCircleOutlined />
        </div>
      </template> 
-->

      <div v-for="item in images" :key="item" class="item">
        <AImage :src="item" :preview="false" :fallback="imgFallback"></AImage>
      </div>
    </ACarousel>
  </div>
</template>

<style lang="scss" scoped>
.carousel {
  width: 100%;
  min-width: 600px;
  height: 100%;
  max-height: var(--home-max-height);
  max-width: var(--home-max-width);

  :deep(.slick-slider) {
    width: 100%;

    min-width: 600px;
    height: 100%;
    max-height: var(--home-max-height);

    .slick-list {
      width: 100%;

      min-width: 600px;
      height: 100%;
      max-height: var(--home-max-height);
    }

    .ant-image {
      height: 100%;
      max-height: var(--home-max-height);
      width: 100%;

      .ant-image-img {
        max-height: var(--home-max-height);
        object-fit: contain;
      }
    }
  }

  .item {
    position: relative;
  }

  // :deep(.slick-arrow) {
  //   &.custom-slick-arrow {
  //     width: 25px;
  //     height: 25px;
  //     font-size: 25px;
  //     color: #fff;
  //     background-color: rgba(31, 45, 61, 0.11);
  //     transition: ease all 0.3s;
  //     opacity: 0.3;
  //     z-index: 1;

  //     > span {
  //       font-size: 36px;
  //     }

  //     &.left {
  //       left: 10px;
  //     }

  //     &.right {
  //       right: 20px;
  //     }

  //     &::before {
  //       display: none;
  //     }

  //     &:hover {
  //       color: #fff;
  //       opacity: 0.5;
  //     }
  //   }
  // }
}
</style>
