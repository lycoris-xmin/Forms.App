import type { EnumMap } from '@/views/shared/types';

export const triggerType: Array<EnumMap> = [
  {
    value: 0,
    label: 'Cron',
    color: 'magenta'
  },
  {
    value: 1,
    label: '普通',
    color: 'processing'
  }
];

export const schedulerStatus: Array<EnumMap> = [
  {
    value: 0,
    label: '未运行',
    color: ''
  },
  {
    value: 1,
    label: '运行中',
    color: 'success'
  },
  {
    value: 2,
    label: '暂停',
    color: 'warning'
  }
];
