import { ref } from 'vue';
import { apiUrl } from '@/service/api';

const isHttpUrl = (url: string) => /^https?:\/\//i.test(url);

export interface DownloadOptions {
  fileName?: string;
  baseUrl?: string;
  headers?: Record<string, string>;
  onSuccess?: () => void;
  onError?: (err: Error) => void;
  onProgress?: (percent: number) => void;
}

const fetchBlobData = async (url: string, headers: Record<string, string>, onProgress: (percent: number) => void) => {
  const response = await fetch(url, {
    method: 'GET',
    mode: 'cors',
    headers
  });

  if (!response.ok) {
    throw new Error(`请求失败：${response.status}`);
  }

  const contentLength = Number(response.headers.get('Content-Length') || '0');
  const reader = response.body?.getReader();

  if (!reader) throw new Error('无法读取文件流');

  let receivedLength = 0;
  const chunks: Uint8Array[] = [];

  // 使用流的方式按块读取
  const stream = new ReadableStream({
    start(controller) {
      const push = async () => {
        const { done, value } = await reader.read();
        if (done) {
          controller.close();
          return;
        }
        chunks.push(value);
        receivedLength += value.length;
        const percent = contentLength ? Math.round((receivedLength / contentLength) * 100) : 0;
        onProgress(percent); // 更新进度
        controller.enqueue(value);
        push(); // 继续读取
      };
      push();
    }
  });

  const blob = await new Response(stream).blob();
  return blob;
};

export function useDownload() {
  const isDownloading = ref(false);
  const error = ref<Error | null>(null);

  const download = async (url: string, options: DownloadOptions = {}) => {
    const { fileName, baseUrl = apiUrl || '', headers = {}, onSuccess, onError, onProgress } = options;

    isDownloading.value = true;
    error.value = null;

    try {
      const fullUrl = isHttpUrl(url) ? url : `${baseUrl}${url}`;
      const blob = await fetchBlobData(fullUrl, headers, onProgress);

      const downloadName = fileName || decodeURIComponent(url.split('/').pop() || 'file');

      const a = document.createElement('a');
      a.style.display = 'none';
      a.href = URL.createObjectURL(blob);
      a.download = downloadName;

      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(a.href);

      onSuccess?.();
      window.$message?.success('下载成功');
    } catch (err: any) {
      error.value = err;
      onError?.(err);
      window.$message?.error(`下载失败：${err.message}`);
    } finally {
      isDownloading.value = false;
    }
  };

  return {
    download,
    isDownloading,
    error
  };
}
