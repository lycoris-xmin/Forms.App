<script setup lang="ts">
import { reactive, watch } from 'vue';
import { fetchGetDeivceCommandRecordList } from '@/service/api';
import { commandStatus as COMMAND_STATUS, commandType as COMMAND_TYPE } from '@/views/shared/enums';
import { imgFallback } from '@/data/empty.json';
import LoadingWrap from '@/components/loading/index.vue';

type Props = {
  id: string;
};

type RecordData = {
  command: number;
  commandMap: object;
  data: string;
  dataJson: object;
  status: number;
  statusMap: object;
  result?: string | null;
  resultJson?: object | null;
  createTime: string;
  ackTime?: string | null;
  completeTime?: string | null;
};

type Model = {
  count: number;
  list: Array<RecordData>;
  loading: boolean;
};

type SearchFilter = Pick<
  Api.Device.CommandRecordSearchFilter,
  'command' | 'beginTime' | 'endTime' | 'pageIndex' | 'pageSize'
>;

defineOptions({
  name: 'DeviceRecordDrawer'
});

const props = defineProps<Props>();

const visible = defineModel<boolean>('visible', { default: false });

const searchFilter = reactive<SearchFilter>({
  command: '',
  beginTime: '',
  endTime: '',
  pageIndex: 1,
  pageSize: 10
});

const model = reactive<Model>({
  count: 0,
  list: [],
  loading: true
});

watch(visible, async value => {
  if (value) {
    await getDeviceCommandRecord();
  }
});

async function getDeviceCommandRecord() {
  model.loading = true;
  try {
    const { data: res, error } = await fetchGetDeivceCommandRecordList({ ...searchFilter, deviceId: props.id });
    if (!error && res && res.code === 0) {
      Object.assign(model.list, res.data.list);
      model.count = res.data.count;

      for (const item of model.list) {
        item.commandMap = getCommandMap(item.command);
        item.statusMap = getStatusMap(item.status);

        item.dataJson = getDataJson(item);
        item.resultJson = getResultJson(item);
      }
    }
  } finally {
    model.loading = false;
  }
}

function getCommandMap(command) {
  const item = COMMAND_TYPE.filter(x => x.value === command);
  if (item && item.length) {
    return item[0];
  }

  return {
    label: '未知指令',
    color: 'error'
  };
}

function getStatusMap(status) {
  const item = COMMAND_STATUS.filter(x => x.value === status);
  if (item && item.length) {
    return item[0];
  }

  return {
    label: '未知状态',
    color: 'error'
  };
}

function getDataJson(item) {
  if (!item.data) {
    return '';
  }

  try {
    const tmp = JSON.parse(item.data);

    if (item.command === 10) {
      delete tmp.keyword;
      delete tmp.skuList;
      delete tmp.price;
      delete tmp.scope;
      delete tmp.count;
      delete tmp.addCart;
      delete tmp.favorite;
      delete tmp.address;
      delete tmp.payType;
      delete tmp.otherPayPhone;
      return tmp;
    }

    return tmp;
  } catch {
    return item.data;
  }
}

function getResultJson(item) {
  try {
    const tmp = JSON.parse(item.result);
    if (tmp && Object.keys(tmp).length) {
      return tmp;
    }

    return item.result;
  } catch {
    return item.result;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="执行记录" width="50vw" :footer="null">
    <div class="body">
      <div class="list" :class="{ loading: model.loading }">
        <AList item-layout="vertical" :data-source="model.list">
          <template #renderItem="{ item }">
            <AListItem key="item.id">
              <AListItemMeta v-if="item.command === 10" :description="`店铺名称：${item.dataJson.shopName}`">
                <template #avatar>
                  <ATag :color="item.commandMap.color">{{ item.commandMap.label }}</ATag>
                </template>

                <template #title>
                  <a
                    v-if="item.dataJson.productUrl"
                    :href="item.dataJson.productUrl"
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    {{ item.dataJson.title }}
                  </a>
                  <p v-else>{{ item.dataJson.title }}</p>
                </template>
              </AListItemMeta>

              <AListItemMeta v-else-if="item.command === 1000 || item.command === 1001">
                <template #avatar>
                  <ATag :color="item.commandMap.color">{{ item.commandMap.label }}</ATag>
                </template>

                <template #title>
                  <p v-if="item.dataJson.isOpenHostpost">开启热点</p>
                  <p v-else>关闭热点</p>
                </template>
              </AListItemMeta>

              <AListItemMeta v-else-if="item.command === 1002">
                <template #avatar>
                  <ATag :color="item.commandMap.color">{{ item.commandMap.label }}</ATag>
                </template>
              </AListItemMeta>

              <AListItemMeta v-else-if="item.command === 1003">
                <template #avatar>
                  <ATag :color="item.commandMap.color">{{ item.commandMap.label }}</ATag>
                </template>
              </AListItemMeta>

              <AListItemMeta v-else>
                <template #avatar>
                  <ATag :color="item.commandMap.color">{{ item.commandMap.label }}</ATag>
                </template>
              </AListItemMeta>

              <template v-if="item.command === 10 && item.dataJson?.productImage" #extra>
                <div class="product-img">
                  <AImage
                    :width="100"
                    :height="100"
                    :alt="item.dataJson.title"
                    :src="item.dataJson.productImage"
                    :fallback="imgFallback"
                  />
                </div>
              </template>

              <!-- <p v-if="item.command === 10" class="mb-3">关键词：{{ item.dataJson.keyword }}</p> -->

              <div v-if="item.command === 1000 && item.dataJson.isOpenHostpost" class="v-center mb-3 flex gap-15px">
                <p>Wifi名称：{{ item.dataJson.wifiName }}</p>
                <p>Wifi密码：{{ item.dataJson.wifiPassword }}</p>
                <p>使用设备：{{ item.dataJson.useWifiDeviceId }}</p>
              </div>

              <div v-if="item.status === 31" class="v-center flex gap-10px">
                <ATag :color="item.statusMap.color">{{ item.statusMap.label }}</ATag>
                <p class="result">失败原因：{{ item.result }}</p>
              </div>
              <div v-else class="v-center flex gap-10px">
                <ATag :color="item.statusMap.color">{{ item.statusMap.label }}</ATag>
              </div>

              <template #actions>
                <span>创建：{{ item.createTime }}</span>
                <span>执行：{{ item.ackTime || '-' }}</span>
                <span>完成：{{ item.completeTime || '-' }}</span>
              </template>
            </AListItem>
          </template>
        </AList>
        <LoadingWrap :loading="model.loading"></LoadingWrap>
      </div>
      <div class="pagination v-center center flex">
        <APagination
          v-model:current="searchFilter.pageIndex"
          size="small"
          :total="model.count"
          show-less-items
          @change="() => getDeviceCommandRecord"
        />
      </div>
    </div>
  </ADrawer>
</template>

<style scoped lang="scss">
.list {
  position: relative;
  height: 80vh;
  overflow: hidden;
  overflow-y: auto;
  border-top: 1px solid #f0f0f0;
  border-bottom: 1px solid #f0f0f0;

  &.loading {
    overflow-y: hidden;
  }

  :deep(.ant-spin-nested-loading) {
    height: 100%;
  }

  :deep(.ant-list-item-meta) {
    display: flex;
    align-items: center;

    .ant-list-item-meta-title {
      margin: 0;
    }
  }

  .product-img {
    overflow: hidden;
    border-radius: 5px;
    height: 100px;
    width: 100px;
  }

  p.result {
    color: #8c8c8c;
  }
}

.pagination {
  padding: 30px 0 10px 0;
}
</style>
