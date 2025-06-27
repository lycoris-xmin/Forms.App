import { apiUrl } from '@/service/api';

type CountdownEverySecondCallback = (remainingSeconds: number) => void;
type CountdownEndCallback = () => void;

export function countdown(
  seconds: number,
  onEverySecond?: CountdownEverySecondCallback,
  onEnd?: CountdownEndCallback
): void {
  let requestId: number | null = null;
  const startTime = performance.now();
  const endTime = startTime + seconds * 1000; // 结束时间（毫秒）
  let lastReportedSecond = seconds;

  function tick() {
    const now = performance.now();
    const remainingTime = Math.max(endTime - now, 0); // 剩余时间（毫秒）
    const remainingSeconds = Math.ceil(remainingTime / 1000); // 转换为剩余秒数

    // 每秒更新回调
    if (remainingSeconds !== lastReportedSecond) {
      lastReportedSecond = remainingSeconds;
      if (onEverySecond) {
        onEverySecond(remainingSeconds);
      }
    }

    // 如果还有剩余时间，则继续递归调用
    if (remainingTime > 0) {
      requestId = requestAnimationFrame(tick);
    } else if (onEnd) {
      onEnd();
    }
  }

  requestId = requestAnimationFrame(tick); // 开始倒计时

  // 添加取消计时功能
  return function cancelCountdown() {
    if (requestId !== null) {
      cancelAnimationFrame(requestId);
      requestId = null;
    }
  };
}

export function formatSecondsToMinutes(seconds: number): string {
  if (seconds < 0) {
    throw new Error('秒数不能为负数');
  }

  const minutes = Math.floor(seconds / 60); // 计算分钟数
  const remainingSeconds = seconds % 60; // 计算剩余秒数

  // 补零格式化
  const formattedMinutes = minutes < 10 ? `0${minutes}` : `${minutes}`;
  const formattedSeconds = remainingSeconds < 10 ? `0${remainingSeconds}` : `${remainingSeconds}`;

  if (formattedMinutes === '00') return `${formattedSeconds} 秒`;
  return `${formattedMinutes} 分钟 ${formattedSeconds} 秒`;
}

export function triggerDownload(url, filename) {
  // 创建一个隐藏的 a 元素
  const a = document.createElement('a');
  a.href = url;

  // 如果需要指定下载的文件名
  if (filename) {
    a.download = filename;
  }

  // 将 a 元素添加到文档中
  document.body.appendChild(a);

  // 模拟点击 a 元素
  a.click();

  // 点击后移除 a 元素
  document.body.removeChild(a);
}

export const downloadFile = async (url, fileName) => {
  try {
    let downloadUrl = url;
    if (!url.startsWith('http')) {
      downloadUrl = `${apiUrl}${url}`;
    }

    const response = await fetch(downloadUrl, { method: 'GET', mode: 'cors' });

    if (!response.ok) {
      window.$message?.error('下载异常');
      return;
    }

    const blob = await response.blob();
    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = URL.createObjectURL(blob);

    if (fileName) {
      a.download = fileName;
    } else {
      a.download = url.split('/').pop();
    }

    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(a.href); // 释放 URL
  } catch (error) {
    window.$message?.error(`下载失败:${error}`);
  }
};

export function copyToClipboard(text) {
  // 创建一个文本区域元素
  const textArea = document.createElement('textarea');

  // 设置文本区域的内容为需要复制的文字
  textArea.value = text;

  // 将文本区域添加到 DOM 中
  document.body.appendChild(textArea);

  // 选择文本区域中的内容
  textArea.select();
  textArea.setSelectionRange(0, textArea.value.length); // 对于移动设备，必须使用这个方法

  // 执行复制操作
  document.execCommand('copy');

  // 删除文本区域元素
  document.body.removeChild(textArea);

  window.$message?.success('复制成功');
}
