import { ref } from 'vue';

export interface CountdownOptions {
  onEverySecond?: (remainingSeconds: number) => void;
  onEnd?: () => void;
}

export function useCountdown(seconds: number, { onEverySecond, onEnd }: CountdownOptions = {}) {
  const isCounting = ref(false); // 倒计时是否在进行
  const remainingTime = ref(seconds); // 剩余时间（秒）

  let requestId: number | null = null;
  const startTime = performance.now();
  const endTime = startTime + seconds * 1000; // 结束时间（毫秒）
  let lastReportedSecond = seconds;

  const tick = () => {
    const now = performance.now();
    const remainingMillis = Math.max(endTime - now, 0); // 剩余时间（毫秒）
    const remainingSeconds = Math.ceil(remainingMillis / 1000); // 转换为剩余秒数

    // 每秒更新回调
    if (remainingSeconds !== lastReportedSecond) {
      lastReportedSecond = remainingSeconds;
      remainingTime.value = remainingSeconds;
      if (onEverySecond) {
        onEverySecond(remainingSeconds);
      }
    }

    // 如果还有剩余时间，则继续递归调用
    if (remainingMillis > 0) {
      requestId = requestAnimationFrame(tick);
    } else if (onEnd) {
      onEnd();
    }
  };

  const start = () => {
    if (!isCounting.value) {
      isCounting.value = true;
      requestId = requestAnimationFrame(tick); // 开始倒计时
    }
  };

  const cancel = () => {
    if (requestId !== null) {
      cancelAnimationFrame(requestId);
      requestId = null;
      isCounting.value = false;
      remainingTime.value = seconds; // 重置为初始时间
    }
  };

  // 返回倒计时状态和取消函数
  return {
    isCounting,
    remainingTime,
    start,
    cancel
  };
}
