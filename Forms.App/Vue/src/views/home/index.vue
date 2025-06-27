<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { imgFallback } from '@/data/empty.json';
import browseImage from '@/assets/imgs/browse.png';
import personificationImage from '@/assets/imgs/personification.png';
import aiImage from '@/assets/imgs/ai.png';
import friendImage from '@/assets/imgs/friend.png';
import illImage from '@/assets/imgs/illustration.png';
import { useAuthStore } from '@/store/modules/auth';
import Carosuel from './modules/carousel.vue';
import Updater from './modules/updater.vue';
import Statistic from './modules/statistic.vue';

const authStore = useAuthStore();

const showPageBody = ref<boolean>(false);

onMounted(() => {
  setTimeout(() => {
    showPageBody.value = true;
  }, 250);
});
</script>

<template>
  <div class="page min-h-500px overflow-hidden lt-sm:overflow-auto">
    <div class="mb-12px">
      <Carosuel></Carosuel>
    </div>

    <div v-if="showPageBody" class="page-body">
      <div>
        <ul class="linner center mb-60px flex">
          <li class="item">
            <div class="mb-8px text-center">
              <AImage :src="browseImage" :height="130" :width="130" :preview="false"></AImage>
            </div>
            <h2 class="text-center">智能浏览</h2>
            <p class="text-center">还原真实购物行为，货比、详情浏览，让浏览更像真人。</p>
          </li>
          <li class="item">
            <div class="mb-8px text-center">
              <AImage :src="personificationImage" :height="130" :width="130" :preview="false"></AImage>
            </div>
            <h2 class="text-center">拟人操作</h2>
            <p class="text-center">模拟真人点击和输入，行为自然，有效应对各类风控检测。</p>
          </li>
          <li class="item">
            <div class="mb-8px text-center">
              <AImage :src="aiImage" :height="130" :width="130" :preview="false"></AImage>
            </div>
            <h2 class="text-center">AI识别</h2>
            <p class="text-center">OCR只能识别图文内容和页面变化，灵活应对各种场景。</p>
          </li>
          <li class="item">
            <div class="mb-8px text-center">
              <AImage :src="friendImage" :height="130" :width="130" :preview="false"></AImage>
            </div>
            <h2 class="text-center">好友配对</h2>
            <p class="text-center">高效安全，实现更精准、高效的自动化匹配。</p>
          </li>
        </ul>
      </div>
      <div class="info-wrap">
        <div class="v-center info flex">
          <div>
            <AImage :src="illImage" :height="350" :width="350" :preview="false" :fallback="imgFallback"></AImage>
          </div>
          <div class="info-content center flex">
            <div>
              <p class="info-sub-title">解放双手、开源节流</p>
              <h2 class="info-title">专注资源共享与运营</h2>
              <ul>
                <li class="info-text">
                  <p>快速发布</p>
                  <p>
                    通过清晰高效的操作流程，大幅节省时间。无需在多个聊天群中来回奔波，使找寻合作对象变得更简单直接。
                  </p>
                </li>
                <li class="info-text">
                  <p>安全审核</p>
                  <p>借助平台提供的好友与店铺审核机制，自主甄别好友的合规性，将风险掌握在自己手里。</p>
                </li>
                <li class="info-text">
                  <p>开源节流</p>
                  <p>您只需专注合作伙伴的店铺真实性及经营优化，其余流程交给我们，让管理更高效、更安全、更安心。</p>
                </li>
              </ul>
            </div>
            <!-- <p class="customer">了解更多 微信群：123456789</p> -->
          </div>
        </div>
      </div>
      <Statistic v-if="!authStore.userInfo.isTenant"></Statistic>
      <Updater></Updater>
    </div>
  </div>
</template>

<style scoped lang="scss">
.page {
  --home-max-height: 900px;
  --home-max-width: 1920px;
  --page-body-max-width: 1200px;
  padding: 0 0 55px 0 !important;
  min-width: 600px;
  overflow-y: auto;

  .page-body {
    margin: 0 auto;
    padding-top: 30px;
    width: 100vw;

    > div {
      &:has(.linner) {
        border-top: 1px solid #e8e5e0;
      }
    }
  }

  .linner {
    max-width: var(--page-body-max-width);
    margin: 0 auto;
    padding: 35px 0 60px 0;

    .item {
      flex: 1;
      padding: 20px 55px;
      border-right: 2px solid #e8e5e0;
      animation: flip-in-hor-bottom 0.5s cubic-bezier(0.25, 0.46, 0.45, 0.94) both;

      &:last-child {
        border: 0;
      }

      :deep(.ant-image) {
        overflow: hidden;
        border-radius: 10%;
        cursor: default;

        img {
          transition: all 0.3s ease-in-out;
        }
      }

      h2 {
        font-weight: 600;
        letter-spacing: 1.5px;
        font-size: 20px;
        margin-bottom: 8px;
      }
    }
  }

  .info-wrap {
    background-color: #f6f7f9;
    border-top: 1px solid #e8e5e0;
  }

  .info {
    padding: 20px 0 80px 0;
    max-width: var(--page-body-max-width);
    margin: 0 auto;
    gap: 24px;
    animation: flip-in-hor-bottom 0.5s cubic-bezier(0.25, 0.46, 0.45, 0.94) both;
    animation-delay: 0.25s;

    :deep(.ant-image) {
      overflow: hidden;
      border-radius: 14px;
      cursor: pointer;

      img {
        transition: all 0.3s ease-in-out;
      }

      &:hover {
        img {
          transform: scale(1.05);
        }
      }
    }

    &-content {
      position: relative;

      ul {
        padding-left: 25px;
        list-style: disc;
      }

      > div {
        min-height: 350px;
        padding: 15px 25px;
      }

      .customer {
        position: absolute;
        bottom: 10px;
        right: 8px;
        cursor: default;
      }
    }

    &-sub-title {
      font-size: 20px;
      font-weight: 400;
      opacity: 0.8;
      margin-bottom: 8px;
    }

    &-title {
      font-size: 32px;
      font-weight: 600;
      margin-bottom: 8px;
    }

    &-text {
      margin-bottom: 10px;

      &:last-child {
        margin-bottom: 0;
      }

      p {
        &:first-child {
          font-size: 18px;
          font-weight: 600;
          line-height: 32px;
          letter-spacing: 1px;
        }

        &:last-child {
          font-size: 14px;
          line-height: 24px;
          opacity: 0.8;
          letter-spacing: 1.5px;
        }
      }
    }
  }
}

@keyframes flip-in-hor-bottom {
  0% {
    transform: rotateX(80deg);
    opacity: 0;
  }
  100% {
    transform: rotateX(0);
    opacity: 1;
  }
}
</style>
