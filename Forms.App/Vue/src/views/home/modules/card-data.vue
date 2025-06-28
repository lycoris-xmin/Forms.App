<script setup lang="ts">
  import { computed, onMounted, reactive } from 'vue';
  import { createReusableTemplate } from '@vueuse/core';
  import { useRouter } from 'vue-router';
  import { dashboardCardDataApi } from '@/service/api';

  defineOptions({
    name: 'CardData'
  });

  interface CardData {
    key: string;
    title: string;
    value: number;
    unit: string;
    color: {
      start: string;
      end: string;
    };
    icon: string;
  }

  interface GradientBgProps {
    gradientColor: string;
  }

  const router = useRouter();

  const model = reactive<Api.Dashboard.VipCardData>({
    totalDevice: 0,
    onlineDevice: 0,
    product: 0,
    planTask: 0
  });

  // ant-design:money-collect-outlined
  const cardData = computed<CardData[]>(() => [
    {
      key: 'totalDevice',
      title: '总设备数',
      value: model.totalDevice,
      unit: '',
      color: {
        start: '#fcbc25',
        end: '#f68057'
      },
      icon: 'heroicons-outline:chip'
    },
    {
      key: 'onlineDevice',
      title: '在线设备数',
      value: model.onlineDevice,
      unit: '',
      color: {
        start: '#6FCF97',
        end: ' #27AE60'
      },
      icon: 'heroicons-outline:chip'
    },
    {
      key: 'product',
      title: '商品数量',
      value: model.product,
      unit: '',
      color: {
        start: '#ec4786',
        end: '#b955a4'
      },
      icon: 'fluent-emoji-high-contrast:wrapped-gift'
    },
    {
      key: 'planTask',
      title: '今日任务数',
      value: model.planTask,
      unit: '',
      color: {
        start: '#865ec0',
        end: '#5144b4'
      },
      icon: 'tabler:topology-star-ring-3'
    }
  ]);

  const [DefineGradientBg, GradientBg] = createReusableTemplate<GradientBgProps>();

  function getGradientColor(color: CardData['color']) {
    return `linear-gradient(to bottom right, ${color.start}, ${color.end})`;
  }

  onMounted(() => {
    // initVipCradData();
  });

  async function initVipCradData() {
    //
    const { data: res, error } = await dashboardCardDataApi();
    if (!error && res && res.code === 0) {
      Object.assign(model, res.data);
    }
  }

  function clickHandler(key) {
    //
    if (key === 'totalDevice') {
      router.push({ name: 'device_list' });
    } else if (key === 'onlineDevice') {
      router.push({
        name: 'device_list'
      });
    } else if (key === 'product') {
      router.push({
        name: 'product_taobao'
      });
    } else {
      router.push({
        name: 'plan-task_list'
      });
    }
  }
</script>

<template>
  <ACard :bordered="false" size="small" class="card-wrapper">
    <!-- define component start: GradientBg -->
    <DefineGradientBg v-slot="{ $slots, gradientColor }">
      <div class="rd-8px px-16px pb-4px pt-8px text-white" :style="{ backgroundImage: gradientColor }">
        <component :is="$slots.default" />
      </div>
    </DefineGradientBg>
    <!-- define component end: GradientBg -->

    <ARow :gutter="[16, 16]">
      <ACol v-for="item in cardData" :key="item.key" :span="24" :md="12" :lg="6">
        <GradientBg :gradient-color="getGradientColor(item.color)" class="pointer flex-1" @click="clickHandler(item.key)">
          <h3 class="text-16px">{{ item.title }}</h3>
          <div class="flex justify-between pt-12px">
            <SvgIcon :icon="item.icon" class="text-32px" />
            <CountTo :prefix="item.unit" :start-value="1" :end-value="item.value" class="text-30px text-white dark:text-dark" />
          </div>
        </GradientBg>
      </ACol>
    </ARow>
  </ACard>
</template>

<style scoped lang="scss">
  .pointer {
    transition: 0.3s ease-in-out;
    cursor: pointer;

    &:hover {
      transform: scale(1.02);
    }
  }
</style>
