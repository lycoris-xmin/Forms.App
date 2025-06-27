import { computed } from 'vue';
import { useCountDown, useLoading } from '@sa/hooks';
import { REG_EMAIL } from '@/constants/reg';

export function useCaptcha() {
  const { loading, startLoading, endLoading } = useLoading();
  const { count, start, stop, isCounting } = useCountDown(59);

  const label = computed(() => {
    let text = '获取验证码';

    const countingLabel = `${count.value}秒`;

    if (loading.value) {
      text = '';
    }

    if (isCounting.value) {
      text = countingLabel;
    }

    return text;
  });

  function isEmailValid(email: string) {
    if (email.trim() === '') {
      window.$message?.error?.('请输入邮箱地址');

      return false;
    }

    if (!REG_EMAIL.test(email)) {
      window.$message?.error?.('邮箱格式不正确');

      return false;
    }

    return true;
  }

  async function getCaptcha(callback) {
    // request
    const result = await callback();

    if (!result) {
      return;
    }

    startLoading();

    window.$message?.success?.('验证码发送成功');

    start();

    endLoading();
  }

  return {
    label,
    start,
    stop,
    isCounting,
    loading,
    isEmailValid,
    getCaptcha
  };
}
