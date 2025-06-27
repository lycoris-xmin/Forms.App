<script setup lang="tsx">
import { reactive, ref, watch, withDefaults } from 'vue';
import { InboxOutlined } from '@ant-design/icons-vue';
import { useAntdForm } from '@/hooks/common/form';
import {
  apiUrl,
  fetchCheckImportPlanTaskComplete,
  fetchMergePlanTaskChunk,
  fetchTaobaoImportPlanTask,
  fetchUploadPlanTaskChunk
} from '@/service/api';
import LoadingWrap from '@/components/loading/index.vue';
import ImportErrorDrawer from './modules/import-error-drawer.vue';
const CHUNK_SIZE = 1024 * 1024 * 5; // 5MB

interface Props {
  userInfo: object;
}

interface Emits {
  (e: 'submitted'): void;
}

type Model = {
  name: string;
  size: string;
  fileExtension: string;
  filePath: string;
  fileList: Array<File>;
  percent: number;
  isStop: boolean;
  importResult: object;
  importErrorVisible: boolean;
};

const { formRef, validate } = useAntdForm();

const loadingModel = reactive({
  submit: false,
  spinLoading: false
});

const visible = defineModel<boolean>('visible', {
  default: false
});

const model = ref(createDefaultModel());

const props = withDefaults(defineProps<Props>(), {});

const emit = defineEmits<Emits>();

watch(
  () => model.value.fileList,
  () => {
    if (model.value.fileList.length) {
      let fileName = model.value.fileList[0].name;
      const fileExtension = fileName.includes('.') ? fileName.split('.').pop()!.toLowerCase() : '';

      fileName = fileName.replace(/\.(xls|xlsx|xlsm)$/i, '');

      if (fileName && fileExtension) {
        model.value.fileExtension = fileExtension;
        model.value.name = fileName;
      }
    } else {
      model.value.name = '';
      model.value.fileExtension = '';
    }
  }
);

function createDefaultModel(): Model {
  return {
    fileList: [],
    percent: 0,
    isStop: true,
    importResultL: () => {},
    importErrorVisible: false
  };
}

async function uploadHandler(): Promise<Api.PlanTask.PlanTaskMergeChunkRequest | null> {
  if (model.value.fileList.length === 0) {
    return;
  }
  const file = model.value.fileList[0].originFileObj;

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
        chunk,
        chunkIndex: i, // 注意这里改为 i，而不是 index
        chunkFolder,
        totalSize: file.size,
        chunkSize: chunk.size,
        totalChunks
      };

      // eslint-disable-next-line no-await-in-loop
      const { data: res, error } = await fetchUploadPlanTaskChunk(data, calculateOverallProgress);

      if (error || !res || res.code !== 0) {
        throw new Error('上传失败');
      }

      chunkFolder = res.data;
    }

    const { data: res, error } = await fetchMergePlanTaskChunk({
      name: model.value.name,
      chunkFolder,
      totalChunks,
      fileExtension: model.value.fileExtension
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
  if (!model.value.filePath && (!model.value.fileList || !model.value.fileList.length)) {
    visible.value = false;
    return;
  }
  await validate();

  loadingModel.submit = true;
  try {
    await uploadHandler();

    window.$message?.success('正在填入数据,请耐心等待');

    const { data: res, error } = await fetchTaobaoImportPlanTask({
      filePath: model.value.filePath,
      isStop: model.value.isStop
    });
    if (!error && res && res.code === 0) {
      // 打开遮罩，防止用户操作
      loadingModel.spinLoading = true;

      checkExportComplete(res.data);
    } else {
      loadingModel.submit = false;
      loadingModel.spinLoading = false;
    }
  } catch {
    loadingModel.submit = false;
    loadingModel.spinLoading = false;
  }
}

function checkExportComplete(excelId) {
  let i = 0;
  const timer = setInterval(async () => {
    if (i > 100) {
      window.$message?.error('填入数据失败，请稍后再试');
      clearInterval(timer);
      loadingModel.submit = false;
      loadingModel.spinLoading = false;
      return;
    }

    const { data: res, error } = await fetchCheckImportPlanTaskComplete(excelId);
    if (!error && res && res.code === 0 && res.data.complete) {
      clearInterval(timer);
      loadingModel.submit = false;
      loadingModel.spinLoading = false;
      model.value.importResult = res.data;
      model.value.fileList = [];
      model.value.filePath = '';
      emit('submitted');
      return;
    }

    i += 1;
  }, 3000);
}

async function downloadHandler(isLinked: boolean) {
  let filename;
  if (isLinked) {
    filename = props.userInfo.tenantType === 20 ? '多链接任务模版.xlsx' : '多链接任务模版(other).xlsx';
  } else {
    filename = props.userInfo.tenantType === 20 ? '单链接任务模版.xlsx' : '单链接任务模版(other).xlsx';
  }

  const url = apiUrl.endsWith('/')
    ? `${apiUrl}exceltemplate/plantask/taobao/${filename}`
    : `${apiUrl}/exceltemplate/plantask/taobao/${filename}`;

  const a = document.createElement('a');
  a.href = url;
  // a.download = downFileName;
  a.style.display = 'none';
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
}

function getColumns() {
  const cols = [
    {
      key: 'rowIndex',
      dataIndex: 'rowIndex',
      title: '行数',
      align: 'center',
      width: 100
    },
    {
      key: 'message',
      dataIndex: 'message',
      title: '失败原因',
      align: 'center'
    }
  ];
  return [...cols];
}

function closeDrawer() {
  model.value.fileList = [];
  visible.value = false;
  loadingModel.submit = false;
  loadingModel.spinLoading = false;
  model.value.importErrorVisible = false;
}

function importErrorHandler() {
  model.value.importErrorVisible = true;
}

// async function downloadExcel() {
//   try {
//     const url = `${apiUrl}/api/plantask/excel/importerror`;

//     const response = await fetch(url, {
//       mode: 'cors'
//     });

//     if (!response.ok) {
//       window.$message?.error('下载异常');
//       return;
//     }

//     const blob = await response.blob();
//     const a = document.createElement('a');
//     a.href = URL.createObjectURL(blob);
//     a.download = exportFileName;
//     document.body.appendChild(a);
//     a.click();
//     document.body.removeChild(a);
//     URL.revokeObjectURL(a.href);
//   } catch (error) {
//     window.$message?.error(`下载失败: ${error}`);
//   }
// }

function generateTitle(result) {
  if (!result) {
    return `导入结果：共读取 0 条，失败 0 条`;
  }
  if (result?.stopCount) {
    return `${result.fileName}导入结果：共读取${result.count ?? 0} 条，失败 ${result.errorList && result.errorList.length ? result.errorList.length : 0} 条，终止 ${result.stopCount} 条`;
  }
  return `${result.fileName}导入结果：共读取${result.count ?? 0} 条，成功 ${result.successCount} 条，失败 ${result.errorList && result.errorList.length ? result.errorList.length : 0} 条`;
}
</script>

<template>
  <ADrawer v-model:open="visible" title="任务导入" :width="750" :push="false">
    <div class="body">
      <AForm ref="formRef" name="operate" layout="vertical" :model="model">
        <AFormItem label="导入模版">
          <ATooltip placement="top" title="下载单链接任务导入模版">
            <AButton type="link" danger @click="downloadHandler(false)">下载单链接模版</AButton>
          </ATooltip>
          <ATooltip placement="top" title="下载多链接任务导入模版">
            <AButton type="link" danger @click="downloadHandler(true)">下载多链接模版</AButton>
          </ATooltip>
        </AFormItem>
        <AFormItem label="导入模式">
          <ARadioGroup
            v-model:value="model.isStop"
            :options="[
              { value: true, label: '发现错误数据，停止导入' },
              { value: false, label: '跳过错误数据，直接导入' }
            ]"
          />
        </AFormItem>

        <AFormItem label="导入文件">
          <AUploadDragger
            v-model:file-list="model.fileList"
            class="upload"
            :before-upload="() => false"
            name="file"
            accept=".xls,.xlsx,applications/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            :show-upload-list="false"
          >
            <div v-if="model.fileList.length === 0" class="upload-box">
              <p class="ant-upload-drag-icon">
                <InboxOutlined></InboxOutlined>
              </p>
              <p class="ant-upload-text">请选择任务文件</p>
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

              <div v-if="loadingModel.submit" class="d-wrap v-center center flex flex-col">
                <AProgress type="circle" :percent="model.percent" :size="80" />
                <p>文件上传中，请勿关闭窗口</p>
              </div>
            </div>
          </AUploadDragger>
        </AFormItem>
      </AForm>
      <ACard
        :title="generateTitle(model.importResult)"
        :bordered="false"
        :body-style="{ flex: 1, overflow: 'hidden' }"
        class="mt-18px flex-col-stretch sm:flex-1-hidden card-wrapper"
      >
        <template #extra>
          <ATooltip placement="top" title="下载失败报表">
            <AButton @click="importErrorHandler">下载失败报表</AButton>
          </ATooltip>
        </template>
        <ATable
          v-if="model.importResult"
          :columns="getColumns()"
          :pagination="{ total: model.importResult.errorList.length, showSizeChanger: true }"
          :data-source="model.importResult.errorList"
          :scroll="{ y: 500 }"
        ></ATable>
      </ACard>
    </div>
    <template #footer>
      <ASpace :size="16">
        <AButton @click="closeDrawer">取消</AButton>
        <AButton type="primary" :loading="loadingModel.submit" @click="submitHandler">保存</AButton>
      </ASpace>
    </template>
    <LoadingWrap :loading="loadingModel.spinLoading" tip="正在导入数据，请勿关闭窗口"></LoadingWrap>
  </ADrawer>

  <ImportErrorDrawer v-model:visible="model.importErrorVisible"></ImportErrorDrawer>
</template>

<style lang="scss" scoped>
.ant-checkbox-wrapper {
  margin-left: 10px;
}
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
