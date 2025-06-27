<script setup lang="tsx">
import { imgFallback } from '@/data/empty.json';
interface Props {
  subProductsData?: Array[any];
}
const props: Props = withDefaults(defineProps<Props>(), {
  subProductsData: () => []
});

const visible = defineModel<boolean>('visible', {
  default: false
});
function redirectToProductPage(url) {
  if (url) {
    window.open(url);
  } else {
    window.$message?.warning('该商品没有添加商品链接，无法跳转');
  }
}
</script>

<template>
  <AModal v-model:open="visible" title="商品详情" :width="800" :footer="null">
    <ul class="products">
      <li v-for="item in props.subProductsData" :key="item.id">
        <div class="v-center mb-6px flex gap-8px">
          <ATag v-if="item.main">主商品</ATag>
          <p class="title" @click="redirectToProductPage(item.url)">{{ item.title }}</p>
        </div>
        <div class="v-center flex gap-8px">
          <div class="products-image">
            <AImage :width="100" :height="100" :src="item.image" :fallback="imgFallback" />
          </div>
          <div class="products-body">
            <div class="sku">
              <div v-if="item.skuList && item.skuList?.length" class="wrap v-center flex gap-5px">
                商品SKU:
                <ATag v-for="sku in item.skuList" :key="sku">{{ sku }}</ATag>
              </div>
              <div v-else class="wrap v-center flex gap-5px">
                商品SKU:
                <ATag>该商品无Sku</ATag>
              </div>
            </div>
            <div class="mb-6px">
              <p>商品关键词:{{ item.keyword ?? '—' }}</p>
            </div>
            <div class="v-center flex gap-8px">
              <div class="v-center num-container flex gap-8px">
                <span class="display-item">
                  <span class="prefix">价格</span>
                  <span class="value highlight">{{ item.price }}</span>
                  <span class="suffix">元</span>
                </span>
                <span class="display-item">
                  <span class="prefix">购买</span>
                  <span class="value">{{ item.count }}</span>
                  <span class="suffix">件</span>
                </span>
                <span class="display-item">
                  <span class="prefix">浮动差额 ±</span>
                  <span class="value highlight">{{ item.scope }}</span>
                  <span class="suffix">元</span>
                </span>
              </div>
            </div>
          </div>
        </div>
      </li>
    </ul>
  </AModal>
</template>

<style lang="scss" scoped>
.products {
  list-style: none;
  min-height: 20px;

  .title {
    overflow: hidden;
    word-break: break-all;
    white-space: nowrap;
    text-overflow: ellipsis;
    cursor: pointer;
    transition: all 0.3s ease-in-out;
    font-weight: 600;

    &:hover {
      color: #646cff;
    }
  }

  :deep(.ant-form-item-label) {
    > label {
      width: 100%;
    }

    .space-between {
      width: 100%;
    }
  }

  li {
    padding: 12px 0;
    margin-bottom: 6px;
    border-bottom: 1px solid #d9d9d9;
  }

  &-image {
    height: 100px;
    width: 100px;
    overflow: hidden;
    border-radius: 5px;
    flex-shrink: 0;
  }

  &-body {
    min-height: 100px;
    width: 100%;

    .sku {
      margin-bottom: 6px;

      :deep(.ant-select) {
        width: 100%;
      }
    }

    .del-btn {
      flex-shrink: 0;
    }
  }
}

.display-item {
  display: inline-flex;
  align-items: center;
  gap: 5px;

  position: relative;

  font-size: 14px;

  &:not(:last-child)::after {
    content: ' | ';
    margin-left: 8px;
    font-size: 14px;
  }

  .value,
  .suffix {
    font-size: 14px;
  }

  .prefix {
    font-size: 12px;
  }

  .value {
    font-weight: 500;
  }

  .value.highlight {
    color: #ff4d4f; /* 高亮颜色 */
  }

  .suffix {
    font-size: 12px;
  }
}

.flex-1 {
  padding-right: 10px;
}

.page {
  position: relative;
  min-height: 70vh;
}
</style>
