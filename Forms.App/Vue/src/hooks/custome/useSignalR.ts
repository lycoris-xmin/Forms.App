import * as signalR from '@microsoft/signalr';
import { useSignalRStore } from '@/store/modules/signalr';
import { useAuthStore } from '@/store/modules/auth';
import { getServiceBaseURL } from '@/utils/service';

const isHttpProxy = import.meta.env.DEV && import.meta.env.VITE_HTTP_PROXY === 'Y';

const { baseURL } = getServiceBaseURL(import.meta.env, isHttpProxy);

export default function useSignalR(url: string, accessToken: string | null = null) {
  const auth = useAuthStore();
  const store = useSignalRStore();

  const hubUrl = generateUrl(url);

  const createConnection = () => {
    if (!store.hasConnection(hubUrl)) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
          accessTokenFactory: () => accessToken || auth.token,
          withCredentials: true
        })
        .configureLogging(signalR.LogLevel.Error)
        .build();

      connection.onclose(() => {
        store.disconnect(hubUrl);
      });

      store.setConnection(hubUrl, connection);
    }
  };

  /** 开始连接 */
  const start = async () => {
    const conn = store.getConnection(hubUrl);
    if (conn && !store.isConnected(hubUrl)) {
      try {
        await conn.hub.start();
        store.connect(hubUrl);
        // eslint-disable-next-line no-console
        console.log(`✅ SignalR 已连接: ${hubUrl}`);
      } catch (err) {
        // eslint-disable-next-line no-console
        console.error(`❌ SignalR 启动失败 [${hubUrl}]:`, err);
      }
    }
  };

  /** 停止连接 */
  const stop = async () => {
    const conn = store.getConnection(hubUrl);
    if (conn && store.isConnected(hubUrl)) {
      await conn.hub.stop();
      store.disconnect(hubUrl);
      // eslint-disable-next-line no-console
      console.log(`🛑 SignalR 已断开: ${hubUrl}`);
    }
  };

  /**
   * 监听
   *
   * @param channel 监听频道
   * @param callback 回调函数
   * @returns
   */
  const on = (channel: string, callback: (message: any) => void) => {
    const conn = store.getConnection(hubUrl);
    if (!conn) return;

    if (!conn.listeners[channel]) {
      conn.hub.on(channel, (message: any) => {
        store.triggerListeners(hubUrl, channel, message);
      });
    }

    store.addListener(hubUrl, channel, callback);
  };

  /**
   * 取消监听
   *
   * @param channel 监听频道
   */
  const off = (channel: string) => {
    const conn = store.getConnection(hubUrl);
    if (conn) {
      conn.hub.off(channel);
      store.removeListener(hubUrl, channel);
    }
  };

  /**
   * 发送消息
   *
   * @param channel 发送频道
   * @param args 发送参数
   * @returns
   */
  const send = (channel: string, ...args: any[]) => {
    const conn = store.getConnection(hubUrl);
    if (conn && store.isConnected(hubUrl)) {
      // eslint-disable-next-line no-console
      return conn.hub.invoke(channel, ...args).catch(console.error);
    }

    return undefined; // 👈 保证始终有返回值
  };

  function generateUrl() {
    let base = baseURL.replace(/\/+$/, ''); // 去除末尾斜杠
    const path = url.replace(/^\/+/, ''); // 去除开头斜杠

    if (!url.includes('/signalr/hub/')) {
      base += '/signalr/hub';
    }

    return `${base}/${path}`;
  }

  /** 初始化连接 */
  createConnection();

  return {
    isConnected: store.isConnected(hubUrl),
    send,
    on,
    off,
    start,
    stop
  };
}
