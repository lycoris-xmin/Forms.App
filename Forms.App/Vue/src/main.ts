import { createApp } from 'vue';
import './plugins/assets';
import FingerprintJS from '@fingerprintjs/fingerprintjs';
import { setupAppVersionNotification, setupDayjs, setupIconifyOffline, setupLoading, setupNProgress } from './plugins';
import { setupStore } from './store';
import { setupRouter } from './router';
import { setupI18n } from './locales';
import directives from './directives';
import App from './App.vue';

async function setupApp() {
  window.$isDebugger = import.meta.env.MODE === 'test';

  await generateFingerprint();

  setupLoading();

  setupNProgress();

  setupIconifyOffline();

  setupDayjs();

  const app = createApp(App);

  setupStore(app);

  await setupRouter(app);

  setupI18n(app);

  setupAppVersionNotification();

  if (directives && directives.length) {
    for (const item of directives) {
      app.directive(item.name, item.directive);
    }
  }

  app.mount('#app');

  async function generateFingerprint() {
    // 初始化 FingerprintJS
    const fp = await FingerprintJS.load();

    // 获取用户的指纹
    const result = await fp.get();

    // 设置指纹到 data 属性
    window.$fingerprint = result.visitorId;
  }
}

setupApp();
