<script setup lang="ts">
import { DollarCircleOutlined, PartitionOutlined, TeamOutlined, UserSwitchOutlined } from '@ant-design/icons-vue';
import { onMounted, ref } from 'vue';
import { getStatisticsSummary } from '@/service/api';
defineOptions({
  name: 'HomeStatistic'
});

// 用于存储统计数据
const stats: { icon: any; value: number | null | undefined; title: string; color: string }[] = ref<any>([]);

onMounted(async () => {
  // 在组件加载时调用获取统计数据的方法
  await getStatistics();
});

// 调用后端接口获取数据
async function getStatistics() {
  const { data: res, error } = await getStatisticsSummary();
  if (!error && res && res.code === 0) {
    stats.value = [
      { icon: TeamOutlined, value: res.data.totalMerchants, title: '入驻商家', color: 'cyan' },
      { icon: UserSwitchOutlined, value: res.data.totalTasks, title: '任务数量', color: 'success' },
      { icon: PartitionOutlined, value: res.data.totalMatchedTasks, title: '配对数量', color: 'pink' },
      { icon: DollarCircleOutlined, value: res.data.totalSettlements, title: '任务结算', color: 'warning' }
    ];
  }
}
</script>

<template>
  <div class="statistic">
    <div class="statistic-body">
      <ACard>
        <div class="grid">
          <div v-for="(item, index) in stats" :key="index" class="grid-item">
            <div class="grid-item-value v-center center flex flex-col">
              <p class="icon" :class="`icon-${item.color}`">
                <component :is="item.icon" />
              </p>
              <AStatistic :value="item.value" />
              <p class="title">{{ item.title }}</p>
            </div>
          </div>
        </div>
      </ACard>
    </div>
  </div>
</template>

<style scoped lang="scss">
.statistic {
  border-top: 1px solid #e8e5e0;
  width: 100%;

  $height: 260px;

  &-body {
    padding: 55px 0;
    max-width: var(--page-body-max-width);
    margin: 0 auto;
    overflow: hidden;

    :deep(.ant-card) {
      height: $height;
      width: 100%;

      .ant-card-body {
        padding: 0 !important;
      }
    }

    .grid {
      grid-template-columns: repeat(4, 1fr);
      height: $height;

      &-item {
        display: flex;
        justify-content: center;
        align-items: center;
        position: relative;
        cursor: default;

        &-value {
          position: relative;
          height: 200px;
          width: 100%;

          &::after {
            content: '';
            position: absolute;
            border-right: 1px solid #e8e5e0;
            right: 0;
            top: 50%;
            height: 150px;
            transform: translateY(-50%);
          }
        }

        &:last-child {
          &-value {
            border: 0;
          }
        }
      }

      .title {
        padding: 10px 0 0 0;
        font-size: 14px;
        opacity: 0.8;
      }
    }

    :deep(.ant-statistic-content-value) {
      font-size: 26px;
      font-weight: 600;
    }
  }

  .icon {
    font-size: 36px;

    &-cyan {
      color: var(--color-cyan);
    }

    &-pink {
      color: var(--color-pink);
    }

    &-warning {
      color: var(--color-warning);
    }

    &-success {
      color: var(--color-success);
    }
  }
}
</style>
