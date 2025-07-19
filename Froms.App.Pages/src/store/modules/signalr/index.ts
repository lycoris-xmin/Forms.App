import { defineStore } from 'pinia';
import { reactive } from 'vue';
import { SetupStoreId } from '@/enum';

export const useSignalRStore = defineStore(SetupStoreId.Signalr, () => {
  // 存储多个连接：key 为 url
  const connections = reactive<Record<string, signalR.HubConnection>>({});

  // 存储连接状态
  const connectionStatus = reactive<Record<string, boolean>>({});

  const listeners = reactive<Record<string, Record<string, (message: any) => void>>>({});

  const isConnected = (url: string): boolean => Boolean(connectionStatus[url]);

  const setConnection = (url: string, conn: signalR.HubConnection): void => {
    connections[url] = conn;
  };

  const getConnection = (url: string): SignalRConnection => {
    return {
      hub: connections[url],
      listeners: listeners[url] ?? {}
    };
  };

  const hasConnection = (url: string): boolean => {
    return connections[url] !== undefined && connections[url] !== null;
  };

  const connect = (url: string): void => {
    connectionStatus[url] = true;
  };

  const disconnect = (url: string): void => {
    connectionStatus[url] = false;
  };

  const addListener = (url: string, channel: string, callback: (message: any) => void): void => {
    listeners[url] ??= {};
    listeners[url][channel] = callback;
  };

  const removeListener = (url: string, channel: string): void => {
    if (listeners[url]?.[channel]) {
      Reflect.deleteProperty(listeners[url], channel);
    }
  };

  const triggerListeners = (url: string, channel: string, message: any): void => {
    const cb = listeners[url]?.[channel];
    if (cb) {
      cb(message);
    }
  };

  return {
    isConnected,
    setConnection,
    getConnection,
    hasConnection,
    connect,
    disconnect,
    addListener,
    removeListener,
    triggerListeners
  };
});

export interface SignalRConnection {
  hub: signalR.HubConnection;
  listeners: Record<string, (message: any) => void>;
}
