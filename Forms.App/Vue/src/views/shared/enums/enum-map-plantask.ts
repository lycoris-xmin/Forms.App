export const planTaskMode = [
  {
    value: 0,
    label: '好友配对',
    info: '与自己所有好友进行任务匹配',
    color: 'magenta'
  },
  {
    value: 1,
    label: '指定好友',
    info: '指定好友进行任务匹配',
    color: 'cyan'
  },
  {
    value: 10,
    label: '私有分配',
    info: '对自己符合条件的设备进行分配',
    color: 'warning'
  },
  {
    value: 20,
    label: '指定设备',
    info: '指定自己的设备直接执行任务',
    color: 'purple'
  }
];

export const planTaskStatus = [
  {
    value: -1,
    label: '准备中',
    color: 'warning'
  },
  {
    value: 0,
    label: '未开始',
    color: 'default'
  },
  {
    value: 10,
    label: '配对中',
    color: 'processing'
  },
  {
    value: 11,
    label: '配对完成',
    color: 'magenta'
  },
  {
    value: 20,
    label: '商品搜索',
    color: 'cyan'
  },
  {
    value: 21,
    label: '等待付款',
    color: 'volcano'
  },
  {
    value: 30,
    label: '购买完成',
    color: 'purple'
  },
  {
    value: 31,
    label: '购买失败',
    color: 'magenta'
  },
  {
    value: 40,
    label: '任务成功',
    color: 'success'
  },
  {
    value: 41,
    label: '任务失败',
    color: 'error'
  },
  // {
  //   value: 100,
  //   label: '任务暂停',
  //   color: 'warning'
  // },
  {
    value: 110,
    label: '任务超时',
    color: 'error'
  },
  {
    value: 120,
    label: '停止任务',
    color: 'error'
  }
];

export const planTaskStep = [
  {
    value: 0,
    label: '商品搜索',
    color: 'cyan'
  },
  {
    value: 10,
    label: '商品浏览',
    color: 'cyan'
  },
  {
    value: 20,
    label: '商品核验',
    color: 'error'
  },
  {
    value: 30,
    label: '订单支付',
    color: 'warning'
  },
  {
    value: 40,
    label: '订单核验',
    color: 'success'
  }
];

export const planFilterStatus = [
  {
    value: -1,
    label: '准备中'
  },
  {
    value: 0,
    label: '未开始'
  },
  {
    value: 1,
    label: '进行中'
  },
  {
    value: 2,
    label: '任务成功'
  },
  {
    value: 3,
    label: '任务失败'
  },
  {
    value: 4,
    label: '任务停止'
  }
];

export const platFilterStatus = [
  {
    value: -1,
    label: '准备中'
  },
  {
    value: 0,
    label: '未开始'
  },
  {
    value: 1,
    label: '进行中'
  },
  {
    value: 2,
    label: '任务成功'
  },
  {
    value: 3,
    label: '任务失败'
  },
  {
    value: 4,
    label: '任务停止'
  }
];
