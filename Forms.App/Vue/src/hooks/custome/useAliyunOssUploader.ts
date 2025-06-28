import { ref } from 'vue';
import { getOssUploadPolicyApi } from '@/service/api';

export interface OssUploadPolicy {
  accessKeyId: string;
  policy: string;
  signature: string;
  host: string;
  dir: string;
  expire: string;
}

export interface OsSUploadOptions {
  file: File;
  dir: string;
  allowedTypes?: string[]; // 允许的 MIME 类型
  maxSizeMB?: number; // 最大体积限制（MB）
  onProgress?: (percent: number) => void; // 上传进度回调
}

export function useOssUploader() {
  const uploading = ref(false);
  const uploadPercent = ref(0);

  /** 文件合法性校验 */
  function validateFile(file: File, allowedTypes?: string[], maxSizeMB = 10): true | string {
    if (allowedTypes?.length && !allowedTypes.includes(file.type)) {
      return `不支持上传该类型文件（${file.type}）`;
    }

    const sizeMB = file.size / 1024 / 1024;
    if (sizeMB > maxSizeMB) {
      return `文件大小不能超过 ${maxSizeMB}MB`;
    }

    return true;
  }

  /** 上传文件核心逻辑 */
  async function uploadFile(options: OsSUploadOptions): Promise<string> {
    const { file, allowedTypes, maxSizeMB = 10, onProgress } = options;
    uploadPercent.value = 0;

    // 校验文件
    const valid = validateFile(file, allowedTypes, maxSizeMB);
    if (valid !== true) {
      window.$message?.error(valid);
      return '';
    }

    uploading.value = true;

    try {
      const policy = await getPolicy(options);

      if (!policy) {
        window.$message?.error('上传失败，请联系管理员');
        return '';
      }

      const filename = `${Date.now()}-${file.name}`;
      const key = `${policy.dir}${filename}`;

      const formData = new FormData();
      formData.append('key', key);
      formData.append('OSSAccessKeyId', policy.accessKeyId);
      formData.append('policy', policy.policy);
      formData.append('signature', policy.signature);
      formData.append('success_action_status', '200');
      formData.append('file', file);

      // 使用 XMLHttpRequest 上传以支持进度回调
      return await new Promise<string>((resolve, reject) => {
        const xhr = new XMLHttpRequest();

        xhr.open('POST', policy.host, true);

        xhr.upload.onprogress = e => {
          if (e.lengthComputable) {
            const percent = Math.round((e.loaded / e.total) * 100);
            uploadPercent.value = percent;
            onProgress?.(percent);
          }
        };

        xhr.onload = () => {
          if (xhr.status === 200) {
            resolve(`${policy.host}/${key}`);
          } else {
            window.$message?.error(`上传失败，状态码：${xhr.status}`);
            reject(new Error('上传失败'));
          }
        };

        xhr.onerror = () => {
          window.$message?.error('上传异常');
          reject(new Error('上传异常'));
        };

        xhr.send(formData);
      });
    } finally {
      uploading.value = false;
    }
  }

  /** 获取 OSS 签名策略 */
  async function getPolicy(options: OsSUploadOptions): Promise<OssUploadPolicy | undefined> {
    const { data: res, error } = await getOssUploadPolicyApi({
      filename: options.file.name,
      dir: options.dir.trimStart('/')
    });

    if (!error && res?.code === 0) {
      return res.data;
    }

    return undefined;
  }

  return {
    uploading,
    uploadPercent,
    uploadFile
  };
}
