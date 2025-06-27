<script setup lang="tsx">
import { reactive, ref, watch } from 'vue';
import { fetchAliSonList } from '@/service/api';
import { imgFallback } from '@/data/empty.json';

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = defineProps({
  taskId: {
    type: String,
    required: true
  }
});

const loading = ref(true);

const model = reactive<{ list: Array<any> }>({
  list: []
});

// 当visible变化时重新加载数据
watch(
  () => visible.value,
  async newVal => {
    if (newVal && props.taskId) {
      await loadData();
    }
  }
);

async function loadData() {
  loading.value = true;
  try {
    const requestParams = {
      pageIndex: 1,
      pageSize: 10,
      taskId: props.taskId
    };

    const { data: res, error } = await fetchAliSonList(requestParams);

    if (!error && res && res.code === 0) {
      model.list = res.data.list;
    }
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="查看商品" :width="600" :footer="null">
    <div v-if="loading" class="flex items-center justify-center py-10">
      <ASpin size="large" />
    </div>
    <div v-else-if="model.list.length === 0" class="py-10 text-center">
      <p>暂无子商品数据</p>
    </div>
    <div v-else class="space-y-6">
      <div
        v-for="item in model.list"
        :key="item.taskId"
        class="border rounded-lg p-4 shadow-sm transition-shadow hover:shadow-md"
      >
        <div class="flex gap-4">
          <div class="h-20 w-20 flex-shrink-0 overflow-hidden rounded-md">
            <AImage
              :src="item.productImage || imgFallback"
              :fallback="imgFallback"
              width="80"
              height="80"
              class="object-cover"
            />
          </div>
          <div class="flex-grow">
            <h3 class="mb-1 text-gray-800 font-medium">{{ item.productTitle || '未命名商品' }}</h3>
            <p class="mb-2 text-sm text-gray-600">
              <span v-if="item.text" class="block">评论：{{ item.text }}</span>
            </p>
            <p class="mb-2 text-sm text-gray-600">
              <span v-if="item.price !== null">
                价格：
                <span class="text-red-500">￥{{ item.price }}</span>
              </span>
            </p>
            <p class="mb-2 text-sm text-gray-600">
              <span v-if="item.count !== null">购买数量：{{ item.count }}</span>
            </p>
            <div v-if="item.images && item.images.length > 0" class="mt-2 flex flex-wrap gap-2">
              <AImage
                v-for="(img, idx) in item.images"
                :key="idx"
                :src="img"
                :fallback="imgFallback"
                width="60"
                height="60"
                class="rounded"
              />
            </div>

            <!--
 <div v-if="item.videos && item.videos.length > 0" class="mt-2 flex gap-2">
              <video
                v-for="(vid, idx) in item.videos"
                :key="idx"
                :src="vid"
                controls
                width="100"
                height="80"
                class="border rounded"
              />
            </div>
-->
          </div>
        </div>
      </div>
    </div>
  </ADrawer>
</template>

<style scoped>
.video-container {
  position: relative;
  width: 100%;
  height: 0;
  padding-bottom: 56.25%;
  /* 16:9 比例 */
}

video {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>
