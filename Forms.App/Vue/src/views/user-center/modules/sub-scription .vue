<script setup lang="ts">
import { nextTick, onMounted, reactive } from 'vue';
import LoadingWrap from '@/components/loading/index.vue';
import { fetchGetProfileSubscription } from '@/service/api';

type Model = {
  unUseDays: number;
  totalDays: number;
  loading: boolean;
};

const model = reactive<Model>({
  unUseDays: 0,
  totalDays: 0,
  loading: true
});

onMounted(async () => {
  setTimeout(async () => {
    try {
      await getSubscription();
    } finally {
      model.loading = false;
    }
  }, 500);
});

async function getSubscription() {
  const { data: res, error } = await fetchGetProfileSubscription();
  if (!error && res && res.code === 0) {
    nextTick(() => {
      Object.assign(model, res.data);
    });
  }
}
</script>

<template>
  <div class="body" :class="{ show: !model.loading }">
    <div class="subscription">
      <ACard>
        <AStatistic title="未使用" :value="model.unUseDays" suffix="天"></AStatistic>
      </ACard>
      <ACard>
        <AStatistic title="累计总使用" :value="model.totalDays" suffix="天"></AStatistic>
      </ACard>
    </div>

    <!-- <ACard title="充值记录"></ACard> -->
    <LoadingWrap v-if="model.loading" :loading="model.loading" bg-color="#fff"></LoadingWrap>
  </div>
</template>

<style lang="scss" scoped>
.body {
  position: relative;
  height: 100%;
  width: 100%;
  overflow: hidden;

  &.show {
    overflow-y: auto;
  }
}

.subscription {
  width: 60%;
  max-width: 800px;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 25px;
  padding-bottom: 25px;
}
</style>
