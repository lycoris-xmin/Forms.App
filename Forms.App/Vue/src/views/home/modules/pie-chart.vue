<script setup lang="ts">
  import { reactive, watch } from 'vue';
  import { useAppStore } from '@/store/modules/app';
  import { useEcharts } from '@/hooks/common/echarts';
  import { dashboardPieChartDataApi } from '@/service/api';

  defineOptions({
    name: 'PieChart'
  });

  const appStore = useAppStore();

  const { domRef, updateOptions } = useEcharts(() => ({
    tooltip: {
      trigger: 'item'
    },
    legend: {
      bottom: '1%',
      left: 'center',
      itemStyle: {
        borderWidth: 0
      }
    },
    series: [
      {
        color: ['#31b268', '#f56c6c', '#ea4787', '#fedc69', '#26deca'],
        name: '差价结算',
        type: 'pie',
        radius: ['45%', '75%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 1
        },
        label: {
          show: false,
          position: 'center'
        },
        emphasis: {
          label: {
            show: true,
            fontSize: '12'
          }
        },
        labelLine: {
          show: false
        },
        data: [] as { name: string; value: number }[]
      }
    ]
  }));

  const chartData = reactive<Api.Dashboard.PieChart>({
    settlemented: 0,
    settlementFailed: 0,
    pairSettlementFailed: 0,
    unSettlement: 0,
    pairUnSettlement: 0
  });

  function updateLocale() {
    updateOptions((opts, factory) => {
      const originOpts = factory();

      opts.series[0].name = originOpts.series[0].name;

      opts.series[0].data = [
        { name: '结算成功', value: chartData.settlemented },
        { name: '己方结算失败', value: chartData.settlementFailed },
        { name: '对方结算失败', value: chartData.pairSettlementFailed },
        { name: '己方未结算', value: chartData.unSettlement },
        { name: '对方未结算', value: chartData.pairUnSettlement }
      ];

      return opts;
    });
  }

  async function init() {
    const { data: res, error } = await dashboardPieChartDataApi();
    if (!error && res && res.code === 0) {
      Object.assign(chartData, res.data);

      updateOptions(opts => {
        opts.series[0].data = [
          { name: '结算成功', value: chartData.settlemented },
          { name: '己方结算失败', value: chartData.settlementFailed },
          { name: '对方结算失败', value: chartData.pairSettlementFailed },
          { name: '己方未结算', value: chartData.unSettlement },
          { name: '对方未结算', value: chartData.pairUnSettlement }
        ];

        return opts;
      });
    }
  }

  watch(
    () => appStore.locale,
    () => {
      updateLocale();
    }
  );

  // init
  // init();
</script>

<template>
  <ACard :bordered="false" class="card-wrapper" title="今日结算汇总">
    <div ref="domRef" class="h-360px overflow-hidden"></div>
  </ACard>
</template>

<style scoped></style>
