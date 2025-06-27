<script setup lang="ts">
import { computed, reactive, watch } from 'vue';
import { apiUrl, fetchBindingQRCode, fetchGetBindingList } from '@/service/api';
import { countdown, formatSecondsToMinutes } from '@/utils/helper.ts';
import { copyToClipboard } from '@/utils/helper';

defineOptions({
  name: 'DeviceBindingDrawer'
});

const visible = defineModel<boolean>('visible', { default: false });

type Props = {
  areaName: string;
  groupName: string;
  sectionName: string;
};

type Model = {
  src: string;
  intervalView: string;
  loading: boolean;
  refreshLoading: boolean;
  timerCancel: (() => void) | null;
  code: string;
  list: Array<string>;
  second: number;
};

type Emits = {
  (e: 'refresh'): void;
};

const props = defineProps<Props>();

const model = reactive<Model>({
  src: '',
  intervalView: '',
  loading: true,
  refreshLoading: false,
  timerCancel: null,
  code: '',
  list: [],
  second: 0
});

const qrcodeStatus = computed(() => {
  if (!model.timerCancel) {
    return 'expired';
  }

  return model.refreshLoading || model.loading ? 'loading' : 'active';
});

const emit = defineEmits<Emits>();

watch(visible, (value, old) => {
  if (value && value !== old) {
    model.loading = true;
    generateQRCode();
  }

  if (!value) {
    model.refreshLoading = true;
    model.list = [];
    stopTimer();
    emit('refresh');
  }
});

async function generateQRCode() {
  model.refreshLoading = true;
  model.list = [];
  stopTimer();

  try {
    const { data: res, error } = await fetchBindingQRCode(props.areaName, props.groupName, props.sectionName);
    if (!error) {
      if (res) {
        if (res.code === 0) {
          model.code = res.data;
          model.src = `${apiUrl}/api/app/binding/${res.data}`;
          model.timerCancel = countdown(120, countdownView, countdownComplete);
        } else if (res.code === 1001) {
          window.$message?.warning(res.msg);
          visible.value = false;
        }
      }
    }
  } finally {
    setTimeout(() => {
      model.refreshLoading = false;
      model.loading = false;
    }, 1000);
  }
}

async function countdownView(seconds) {
  model.intervalView = formatSecondsToMinutes(seconds);
  await getBindingList(seconds);
}

function countdownComplete() {
  setTimeout(() => {
    stopTimer();
    model.code = '';
    model.second = 0;
  }, 1000);
}

function stopTimer() {
  if (model.timerCancel && typeof model.timerCancel === 'function') {
    model.timerCancel();
    model.timerCancel = void 0;
  }
}

async function getBindingList() {
  if (model.code && model.second === 2) {
    try {
      const { data: res, error } = await fetchGetBindingList(model.code);
      if (!error && res && res.code === 0) {
        model.list = res.data.list;
      }
    } catch {
      //
    }
  }

  if (model.second === 2) {
    model.second = 0;
  } else {
    model.second += 1;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="绑定新设备" :width="500">
    <div class="body">
      <div class="center flex">
        <AQrcode :size="220" :value="model.src" :status="qrcodeStatus" @refresh="generateQRCode" />
      </div>
      <p class="text-center">扫一扫绑定设备</p>
      <p v-if="qrcodeStatus == 'active'" class="text-center">
        <AInputSearch v-model:value="model.src" readonly>
          <template #enterButton>
            <ATooltip placement="top" title="点击即可复制链接">
              <span @click="copyToClipboard(model.src)">复制绑定链接</span>
            </ATooltip>
          </template>
        </AInputSearch>
      </p>
      <p class="danger text-center">
        <span v-if="!model.loading && !model.refreshLoading">{{ model.intervalView }} 后关闭</span>
        <span v-else></span>
      </p>
      <div>
        <p class="info">已绑定 {{ model.list.length }} 台</p>
        <ul class="binding-list">
          <TransitionGroup name="list" tag="ul">
            <li v-for="(item, index) of model.list" :key="item.id">
              <AAlert
                :message="`第 ${index + 1} 台  &nbsp; 品牌：${item.brand} &nbsp; ${item.id} `"
                type="info"
              ></AAlert>
            </li>
          </TransitionGroup>
        </ul>
      </div>
    </div>
  </ADrawer>
</template>

<style lang="scss" scoped>
.body {
  position: relative;
  padding: 60px 10px 10px 10px;
}

.flex {
  padding: 10px 0;

  .qrcode {
    height: 200px;
    width: 200px;
  }

  div:has(.qrcode) {
    padding: 2px;
    border: 1px solid #b6b4b4;
  }
}

.text-center {
  font-size: 14px;
  font-weight: 600;
  letter-spacing: 1.5px;

  &:first-child {
    padding-top: 55px;
  }

  &.danger {
    height: 20px;
    color: var(--color-danger);
    margin-bottom: 14px;
  }
}

.info {
  font-weight: 600;
}

.binding-list {
  padding: 20px 0;

  li {
    margin-bottom: 10px;

    .index {
      padding-right: 20px;
      font-weight: 600;
    }

    &:last-child {
      margin-bottom: 0;
    }
  }
}

.list-enter-active,
.list-leave-active {
  transition: all 0.5s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style>
