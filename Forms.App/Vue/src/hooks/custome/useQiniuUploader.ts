import { ref } from 'vue';
import * as qiniu from 'qiniu-js';
import { useQiniuTokenStore } from '@/store/modules/qiniu';

export enum QiniuUploadStatus {
  DEFAULT = 'default',
  UPLOADING = 'uploading',
  UPLOADED = 'uploaded',
  ERROR = 'error'
}

export interface QiniuUploadFile {
  raw: File;
  name: string;
  type: string;
  preview: string;
  key?: string;
  src?: string;
  percent: number;
  loaded: number;
  size: number;
  status: QiniuUploadStatus;
  error?: string;
}

interface QiniuUploadOptions {
  path?: string;
  maxSize?: number;
}

export function useQiniuUploader(options: QiniuUploadOptions = {}) {
  const Files = ref<QiniuUploadFile[]>([]);

  const qiniuStore = useQiniuTokenStore();

  const opt: QiniuUploadOptions = options || { path: '/default', maxSize: 6 };
  if (typeof opt.maxSize === 'string') {
    opt.maxSize = Number.parseInt(opt.maxSize, 10);
  }

  if (Number.isNaN(opt.maxSize) || typeof opt.maxSize !== 'number') {
    opt.maxSize = 5;
  }

  const config: qiniu.Config = { useCdnDomain: true, region: qiniu.region.z0 };

  async function initToken() {
    if (!qiniuStore.token || qiniuStore.expiredAt <= Date.now()) {
      await qiniuStore.fetchToken();
    }
  }

  function pushFile(file: File) {
    const maxSize = opt.maxSize * 1024 * 1024;

    if (file.size > maxSize) {
      window.$message?.warning('文件超出大小限制');
    }

    Files.value.push({
      raw: file,
      name: file.name,
      type: file.type,
      preview: URL.createObjectURL(file),
      percent: 0,
      loaded: 0,
      size: file.size,
      status: QiniuUploadStatus.DEFAULT
    });
  }

  function deleteFile(file: QiniuUploadFile) {
    const index = Files.value.indexOf(file);
    if (index !== -1) {
      Files.value.splice(index, 1);
    }
  }

  async function upload(): Promise<QiniuUploadFile[]> {
    await initToken();

    const uploadTasks = Files.value
      .filter(file => file.status === QiniuUploadStatus.DEFAULT)
      .map(file => {
        const ext = file.raw.name.split('.').pop();

        const randomName = `${Math.random().toString(16).slice(2)}${Date.now()}${Math.random().toString(16).slice(2)}.${ext}`;

        const uploadPath = options.path?.replace(/^\/|\/$/g, '') || 'default';

        const key = `${uploadPath}/${randomName}`;

        file.key = key;
        file.src = `//${qiniuStore.domain}/${key}`;
        file.status = QiniuUploadStatus.UPLOADING;

        return new Promise<void>((resolve, reject) => {
          const observable = qiniu.upload(file.raw, key, qiniuStore.token, { fname: file.name }, config);

          observable.subscribe({
            next({ total }) {
              file.loaded = total.loaded;
              file.percent = Math.floor(total.percent * 100) / 100;
              file.size = total.size;
            },
            error(err) {
              file.status = QiniuUploadStatus.ERROR;
              file.error = err.message || String(err);
              reject(err);
            },
            complete() {
              file.status = QiniuUploadStatus.UPLOADED;
              resolve();
            }
          });
        });
      });

    const results = await Promise.allSettled(uploadTasks);

    if (results.some(r => r.status === 'rejected')) {
      window.$message?.error('部分文件上传失败');
    }

    return Files.value;
  }

  return {
    pushFile,
    deleteFile,
    upload,
    files: Files
  };
}
