<script setup lang="ts">
import { reactive, watch } from 'vue';
import { fetchGetPlanTaskDetail } from '@/service/api';
import { payType as PAY_TYPE } from '@/views/shared/enums';
import LoadingWrap from '@/components/loading/index.vue';

type Poprs = {
  id?: string;
  planTaskStatus?: Array;
  mode?: number;
};

type Model = Api.PlanTask.PlanTaskDetail & {
  id: string;
  loading: boolean;
};

defineOptions({
  name: 'PlanTaskDetailDrawer'
});

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = withDefaults(defineProps<Poprs>(), {
  id: '',
  planTaskStatus: () => [],
  mode: () => undefined
});

const model = reactive<Model>({
  id: '',
  owner: {},
  pair: {},
  loading: true
});

watch(visible, async value => {
  if (value) {
    await getDetail();
  }
});

async function getDetail() {
  model.loading = true;
  try {
    const { data: res, error } = await fetchGetPlanTaskDetail(props.id);
    if (error || !res || res.code !== 0) {
      return;
    }

    Object.assign(model.owner, res.data.owner);
    model.owner.statusMap = getStatusMap(model.owner?.status);
    model.owner.payTypeMap = getPayTypeMap(model.owner?.payType);

    if (props.mode === 0) {
      Object.assign(model.pair, res.data.pair);
      model.pair.statusMap = getStatusMap(model.pair?.status);
      model.pair.payTypeMap = getPayTypeMap(model.pair?.payType);
    }

    model.id = props.id;
  } catch {
    visible.value = false;
  } finally {
    model.loading = false;
  }
}

function getStatusMap(status) {
  const statusItem = props.planTaskStatus.find(x => x.value === status);
  return {
    label: statusItem?.label || '未知异常',
    color: statusItem?.color || 'error'
  };
}

function getPayTypeMap(payType) {
  const payTypeItem = PAY_TYPE.find(x => x.value === payType);
  return {
    label: payTypeItem?.label || '未知',
    color: payTypeItem?.color || 'error'
  };
}
</script>

<template>
  <AModal v-model:open="visible" title="任务详情" :width="props.mode === 0 ? '70vw' : '40vw'" :footer="null">
    <div class="body">
      <div v-if="props.mode === 0" class="detail grid-mode grid">
        <div>
          <p class="title">我方任务详情</p>
          <div class="descriptions">
            <div class="grid">
              <div class="item">
                <div class="label">
                  <p>商品标题</p>
                </div>
                <div class="content">
                  <p>{{ model.owner?.title }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>店铺名称</p>
                </div>
                <div class="content">
                  <p>{{ model.owner?.shopName }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>关键词</p>
                </div>
                <div class="content">
                  <p>{{ model.owner?.keyword }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>商品SKU</p>
                </div>
                <div class="content">
                  <div v-if="model.owner?.skuList && model.owner.skuList.length" class="wrap v-center flex gap-5px">
                    <ATag v-for="item in model.owner?.skuList" :key="item">{{ item }}</ATag>
                  </div>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>购买件数</p>
                </div>
                <div class="content">
                  <p>x {{ model.owner?.count }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>购买单价</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.owner?.price }}</p>
                </div>
              </div>
            </div>

            <div class="grid">
              <div class="item">
                <div class="label">
                  <p>商品总价</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.owner?.totalPrice }}</p>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>购买差额</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.owner?.scope }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>配对差额</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.owner?.pairScope }}</p>
                </div>
              </div>
            </div>

            <div class="grid">
              <div class="item">
                <div class="label">
                  <p>商品互动</p>
                </div>
                <div class="content">
                  <ATag v-if="model.owner?.addCart" color="orange">加购</ATag>
                  <ATag v-if="model.owner?.favorite" color="pink">收藏</ATag>
                  <ATag v-if="!model.owner?.addCart && !model.owner?.favorite">无</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>排除区域</p>
                </div>
                <div v-if="model.owner?.excludeArea && model.owner.excludeArea.length" class="content">
                  <ATag v-for="item in model.owner?.excludeArea" :key="item">{{ item }}</ATag>
                </div>
                <div class="content">
                  <ATag>无</ATag>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>购买状态</p>
                </div>
                <div class="content">
                  <ATag v-if="model.owner?.success" color="success">购买成功</ATag>
                  <ATag v-else-if="model.owner?.status >= 30" color="error">购买失败</ATag>
                  <ATag v-else>-</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>任务状态</p>
                </div>
                <div class="content">
                  <ATag v-if="model.owner?.statusMap" :color="model.owner?.statusMap.color">
                    {{ model.owner?.statusMap.label }}
                  </ATag>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>订单号</p>
                </div>
                <div class="content">
                  <p v-if="model.owner?.success">{{ model.owner?.orderId }}</p>
                  <ATag v-else>-</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>支付金额</p>
                </div>
                <div class="content">
                  <p v-if="model.owner?.success" class="cny">{{ model.owner?.payPrice }}</p>
                  <ATag v-else>-</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>支付方式</p>
                </div>
                <div class="content">
                  <ATag :color="model.owner?.payTypeMap?.color">{{ model.owner?.payTypeMap?.label }}</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>支付时间</p>
                </div>
                <div class="content">
                  <p v-if="model.owner?.success">{{ model.owner?.completeTime }}</p>
                  <ATag v-else>-</ATag>
                </div>
              </div>
            </div>

            <div class="grid">
              <div class="item item-border-hide">
                <div class="label">
                  <p>任务差额</p>
                </div>
                <div class="content">
                  <p v-if="model.owner?.diffPrice && Number.parseFloat(model.owner.diffPrice) !== 0">
                    {{ model.owner.diffPrice.includes('-') ? '我方需补差额 ' : '对方需补差额 ' }}
                    <span class="cny">{{ model.owner.diffPrice.replace('-', '') }}</span>
                  </p>
                  <ATag v-else type="success">无需补差价</ATag>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div v-if="model.pair">
          <p class="title">对方任务详情</p>
          <div class="descriptions">
            <div class="grid">
              <div class="item">
                <div class="label">
                  <p>商品标题</p>
                </div>
                <div class="content">
                  <p>{{ model.pair?.title }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>店铺名称</p>
                </div>
                <div class="content">
                  <p>{{ model.pair?.shopName }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>关键词</p>
                </div>
                <div class="content">
                  <p>{{ model.pair?.keyword }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>商品SKU</p>
                </div>
                <div class="content">
                  <div v-if="model.pair?.skuList && model.pair.skuList.length" class="wrap v-center flex gap-5px">
                    <ATag v-for="item in model.pair?.skuList" :key="item">{{ item }}</ATag>
                  </div>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>购买件数</p>
                </div>
                <div class="content">
                  <p>x {{ model.pair?.count }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>购买单价</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.pair?.price }}</p>
                </div>
              </div>
            </div>

            <div class="grid">
              <div class="item">
                <div class="label">
                  <p>商品总价</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.pair?.totalPrice }}</p>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>购买差额</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.pair?.scope }}</p>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>配对差额</p>
                </div>
                <div class="content">
                  <p class="cny">{{ model.pair?.pairScope }}</p>
                </div>
              </div>
            </div>

            <div class="grid">
              <div class="item">
                <div class="label">
                  <p>商品互动</p>
                </div>
                <div class="content">
                  <ATag v-if="model.pair?.addCart" color="orange">加购</ATag>
                  <ATag v-if="model.pair?.favorite" color="pink">收藏</ATag>
                  <ATag v-if="!model.pair?.addCart && !model.pair?.favorite">无</ATag>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>购买状态</p>
                </div>
                <div class="content">
                  <ATag v-if="model.pair?.success" color="success">购买成功</ATag>
                  <ATag v-else-if="model.pair?.status >= 30" color="error">购买失败</ATag>
                  <ATag v-else>-</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>任务状态</p>
                </div>
                <div class="content">
                  <ATag v-if="model.pair?.statusMap" :color="model.pair?.statusMap.color">
                    {{ model.pair?.statusMap.label }}
                  </ATag>
                </div>
              </div>
            </div>

            <div class="grid-2 grid">
              <div class="item">
                <div class="label">
                  <p>订单号</p>
                </div>
                <div class="content">
                  <p v-if="model.pair?.success">{{ model.pair?.orderId }}</p>
                  <ATag v-else>-</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>支付金额</p>
                </div>
                <div class="content">
                  <p v-if="model.pair?.success" class="cny">{{ model.pair?.payPrice }}</p>
                  <ATag v-else>-</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>支付方式</p>
                </div>
                <div class="content">
                  <ATag :color="model.pair?.payTypeMap?.color">{{ model.pair?.payTypeMap?.label }}</ATag>
                </div>
              </div>
              <div class="item">
                <div class="label">
                  <p>支付时间</p>
                </div>
                <div class="content">
                  <p v-if="model.pair?.success">{{ model.pair?.completeTime }}</p>
                  <ATag v-else>-</ATag>
                </div>
              </div>
            </div>

            <div class="grid">
              <div class="item item-border-hide">
                <div class="label">
                  <p>执行设备</p>
                </div>
                <div class="content">
                  {{ `${model.pair?.deviceName} ( ${model.pair?.deviceId} )` }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div v-else class="detail">
        <div class="descriptions">
          <div class="grid">
            <div class="item">
              <div class="label">
                <p>商品标题</p>
              </div>
              <div class="content">
                <p>{{ model.owner?.title }}</p>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>店铺名称</p>
              </div>
              <div class="content">
                <p>{{ model.owner?.shopName }}</p>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>关键词</p>
              </div>
              <div class="content">
                <p>{{ model.owner?.keyword }}</p>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>商品SKU</p>
              </div>
              <div class="content">
                <div v-if="model.owner?.skuList && model.owner.skuList.length" class="wrap v-center flex gap-5px">
                  <ATag v-for="item in model.owner?.skuList" :key="item">{{ item }}</ATag>
                </div>
              </div>
            </div>
          </div>

          <div class="grid-2 grid">
            <div class="item">
              <div class="label">
                <p>购买件数</p>
              </div>
              <div class="content">
                <p>x {{ model.owner?.count }}</p>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>购买单价</p>
              </div>
              <div class="content">
                <p class="cny">{{ model.owner?.price }}</p>
              </div>
            </div>
          </div>

          <div class="grid">
            <div class="item">
              <div class="label">
                <p>商品总价</p>
              </div>
              <div class="content">
                <p class="cny">{{ model.owner?.totalPrice }}</p>
              </div>
            </div>
          </div>

          <div class="grid-2 grid">
            <div class="item">
              <div class="label">
                <p>购买差额</p>
              </div>
              <div class="content">
                <p class="cny">{{ model.owner?.scope }}</p>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>配对差额</p>
              </div>
              <div class="content">
                <p>
                  <ATag>-</ATag>
                </p>
              </div>
            </div>
          </div>

          <div class="grid">
            <div class="item">
              <div class="label">
                <p>商品互动</p>
              </div>
              <div class="content">
                <ATag v-if="model.owner?.addCart" color="orange">加购</ATag>
                <ATag v-if="model.owner?.favorite" color="pink">收藏</ATag>
                <ATag v-if="!model.owner?.addCart && !model.owner?.favorite">无</ATag>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>排除区域</p>
              </div>
              <div v-if="model.owner?.excludeArea && model.owner.excludeArea.length" class="content">
                <ATag v-for="item in model.owner?.excludeArea" :key="item">{{ item }}</ATag>
              </div>
              <div class="content">
                <ATag>无</ATag>
              </div>
            </div>
          </div>

          <div class="grid-2 grid">
            <div class="item">
              <div class="label">
                <p>购买状态</p>
              </div>
              <div class="content">
                <ATag v-if="model.owner?.success" color="success">购买成功</ATag>
                <ATag v-else-if="model.owner?.status >= 30" color="error">购买失败</ATag>
                <ATag v-else>-</ATag>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>任务状态</p>
              </div>
              <div class="content">
                <ATag v-if="model.owner?.statusMap" :color="model.owner?.statusMap.color">
                  {{ model.owner?.statusMap.label }}
                </ATag>
              </div>
            </div>
          </div>

          <div class="grid-2 grid">
            <div class="item">
              <div class="label">
                <p>订单号</p>
              </div>
              <div class="content">
                <p v-if="model.owner?.success">{{ model.owner?.orderId }}</p>
                <ATag v-else>-</ATag>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>支付金额</p>
              </div>
              <div class="content">
                <p v-if="model.owner?.success" class="cny">{{ model.owner?.payPrice }}</p>
                <ATag v-else>-</ATag>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>支付方式</p>
              </div>
              <div class="content">
                <ATag :color="model.owner?.payTypeMap?.color">{{ model.owner?.payTypeMap?.label }}</ATag>
              </div>
            </div>
            <div class="item">
              <div class="label">
                <p>支付时间</p>
              </div>
              <div class="content">
                <p v-if="model.owner?.success">{{ model.owner?.completeTime }}</p>
                <ATag v-else>-</ATag>
              </div>
            </div>
          </div>

          <div class="grid">
            <div class="item item-border-hide">
              <div class="label">
                <p>执行设备</p>
              </div>
              <div class="content">
                {{ `${model.owner?.deviceName} ( ${model.owner?.deviceId} )` }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <LoadingWrap :loading="model.loading"></LoadingWrap>
    </div>
  </AModal>
</template>

<style scoped lang="scss">
.body {
  position: relative;
}
.grid-mode {
  width: 100%;
  grid-template-columns: repeat(2, 50%);

  > div {
    &:first-child {
      padding-right: 15px;
    }

    &:last-child {
      padding-left: 15px;
    }

    .title {
      font-size: 16px;
      font-weight: 600;
      letter-spacing: 1.5px;
      padding-bottom: 8px;
    }
  }
}

.detail {
  margin-top: 15px;

  :deep(.ant-tag) {
    overflow: hidden;
    white-space: break-spaces;
  }
}

.detail {
  .descriptions {
    border: 1px solid #f0f0f0;
    border-radius: 10px;
    overflow: hidden;

    .grid {
      grid-template-columns: minmax(400px, 1fr);
      gap: 0px;
      padding: 0;
      margin: 0;

      &-2 {
        grid-template-columns: repeat(2, 50%);

        .item {
          &:last-child {
            border-left: 1px solid #f0f0f0;
          }
        }
      }
    }

    .item {
      display: grid;
      grid-template-columns: 100px minmax(300px, 1fr);
      border-bottom: 1px solid #f0f0f0;

      &-border-hide {
        border: 0;
      }

      .label {
        width: 100%;
        background-color: #fafafa;
        border-right: 1px solid #f0f0f0;
        display: flex;
        justify-content: center;
        align-items: center;

        p {
          padding: 8px;
        }
      }

      .content {
        padding: 8px;
        width: 100%;
        display: flex;
        align-items: center;
      }
    }
  }
}
</style>
