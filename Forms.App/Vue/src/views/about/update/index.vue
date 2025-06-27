<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import LoadingWrap from '@/components/loading/index.vue';
import initJson from '@/data/initupdate.json';
import UseVisual from './shard';

type UpdateData = {
  time: string;
  title: string;
  content: Array<string>;
};

type Model = {
  loading: boolean;
  visual: object;
  list: Array<UpdateData>;
};

const model = reactive<Model>({
  loading: true,
  visual: {},
  list: []
});

onMounted(() => {
  model.visual = new UseVisual('#canvas');

  initUpdateData();

  model.loading = false;
});

function initUpdateData() {
  initUpdateV1Data();
}

function initUpdateV1Data() {
  // 从initJson中获取数据并转换为需要的格式
  const versionData = initJson.initupdateV1.content;

  versionData.forEach(item => {
    model.list.push({
      time: item.time,
      title: item.title,
      content: item.text
    });
  });

  // model.list.push({
  //   time: '2020-01-01',
  //   title: 'v1.0.0',
  //   content: ['任务模式增加指定设备', '设备心跳模式调整']
  // });

  // model.list.push({
  //   time: '2020-01-01',
  //   title: 'v1.0.1',
  //   content: ['描述测试', '描述测试', '描述测试', '描述测试', '描述测试']
  // });

  // model.list.push({
  //   time: '2020-01-01',
  //   title: 'v1.0.2',
  //   content: ['描述测试', '描述测试', '描述测试', '描述测试', '描述测试']
  // });

  // model.list.push({
  //   time: '2020-01-01',
  //   title: 'v1.0.3',
  //   content: ['描述测试', '描述测试', '描述测试', '描述测试', '描述测试']
  // });

  // model.list.push({
  //   time: '2020-01-01',
  //   title: 'v1.0.4',
  //   content: ['描述测试', '描述测试', '描述测试', '描述测试', '描述测试']
  // });
}
</script>

<template>
  <div class="version-update min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <canvas id="canvas"></canvas>
    <div class="update-container">
      <div>
        <div class="title text-center">
          <p>版本更新说明</p>
        </div>
        <ATimeline mode="alternate" :reverse="true">
          <ATimelineItem v-for="item in model.list" :key="item.time" :label="item.time">
            <ACard class="item" :title="`${item.title} 版本更新`">
              <ul>
                <li v-for="(content, index) in item.content" :key="index">
                  <p class="content">
                    <span>{{ index + 1 }}.</span>
                    {{ content }}
                  </p>
                </li>
              </ul>
            </ACard>
          </ATimelineItem>
        </ATimeline>
      </div>
    </div>
    <LoadingWrap :loading="model.loading"></LoadingWrap>
  </div>
</template>

<style scoped lang="scss">
.version-update {
  position: relative;
  height: 100vh;
  width: 100%;

  #canvas {
    display: block;
    width: 100%;
    height: 100%;
  }

  .update-container {
    position: absolute;
    inset: 0;
    overflow-y: auto;
    display: block;

    > div {
      margin: 24px auto;
      max-width: 80%;
    }

    .title {
      font-size: 42px;
      padding-bottom: 24px;
    }

    :deep(.ant-timeline-item) {
      .ant-timeline-item-label {
        font-weight: 600;
        letter-spacing: 1.5px;
        font-size: 18px;
      }

      .ant-timeline-item-head {
        top: 3px;
      }

      .ant-timeline-item-content {
        display: flex;
      }

      &.ant-timeline-item-right {
        .ant-timeline-item-content {
          justify-content: end;
        }
      }
    }

    .item {
      min-width: 400px;
      opacity: 0.8;

      li {
        margin-bottom: 8px;

        &:last-child {
          margin-bottom: 0;
        }
      }

      .content {
        font-weight: 500;
        letter-spacing: 1.5px;
        text-align: left;
      }
    }
  }
}
</style>
