<script setup lang="ts">
import { computed, ref, watch, withDefaults } from 'vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { apiUrl, fetchCreatePlanTaskComment } from '@/service/api';

defineOptions({
  name: 'PlanTaskCommentDrawer'
});

type Model = Api.PlanTaskComment.CreateComment & {
  loading: false;
  row?: object;
  imageFiles: Array;
  videoFiles: Array;
};

type Props = {
  row?: object;
};

interface Emits {
  (e: 'submitted'): void;
}

const visible = defineModel<boolean>('visible', {
  default: false
});

const model: Model = ref(createDefaultModel());

const previewVisible = ref(false);
const previewImage = ref('');
const previewTitle = ref('');

const props = withDefaults(defineProps<Props>(), {
  row: () => {}
});

const emit = defineEmits<Emits>();

watch(visible, value => {
  if (value) {
    model.value = createDefaultModel();

    Object.assign(model.value.row, props.row || {});

    let i = 0;
    model.value.imageFiles = model.value.images?.map(x => {
      return {
        uid: (i -= 1),
        name: `${i}.png`,
        status: 'done',
        url: window.$isDbugger ? `${apiUrl}${x}` : x,
        thumbUrl: window.$isDbugger ? `${apiUrl}${x}` : x
      };
    });

    model.value.videoFiles = model.value.videos?.map(x => {
      return {
        uid: (i -= 1),
        name: `${i}.png`,
        status: 'done',
        url: window.$isDbugger ? `${apiUrl}${x}` : x,
        thumbUrl: window.$isDbugger ? `${apiUrl}${x}` : x
      };
    });
  }
});

const fileCount = computed(() => {
  return model.value.imageFiles.length + model.value.videoFiles.length;
});

function createDefaultModel(): Model {
  return {
    taskId: '',
    mode: 0,
    text: '',
    images: [],
    imageFiles: [],
    videos: [],
    videoFiles: [],
    loading: false,
    row: {}
  };
}

const handleCancel = () => {
  previewVisible.value = false;
  previewTitle.value = '';
};

const handlePreview = async (file: UploadProps['fileList'][number]) => {
  if (!file.url && !file.preview) {
    file.preview = (await getBase64(file.originFileObj)) as string;
  }
  previewImage.value = file.url || file.preview;
  previewVisible.value = true;
  previewTitle.value = file.name || file.url.substring(file.url.lastIndexOf('/') + 1);
};

function getBase64(file: File) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = error => reject(error);
  });
}

function closeDrawer() {
  visible.value = false;
}

async function submitHandler() {
  const data: Api.PlanTaskComment.CreateComment = {
    taskId: props.row.id,
    mode: model.value.mode,
    text: model.value.text,
    images: model.value.imageFiles.map(x => {
      return x.originFileObj;
    }),
    videos: model.value.videoFiles.map(x => {
      return x.originFileObj;
    })
  };

  if (data.text === 'undefined' && data.images?.length === 0 && data.videos?.length === 0) {
    window.$message?.warning('至少要有一个评价内容(文字、图片、视频)');
    return;
  }

  model.value.loading = true;
  try {
    const { data: res, error } = await fetchCreatePlanTaskComment(data);
    if (!error && res && res.code === 0) {
      window.$message?.success('发布成功');
      emit('submitted');
      closeDrawer();
    }
  } finally {
    model.value.loading = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" title="发布评价" :width="650">
    <AForm name="operate" layout="vertical" :model="model">
      <p class="title">
        <a :href="props.row.productUrl" target="_blank" rel="noopener noreferrer">
          {{ props.row.title }}
        </a>
      </p>
      <AFormItem name="text" label="评价内容">
        <ATextarea v-model:value="model.text" placeholder="请输入评价内容" :rows="10" :maxlength="300" show-count />
      </AFormItem>
      <AFormItem name="imageFiles" label="评价图片" list-type="picture-card">
        <AUpload
          v-model:file-list="model.imageFiles"
          :max-count="6"
          multiple
          class="uploader"
          list-type="picture-card"
          :before-upload="() => false"
          accept="image/jpeg,image/png,image/webp"
          :disabled="(model.imageFiles.length < 6 && fileCount >= 6) || model.loading"
          @preview="handlePreview"
        >
          <div v-if="fileCount < 6">
            <PlusOutlined />
            <div class="ant-upload-text">上传</div>
          </div>
          <div v-else-if="model.imageFiles.length < 6">
            <p>数量已达限制</p>
          </div>
        </AUpload>
      </AFormItem>

      <AFormItem name="videoFiles" label="评价视频" list-type="picture-card">
        <AUpload
          v-model:file-list="model.videoFiles"
          :max-count="6"
          multiple
          class="uploader"
          list-type="picture-card"
          :before-upload="() => false"
          accept="video/*"
          :disabled="(model.videoFiles.length < 6 && fileCount >= 6) || model.loading"
          @preview="handlePreview"
        >
          <div v-if="fileCount < 6">
            <PlusOutlined />
            <div class="ant-upload-text">上传</div>
          </div>
          <div v-else-if="model.videoFiles.length < 6">
            <p>数量已达限制</p>
          </div>
        </AUpload>
      </AFormItem>
    </AForm>

    <AModal :open="previewVisible" :title="previewTitle" :footer="null" @cancel="handleCancel">
      <img class="preview" :src="previewImage" />
    </AModal>

    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="model.loading" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.title {
  margin-bottom: 24px;

  a {
    font-size: 16px;
    font-weight: 600;
  }
}

.uploader {
  .ant-upload-select-picture-card {
    i {
      font-size: 32px;
      color: #999;
    }

    .ant-upload-text {
      margin-top: 8px;
      color: #666;
    }
  }

  :deep(.ant-upload-list-item-actions) {
    display: flex;
    justify-content: center;
    align-items: center;

    a {
      height: 18px;
    }
  }
}

.preview {
  width: 100%;
}
</style>
