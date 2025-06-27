<script setup lang="ts">
import { computed, reactive, ref, watch, withDefaults } from 'vue';
import { InboxOutlined } from '@ant-design/icons-vue';
import { useAntdForm, useFormRules } from '@/hooks/common/form';
import { fetchCreateJsScript, fetchMergeJsScriptChunk, fetchUploadJsScriptChunk } from '@/service/api';

defineOptions({
  name: 'AutoJsScriptOperateDrawer'
});

interface Props {
  operateType?: AntDesign.TableOperateType;
  rowData?: Api.Product.TaobaoData | null;
}

interface Emits {
  (e: 'submitted'): void;
}

type LoadingMap = {
  submit: boolean;
};

type Model = Pick<Api.AutoJsScript.AutoJsScriptData, 'name' | 'version' | 'remark' | 'size' | 'filePath'> & {
  fileList: Array<File>;
  percent: number;
};

const CHUNK_SIZE = 1024 * 1024 * 5;

const { formRef, validate, resetFields } = useAntdForm();

const visible = defineModel<boolean>('visible', {
  default: false
});

const loadingMap = reactive<LoadingMap>({
  submit: false
});

const props = withDefaults(defineProps<Props>(), {
  operateType: 'add',
  rowData: null
});

const emit = defineEmits<Emits>();

const title = computed(() => {
  const titles: Record<AntDesign.TableOperateType, string> = {
    add: '新增脚本',
    edit: '编辑脚本'
  };
  return titles[props.operateType];
});

const isEdit = computed<boolean>(() => {
  return props.operateType === 'edit';
});

const model = ref(createDefaultModel());

const { defaultRequiredRule } = useFormRules();

const rules = {
  name: defaultRequiredRule,
  version: defaultRequiredRule,
  reamrk: defaultRequiredRule
};

watch(visible, () => {
  if (visible.value) {
    initModelHandler();
    resetFields();
  }
});

watch(
  () => model.value.fileList,
  () => {
    if (model.value.fileList.length) {
      let fileName = model.value.fileList[0].name;
      fileName = fileName.replace('.zip', '');
      // 操作助手-1.1.6.zip
      const tempArray = fileName.split('-');
      if (tempArray.length === 2) {
        model.value.name = tempArray[0];
        model.value.version = tempArray[1];
      }
    } else {
      model.value.name = '';
      model.value.version = '';
    }
  }
);

function createDefaultModel(): Model {
  return {
    name: '',
    version: '',
    filePath: '',
    size: '',
    reamrk: '',
    fileList: [],
    percent: 0
  };
}

function initModelHandler() {
  model.value = createDefaultModel();

  if (isEdit.value) {
    if (props.rowData) {
      Object.assign(model.value, props.rowData);
    }
  }
}

function closeDrawer() {
  visible.value = false;
}

async function uploadHandler(): Promise<Api.AutoJsScript.MergeChunkRequest | null> {
  //
  if (model.value.fileList.length === 0) {
    return;
  }

  const file = model.value.fileList[0].originFileObj;

  // 跟踪所有分片的上传进度
  const totalBytes = file.size;
  const totalChunks = Math.ceil(totalBytes / CHUNK_SIZE);
  let uploadedBytes = 0;
  let chunkFolder = '';

  function calculateOverallProgress(progressEvent) {
    const { loaded } = progressEvent;
    uploadedBytes += loaded;
    const overallProgress = (uploadedBytes / totalBytes) * 100;
    const percentage = Math.round(overallProgress);
    model.value.percent = percentage >= 100 ? 99 : percentage;
  }

  try {
    for (let i = 0; i < totalChunks; i += 1) {
      const start = i * CHUNK_SIZE;
      const end = Math.min(file.size, start + CHUNK_SIZE);
      const chunk = file.slice(start, end);

      const data = {
        name: model.value.name,
        version: model.value.version,
        chunk,
        chunkIndex: i, // 注意这里改为 i，而不是 index
        chunkFolder,
        totalSize: file.size,
        chunkSize: chunk.size,
        totalChunks
      };

      // 调用上传函数
      // eslint-disable-next-line no-await-in-loop
      const { data: res, error } = await fetchUploadJsScriptChunk(data, calculateOverallProgress);

      // 处理错误和返回的响应
      if (error || !res || res.code !== 0) {
        throw new Error('上传失败');
      }

      // 更新 chunkFolder
      chunkFolder = res.data;
    }

    const { data: res, error } = await fetchMergeJsScriptChunk({
      name: model.value.name,
      version: model.value.version,
      chunkFolder,
      totalChunks
    });

    if (!error && res && res.code === 0) {
      model.value.filePath = res.data.filePath;
      model.value.size = res.data.size;
      model.value.percent = 100;
    }
  } catch (error) {
    window.$message?.error('上传文件失败');
    throw error;
  }
}

function convertFileSize(fileSizeInBytes) {
  if (fileSizeInBytes === null || fileSizeInBytes === void 0) {
    return '0';
  }

  const Kilobyte = 1024;
  const Megabyte = Kilobyte * 1024;
  const Gigabyte = Megabyte * 1024;

  let sizeString;

  if (fileSizeInBytes < Kilobyte) {
    sizeString = `${fileSizeInBytes} B`;
  } else if (fileSizeInBytes < Megabyte) {
    sizeString = `${(fileSizeInBytes / Kilobyte).toFixed(2)} KB`;
  } else if (fileSizeInBytes < Gigabyte) {
    sizeString = `${(fileSizeInBytes / Megabyte).toFixed(2)} MB`;
  } else {
    sizeString = `${(fileSizeInBytes / Gigabyte).toFixed(2)} GB`;
  }

  return sizeString;
}

async function submitHandler() {
  await validate();

  loadingMap.submit = true;
  try {
    await uploadHandler();

    const { data: res, error } = await fetchCreateJsScript({ ...model.value });
    if (!error && res && res.code === 0) {
      window.$message?.success('保存成功');
      closeDrawer();
      emit('submitted');
    }
  } finally {
    loadingMap.submit = false;
  }
}
</script>

<template>
  <ADrawer v-model:open="visible" :title="title" :width="550" :mask-closable="false">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model" :rules="rules">
        <AFormItem label="脚本名称" name="name">
          <AInput v-model:value="model.name" autocomplete="off" placeholder="脚本名称" />
        </AFormItem>
        <AFormItem label="脚本版本" name="version">
          <AInput v-model:value="model.version" autocomplete="off" placeholder="脚本版本" />
        </AFormItem>
        <AFormItem label="脚本备注" name="remark">
          <ATextarea v-model:value="model.remark" :auto-size="{ minRows: 4 }" :maxlength="100" show-count />
        </AFormItem>
        <AFormItem label="脚本文件">
          <AUploadDragger
            v-model:file-list="model.fileList"
            class="upload"
            :before-upload="() => false"
            name="file"
            accept=".zip, application/zip"
            :show-upload-list="false"
          >
            <div v-if="model.fileList.length === 0" class="upload-box">
              <p class="ant-upload-drag-icon">
                <InboxOutlined></InboxOutlined>
              </p>
              <p class="ant-upload-text">请选择脚本文件</p>
              <p class="ant-upload-hint">支持拖拽上传，上传期间请不要关闭窗口</p>
            </div>
            <div v-else class="upload-box view v-center flex">
              <p class="space-between v-center flex">
                <span>{{ model.fileList[0].name }}</span>
                <span>{{ convertFileSize(model.fileList[0].size) }}</span>
              </p>

              <div class="d-wrap v-center center del flex" @click.stop="() => {}">
                <AButton danger @click.stop="() => (model.fileList = [])">删除</AButton>
              </div>

              <div v-if="loadingMap.submit" class="d-wrap v-center center flex flex-col">
                <AProgress type="circle" :percent="model.percent" :size="80" />
                <p>文件上传中，请勿关闭窗口</p>
              </div>
            </div>
          </AUploadDragger>
        </AFormItem>
      </AForm>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingMap.submit" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
  </ADrawer>
</template>

<style scoped lang="scss">
.upload {
  :deep(.ant-upload) {
    padding: 0;
  }
}

.upload-box {
  height: 145px;
  padding: 15px 0;

  &.view {
    position: relative;
    overflow: hidden;
    border-radius: 3px;
    padding: 0 25px;

    .space-between {
      width: 100%;
      gap: 5px;

      span {
        &:first-child {
          overflow: hidden;
          text-overflow: ellipsis;
          word-break: break-all;
          white-space: nowrap;
        }

        &:last-child {
          flex-shrink: 0;
        }
      }
    }

    .d-wrap {
      position: absolute;
      inset: 0;
      background-color: rgba(0, 0, 0, 0.4);
      opacity: 0;
      transition: 0.3s ease-in-out;

      &.flex-col {
        opacity: 1;

        p {
          color: #fff;
          padding: 5px;
        }
      }
    }

    &:hover {
      .del {
        opacity: 1;
      }

      &:has(.flex-col) {
        .del {
          opacity: 0 !important;
        }
      }
    }
  }
}
</style>
