<script setup lang="ts">
import { LeftOutlined, RightOutlined } from '@ant-design/icons-vue';
import { onMounted, ref } from 'vue';
import { useThemeStore } from '@/store/modules/theme';
import updateJson from '@/data/initupdate.json';

defineOptions({
  name: 'HomeUpdater'
});

const themeStore = useThemeStore();

const updateList = ref<Array<any>>([...updateJson.initupdateV1.content].reverse());
const leftPx = ref(0);
let uintPx = 392.5;
let maxWidth = 0;

onMounted(() => {
  uintPx = document.querySelector('.updater-card').getBoundingClientRect().width;
  maxWidth = document.querySelector('.update-list').scrollWidth - 3 * uintPx;
});

const prevHandler = () => {
  if (leftPx.value === 0) {
    return;
  }

  leftPx.value += uintPx;
};

const nextHandler = () => {
  if (Math.abs(leftPx.value - uintPx) >= maxWidth) {
    return;
  }

  leftPx.value -= uintPx;
};
</script>

<template>
  <div class="updater">
    <div class="top" :class="{ dark: themeStore.darkMode }"></div>
    <div class="content v-center space-between flex">
      <div class="actions">
        <div class="v-center space-between flex">
          <div>
            <p class="title">版本更新日志</p>
          </div>
          <div class="v-center flex gap-10px">
            <span class="pre v-center center flex" :class="{ dark: themeStore.darkMode }" @click="prevHandler">
              <LeftOutlined />
            </span>
            <span class="next v-center center flex" :class="{ dark: themeStore.darkMode }" @click="nextHandler">
              <RightOutlined />
            </span>
          </div>
        </div>
      </div>
      <div class="update-list-body">
        <div class="space-between update-list flex" :style="{ '--left-width': `${leftPx}px` }">
          <div v-for="item in updateList" :key="item.time" class="updater-card">
            <ACard>
              <div class="header">
                <p class="version mb-8px">{{ item.title }}</p>
                <p class="time">发布时间：{{ item.time }}</p>
              </div>

              <p class="mb-8px">版本更新说明</p>
              <div class="updater-card-text">
                <p v-for="(text, index) in item.text" :key="text" class="text">{{ index + 1 }}. {{ text }}</p>
              </div>
            </ACard>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.updater {
  width: 100%;
  border-top: 1px solid #e8e5e0;
  border-bottom: 1px solid #e8e5e0;
  position: relative;
  padding-bottom: 55px;

  .top {
    position: absolute;
    top: 0;
    left: 0;
    background-color: #f2f0ec;
    height: 280px;
    width: 100%;
    transition: all 0.3s ease-in-out;

    &.dark {
      background-color: #c2c2c2;
    }
  }

  .content {
    position: relative;
    max-width: var(--page-body-max-width);
    margin: 0 auto;
    padding-top: 100px;

    .actions {
      position: absolute;
      top: 16px;
      right: 0;
      width: 100%;

      .title {
        font-size: 26px;
        font-size: 600;
        letter-spacing: 1.5px;
      }

      .pre,
      .next {
        font-size: 18px;
        height: 35px;
        width: 35px;
        border-radius: 50%;
        background-color: #fff;
        cursor: pointer;

        &.dark {
          > span {
            color: #000 !important;
          }
        }
      }
    }

    .update-list {
      --left-width: 0px;
      width: auto;
      align-items: stretch;
      max-width: var(--page-body-max-width);
      transform: translateX(var(--left-width));
      transition: all 0.35s ease-in-out;
      padding: 10px;

      .updater-card {
        padding: 15px 20px;
        width: calc(var(--page-body-max-width) / 3);
      }
    }

    :deep(.ant-card) {
      height: 100%;
      width: 100%;
      border-radius: 10px;
      width: calc(var(--page-body-max-width) / 3 - 40px);
      cursor: pointer;
      position: relative;
    }
  }

  .update-list-body {
    max-width: var(--page-body-max-width);
    overflow: hidden;
  }

  .header {
    margin-bottom: 18px;

    .version {
      font-size: 28px;
      font-weight: 600;
      letter-spacing: 1.5px;
    }

    .time {
      opacity: 0.8;
      font-size: 14px;
    }
  }

  .text {
    margin-bottom: 8px;

    &:last-child {
      margin-bottom: 0;
    }
  }
}
</style>
