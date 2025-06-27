<script setup lang="ts">
import type { DocView } from '../shard';

const props = defineProps<DocView>();
</script>

<template>
  <div class="doc-view">
    <div class="title text-center">{{ props.title }}</div>
    <div class="container">
      <div v-for="item in props.content" :key="item.title" class="row">
        <p class="title">{{ item.title }}</p>
        <div v-if="item.text && item.text.length">
          <p v-for="text in item.text" :key="text">{{ text }}</p>
        </div>

        <div v-else-if="item.list && item.list.length">
          <div v-for="(list, index) in item.list" :key="index">
            <p class="sub-title">{{ list.title }}</p>
            <ul class="sub-li">
              <li v-for="text in list.text" :key="text">
                <p>{{ text }}</p>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.doc-view {
  letter-spacing: 1.5px;
  padding: 0 15px;

  > .title {
    font-size: 32px;
    font-weight: 600;
    margin-bottom: 60px;
  }

  .container {
    margin: 0 auto;

    .row {
      margin-bottom: 24px;
    }

    .title {
      font-size: 24px;
      font-weight: 600;
    }

    .sub-title {
      font-size: 18px;
      font-weight: 600;
    }

    .sub-li {
      list-style: disc;
      padding-left: 26px;
    }

    p {
      margin-bottom: 12px;
    }
  }
}
</style>
