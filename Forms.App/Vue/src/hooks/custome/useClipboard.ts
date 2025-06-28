import { ref } from 'vue';

export interface CopyOptions {
  resetAfter?: number;
  delay?: number;
  onSuccess?: () => void;
  onError?: (err: Error) => void;
}

function sleep(ms: number) {
  return new Promise<void>(resolve => {
    setTimeout(resolve, ms);
  });
}

type CopyFn = (text: string, options?: CopyOptions) => Promise<void>;

function createCopyFunction(): CopyFn {
  if (navigator.clipboard?.writeText) {
    return async (text: string, options = {}) => {
      const { delay = 0, onSuccess, onError } = options;

      if (delay > 0) await sleep(delay);

      try {
        await navigator.clipboard.writeText(text);
        onSuccess?.();
        window.$message?.success('复制成功');
      } catch (err: any) {
        onError?.(err);
        window.$message?.error('复制失败');
        throw err;
      }
    };
  }

  return async (text: string, options = {}) => {
    const { delay = 0, onSuccess, onError } = options;

    if (delay > 0) await sleep(delay);

    try {
      const textArea = document.createElement('textarea');
      textArea.value = text;
      textArea.style.position = 'fixed';
      textArea.style.opacity = '0';
      document.body.appendChild(textArea);
      textArea.select();
      textArea.setSelectionRange(0, textArea.value.length);
      document.execCommand('copy');
      document.body.removeChild(textArea);

      onSuccess?.();
      window.$message?.success('复制成功');
    } catch (err: any) {
      onError?.(err);
      window.$message?.error('复制失败');
      throw err;
    }
  };
}

export function useClipboard() {
  const isSuccess = ref<boolean | null>(null);
  const error = ref<Error | null>(null);
  const realCopy = createCopyFunction();

  const copy: CopyFn = async (text, options = {}) => {
    const { resetAfter = 1000, onSuccess, onError } = options;

    try {
      await realCopy(text, {
        ...options,
        onSuccess: () => {
          isSuccess.value = true;
          error.value = null;
          onSuccess?.();
        },
        onError: err => {
          isSuccess.value = false;
          error.value = err;
          onError?.(err);
        }
      });

      if (resetAfter > 0) {
        setTimeout(() => {
          isSuccess.value = null;
          error.value = null;
        }, resetAfter);
      }
    } catch {
      // 错误已经处理过了
    }
  };

  return {
    copy,
    isSuccess,
    error
  };
}
