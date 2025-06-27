<script setup lang="tsx">
import { ref, watch } from 'vue';
import { fetchGetDeviceScriptList } from '@/service/api';
import { ScriptRunStatus as SCRIPTLOG_STATUS } from '@/views/shared/enums';

defineOptions({
  name: 'DeviceOperationDetailDrawer'
});

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = defineProps<{
  commandId?: string;
}>();

const loading = ref<boolean>(true);
const list = ref<any[]>([]);

watch(
  () => visible.value,
  async val => {
    if (val) {
      await getList();
    } else {
      list.value = [];
    }
  }
);

async function getList() {
  loading.value = true;
  try {
    const { data: res, error } = await fetchGetDeviceScriptList(props.commandId);
    if (!error && res && res.code === 0) {
      list.value = res.data.list.map(item => ({
        ...item,
        statusLabel: getStatusLabel(item.status),
        statusCode: item.status
      }));
    }
  } finally {
    loading.value = false;
  }
}

function getStatusLabel(status: number): string {
  const statusItem = SCRIPTLOG_STATUS.find(item => item.value === status);
  return statusItem ? statusItem.label : '未知状态';
}

function closeDrawer() {
  visible.value = false;
}
</script>

<template>
  <ADrawer :open="props.visible" title="设备运行详情" width="50%">
    <AList item-layout="vertical" :data-source="list" :loading="loading" class="custom-list">
      <template #renderItem="{ item }">
        <ACard class="list-card">
          <ARow :gutter="16">
            <ACol :span="12">
              <div class="info-item">
                <span class="info-label">任务ID：</span>
                <span class="info-value">{{ item.taskId || '无' }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">指令内容：</span>
                <pre class="command-content">{{ item.commandContent }}</pre>
              </div>
            </ACol>

            <ACol :span="12">
              <div class="info-item">
                <span class="info-label">运行状态：</span>
                <ATag :color="item.statusCode >= 2 && item.statusCode <= 5 ? 'error' : 'processing'">
                  {{ item.statusLabel }}
                </ATag>
              </div>
              <div class="info-item">
                <span class="info-label">开始时间：</span>
                <span class="info-value">{{ item.runTime }}</span>
              </div>
            </ACol>
          </ARow>

          <template v-if="item.statusCode >= 2 && item.statusCode <= 5">
            <div class="error-info">
              <span class="error-label">错误信息：</span>
              <div class="error-content">{{ item.errorMessage || '无' }}</div>
            </div>
          </template>
          <template v-else-if="[0, 1, 6].includes(item.statusCode)">
            <div class="right-info">
              <span class="right-label">执行状态：</span>
              <div class="right-content">无错误，执行成功</div>
            </div>
          </template>
        </ACard>
      </template>
    </AList>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">关闭</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.custom-list {
  :deep(.ant-list-item) {
    padding: 0;
    margin-bottom: 16px;
  }
}

.list-card {
  margin: 8px;
  transition: box-shadow 0.3s;
  &:hover {
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  }
}

.info-item {
  margin-bottom: 12px;
  display: flex;
  align-items: baseline;
  .info-label {
    flex: 0 0 80px;
    color: rgba(0, 0, 0, 0.65);
    font-weight: 500;
  }
  .info-value {
    flex: 1;
    word-break: break-all;
  }
}

.command-content {
  background: #f5f5f5;
  padding: 8px;
  border-radius: 4px;
  margin: 8px 0;
  white-space: pre-wrap;
  font-family: Consolas, Monaco, monospace;
}

.error-info {
  margin-top: 16px;
  padding: 12px;
  background: #fff1f0;
  border-radius: 4px;
  border: 1px solid #ffccc7;
  .error-label {
    color: #cf1322;
    font-weight: 500;
    display: block;
    margin-bottom: 4px;
  }
  .error-content {
    color: #cf1322;
    white-space: pre-wrap;
  }
}

.right-info {
  margin-top: 16px;
  padding: 12px;
  background: #f6ffed;
  border-radius: 4px;
  border: 1px solid #b7eb8f;
  .right-label {
    color: #52c41a;
    font-weight: 500;
    display: block;
    margin-bottom: 4px;
  }
  .right-content {
    color: #52c41a;
    white-space: pre-wrap;
  }
}
</style>
