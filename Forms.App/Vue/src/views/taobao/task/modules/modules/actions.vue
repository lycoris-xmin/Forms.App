<script setup lang="ts">
import { Popover } from 'ant-design-vue';
import { ref } from 'vue';
import { QuestionCircleOutlined } from '@ant-design/icons-vue';

defineOptions({
  name: 'PlanTaskActions'
});

const model = defineModel<Api.PlanTask.Actions>('actions', {
  default: {
    shopAround: {
      min: 3,
      max: 5
    },
    addCart: false,
    favorite: false,
    lookComment: true,
    lookPlantingGrass: true,
    lookAskEveryone: true,
    useFirstBrowse: true,
    useFindBrowse: true,
    useWaitForBuy: true
  }
});

const popoverItems = ref([
  {
    key: 'addCart',
    label: '商品加购',
    content: [
      {
        text: '将商品添加至购物车，不勾选则根据随机进行购物车或立即购买流程'
      },
      { text: '注意：多链接商品只能走加购流程' }
    ]
  },
  {
    key: 'favorite',
    label: '商品收藏',
    content: [{ text: '收藏商品，不勾选不收藏' }, { text: '注意：多链接情况下，该行为只会在主商品下生效' }]
  },
  {
    key: 'useFirstBrowse',
    label: '随机搜索浏览多次',
    content: [
      {
        text: '根据关键词搜索相关推荐词进行浏览，不勾选不进行'
      },
      { text: '注意：推荐词不一定是当前商品类目,多链接情况下，该行为只会在主商品下生效' }
    ]
  },
  {
    key: 'lookAskEveryone',
    label: '随机看问大家',
    extra: '（3-10次动态）',
    content: [
      { text: '浏览商品页问大家随机页数，如果商品有的情况下进行浏览，不勾选不进行' },
      { text: '注意：多链接情况下，该行为只会在主商品下生效' }
    ]
  },
  {
    key: 'lookComment',
    label: '随机看商品评价',
    extra: '（3-10次动态）',
    content: [
      {
        text: '浏览商品评价随机页数，不勾选则不进行'
      },
      { text: '注意：多链接情况下，该行为只会在主商品下生效' }
    ]
  },
  {
    key: 'lookPlantingGrass',
    label: '随机看种草信息',
    extra: '（3-10次动态）',
    content: [
      {
        text: '浏览商品种草随机页数，如果商品有的情况下进行浏览，不勾选不进行'
      },
      { text: '注意：多链接情况下，该行为只会在主商品下生效' }
    ]
  },
  {
    key: 'useWaitForBuy',
    label: '浏览后随机等待分钟数付款',
    content: [
      {
        text: '去往购物车付款之前会返回桌面等待1-3分钟后进行购买,不勾选不进行'
      },
      { text: '注意：必须勾选商品加购流程才会触发，如果是多链接情况下，该行为只会在主商品下生效' }
    ]
  },
  {
    key: 'useFindBrowse',
    label: '找到指定商品后不购买，再次随机浏览后下单',
    content: [
      {
        text: '找到目标商品后，继续向下随机浏览多次再返回下单，不勾选不进行'
      },
      { text: '注意：多链接情况下，该行为只会在主商品下生效' }
    ]
  }
]);
</script>

<template>
  <AFormItem>
    <template #label>
      <p>
        货比三家
        <small class="text-danger ml-1">* 都为0不进行货比</small>
      </p>
    </template>
    <div class="v-center flex gap-8px">
      <AInputNumber v-model:value="model.shopAround.min" :min="0" :controls="false" :precision="0">
        <template #addonBefore>
          <span>最少</span>
        </template>
        <template #addonAfter>
          <span>次</span>
        </template>
      </AInputNumber>
      <AInputNumber v-model:value="model.shopAround.max" :min="0" :controls="false" :precision="0">
        <template #addonBefore>
          <span>最多</span>
        </template>
        <template #addonAfter>
          <span>次</span>
        </template>
      </AInputNumber>
    </div>
  </AFormItem>

  <AFormItem label="商品互动">
    <ul class="actions">
      <li v-for="item in popoverItems" :key="item.key">
        <ACheckbox v-model:checked="model[item.key]">
          {{ item.label }}
          <span v-if="item.extra" class="text-danger">{{ item.extra }}</span>
          <Popover>
            <template #content>
              <div class="popover-content">
                <p
                  v-for="(content, index) in item.content"
                  :key="index"
                  :class="{ 'text-bold': content.text.includes('注意') }"
                >
                  {{ content.text }}
                </p>
              </div>
            </template>
            <QuestionCircleOutlined
              class="small-icon"
              @click.stop="
                e => {
                  e.preventDefault();
                }
              "
            />
          </Popover>
        </ACheckbox>
      </li>
    </ul>
  </AFormItem>
</template>

<style scoped lang="scss">
.actions {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 8px;

  li {
    list-style: none;
    margin-bottom: 14px;
  }
}

.ant-input-number-group-wrapper {
  width: 100% !important;
}

.small-icon {
  font-size: 14px;
}

.v-center {
  margin-bottom: 16px;
}

.small-icon:hover {
  cursor: pointer;
}

.text-bold {
  font-weight: bold;
}
</style>
