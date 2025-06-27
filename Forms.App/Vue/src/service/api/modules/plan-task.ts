import { useAuthStore } from '@/store/modules/auth';
import { flatRequest, objectToFormData } from '../../request';

export function fetchGetTaobaoPlanTaskList(input: Api.PlanTask.ListSearchFilter) {
  const { pageIndex, pageSize, platform } = input;

  const data: Api.PlanTask.ListSearchFilter = {
    platform,
    pageIndex,
    pageSize
  };

  if (input.status && input.status.length) {
    data.status = input.status;
  }

  if (typeof input.mode === 'number') {
    data.mode = input.mode;
  }

  if (input.deviceId) {
    data.deviceId = input.deviceId;
  }

  if (input.shopId) {
    data.shopId = input.shopId;
  } else if (input.shopName) {
    data.shopName = input.shopName;
  }

  if (typeof input.success === 'boolean') {
    data.success = input.success;
  }

  if (input.title) {
    data.title = input.title;
  }

  if (input.orderId) {
    data.orderId = input.orderId;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  const authStore = useAuthStore();
  if (!authStore.userInfo.isTenant && input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/plantask/taobao/list',
    params: {
      ...data
    }
  });
}

export function fetchCreateTaobaoPlanTask(input: Api.PlanTask.CreatePlanTask) {
  const { mode, platform, shopId, shopName, products, count, actions, beginTime, endTime, chatMessages } = input;
  const json: Api.PlanTask.CreatePlanTask = {
    mode,
    platform,
    shopId,
    shopName,
    products,
    count,
    actions,
    beginTime,
    endTime,
    excludeArea: null,
    chatMessages
  };

  if (input.friendId) {
    json.friendId = input.friendId;
  }

  if (input.deviceId) {
    json.deviceId = input.deviceId;
  }

  if (input.chatMessages) {
    json.chatMessages = input.chatMessages;
  }

  if (input.pairScope !== void 0 && input.pairScope !== '') {
    json.pairScope = input.pairScope;
  }

  if (input.maxPairProductCount !== '' && typeof input.maxPairProductCount === 'number') {
    json.maxPairProductCount = input.maxPairProductCount;
  }

  if (input.excludeArea && input.excludeArea.length) {
    json.excludeArea = input.excludeArea;
  }

  if (input.multiplePlatform && input.multiplePlatform.length) {
    json.multiplePlatform = input.multiplePlatform;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: '/api/plantask/taobao',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchTaobaoStartPlanTask(ids?: string[]) {
  return flatRequest({
    url: '/api/plantask/taobao/start',
    method: 'post',
    data: {
      ids
    }
  });
}

export function fetchUpdatePlanTask(input: Api.PlanTask.UpdatePlanTask) {
  const { id, keyword, count, scope, actions, beginTime, endTime } = input;
  const json: Api.PlanTask.UpdatePlanTask = {
    id,
    keyword,
    count,
    scope,
    actions,
    beginTime,
    endTime,
    skuList: null,
    excludeArea: null
  };

  if (input.skuList && input.skuList.length) {
    json.skuList = input.skuList;
  }

  if (input.excludeArea && input.excludeArea.length) {
    json.excludeArea = input.excludeArea;
  }

  return flatRequest({
    url: '/api/plantask/taobao',
    method: 'put',
    data: json
  });
}

// 任务详情
export function fetchGetPlanTaskDetail(id: string) {
  return flatRequest({
    url: `/api/plantask/${id}`
  });
}

export function fetchDeletePlanTask(ids: Array<string>) {
  return flatRequest({
    url: '/api/plantask/taobao',
    method: 'delete',
    params: {
      ids
    }
  });
}

export function fetchAgainTaobaoPlanTask(input: Api.PlanTask.AgainPlanTask) {
  const json = buildAgainTaskJson(input);
  const formData = buildAgainTaskFormData(input, json);

  return flatRequest({
    url: '/api/plantask/taobao/again',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchGetPlanTaskSkuContinue(id: string) {
  return flatRequest({
    url: `/api/plantask/sku/continue/${id}`
  });
}

export function fetchPlanTaskContinue(input: Api.PlanTask.PlanTaskContinue) {
  const { id, cancel, skuList, count } = input;
  return flatRequest({
    url: '/api/plantask/continue',
    method: 'post',
    data: {
      id,
      continue: input.continue,
      cancel,
      skuList,
      count
    }
  });
}

export function fetchGetPlanTaskCompletion(id: string) {
  return flatRequest({
    url: `/api/plantask/completion/${id}`
  });
}

export function fetchUpdatePlanTaskCompletion(input: Api.PlanTask.PlanTaskCompletion) {
  const { id, success } = input;
  const data: Api.PlanTask.PlanTaskCompletion = {
    id,
    success: success === 1
  };
  if (data.success) {
    data.orderId = input.orderId;
    data.payPrice = input.payPrice;
    data.province = input.province;
    data.city = input.city;
    data.address = input.address;
    data.name = input.name;
    data.phone = input.phone;
    if (input.lovePayId) {
      if (input.lovePayId > 0) {
        data.lovePayId = input.lovePayId.toString();
      } else if (input.lovePayName) {
        data.lovePayName = input.lovePayName;
      }
    }
  }

  if (input.note) {
    data.note = input.note;
  }

  return flatRequest({
    url: '/api/plantask/completion',
    method: 'put',
    data
  });
}

export function fetchExportPlanTask(input: Api.PlanTask.ListSearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  input.platform = Number.parseInt(input.platform, 10);
  if (!Number.isNaN(input.platform)) {
    data.platform = input.platform;
  }

  if (input.title) {
    data.title = input.title;
  }

  input.status = Number.parseInt(input.status, 10);
  if (!Number.isNaN(input.status)) {
    data.status = input.status;
  }

  if (typeof input.mode === 'number') {
    data.mode = input.mode;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  const authStore = useAuthStore();
  if (!authStore.userInfo.isTenant && input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/plantask/taobao/export',
    method: 'post',
    data: {
      ...data
    }
  });
}

export function fetchTaobaoPlanTaskAmountList(input: any) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };
  return flatRequest({
    url: '/api/plantask/taobao/export/amount/list',
    params: {
      ...data
    }
  });
}

export function fetchTaobaoImportErrorExportList(input: any) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };
  return flatRequest({
    url: '/api/plantask/taobao/export/importerror/list',
    params: {
      ...data
    }
  });
}

export function fetchCheckExportPlanTaskComplete(excelId: string) {
  return flatRequest({
    url: `/api/plantask/checkExcel/${excelId}`
  });
}

export function fetchStopPlanTask(id: string) {
  return flatRequest({
    url: `/api/plantask/stop/${id}`,
    method: 'put'
  });
}

export function fetchGetAliPlanTaskList(input: Api.PlanTask.ListSearchFilter) {
  const { pageIndex, pageSize, platform } = input;

  const data: Api.PlanTask.ListSearchFilter = {
    platform,
    pageIndex,
    pageSize
  };

  if (input.status && input.status.length) {
    data.status = input.status;
  }

  if (typeof input.mode === 'number') {
    data.mode = input.mode;
  }

  if (typeof input.success === 'boolean') {
    data.success = input.success;
  }

  if (input.deviceId) {
    data.deviceId = input.deviceId;
  }

  if (input.shopId) {
    data.shopId = input.shopId;
  } else if (input.shopName) {
    data.shopName = input.shopName;
  }

  if (input.title) {
    data.title = input.title;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  if (input.orderId) {
    data.orderId = input.orderId;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  return flatRequest({
    url: '/api/plantask/ali/list',
    params: {
      ...data
    }
  });
}

export function fetchCreateAliPlanTask(input: Api.PlanTask.CreatePlanTask) {
  const { mode, platform, shopId, shopName, products, count, actions, beginTime, endTime } = input;
  const json: Api.PlanTask.CreatePlanTask = {
    mode,
    platform,
    shopId,
    shopName,
    products,
    count,
    actions,
    beginTime,
    endTime
  };

  if (input.friendId) {
    json.friendId = input.friendId;
  }

  if (input.deviceId) {
    json.deviceId = input.deviceId;
  }

  if (input.pairScope !== void 0 && input.pairScope !== '') {
    json.pairScope = input.pairScope;
  }

  if (input.maxPairProductCount !== '' && typeof input.maxPairProductCount === 'number') {
    json.maxPairProductCount = input.maxPairProductCount;
  }

  if (input.excludeArea && input.excludeArea.length) {
    json.excludeArea = input.excludeArea;
  }

  if (input.multiplePlatform && input.multiplePlatform.length) {
    json.multiplePlatform = input.multiplePlatform;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: '/api/plantask/ali',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchDeleteAliPlanTask(ids: Array<string>) {
  return flatRequest({
    url: '/api/plantask/ali',
    method: 'delete',
    params: {
      ids
    }
  });
}

export function fetchExportAliPlanTask(input: Api.PlanTask.ListSearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  input.platform = Number.parseInt(input.platform, 10);
  if (!Number.isNaN(input.platform)) {
    data.platform = input.platform;
  }

  if (input.title) {
    data.title = input.title;
  }

  input.status = Number.parseInt(input.status, 10);
  if (!Number.isNaN(input.status)) {
    data.status = input.status;
  }

  if (typeof input.mode === 'number') {
    data.mode = input.mode;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  const authStore = useAuthStore();
  if (!authStore.userInfo.isTenant && input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/plantask/ali/export',
    method: 'post',
    data: {
      ...data
    }
  });
}

export function fetchGetPlanTaskAgainDetail(id: string) {
  return flatRequest({
    url: `/api/plantask/taobao/againDetail/${id}`
  });
}

// 抖音列表显示
export function fetchGetDouyinPlanTaskList(input: Api.PlanTask.ListSearchFilter) {
  const { pageIndex, pageSize, platform } = input;

  const data: Api.PlanTask.ListSearchFilter = {
    platform,
    pageIndex,
    pageSize
  };

  if (input.status && input.status.length) {
    data.status = input.status;
  }

  if (typeof input.mode === 'number') {
    data.mode = input.mode;
  }

  if (input.deviceId) {
    data.deviceId = input.deviceId;
  }

  if (input.shopId) {
    data.shopId = input.shopId;
  } else if (input.shopName) {
    data.shopName = input.shopName;
  }

  if (typeof input.success === 'boolean') {
    data.success = input.success;
  }

  if (input.title) {
    data.title = input.title;
  }

  if (input.orderId) {
    data.orderId = input.orderId;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  const authStore = useAuthStore();
  if (!authStore.userInfo.isTenant && input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/plantask/douyin/list',
    params: {
      ...data
    }
  });
}

// 抖音任务添加
export function fetchCreateDouyinPlanTask(input: Api.PlanTask.CreatePlanTask) {
  const { mode, platform, shopId, shopName, products, count, actions, beginTime, endTime } = input;
  const json: Api.PlanTask.CreatePlanTask = {
    mode,
    platform,
    shopId,
    shopName,
    products,
    count,
    actions,
    beginTime,
    endTime,
    excludeArea: null
  };

  if (input.friendId) {
    json.friendId = input.friendId;
  }

  if (input.deviceId) {
    json.deviceId = input.deviceId;
  }

  if (input.pairScope !== void 0 && input.pairScope !== '') {
    json.pairScope = input.pairScope;
  }

  if (input.maxPairProductCount !== '' && typeof input.maxPairProductCount === 'number') {
    json.maxPairProductCount = input.maxPairProductCount;
  }

  if (input.excludeArea && input.excludeArea.length) {
    json.excludeArea = input.excludeArea;
  }

  if (input.multiplePlatform && input.multiplePlatform.length) {
    json.multiplePlatform = input.multiplePlatform;
  }

  const formData = objectToFormData(json);

  return flatRequest({
    url: '/api/plantask/douyin',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

// 抖音删除
export function fetchDeleteDouyinPlanTask(ids: Array<string>) {
  return flatRequest({
    url: '/api/plantask/douyin',
    method: 'delete',
    params: {
      ids
    }
  });
}

// 抖音再来一单
export function fetchAgainDouyinPlanTask(input: Api.PlanTask.AgainPlanTask) {
  const json = buildAgainTaskJson(input);
  const formData = buildAgainTaskFormData(input, json);

  return flatRequest({
    url: '/api/plantask/douyin/again',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

// 导出报表
export function fetchExportDouyinPlanTask(input: Api.PlanTask.ListSearchFilter) {
  const { pageIndex, pageSize } = input;

  const data = {
    pageIndex,
    pageSize
  };

  input.platform = Number.parseInt(input.platform, 10);
  if (!Number.isNaN(input.platform)) {
    data.platform = input.platform;
  }

  if (input.title) {
    data.title = input.title;
  }

  input.status = Number.parseInt(input.status, 10);
  if (!Number.isNaN(input.status)) {
    data.status = input.status;
  }

  if (typeof input.mode === 'number') {
    data.mode = input.mode;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  const authStore = useAuthStore();
  if (!authStore.userInfo.isTenant && input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/plantask/douyin/export',
    method: 'post',
    data: {
      ...data
    }
  });
}

export function fetchGetAliPlanTaskAgainDetail(id: string) {
  return flatRequest({
    url: `/api/plantask/ali/againDetail/${id}`
  });
}

export function fetchAgainAliPlanTask(input: Api.PlanTask.AgainPlanTask) {
  const json = buildAgainTaskJson(input);
  const formData = buildAgainTaskFormData(input, json);

  return flatRequest({
    url: '/api/plantask/ali/again',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  });
}

export function fetchUploadPlanTaskChunk(input: Api.PlanTask.PlanTaskUploadChunkRequest, onUploadProgress: () => void) {
  const formData = new FormData();

  Object.keys(input).forEach(x => {
    formData.append(x, input[x]);
  });

  return flatRequest({
    url: '/api/plantask/chunk/upload',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    },
    onUploadProgress
  });
}

export function fetchMergePlanTaskChunk(input: Api.PlanTask.PlanTaskMergeChunkRequest) {
  const { name, totalChunks, chunkFolder, fileExtension } = input;
  return flatRequest({
    url: '/api/plantask/chunk/merge',
    method: 'post',
    data: {
      name,
      totalChunks,
      chunkFolder,
      fileExtension
    }
  });
}

export function fetchTaobaoImportPlanTask(input: any) {
  const { filePath, isStop } = input;
  return flatRequest({
    url: '/api/plantask/taobao/import',
    method: 'post',
    data: { filePath, isStop }
  });
}

export function fetchCheckImportPlanTaskComplete(excelId: string) {
  return flatRequest({
    url: `/api/plantask/CheckImport/${excelId}`
  });
}

function buildAgainTaskJson(input: Api.PlanTask.AgainPlanTask): Api.PlanTask.AgainPlanTask {
  const {
    oldTaskId,
    mode,
    shopId,
    shopName,
    products,
    price,
    count,
    scope,
    actions,
    beginTime,
    endTime,
    chatMessages
  } = input;

  const json: Api.PlanTask.AgainPlanTask = {
    oldTaskId,
    mode,
    shopId,
    shopName,
    products,
    price,
    count,
    scope,
    actions,
    beginTime,
    endTime,
    chatMessages,
    excludeArea: null
  };

  if (input.deviceId) {
    json.deviceId = input.deviceId;
  }

  if (input.friendId) {
    json.friendId = input.friendId;
  }

  if (typeof input.pairScope === 'number') {
    json.pairScope = input.pairScope;
  }

  if (input.maxPairProductCount !== '' && typeof input.maxPairProductCount === 'number') {
    json.maxPairProductCount = input.maxPairProductCount;
  }

  if (input.excludeArea?.length) {
    json.excludeArea = input.excludeArea;
  }

  if (input.multiplePlatform?.length) {
    json.multiplePlatform = input.multiplePlatform;
  }

  return json;
}

function buildAgainTaskFormData(input: Api.PlanTask.AgainPlanTask, json: any): FormData {
  const formData = objectToFormData(json);

  if (typeof input.commentMode === 'number') {
    formData.append('commentMode', input.commentMode.toString());
    formData.append('text', input.text!);

    if (input.imageUrls?.length) {
      formData.imageUrls = input.imageUrls;
    } else if (input.images?.length) {
      for (const file of input.images) {
        formData.append('images', file);
      }
    }

    if (input.videoUrls?.length) {
      formData.videoUrls = input.videoUrls;
    } else if (input.videos?.length) {
      for (const file of input.videos) {
        formData.append('videos', file);
      }
    }
  }

  return formData;
}
