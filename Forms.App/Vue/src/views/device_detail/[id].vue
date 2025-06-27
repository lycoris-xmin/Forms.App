<script setup lang="ts">
import { computed, onActivated, onMounted, reactive } from 'vue';
import { useRoute } from 'vue-router';
import { fetchGetDeviceDetail } from '@/service/api';
import { useAuthStore } from '@/store/modules/auth';
import LoadingWrap from '@/components/loading/index.vue';
import { deviceStatus as DEVICEC_STATUS, deviceType as DEVICE_TYPE } from '../shared/enums';
import { AndroidVersionMap } from './shared';

type Model = Api.Device.DeviceDetail & {
  loading: boolean;
};

const route = useRoute();

const authStore = useAuthStore();

const model: Model = reactive({
  loading: true
});

const deviceType = computed(() => {
  if (model && typeof model.type === 'number') {
    const item = DEVICE_TYPE.filter(x => x.value === model.type);
    return item[0];
  }
  return DEVICE_TYPE[0];
});

const deviceStatus = computed(() => {
  if (model && typeof model.status === 'number') {
    const item = DEVICEC_STATUS.filter(x => x.value === model.status);
    return item[0];
  }
  return DEVICE_TYPE[0];
});

const isTenant = computed(() => {
  return authStore.userInfo.isTenant;
});

onMounted(() => {
  getDeviceTail();
});

onActivated(() => {
  getDeviceTail();
});

async function getDeviceTail() {
  model.loading = true;
  try {
    const { data: res, error } = await fetchGetDeviceDetail(route.params.id);
    if (!error && res && res.code === 0) {
      Object.assign(model, res.data);
    }
  } finally {
    model.loading = false;
  }
}

function formatDuration(minutes) {
  const value = Number.parseFloat(minutes);
  if (Number.isNaN(value)) {
    return '-';
  }
  const totalSeconds = Math.round(value * 60); // 转换为秒
  const hours = Math.floor(totalSeconds / 3600);
  const minutesLeft = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;

  const parts = [];
  if (hours > 0) parts.push(`${hours}小时 `);
  if (minutesLeft > 0 || hours > 0) parts.push(`${minutesLeft}分 `);
  parts.push(`${seconds}秒`);

  return parts.join('');
}
</script>

<template>
  <div
    class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto"
    :class="{ loading: model.loading }"
  >
    <ACard title="设备详情">
      <ADescriptions bordered :column="{ xs: 2, sm: 2, md: 3 }">
        <ADescriptionsItem label="设备编号" :span="1">{{ route.params.id }}</ADescriptionsItem>
        <ADescriptionsItem label="设备名称" :span="1">{{ model.name }}</ADescriptionsItem>
        <ADescriptionsItem label="分组名称" :span="1">{{ model.groupName }}</ADescriptionsItem>

        <ADescriptionsItem label="设备类型" :span="1">
          <ATag :color="deviceType.color">{{ deviceType.label }}</ATag>
        </ADescriptionsItem>
        <ADescriptionsItem label="绑定时间" :span="1">{{ model.createTime }}</ADescriptionsItem>
        <ADescriptionsItem label="过期时间" :span="1">{{ model.expiredTime || '-' }}</ADescriptionsItem>

        <ADescriptionsItem label="设备品牌" :span="1">{{ model.deviceBrand }}</ADescriptionsItem>
        <ADescriptionsItem label="设备型号" :span="1">{{ model.deviceModel }}</ADescriptionsItem>
        <ADescriptionsItem label="设备分辨率" :span="1">
          {{ model.screenWidth }} * {{ model.screenHeight }}
        </ADescriptionsItem>

        <ADescriptionsItem label="App版本" :span="1">{{ model.appVersion }}</ADescriptionsItem>
        <ADescriptionsItem label="脚本版本" :span="1">{{ model.scriptVersion }}</ADescriptionsItem>
        <ADescriptionsItem label="系统版本" :span="1">
          {{ AndroidVersionMap[Number(model.systemVersion)] }}
        </ADescriptionsItem>

        <ADescriptionsItem label="心跳时间" :span="1">{{ model.lastOnlineTime }}</ADescriptionsItem>
        <ADescriptionsItem label="离线时长" :span="1">
          {{ model.offLineMinutes === '0' ? '-' : formatDuration(model.offLineMinutes) }}
        </ADescriptionsItem>
        <ADescriptionsItem label="剩余天数" :span="1">{{ model.days }} 天</ADescriptionsItem>

        <ADescriptionsItem label="设备状态" :span="1">
          <ATag :color="deviceStatus.color">{{ deviceStatus.label }}</ATag>
        </ADescriptionsItem>
        <ADescriptionsItem label="设备IP" :span="1">{{ model.ip }}</ADescriptionsItem>
        <ADescriptionsItem label="设备IP归属地" :span="1">{{ model.ipAddress }}</ADescriptionsItem>

        <ADescriptionsItem label="今日执行次数" :span="1">{{ model.todayRunCount }} 次</ADescriptionsItem>
        <ADescriptionsItem label="Wifi名称" :span="1">{{ model.wifiName || '-' }}</ADescriptionsItem>
        <ADescriptionsItem label="Wifi密码" :span="1">{{ model.wifiPassword || '-' }}</ADescriptionsItem>

        <ADescriptionsItem v-if="!isTenant" label="运行脚本个数" :span="1">
          {{ model.runCount }}
        </ADescriptionsItem>
        <ADescriptionsItem v-if="!isTenant" label="核心脚本个数" :span="2">{{ model.coreCount }}</ADescriptionsItem>
        <ADescriptionsItem v-if="!isTenant" label="运行脚本详情" :span="2">
          <ul>
            <li v-for="item of model.runScript" :key="item">{{ item }}</li>
          </ul>
        </ADescriptionsItem>
      </ADescriptions>
    </ACard>

    <LoadingWrap :loading="model.loading"></LoadingWrap>
  </div>
</template>

<style scoped lang="scss">
.min-h-500px {
  position: relative;
  overflow-y: auto;

  &.loading {
    height: 100%;
    overflow: hidden;
  }
}
</style>
