<script setup lang="ts">
  import { computed, onMounted, reactive } from 'vue';
  import { useAuthStore } from '@/store/modules/auth';
  import { dashboardHeaderDataApi } from '@/service/api';

  defineOptions({
    name: 'HeaderBanner'
  });

  const authStore = useAuthStore();

  interface StatisticData {
    id: number;
    title: string;
    value: string;
  }

  const model = reactive<Api.Dashboard.HeaderData>({
    unSettlement: 0,
    settlemented: 0,
    settlementFailed: 0
  });

  const statisticData = computed<StatisticData[]>(() => [
    {
      id: 0,
      title: '未结算',
      value: model.unSettlement
    },
    {
      id: 1,
      title: '已结算',
      value: model.settlemented
    },
    {
      id: 2,
      title: '结算异常',
      value: model.settlementFailed
    }
  ]);

  onMounted(async () => {
    //
    // await getDashboardHeaderData();
  });

  async function getDashboardHeaderData() {
    const { data: res, error } = await dashboardHeaderDataApi();
    if (!error && res && res.code === 0) {
      Object.assign(model, res.data);
    }
  }
</script>

<template>
  <ACard :bordered="false" class="card-wrapper">
    <ARow :gutter="[16, 16]">
      <ACol :span="24" :md="18">
        <div class="flex-y-center">
          <div class="size-72px shrink-0 overflow-hidden rd-1/2">
            <img src="@/assets/imgs/soybean.jpg" class="size-full" />
          </div>
          <div class="pl-12px">
            <h3 class="text-18px font-semibold">欢迎回来, {{ authStore.userInfo.nickName }}, 今天又是充满活力的一天!</h3>
            <p class="text-#999 leading-30px">''</p>
          </div>
        </div>
      </ACol>
      <ACol :span="24" :md="6">
        <ASpace class="w-full justify-end" :size="24">
          <AStatistic v-for="item in statisticData" :key="item.id" class="whitespace-nowrap" v-bind="item" />
        </ASpace>
      </ACol>
    </ARow>
  </ACard>
</template>

<style scoped lang="scss">
  .w-full {
    :deep(.ant-statistic-content) {
      display: flex;
      justify-content: center;
    }
  }
</style>
