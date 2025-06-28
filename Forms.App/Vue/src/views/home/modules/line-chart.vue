<script setup lang="ts">
  import { onMounted, watch } from 'vue';
  import { useAppStore } from '@/store/modules/app';
  import { useEcharts } from '@/hooks/common/echarts';
  import { dashboardLineChartDataApi } from '@/service/api';

  defineOptions({
    name: 'LineChart'
  });

  const appStore = useAppStore();

  const { domRef, updateOptions } = useEcharts(() => ({
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross',
        label: {
          backgroundColor: '#6a7985'
        }
      }
    },
    legend: {
      data: ['发布任务', '任务成功', '任务失败']
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: [] as string[]
    },
    yAxis: {
      type: 'value',
      min: 0
    },
    series: [
      {
        color: '#8e9dff',
        name: '发布任务',
        type: 'line',
        smooth: true,
        // stack: 'Total',
        areaStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              {
                offset: 0.25,
                color: '#8e9dff'
              },
              {
                offset: 1,
                color: '#fff'
              }
            ]
          }
        },
        emphasis: {
          focus: 'series'
        },
        data: [] as number[]
      },
      {
        color: '#26deca',
        name: '任务成功',
        type: 'line',
        smooth: true,
        // stack: 'Total',
        areaStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              {
                offset: 0.25,
                color: '#26deca'
              },
              {
                offset: 1,
                color: '#fff'
              }
            ]
          }
        },
        emphasis: {
          focus: 'series'
        },
        data: []
      },
      {
        color: '#cf4f97',
        name: '任务失败',
        type: 'line',
        smooth: true,
        // stack: 'Total',
        areaStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              {
                offset: 0.25,
                color: '#cf4f97'
              },
              {
                offset: 1,
                color: '#fff'
              }
            ]
          }
        },
        emphasis: {
          focus: 'series'
        },
        data: []
      }
    ]
  }));

  function updateLocale() {
    updateOptions((opts, factory) => {
      const originOpts = factory();

      opts.legend.data = originOpts.legend.data;
      opts.series[0].name = originOpts.series[0].name;
      opts.series[1].name = originOpts.series[1].name;
      opts.series[2].name = originOpts.series[2].name;

      return opts;
    });
  }

  async function init() {
    const { data: res, error } = await dashboardLineChartDataApi();
    if (!error && res && res.code === 0) {
      updateOptions(opts => {
        opts.xAxis.data = res.data.xAxis;
        opts.series[0].data = res.data.total;
        opts.series[1].data = res.data.success;
        opts.series[2].data = res.data.failed;

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

  onMounted(() => {
    // init();
  });
</script>

<template>
  <ACard :bordered="false" class="card-wrapper" title="近期任务走势">
    <div ref="domRef" class="h-360px overflow-hidden"></div>
  </ACard>
</template>

<style scoped></style>
