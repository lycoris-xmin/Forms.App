<script setup lang="ts">
  import { nextTick, onMounted, reactive } from 'vue';
  import { DownOutlined } from '@ant-design/icons-vue';
  import VueJsonPretty from 'vue-json-pretty';
  import { deleteRedisKeyApi, getRedisKeyListApi, getRedisMonitorApi, getRedisValueApi } from '@/service/api';
  import 'vue-json-pretty/lib/styles.css';
  import '@/styles/css/vue-json-pretty.css';
  import { useThemeStore } from '@/store/modules/theme';
  import { useClipboard } from '@/hooks/custome/useClipboard';
  import LoadingWrap from '@/components/loading/index.vue';

  interface RedisNode {
    title: string;
    key: string;
    type?: string;
    ttl?: string;
    isEnd?: string;
    children: Array<RedisNode>;
  }

  interface Model {
    list: Array<RedisNode>;
    currentKey: string;
    keyType: string;
    ttl: string;
    value: any;
    expandedKeys: Array;
    selectedKeys: Array;
    showType: number;
    showValue: string;
    showJsonValue?: object;
    mointor?: Api.Redis.Monitor;
  }

  const themeStore = useThemeStore();

  const { copy } = useClipboard();

  const model: Model = reactive({
    list: [],
    currentKey: '',
    keyType: '',
    ttl: '-1',
    value: '',
    expandedKeys: [],
    selectedKeys: [],
    showType: 0,
    showValue: '',
    showJsonValue: undefined,
    mointor: {}
  });

  const loading = reactive({
    keyLoading: false,
    detailLoading: false,
    deleteLoading: false
  });

  onMounted(() => {
    getRedisNodeList();
    monitorHandler();
  });

  async function getRedisNodeList() {
    loading.keyLoading = true;
    try {
      const { data: res, error } = await getRedisKeyListApi();
      if (!error && res && res.code === 0) {
        model.list = res.data.list;
      }
    } finally {
      loading.keyLoading = false;
    }
  }

  function keySelectHander(_, e) {
    if (e.node.isEnd) {
      model.showType = 0;
      getRedisValue(e.node.key, e.node.type);
    }
  }

  async function getRedisValue(key: string, keyType: string) {
    loading.detailLoading = true;
    try {
      const { data: res, error } = await getRedisValueApi(keyType, key);
      if (!error && res && res.code === 0) {
        if (res.data.ttl === 0) {
          window.$message?.warning('缓存已过期！');
          getRedisNodeList();
          return;
        }

        model.currentKey = key;
        model.keyType = keyType;
        model.ttl = res.data.ttl;
        model.showValue = '';
        model.showJsonValue = undefined;

        if (model.keyType === 'string') {
          model.value = res.data.value;
          model.showValue = res.data.value;
        } else {
          model.value = JSON.parse(res.data.value);

          if (model.keyType === 'zset' || model.keyType === 'hash') {
            model.value = model.value.map(x => {
              return {
                field: x.field,
                value: x.value,
                active: false
              };
            });

            liActiveHandler(0);
          }
        }
      }
    } finally {
      loading.detailLoading = false;
    }
  }

  async function getRedisMonitor() {
    loading.detailLoading = true;
    try {
      const { data: res, error } = await getRedisMonitorApi();
      if (!error && res && res.code === 0) {
        Object.assign(model.mointor, res.data);
      }
    } finally {
      loading.detailLoading = false;
    }
  }

  function refreshKeyHandler() {
    model.expandedKeys = [];
    nextTick(getRedisNodeList());
  }

  function showTypeChangeHandler() {
    if (model.showType === 1) {
      try {
        model.showJsonValue = JSON.parse(model.showValue);
      } catch {
        window.$message?.warning('当前字符串非json字符串，无法转成json格式');
        model.showType = 0;
      }
    } else {
      model.showValue = JSON.stringify(model.showJsonValue);
    }
  }

  function copyHandler() {
    if (model.showType !== 0) {
      model.showValue = JSON.stringify(model.showJsonValue);
    }

    copy(model.showValue);
  }

  function liActiveHandler(index) {
    model.value.forEach(x => (x.active = false));
    model.value[index].active = true;
    model.showValue = model.value[index].value;
    if (model.showType === 1) {
      try {
        model.showJsonValue = JSON.parse(model.showValue);
      } catch {
        window.$message?.warning('当前字符串非json字符串，无法转成json格式');
        model.showType = 0;
      }
    }
  }

  function monitorHandler() {
    model.currentKey = '';
    getRedisMonitor();
  }

  async function deleteKeyHandler() {
    loading.deleteLoading = true;

    try {
      const { data: res, error } = await deleteRedisKeyApi(model.currentKey);
      if (!error && res && res.code === 0) {
        window.$message?.success('删除成功');
        model.currentKey = '';
        model.value = '';
        refreshKeyHandler();
      }
    } finally {
      loading.deleteLoading = false;
    }
  }
</script>

<template>
  <div class="min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <div class="body">
      <ACard class="key" title="Key列表">
        <template #extra>
          <AButton type="primary" size="small" class="mr-3" ghost @click="monitorHandler">状态</AButton>
          <AButton type="primary" size="small" ghost @click="refreshKeyHandler">刷新</AButton>
        </template>
        <div>
          <ATree v-model:expanded-keys="model.expandedKeys" v-model:selected-keys="model.selectedKeys" show-line :tree-data="model.list" @select="keySelectHander">
            <template #switcherIcon="{ switcherCls }">
              <DownOutlined :class="switcherCls" />
            </template>
          </ATree>
        </div>

        <LoadingWrap :loading="loading.keyLoading"></LoadingWrap>
      </ACard>
      <ACard class="detail" title="详情">
        <div v-if="model.currentKey">
          <AForm>
            <div class="v-center flex gap-8px">
              <AFormItem class="detail-key" :label="model.keyType.toUpperCase()">
                <AInput v-model:value="model.currentKey" disabled></AInput>
              </AFormItem>
              <AFormItem class="detail-ttl" label="TTL">
                <AInput v-model:value="model.ttl" disabled></AInput>
              </AFormItem>
              <AFormItem class="detail-action">
                <AButton type="primary" size="small" ghost @click="() => getRedisValue(model.currentKey, model.keyType)">刷新</AButton>
              </AFormItem>
              <AFormItem class="detail-action">
                <APopconfirm title="确定要删除该缓存吗?" ok-text="确认" cancel-text="取消" @confirm="deleteKeyHandler">
                  <AButton danger size="small" :loading="loading.deleteLoading">删除</AButton>
                </APopconfirm>
              </AFormItem>
            </div>
            <div v-if="model.keyType === 'string'" class="value-container value-container-string">
              <div class="end v-center flex gap-8px">
                <AFormItem label="展示形式">
                  <ASelect v-model:value="model.showType" @change="showTypeChangeHandler">
                    <ASelectOption :value="0">文本</ASelectOption>
                    <ASelectOption :value="1">JSON</ASelectOption>
                  </ASelect>
                </AFormItem>
                <AFormItem>
                  <AButton type="primary" size="small" ghost @click="copyHandler">复制</AButton>
                </AFormItem>
                <AFormItem>
                  <AButton type="primary" size="small" ghost>保存</AButton>
                </AFormItem>
              </div>
              <div class="content">
                <ATextarea v-if="model.showType === 0" v-model:value="model.showValue" placeholder="" />
                <ACard v-else-if="model.showJsonValue">
                  <VueJsonPretty
                    v-model:data="model.showJsonValue"
                    class="json-editor"
                    :class="{ 'dark-mode': themeStore.darkMode }"
                    :show-line-number="true"
                    :editable="true"
                    editable-trigger="dblclick"
                    :theme="themeStore.darkMode ? 'dark' : 'light'"
                    :show-icon="true"
                  ></VueJsonPretty>
                </ACard>
              </div>
            </div>
            <div v-else-if="model.keyType === 'hash'" class="value-container value-container-hash">
              <div class="mb-3">
                <ul>
                  <li v-for="(item, index) in model.value" :key="item.field" class="hash" :class="{ active: item.active }" @click="() => liActiveHandler(index)">
                    <div>
                      <p>{{ item.field }}</p>
                    </div>
                    <div>
                      <p>{{ item.value }}</p>
                    </div>
                  </li>
                </ul>
              </div>
              <div>
                <div class="end v-center flex gap-8px">
                  <AFormItem label="展示形式">
                    <ASelect v-model:value="model.showType" @change="showTypeChangeHandler">
                      <ASelectOption :value="0">文本</ASelectOption>
                      <ASelectOption :value="1">JSON</ASelectOption>
                    </ASelect>
                  </AFormItem>
                  <AFormItem>
                    <AButton type="primary" ghost @click="copyHandler">复制</AButton>
                  </AFormItem>
                  <AFormItem>
                    <AButton type="primary" ghost>保存</AButton>
                  </AFormItem>
                </div>

                <div class="content">
                  <ATextarea v-if="model.showType === 0" v-model:value="model.showValue" placeholder="" />
                  <ACard v-else-if="model.showJsonValue">
                    <VueJsonPretty
                      v-model:data="model.showJsonValue"
                      class="json-editor"
                      :class="{ 'dark-mode': themeStore.darkMode }"
                      :show-line-number="true"
                      :editable="true"
                      editable-trigger="dblclick"
                      :theme="themeStore.darkMode ? 'dark' : 'light'"
                      :show-icon="true"
                    ></VueJsonPretty>
                  </ACard>
                </div>
              </div>
            </div>
          </AForm>
        </div>
        <div v-else-if="Object.keys(model.mointor).length">
          <div class="wrap flex gap-12px">
            <ACard class="server" title="基础信息">
              <div class="grid">
                <p class="text-right">redis版本：</p>
                <p>{{ model.mointor?.server.redisVersion }}</p>

                <p class="text-right">系统版本：</p>
                <p>{{ model.mointor?.server.os }}</p>

                <p class="text-right">运行天数：</p>
                <p>{{ model.mointor?.server.uptimeInDays }} 天</p>
              </div>
            </ACard>

            <ACard class="cpu" title="CPU信息">
              <div class="grid">
                <p class="text-right">系统级使用时间：</p>
                <p>{{ model.mointor?.cpu.usedCpuSys }} 秒</p>

                <p class="text-right">用户级使用时间：</p>
                <p>{{ model.mointor?.cpu.usedCpuUser }} 秒</p>

                <p class="text-right">运行负载：</p>
                <p>
                  <ATag v-if="model.mointor?.cpu.loadDescription.includes('低')" color="success">{{ model.mointor?.cpu.loadDescription }}</ATag>
                  <ATag v-else-if="model.mointor?.cpu.loadDescription.includes('中')" color="warning">{{ model.mointor?.cpu.loadDescription }}</ATag>
                  <ATag v-else color="error">{{ model.mointor?.cpu.loadDescription }}</ATag>
                </p>
              </div>
            </ACard>

            <ACard class="memory" title="内存信息">
              <div class="grid">
                <p class="text-right">已使用：</p>
                <p>{{ model.mointor?.memory.usedMemoryHuman }}</p>

                <p class="text-right">总内存：</p>
                <p>{{ model.mointor?.memory.maxMemHuman || '-' }}</p>

                <p class="text-right">运行负载：</p>
                <p>
                  <ATag v-if="model.mointor?.memory.loadDescription.includes('低')" color="success">{{ model.mointor?.memory.loadDescription }}</ATag>
                  <ATag v-else-if="model.mointor?.memory.loadDescription.includes('中')" color="warning">{{ model.mointor?.memory.loadDescription }}</ATag>
                  <ATag v-else-if="model.mointor?.memory.loadDescription.includes('高')" color="error">{{ model.mointor?.memory.loadDescription }}</ATag>
                  <ATag v-else>{{ model.mointor?.memory.loadDescription }}</ATag>
                </p>
              </div>
            </ACard>

            <ACard class="clients" title="客户端信息">
              <div class="grid">
                <p class="text-right">连接数：</p>
                <p>{{ model.mointor?.clients.connectedClients }}</p>

                <p class="text-right">运行负载：</p>
                <p>
                  <ATag v-if="model.mointor?.clients.loadDescription.includes('低')" color="success">{{ model.mointor?.clients.loadDescription }}</ATag>
                  <ATag v-else-if="model.mointor?.clients.loadDescription.includes('中')" color="warning">{{ model.mointor?.clients.loadDescription }}</ATag>
                  <ATag v-else-if="model.mointor?.clients.loadDescription.includes('高')" color="error">{{ model.mointor?.clients.loadDescription }}</ATag>
                  <ATag v-else>{{ model.mointor?.clients.loadDescription }}</ATag>
                </p>
              </div>
            </ACard>

            <ACard class="key" title="键信息">
              <div class="grid">
                <p class="text-right">键总数量：</p>
                <p>{{ model.mointor?.keyspace.keys }}</p>

                <p class="text-right">过期时间键数量：</p>
                <p>{{ model.mointor?.keyspace.expires }}</p>

                <p class="text-right">平均存活时间：</p>
                <p>{{ model.mointor?.keyspace.avgTtl }}</p>
              </div>
            </ACard>
          </div>
        </div>
        <LoadingWrap :loading="loading.detailLoading"></LoadingWrap>
      </ACard>
    </div>
  </div>
</template>

<style scoped lang="scss">
  .body {
    position: relative;
    display: grid;
    grid-template-columns: 350px 1fr;
    gap: 12px;
    height: 100%;
    overflow: hidden;

    .key {
      height: 100%;
      overflow-y: auto;

      :deep(.ant-card-body) {
        position: relative;
        height: calc(100% - 55px);
        overflow: hidden;
        overflow-y: auto;
      }

      :deep(.ant-tree-list) {
        overflow: hidden;

        .ant-tree-title {
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
          word-break: break-word;
          width: 250px;
          display: block;
        }
      }
    }

    .detail {
      height: 100%;
      overflow-y: auto;

      &-key {
        flex: 1;
      }

      &-ttl {
        width: 100px;
        flex-shrink: 0;
      }

      &-action {
        width: 48px;
        flex-shrink: 0;
      }

      :deep(.ant-card-body) {
        position: relative;
        height: calc(100% - 55px);
        overflow: hidden;
      }
    }

    .value-container {
      position: relative;
      overflow: hidden;

      &-string {
        .content {
          height: calc(100vh - 390px);
        }
      }

      &-hash {
        .content {
          height: calc(100vh - 705px);
        }
      }

      :deep(.ant-form-item) {
        &:has(.ant-select) {
          width: 200px;
          flex-shrink: 0;
        }

        .ant-select {
          width: 100%;
        }
      }
    }

    .content {
      position: relative;

      :deep(textarea) {
        height: 100% !important;
      }

      :deep(.ant-card) {
        height: 100%;

        .ant-card-body {
          overflow-y: auto;
        }
      }
    }

    ul {
      padding: 5px 10px;
      height: 300px;
      overflow-y: auto;
    }

    .hash {
      display: grid;
      grid-template-columns: repeat(2, 50%);
      border: 1px solid #d9d9d9;
      border-bottom: 0;
      cursor: pointer;

      &.active {
        background-color: #f7f0ff;

        * {
          color: #8b5cf6;
        }
      }

      &:last-child {
        border-bottom: 1px solid #d9d9d9;
      }

      > div {
        flex: 1;
        flex-shrink: 0;
        border-right: 1px solid #d9d9d9;
      }

      p {
        padding: 6px 10px;
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-all;
        white-space: nowrap;

        &:first-child {
          text-align: center;
        }
      }
    }

    .server {
      .grid {
        grid-template-columns: 75px 200px;
        gap: 8px;
      }
    }

    .cpu {
      .grid {
        grid-template-columns: 115px 200px;
        gap: 8px;
      }
    }

    .memory {
      .grid {
        grid-template-columns: 75px 200px;
        gap: 8px;
      }
    }

    .clients {
      .grid {
        grid-template-columns: 75px 200px;
        gap: 8px;
      }
    }

    .key {
      .grid {
        grid-template-columns: 115px 200px;
        gap: 8px;
      }
    }
  }
</style>
