<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { InfoCircleOutlined } from '@ant-design/icons-vue';
import { fetchGetNetworkPairingSetting, fetchSetNetworkPairingSetting } from '@/service/api/modules/network-pairing';
import { fetchGetNetworkpairingConfigFromEnums, fetchGetTenantPlatformEnum } from '@/service/api/modules/enums';
// 类型定义
interface LevelConfig {
  level: number;
  dayMinLimit: number;
  dayMaxLimit: number;
  monthLimit: number;
  effectiveTime?: string;
  modifyTime?: string;
  isEffective?: boolean;
  matchLevel?: number;
  ruleLevel?: number;
  isFirstTimeConfig?: boolean;
}

interface PlatformConfig {
  current: LevelConfig | null;
  last?: LevelConfig | null;
}

// 添加计算生效等级的方法
interface EffectiveConfig {
  level: number;
  dayMinLimit: number;
  dayMaxLimit: number;
  monthLimit: number;
}

type PlatformType = 'TB' | 'JD' | 'DY' | 'ALIBABA';

interface ModelState {
  platforms: Record<PlatformType, LevelConfig[]>;
  config: Record<PlatformType, PlatformConfig> | null;
}

// 常量
const PLATFORM_NAMES: Record<PlatformType, string> = {
  TB: 'TB',
  JD: 'JD',
  DY: 'DY',
  ALIBABA: '1688'
};

const PLATFORMS: PlatformType[] = ['TB', 'JD', 'DY', 'ALIBABA'];

// 响应式状态
const loading = ref(false);
const activeKeys = ref<PlatformType[]>([]);
const showConfirmModal = ref(false);
const confirmModalContent = ref('');
const pendingLevelChange = ref<{
  platform: PlatformType;
  item: LevelConfig;
} | null>(null);

const model = ref<ModelState>({
  platforms: {
    TB: [],
    JD: [],
    DY: [],
    ALIBABA: []
  },
  config: null
});

const showErrorModal = ref(false);
const errorModalContent = ref('');
const showSuccessModal = ref(false);
const successModalContent = ref('');

const allowedPlatforms = ref<PlatformType[]>([]);

// 获取商户平台权限
async function getTenantPlatforms() {
  const { data: res } = await fetchGetTenantPlatformEnum();
  if (res?.code === 0 && res.data?.list) {
    allowedPlatforms.value = res.data.list
      .map((platformEnum: number) => {
        switch (platformEnum) {
          case 0:
            return 'TB';
          case 10:
            return 'JD';
          case 30:
            return 'DY';
          case 40:
            return 'ALIBABA';
          default:
            return '';
        }
      })
      .filter(p => p) as PlatformType[];
  }
}

onMounted(async () => {
  try {
    loading.value = true;
    await Promise.all([getNetworkpairingConfig(), getTenantPlatforms()]);
  } catch {
    errorModalContent.value = '初始化失败，请刷新页面重试';
    showErrorModal.value = true;
  } finally {
    loading.value = false;
  }
});

async function getAvailableLevels() {
  const { data: res } = await fetchGetNetworkpairingConfigFromEnums();

  if (res?.code !== 0) {
    throw new Error(res?.message || '未知错误');
  }

  return res.data;
}

// 获取当前配置
async function getCurrentConfig() {
  const { data: res } = await fetchGetNetworkPairingSetting();

  if (res?.code !== 0) {
    throw new Error(res?.message || '未知错误');
  }

  return res.data;
}

// 方法定义
async function getNetworkpairingConfig() {
  try {
    const [enumData, settingData] = await Promise.all([getAvailableLevels(), getCurrentConfig()]);

    if (!enumData) {
      throw new Error('配置数据为空');
    }

    const platformConfigs = {
      TB: enumData.tb || [],
      JD: enumData.jd || [],
      DY: enumData.dy || [],
      ALIBABA: enumData.alibaba || []
    };

    const currentConfig = {
      TB: settingData?.tb || { current: null, last: null },
      JD: settingData?.jd || { current: null, last: null },
      DY: settingData?.dy || { current: null, last: null },
      ALIBABA: settingData?.alibaba || { current: null, last: null }
    };

    model.value.platforms = platformConfigs;
    model.value.config = currentConfig;

    return platformConfigs;
  } catch (error) {
    errorModalContent.value = error.message || '获取配置失败';
    showErrorModal.value = true;
    return null;
  }
}

const isLevelSelectable = (platform: PlatformType, targetLevel: number) => {
  const currentConfig = model.value.config?.[platform]?.current;
  if (!currentConfig) return true;

  const currentLevel = currentConfig.level;

  // 只检查是否是同级
  return targetLevel !== currentLevel;
};

function handleLevelChange(platform: PlatformType, item: LevelConfig) {
  if (!item) {
    errorModalContent.value = '配置数据错误';
    showErrorModal.value = true;
    return;
  }

  const currentConfig = model.value.config?.[platform]?.current;
  const currentLevel = currentConfig?.level || 0;

  // 检查是否在同等级修改
  if (item.level === currentLevel) {
    errorModalContent.value = '不能在同等级内进行调整';
    showErrorModal.value = true;
    return;
  }

  // 检查是否是相邻等级
  if (Math.abs(item.level - currentLevel) !== 1) {
    errorModalContent.value = `当前等级${currentLevel}，只能逐级调整等级。`;
    showErrorModal.value = true;
    return;
  }

  const confirmContent = `您确定要将${PLATFORM_NAMES[platform]}平台的组网配对修改为 "等级${item.level}（每日${item.dayMinLimit}-${item.dayMaxLimit}单，每月${item.monthLimit}单）" 吗？`;
  pendingLevelChange.value = { platform, item };
  confirmModalContent.value = confirmContent;
  showConfirmModal.value = true;
}

function formatEffectiveTimeDisplay(effectiveTime: Date | string): string {
  const date = typeof effectiveTime === 'string' ? new Date(effectiveTime) : effectiveTime;
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: false,
    timeZone: 'Asia/Shanghai'
  });
}

function formatModifyTimeDisplay(modifyTime: Date | string): string {
  const date = typeof modifyTime === 'string' ? new Date(modifyTime) : modifyTime;
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: false,
    timeZone: 'Asia/Shanghai'
  });
}

async function handleConfirmLevelChange() {
  if (!pendingLevelChange.value) return;

  const { platform, item } = pendingLevelChange.value;
  loading.value = true;

  try {
    const data = {};
    data[platform.toLowerCase()] = {
      level: item.level,
      dayMinLimit: item.dayMinLimit,
      dayMaxLimit: item.dayMaxLimit,
      monthLimit: item.monthLimit
    };

    const response = await fetchSetNetworkPairingSetting(data);

    if (response.data?.code === 0) {
      await getNetworkpairingConfig();
      const effectiveTime = model.value.config[platform]?.current?.effectiveTime;
      successModalContent.value = `已成功修改${PLATFORM_NAMES[platform]}平台的组网配对等级，将在 ${formatEffectiveTimeDisplay(effectiveTime)} 生效`;
      showSuccessModal.value = true;
    }
  } finally {
    loading.value = false;
    showConfirmModal.value = false;
    pendingLevelChange.value = null;
  }
}

const getEffectiveConfig = (platform: PlatformType): EffectiveConfig | null => {
  const config = model.value.config?.[platform];
  if (!config?.current || !config?.last) return null;

  if (config.current.level < config.last.level) {
    return {
      level: config.last.level,
      dayMinLimit: config.last.dayMinLimit,
      dayMaxLimit: config.last.dayMaxLimit,
      monthLimit: config.last.monthLimit
    };
  }
  return {
    level: config.current.level,
    dayMinLimit: config.current.dayMinLimit,
    dayMaxLimit: config.current.dayMaxLimit,
    monthLimit: config.current.monthLimit
  };
};

// 添加升降级状态计算
const getLevelChangeStatus = (
  platform: PlatformType
): { type: 'upgrade' | 'downgrade' | 'same' | null; text: string } => {
  const config = model.value.config?.[platform];
  if (!config?.current || !config?.last) {
    return { type: null, text: '首次配置' };
  }

  const currentLevel = config.current.level;
  const lastLevel = config.last.level;

  if (currentLevel < lastLevel) {
    return { type: 'upgrade', text: '升级中' };
  } else if (currentLevel > lastLevel) {
    return { type: 'downgrade', text: '降级中' };
  }
  return { type: 'same', text: '等级不变' };
};
</script>

<template>
  <div class="platform-setting">
    <ASpin :spinning="loading">
      <div class="page-header">
        <div class="header-title">平台设置</div>
      </div>

      <div class="page-content">
        <div class="content-section">
          <div class="section-header">
            <div class="header-left">
              <span class="section-title">组网配对设置</span>
              <ATooltip>
                <template #title>
                  <div class="tooltip-content">修改配对等级后将在指定时间后生效，请谨慎操作</div>
                </template>
                <InfoCircleOutlined class="info-icon" />
              </ATooltip>
            </div>
          </div>
          <div class="section-body">
            <ACollapse v-model:active-key="activeKeys">
              <ACollapsePanel v-for="platform in PLATFORMS.filter(p => allowedPlatforms.includes(p))" :key="platform">
                <template #header>
                  <div class="panel-header">
                    <span class="platform-name" :class="`platform-${platform}`">
                      {{ PLATFORM_NAMES[platform] }}平台配置
                    </span>
                    <div class="status-container">
                      <template v-if="model.config?.[platform]?.current">
                        <div class="status-info">
                          <div class="current-level">
                            <span class="level-label">当前等级:</span>
                            <span class="level-value">{{ model.config[platform].current.level }}</span>
                          </div>
                          <div v-if="getEffectiveConfig(platform)" class="effective-level">
                            <span class="level-label">生效等级:</span>
                            <span class="level-value">{{ getEffectiveConfig(platform)?.level }}</span>
                            <span class="level-change-status" :class="getLevelChangeStatus(platform).type">
                              ({{ getLevelChangeStatus(platform).text }})
                            </span>
                          </div>
                        </div>
                        <span
                          class="effective-status"
                          :class="{ 'is-effective': model.config[platform].current.isEffective }"
                        >
                          <template v-if="model.config[platform].current.isEffective">已生效</template>
                          <template v-else>
                            将在 {{ formatEffectiveTimeDisplay(model.config[platform].current.effectiveTime) }} 生效
                          </template>
                        </span>
                      </template>
                      <span v-else class="level-status">未设置</span>
                    </div>
                  </div>
                </template>

                <div class="platform-config">
                  <!-- 当前配置信息 -->
                  <div class="current-config-box">
                    <div class="box-title">当前配置</div>
                    <div v-if="model.config?.[platform]?.current" class="box-content">
                      <div class="config-group">
                        <h4 class="group-title">设置配置</h4>
                        <div class="config-item">
                          <span class="label">等级：</span>
                          <span class="value">{{ model.config[platform].current.level }}</span>
                        </div>
                        <div class="config-item">
                          <span class="label">日限制：</span>
                          <span class="value">
                            {{ model.config[platform].current.dayMinLimit }}-{{
                              model.config[platform].current.dayMaxLimit
                            }}单
                          </span>
                        </div>
                        <div class="config-item">
                          <span class="label">月限制：</span>
                          <span class="value">{{ model.config[platform].current.monthLimit }}单</span>
                        </div>
                      </div>

                      <!-- 生效配置显示 -->
                      <div v-if="getEffectiveConfig(platform)" class="config-group">
                        <h4 class="group-title">
                          生效配置
                          <span v-if="getLevelChangeStatus(platform).type === 'upgrade'" class="config-note">
                            (升级过程中使用历史配置)
                          </span>
                          <span v-else class="config-note">(使用当前配置)</span>
                        </h4>
                        <div class="config-item">
                          <span class="label">等级：</span>
                          <span class="value">{{ getEffectiveConfig(platform)?.level }}</span>
                        </div>
                        <div class="config-item">
                          <span class="label">日限制：</span>
                          <span class="value">
                            {{ getEffectiveConfig(platform)?.dayMinLimit }}-{{
                              getEffectiveConfig(platform)?.dayMaxLimit
                            }}单
                          </span>
                        </div>
                        <div class="config-item">
                          <span class="label">月限制：</span>
                          <span class="value">{{ getEffectiveConfig(platform)?.monthLimit }}单</span>
                        </div>
                      </div>

                      <!-- 时间信息 -->
                      <div class="config-group">
                        <h4 class="group-title">时间信息</h4>
                        <div v-if="model.config[platform].current.modifyTime" class="config-item">
                          <span class="label">修改时间：</span>
                          <span class="value">
                            {{ formatModifyTimeDisplay(model.config[platform].current.modifyTime) }}
                          </span>
                        </div>
                        <div v-if="model.config[platform].current.effectiveTime" class="config-item">
                          <span class="label">生效时间：</span>
                          <span class="value">
                            {{ formatEffectiveTimeDisplay(model.config[platform].current.effectiveTime) }}
                          </span>
                        </div>
                        <div class="config-item">
                          <span class="label">状态：</span>
                          <span
                            class="value"
                            :class="{ 'status-effective': model.config[platform].current.isEffective }"
                          >
                            {{ model.config[platform].current.isEffective ? '已生效' : '未生效' }}
                          </span>
                        </div>
                      </div>
                    </div>
                    <div v-else class="empty box-content">未设置</div>
                  </div>

                  <!-- 历史配置信息 -->
                  <div v-if="model.config?.[platform]?.last" class="history-config-box">
                    <div class="box-title">历史配置</div>
                    <div class="box-content">
                      <div class="config-item">
                        <span class="label">等级：</span>
                        <span class="value">{{ model.config[platform].last.level }}</span>
                      </div>
                      <div class="config-item">
                        <span class="label">日限制：</span>
                        <span class="value">
                          {{ model.config[platform].last.dayMinLimit }}-{{ model.config[platform].last.dayMaxLimit }}单
                        </span>
                      </div>
                      <div class="config-item">
                        <span class="label">月限制：</span>
                        <span class="value">{{ model.config[platform].last.monthLimit }}单</span>
                      </div>
                      <div v-if="model.config[platform].last.modifyTime" class="config-item">
                        <span class="label">修改时间：</span>
                        <span class="value">
                          {{ formatModifyTimeDisplay(model.config[platform].last.modifyTime) }}
                        </span>
                      </div>
                    </div>
                  </div>

                  <!-- 等级选择区域 -->
                  <div class="select-area">
                    <div class="select-label">选择等级</div>
                    <div class="level-cards">
                      <div
                        v-for="item in model.platforms[platform]"
                        :key="item.level"
                        class="level-card"
                        :class="{
                          selected: model.config?.[platform]?.current?.level === item.level,
                          'history-selected': model.config?.[platform]?.last?.level === item.level,
                          disabled: loading || item.level === model.config?.[platform]?.current?.level
                        }"
                        @click="
                          !loading &&
                          item.level !== model.config?.[platform]?.current?.level &&
                          handleLevelChange(platform, item)
                        "
                      >
                        <div class="level-title">等级{{ item.level }}</div>
                        <div class="level-limits">
                          <div class="limit-item">
                            <span class="limit-label">日限制：</span>
                            <span class="limit-value">{{ item.dayMinLimit }}-{{ item.dayMaxLimit }}单</span>
                          </div>
                          <div class="limit-item">
                            <span class="limit-label">月限制：</span>
                            <span class="limit-value">{{ item.monthLimit }}单</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </ACollapsePanel>
            </ACollapse>
          </div>
        </div>
      </div>
    </ASpin>

    <!-- 确认弹窗组件 -->
    <AModal
      v-model:open="showConfirmModal"
      title="确认修改配对等级"
      ok-text="确认"
      cancel-text="取消"
      @ok="handleConfirmLevelChange"
      @cancel="showConfirmModal = false"
    >
      <div class="confirm-modal-content">
        <p class="confirm-message">{{ confirmModalContent }}</p>
        <div class="warning-section">
          <h4>注意事项：</h4>
          <ul>
            <li>首次配置将在次日 00:00:00 生效</li>
            <li>修改配置将在 6 天后的 00:00:00 生效</li>
            <li>生效期间请勿重复修改配置</li>
            <li>请确保系统时间准确，以免影响配置生效</li>
          </ul>
        </div>
      </div>
    </AModal>
    <AModal v-model:open="showSuccessModal" title="成功" :footer="null" @cancel="showSuccessModal = false">
      <p>{{ successModalContent }}</p>
    </AModal>
    <AModal v-model:open="showErrorModal" :closable="true" :mask-closable="true" :footer="null" title="提示">
      <div>{{ errorModalContent }}</div>
    </AModal>
  </div>
</template>

<style scoped>
.platform-setting {
  height: 100%;
  background-color: #fff;
}

.page-header {
  padding: 16px 24px;
  border-bottom: 1px solid #f0f0f0;
}

.header-title {
  font-size: 14px;
  color: #1f1f1f;
  line-height: 22px;
}

.page-content {
  padding: 24px;
}

.section-header {
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.section-title {
  font-size: 14px;
  color: #1f1f1f;
}

.info-icon {
  color: #1890ff;
  cursor: pointer;
  font-size: 14px;
}

.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}

.status-container {
  display: flex;
  align-items: center;
  gap: 16px;
}

.level-status {
  color: #666;
}

.effective-status {
  padding: 2px 8px;
  border-radius: 2px;
  background-color: #f5f5f5;
  color: #999;
}

.effective-status.is-effective {
  background-color: #f6ffed;
  color: #52c41a;
}

.platform-config {
  padding: 16px;
}

.box-title {
  font-size: 14px;
  font-weight: 500;
  color: #1f1f1f;
  margin-bottom: 12px;
}

.box-content.empty {
  color: #999;
  font-style: italic;
}

.config-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.label {
  color: #666;
  min-width: 80px;
}

.value {
  color: #1f1f1f;
}

.status-effective {
  color: #52c41a;
}

.select-area {
  margin-top: 24px;
}

.select-label {
  font-size: 14px;
  color: #1f1f1f;
  margin-bottom: 16px;
  font-weight: 500;
}

.level-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
}

.level-card {
  position: relative;
  background: #fff;
  border: 1px solid #e8e8e8;
  border-radius: 2px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.3s;
}

.level-card:hover:not(.disabled) {
  border-color: #1890ff;
  box-shadow: 0 2px 8px rgba(24, 144, 255, 0.15);
}

.level-card.selected {
  border-color: #1890ff;
  background-color: #e6f7ff;
}

.level-card.disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.level-card.history-selected {
  border: 1px dashed #1890ff;
  background-color: #f0f9ff;
}

.level-card.history-selected::after {
  content: '上次选择';
  position: absolute;
  top: 8px;
  right: 8px;
  font-size: 12px;
  color: #1890ff;
  background-color: #e6f7ff;
  padding: 2px 6px;
  border-radius: 2px;
}

.confirm-modal-content {
  padding: 16px 0;
}

.confirm-message {
  font-size: 14px;
  color: #1f1f1f;
  margin-bottom: 16px;
}

.warning-section {
  background-color: #fffbe6;
  border: 1px solid #ffe58f;
  padding: 12px 16px;
  border-radius: 2px;

  h4 {
    color: #d48806;
    margin-bottom: 8px;
    font-size: 14px;
  }

  ul {
    margin: 0;
    padding-left: 20px;
  }

  li {
    color: #666;
    font-size: 13px;
    line-height: 1.8;
  }
}

.status-info {
  display: flex;
  align-items: center;
  gap: 16px;
}

.current-level,
.effective-level {
  display: flex;
  align-items: center;
  gap: 4px;
}

.level-label {
  color: #666;
  font-size: 14px;
}

.level-value {
  color: #1f1f1f;
  font-weight: 500;
}

.config-group {
  background-color: #fff;
  padding: 16px;
  border-radius: 4px;
  border: 1px solid #f0f0f0;
}

.group-title {
  font-size: 14px;
  color: #1f1f1f;
  margin-bottom: 12px;
  font-weight: 500;
}

.box-content {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 16px;
}

.level-change-status {
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 2px;
  margin-left: 4px;
}

.level-change-status.upgrade {
  color: #1890ff;
  background-color: #e6f7ff;
}

.level-change-status.downgrade {
  color: #ff4d4f;
  background-color: #fff1f0;
}

.level-change-status.same {
  color: #52c41a;
  background-color: #f6ffed;
}

.config-note {
  font-size: 12px;
  color: #666;
  font-weight: normal;
  margin-left: 8px;
}

/* 在折叠面板标题部分应用样式 */
.panel-header .platform-name {
  display: inline-block;
}
.section-body {
  padding: 0 12px;
}

/* 折叠面板样式优化 */
:deep(.ant-collapse) {
  background: transparent;
  border: none;
}

:deep(.ant-collapse-item) {
  margin-bottom: 12px;
  border-radius: 8px !important;
  border: 1px solid #f0f0f0;
  overflow: hidden;
}

:deep(.ant-collapse-header) {
  background: #fafafa;
  transition: all 0.3s ease;
}

:deep(.ant-collapse-header:hover) {
  background: #f5f5f5;
}

/* 平台标题样式优化 */
.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 8px 0;
}

.platform-name {
  font-weight: 500;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 14px;
  transition: all 0.3s ease;
}

/* 平台特定样式优化 */
.platform-TB {
  color: #ff5000;
  background: linear-gradient(45deg, rgba(255, 80, 0, 0.08), rgba(255, 80, 0, 0.15));
  border: 1px solid rgba(255, 80, 0, 0.2);
}

.platform-JD {
  color: #ff0f23;
  background: linear-gradient(45deg, rgba(255, 15, 35, 0.08), rgba(255, 15, 35, 0.15));
  border: 1px solid rgba(255, 15, 35, 0.2);
}

.platform-DY {
  color: #000;
  background: linear-gradient(45deg, rgba(0, 0, 0, 0.05), rgba(0, 0, 0, 0.08));
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.platform-ALIBABA {
  color: #ff5000;
  background: linear-gradient(45deg, rgba(255, 80, 0, 0.08), rgba(255, 80, 0, 0.15));
  border: 1px solid rgba(255, 80, 0, 0.2);
}

/* 状态显示优化 */
.status-container {
  display: flex;
  align-items: center;
  gap: 16px;
  padding-right: 8px;
}

.effective-status {
  padding: 4px 12px;
  border-radius: 4px;
  font-size: 13px;
  transition: all 0.3s ease;
}

.effective-status.is-effective {
  background-color: #f6ffed;
  color: #52c41a;
  border: 1px solid #b7eb8f;
}

/* 选择卡片样式优化 */
.level-card {
  position: relative;
  background: #fff;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);
}

.level-card:hover:not(.disabled) {
  border-color: #1890ff;
  box-shadow: 0 4px 12px rgba(24, 144, 255, 0.15);
  transform: translateY(-2px);
}

.level-card.selected {
  border-color: #1890ff;
  background: linear-gradient(45deg, rgba(24, 144, 255, 0.05), rgba(24, 144, 255, 0.1));
}

/* 配置信息框样式优化 */
.current-config-box,
.history-config-box {
  background: #fff;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  border: 1px solid #f0f0f0;
}

.box-title {
  font-size: 15px;
  font-weight: 600;
  color: #1f1f1f;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f0f0f0;
}
</style>
