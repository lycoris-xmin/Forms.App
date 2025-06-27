<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { Cropper } from 'vue-advanced-cropper';
import 'vue-advanced-cropper/dist/style.css';
import 'vue-advanced-cropper/dist/theme.compact.css';
import type { CropperRef } from 'vue-advanced-cropper';
import { InboxOutlined } from '@ant-design/icons-vue';

interface CropSize {
  height: number;
  width: number;
}

interface Props {
  title?: string;
  cropSize: CropSize;
  /* 允许裁剪后的最大文件大小 默认600kb */
  maxCropSize?: number;
  height?: string | number;
  width?: string | number;
}

interface Emits {
  (e: 'crop', file: File): void;
}

const fileInputRef = ref<HTMLInputElement | null>(null);
const cropperRef = ref<CropperRef | null>(null);
const cropperSrc = ref<string>('');

const visible = defineModel<boolean>('visible', {
  default: false
});

const props = withDefaults(defineProps<Props>(), {
  title: '图像裁剪',
  maxCropSize: 600,
  height: '100%',
  width: '100%'
});

const stencilProps = computed(() => {
  const ratio = props.cropSize.width / props.cropSize.height;
  return {
    aspectRatio: Number(ratio.toFixed(2))
  };
});

const imageHeight = computed(() => {
  let height = '100%';
  if (props.height && typeof props.height === 'number') {
    height = `${Number.parseInt(props.height, 10)}px`;
  } else {
    height = props.height;
  }

  return height;
});

const imageWidth = computed(() => {
  let width = '100%';
  if (props.width && typeof props.width === 'number') {
    width = `${Number.parseInt(props.width, 10)}px`;
  } else {
    width = props.width;
  }

  return width;
});

watch(visible, val => {
  if (!val) {
    cropperSrc.value = '';
    if (fileInputRef.value) {
      fileInputRef.value.value = '';
    }
  }
});

const emit = defineEmits<Emits>();

function closeModal() {
  visible.value = false;
}

function roateHandler(angle) {
  cropperRef.value.rotate(angle);
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function flipHandler(x, y) {
  // 水平翻转 flip(true, false)
  // 垂直翻转 flip(false, true)
  // 水平 + 垂直翻转 flip(true, true)
  cropperRef.value.flip(x, y);
}

function errorHandler() {
  window.$message?.error('裁剪失败');
}

async function cropImageHandler() {
  const cropper = cropperRef.value;
  if (cropper) {
    const canvas = cropper.getResult({
      size: {
        width: props.cropSize.width,
        height: props.cropSize.height
      }
    })?.canvas;

    if (canvas) {
      const input = await compressCanvas(canvas);
      emit('crop', input);
      closeModal();
    }
  }
}

function downloadHandler() {
  const cropper = cropperRef.value;
  if (cropper) {
    const canvas = cropper.getResult()?.canvas;

    if (canvas) {
      canvas.toBlob(
        blob => {
          if (blob) {
            const input = new File([blob], 'cropped-image.png', {
              type: blob.type
            });

            // 下载文件
            downloadFile(input);
          }
        },
        'image/png',
        0.8
      );
    }
  }
}

function downloadFile(input: File) {
  const url = URL.createObjectURL(input);
  const a = document.createElement('a');
  a.href = url;
  a.download = input.name; // 例如 'cropped-image.png'
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url); // 释放内存
}

// 封装压缩逻辑
async function compressCanvas(canvas: HTMLCanvasElement, initialQuality = 0.8): Promise<File> {
  let quality = initialQuality;

  const compress = (): Promise<Blob> =>
    new Promise(resolve => {
      canvas.toBlob(
        blob => {
          if (blob) resolve(blob);
        },
        'image/webp',
        quality
      );
    });

  let blob = await compress();

  while (blob.size / 1024 > props.maxCropSize && quality > 0.5) {
    quality -= 0.1;
    // eslint-disable-next-line no-await-in-loop
    blob = await compress();
  }

  return new File([blob], 'cropped-image.webp', { type: 'image/webp' });
}

function triggerFileSelectHandler() {
  fileInputRef.value?.click();
}

function fileSelectHandler(e) {
  if (e.target.files && e.target.files.length) {
    cropperSrc.value = URL.createObjectURL(e.target.files[0]);
  }
}

// <Cropper v-model:visible="model.cropper" :crop-size="{ height: 200, width: 200 }" @crop="cropHandler"></Cropper>
</script>

<template>
  <AModal v-model:open="visible" :title="props.title" :mask-closable="false" :footer="false">
    <input
      ref="fileInputRef"
      hidden
      type="file"
      accept="image/jpeg,image/png,image/gif,image/webp"
      @change="fileSelectHandler"
    />
    <div class="copper-contaner center mb-12px flex">
      <Cropper
        v-show="cropperSrc"
        ref="cropperRef"
        class="cropper"
        :style="{ height: imageHeight, width: imageWidth }"
        :src="cropperSrc"
        :stencil-props="stencilProps"
        @error="errorHandler"
      ></Cropper>
      <div
        v-show="!cropperSrc"
        class="v-center center select flex"
        :style="{ height: imageHeight, width: imageWidth }"
        @click="triggerFileSelectHandler"
      >
        <div>
          <p class="text-center">
            <InboxOutlined></InboxOutlined>
          </p>
          <p class="note text-center">选择图片</p>
        </div>
      </div>
    </div>
    <div class="v-center space-between flex">
      <div class="v-center flex gap-8px">
        <AButton size="small" :disabled="!cropperSrc" @click="() => roateHandler(-90)">↺</AButton>
        <AButton size="small" :disabled="!cropperSrc" @click="() => roateHandler(90)">↻</AButton>
        <AButton size="small" :disabled="!cropperSrc" @click="downloadHandler">导出裁剪</AButton>
        <AButton v-show="cropperSrc" size="small" @click="triggerFileSelectHandler">重新选择</AButton>
      </div>
      <div class="v-center flex gap-8px">
        <AButton size="small" @click="closeModal">取消</AButton>
        <AButton size="small" @click="cropImageHandler">确定</AButton>
      </div>
    </div>
  </AModal>
</template>

<style lang="scss" scoped>
.cropper {
  min-height: 300px;
  max-height: 600px;
  min-width: 300px;
  max-width: 600px;
  border: 1px solid var(--border-color);
  border-radius: 5px;
  overflow: hidden;
}

.select {
  min-height: 300px;
  max-height: 600px;
  min-width: 300px;
  max-width: 600px;
  border: 1px solid var(--border-color);
  border-radius: 5px;
  overflow: hidden;
  font-size: 42px;
  cursor: pointer;
  transition: 0.3s ease-in-out;

  > div {
    * {
      transition: 0.3s ease-in-out;
    }
  }
  .note {
    font-size: 16px;
  }

  &:hover {
    border-color: rgb(var(--primary-color));

    > div {
      color: rgb(var(--primary-color));
    }
  }
}
</style>
