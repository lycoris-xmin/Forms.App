export * from './enum-map-device';
export * from './enum-map-plantask';
export * from './enum-map-product';
export * from './enum-map-command';
export * from './enum-map-logger';
export * from './enum-map-settlement';
export * from './enum-map-user';
export * from './enum-map-plantask-comment';
export * from './enum-map-friend';
export * from './enum-map-alipaylovepay';

export const gender = [
  {
    value: 0,
    label: '保密',
    color: 'default'
  },
  {
    value: 1,
    label: '男',
    color: 'processing'
  },
  {
    value: 2,
    label: '女',
    color: 'magenta'
  }
];

export const payType = [
  {
    value: 0,
    label: 'TB-ZFB免密支付',
    color: '#ff5000'
  },
  {
    value: 1,
    label: 'TB-ZFB亲密付',
    color: '#ff5000'
  },
  {
    value: 2,
    label: 'TB-ZFB他人代付',
    color: '#ff5000'
  },
  {
    value: 10,
    label: 'JD免密支付',
    color: '#ff0f23'
  },
  {
    value: 20,
    label: 'PDD-ZFB免密支付',
    color: '#d60a22'
  },
  {
    value: 30,
    label: 'DY-ZFB免密支付',
    color: '#000'
  },
  {
    value: 31,
    label: 'DY-ZFB亲密付',
    color: '#000'
  },
  {
    value: 40,
    label: '1688-ZFB亲密付',
    color: '#ff5000'
  }
];

export const platformAccountInit = [
  {
    value: 0,
    label: '等待初始化中',
    color: 'default'
  },
  {
    value: 1,
    label: '初始化中',
    color: 'warning'
  },
  {
    value: 2,
    label: '初始化完成',
    color: 'success'
  },
  {
    value: 3,
    label: '初始化失败',
    color: 'error'
  }
];

export const platform = [
  {
    value: 0,
    label: 'TB',
    color: '#ff5000'
  },
  {
    value: 10,
    label: 'JD',
    color: '#ff0f23'
  },
  {
    value: 20,
    label: 'PDD',
    color: '#d60a22'
  },
  {
    value: 30,
    label: 'DY',
    color: '#000'
  },
  {
    value: 40,
    label: '1688',
    color: '#ff5000'
  },
  {
    value: 50,
    label: 'RedNote',
    color: '#ff2442'
  }
];
