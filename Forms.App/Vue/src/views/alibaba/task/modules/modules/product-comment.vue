<script setup lang="ts">
import { computed, nextTick, ref, watch } from 'vue';
import { DeleteOutlined, EyeOutlined, PlusOutlined, StopOutlined } from '@ant-design/icons-vue';
import { imgFallback } from '@/data/empty.json';
import { useAntdForm } from '@/hooks/common/form';
// import type { VideoInfo } from '@/utils/video';
// import videoInfo from '@/utils/video';
import type { ProductData } from '../types';

defineOptions({
  name: 'AliPlanTaskComment'
});

const imageUploadRef = ref<HTMLInputElement | null>(null);
// const videoUploadRef = ref<HTMLInputElement | null>(null);

const { formRef, resetFields } = useAntdForm();

const products = defineModel<Array<ProductData>>('products', {
  default: []
});

const model = ref(createDefaultModel());

const isMaxFileCount = computed(() => model.value.images.length + model.value.videos.length === 6);

watch(
  products,
  value => {
    if (value && value.length) {
      const list = value.map(x => {
        const data = { ...x };

        if (!data.comment || !data.comment.hasComment) {
          data.comment = createDefaultComment();
        }

        return data;
      });

      const keys = value.map(x => x.id);

      model.value.products = model.value.products.filter(x => keys.includes(x.id));

      for (const item of list) {
        const i = model.value.products.findIndex(x => x.id === item.id);
        if (i > -1) {
          Object.assign(model.value.products[i], item);
        } else {
          model.value.products.push(item);
        }
      }
    } else {
      model.value.products = [];
    }
  },
  { deep: true }
);

function createDefaultModel() {
  return {
    products: [],
    visible: false,
    index: -1,
    title: '',
    text: '',
    images: [],
    videos: [],
    preview: {
      visible: false,
      src: '',
      type: ''
    }
  };
}

function createDefaultComment() {
  return {
    hasComment: false,
    text: '',
    images: [],
    videos: []
  };
}

function addCommentHandler(index, item) {
  model.value.index = index;
  model.value.title = `商品：${item.title}`;
  model.value.visible = true;
}

function deleteCommentHander(index) {
  model.value.products[index].comment = createDefaultComment();
}

function imageUplodaHandler() {
  if (!isMaxFileCount.value && model.value.videos.length < 6) {
    imageUploadRef.value?.click();
  }
}

// function videoUplodaHandler() {
//   if (!isMaxFileCount.value && model.value.videos.length === 0) {
//     videoUploadRef.value?.click();
//   }
// }

function imageUploadChangeHandler(e) {
  try {
    if (e.target.files && e.target.files.length) {
      for (const item of e.target.files) {
        if (isMaxFileCount.value) {
          break;
        }
        const data = {
          src: URL.createObjectURL(item),
          file: item,
          visible: false
        };

        model.value.images.push(data);
      }
    }
  } finally {
    imageUploadRef.value.value = '';
  }
}

// async function videoFileChangeHandler(e) {
//   if (e.target.files && e.target.files.length) {
//     try {
//       const info: VideoInfo = await videoInfo(e.target.files[0]);

//       if (info.isFileSizeOver(5)) {
//         window.$message?.warning('文件过大，请更换文件');
//         info.dispose();
//         return;
//       }

//       if (info.duration < 5 || info.duration > 15) {
//         window.$message?.warning('视频时长需要在5-15秒之间');
//         info.dispose();
//         return;
//       }

//       model.value.videos.push({
//         file: info.file,
//         size: info.sizeText,
//         preview: info.thumbnail,
//         src: info.src
//       });
//     } finally {
//       videoUploadRef.value.value = '';
//     }
//   }
// }

function imageDeleteHandler(index) {
  model.value.images.splice(index, 1);
}

function videoPreviewHandler({ src, type }: { src: string; type: string }) {
  model.value.preview.visible = true;
  model.value.preview.src = src;
  model.value.preview.type = type;
}

async function submittedHandler() {
  products.value[model.value.index].comment = {
    hasComment: true,
    mode: 0,
    text: model.value.text,
    images: model.value.images.map(x => {
      return {
        file: x.file,
        src: x.src
      };
    }),
    videos: model.value.videos.map(x => {
      return {
        file: x.file,
        preview: x.preview,
        src: x.src,
        type: x.type
      };
    })
  };

  model.value.visible = false;

  nextTick(() => {
    model.value.text = '';
    model.value.images = [];
    model.value.videos = [];
  });
}

defineExpose({
  initComment: () => {
    resetFields();
    model.value = createDefaultModel();
  }
});
</script>

<template>
  <div class="mb-12px">
    <p class="form-label mb-12px">商品评价</p>
    <AList item-layout="vertical" :data-source="model.products">
      <template #renderItem="{ item, index }">
        <AListItem :key="item.id">
          <AListItemMeta :description="item.skuList.join(';') || '-'">
            <template #title>
              <p>
                <ATag v-if="item.main" color="purple">主商品</ATag>
                {{ item.title }}
              </p>
            </template>
            <template #avatar>
              <AImage height="60px" width="60px" :src="item.image" :fallback="imgFallback" />
            </template>
          </AListItemMeta>

          <div v-if="item.comment.hasComment">
            <p class="mb-8px">{{ item.comment.text }}</p>
            <div class="wrap flex gap-8px">
              <div v-for="img in item.comment.images" :key="img" class="comment-preview">
                <AImage :src="img.src" :height="80" :width="80"></AImage>
              </div>
              <div v-if="item.comment.videos.length" class="comment-preview">
                <img :src="item.comment.videos[0].preview" :height="80" :width="80" />
                <div class="wrap v-center center flex gap-8px">
                  <p @click="() => videoPreviewHandler(item.comment.videos[0])">
                    <EyeOutlined />
                    预览
                  </p>
                </div>
              </div>
            </div>
          </div>

          <template #actions>
            <APopconfirm
              v-if="item.comment.hasComment"
              title="确认删除吗？"
              placement="top"
              @confirm="() => deleteCommentHander(index)"
            >
              <AButton size="small" danger>删除评价</AButton>
            </APopconfirm>
            <AButton v-else size="small" @click="addCommentHandler(index, item)">添加评价</AButton>
          </template>
        </AListItem>
      </template>
    </AList>
  </div>

  <ADrawer v-model:open="model.visible" :title="model.title" :width="800">
    <AForm ref="formRef" name="operate" layout="vertical" :model="model">
      <AFormItem label="文字" name="text">
        <ATextarea
          v-model:value="model.text"
          :auto-size="{ minRows: 6 }"
          show-count
          :maxlength="300"
          placeholder="请输入文字评价"
        />
      </AFormItem>

      <AFormItem>
        <template #label>
          <p>
            图片
            <small class="text-danger ml-4px">* 图片+视频最多6个，且图片必须小于3M</small>
          </p>
        </template>
        <input
          ref="imageUploadRef"
          type="file"
          accept="image/jpeg,image/png"
          class="upload-hidden"
          multiple
          @change="imageUploadChangeHandler"
        />

        <div class="wrap flex gap-8px">
          <TransitionGroup v-if="model.images.length" tag="ul" name="list" class="wrap flex gap-8px">
            <li v-for="(item, index) in model.images" :key="item.src" class="preview uploder">
              <AImage
                :width="200"
                :preview="{
                  visible: item.visible,
                  onVisibleChange: value => (item.visible = value)
                }"
                :src="item.src"
              />
              <img :src="item.src" />
              <div class="wrap v-center center flex gap-8px">
                <EyeOutlined @click="item.visible = true" />
                <DeleteOutlined @click="imageDeleteHandler(index)" />
              </div>
            </li>
          </TransitionGroup>
          <div
            v-if="model.images.length < 6 && !isMaxFileCount"
            class="uploder center v-center flex"
            @click="imageUplodaHandler"
          >
            <PlusOutlined v-if="model.images.length < 6 && !isMaxFileCount" />
            <div v-else-if="model.images.length < 6" class="stop">
              <p class="text-center">
                <StopOutlined />
              </p>
              <p class="text-center">数量已上限</p>
            </div>
          </div>
        </div>
      </AFormItem>

      <!--
 <AFormItem name="videos">
        <template #label>
          <p>
            视频
            <small class="text-danger ml-4px">* 视频最多一个，且视频时长必须5-15秒之间</small>
          </p>
        </template>
        <input
          ref="videoUploadRef"
          type="file"
          accept="video/*"
          class="upload-hidden"
          @change="videoFileChangeHandler"
        />

        <div v-if="model.videos.length" class="preview uploder">
          <img :src="model.videos[0].preview" />
          <div class="wrap v-center center flex gap-8px">
            <EyeOutlined @click="() => videoPreviewHandler(model.videos[0])" />
            <DeleteOutlined @click="() => (model.videos = [])" />
          </div>
        </div>
        <div v-if="!model.videos.length" class="uploder center v-center flex" @click="videoUplodaHandler">
          <PlusOutlined v-if="model.videos.length === 0 && !isMaxFileCount" />
          <div v-else-if="model.videos.length === 0" class="stop">
            <p class="text-center">
              <StopOutlined />
            </p>
            <p class="text-center">数量已上限</p>
          </div>
        </div>
      </AFormItem> 
-->
    </AForm>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="() => (model.visible = false)">取消</AButton>
        <AButton type="primary" @click="submittedHandler">确定</AButton>
      </ASpace>
    </template>
  </ADrawer>

  <Teleport to="body">
    <Transition name="fade" mode="out-in">
      <div
        v-if="model.preview.visible"
        class="file-preview center v-center flex"
        @click="model.preview.visible = false"
      >
        <div class="video">
          <video controls autoplay @click.stop="() => undefined">
            <source :src="model.preview.src" :type="model.preview.type" />
          </video>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style lang="scss" scoped>
.form-label {
  height: 32px;
  font-size: 14px;
}

:deep(.ant-list-items) {
  .ant-list-item-meta-description {
    overflow: hidden;
    word-break: break-all;
    white-space: nowrap;
    text-overflow: ellipsis;
  }

  .ant-list-item-action {
    display: flex;
    justify-content: end;
    align-items: center;
  }
}

.upload-hidden {
  display: none !important;
}

.uploder {
  overflow: hidden;
  height: 100px;
  width: 100px;
  border: 1px solid #f0f0f0;
  border-radius: 5px;
  cursor: pointer;
  transition: all 0.3s ease-in-out;

  &.preview {
    position: relative;

    img {
      height: 100px;
      width: 100px;
      object-fit: cover;
    }

    .wrap {
      position: absolute;
      inset: 0;
      background-color: rgba(0, 0, 0, 0.45);
      opacity: 0;
      transition: all 0.3s ease-in-out;

      span {
        font-size: 18px;
        transition: inherit;
        color: #f0f0f0;

        &.anticon-eye {
          &:hover {
            color: #646cff;
          }
        }

        &.anticon-delete {
          &:hover {
            color: var(--color-danger);
          }
        }
      }
    }

    &:hover {
      .wrap {
        opacity: 1;
      }
    }

    :deep(.ant-image) {
      display: none !important;
    }
  }

  span {
    font-size: 24px;
    transition: inherit;
  }

  &:hover {
    border-color: #646cff;

    > span {
      color: #646cff;
    }
  }

  &:has(.stop) {
    background-color: #f5f5f5;

    p {
      font-weight: 400;
      font-size: 12px;

      &:has(span) {
        font-size: 20px;
      }
    }

    &:hover {
      border-color: #f0f0f0;
    }
  }
}

.file-preview {
  position: absolute;
  left: 0;
  top: 0;
  height: 100vh;
  width: 100%;
  background-color: rgba(0, 0, 0, 0.45);
  z-index: 99999;

  .video {
    video {
      height: 40vh;
      width: 60vw;
    }
  }

  .image {
    img {
      max-width: 80vw;
    }
  }
}

.comment-preview {
  position: relative;
  height: 80px;
  width: 80px;
  overflow: hidden;
  border-radius: 5px;

  :deep(.ant-image-img) {
    object-fit: cover;
  }

  .wrap {
    position: absolute;
    inset: 0;
    background-color: rgba(0, 0, 0, 0.45);
    opacity: 0;
    transition: all 0.3s ease-in-out;

    p {
      color: #f0f0f0;
      cursor: pointer;
    }
  }

  &:has(.wrap) {
    &:hover {
      .wrap {
        opacity: 1;
      }
    }
  }
}
</style>
