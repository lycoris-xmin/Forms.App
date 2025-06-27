import { flatRequest, objectToFormData } from '../../request';

export const fetchGetTaobaoPlanTaskCommentList = (input: Api.PlanTaskComment.SearchFilter) => {
  const { pageIndex, pageSize } = input;
  const data: Api.PlanTaskComment.SearchFilter = {
    pageIndex,
    pageSize
  };

  if (input.taskId) {
    data.taskId = input.taskId;
  }

  if (input.deviceId) {
    data.deviceId = input.deviceId;
  }

  if (input.orderId) {
    data.orderId = input.orderId;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  if (input.tenantId) {
    data.tenantId = input.tenantId;
  }

  return flatRequest({
    url: '/api/plantask/comment/taobao/list',
    params: { ...data }
  });
};

export const fetchCreatePlanTaskComment = (input: Api.PlanTaskComment.CreateComment) => {
  const { taskId, mode } = input;

  const json = {
    taskId,
    mode
  };

  if (input.text) {
    json.text = input.text;
  }

  const formData = objectToFormData(json);

  if (input.images && input.images.length) {
    for (const file of input.images) {
      formData.append('images', file);
    }
  }

  if (input.videos && input.videos.length) {
    for (const file of input.videos) {
      formData.append('videos', file);
    }
  }

  return flatRequest({
    url: '/api/plantask/comment',
    method: 'post',
    data: formData,
    headers: {
      'Content-Type':
        (input.images && input.images.length) || (input.videos && input.videos.length)
          ? 'multipart/form-data'
          : 'application/x-www-form-urlencoded'
    }
  });
};

export const fetchUpdatePlanTaskComment = (input: Api.PlanTaskComment.UpdateComment) => {
  const { id, taskId } = input;

  const formData = new FormData();

  formData.append('id', id);
  formData.append('taskId', taskId);

  if (input.text) {
    formData.append('text', input.text);
  }

  if (input.images && input.images.length) {
    for (let i = 0; i < input.images.length; i += 1) {
      formData.append(`images[${i}]`, input.images[i]);
    }
  }

  if (input.videos && input.videos.length) {
    for (let i = 0; i < input.videos.length; i += 1) {
      formData.append(`videos[${i}]`, input.videos[i]);
    }
  }

  return flatRequest({
    url: '/api/plantask/comment',
    method: 'post',
    data: formData,
    headers: {
      'Content-Type':
        (input.images && input.images.length) || (input.videos && input.videos.length)
          ? 'multipart/form-data'
          : 'application/x-www-form-urlencoded'
    }
  });
};

export const fetchDeleteTaobaoPlanTaskComment = (ids: Array<string>) => {
  return flatRequest({
    url: '/api/plantask/comment/taobao',
    method: 'delete',
    params: { ids }
  });
};

export const fetchRunRightNowTaobaoPlanTaskComment = (id: string) => {
  return flatRequest({
    url: `/api/plantask/comment/taobao/run/${id}`,
    method: 'put'
  });
};

export const fetchTaobaoSonList = (input: Api.PlanTaskComment.PlanTaskCommentListInput) => {
  const { pageIndex, pageSize } = input;
  const data: Api.PlanTaskComment.PlanTaskCommentListInput = {
    pageIndex,
    pageSize
  };

  if (input.taskId) {
    data.taskId = input.taskId;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  return flatRequest({
    url: '/api/plantask/comment/taobao/SonList',
    method: 'get',
    params: { ...data }
  });
};

export const fetchGetAliPlanTaskCommentList = (input: Api.PlanTaskComment.SearchFilter) => {
  const { pageIndex, pageSize } = input;
  const data: Api.PlanTaskComment.SearchFilter = {
    pageIndex,
    pageSize
  };

  if (input.taskId) {
    data.taskId = input.taskId;
  }

  if (input.deviceId) {
    data.deviceId = input.deviceId;
  }

  if (input.orderId) {
    data.orderId = input.orderId;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  if (input.tenantId) {
    data.tenantId = input.tenantId;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/plantask/comment/ali/list',
    params: { ...data }
  });
};

export const fetchDeleteAliPlanTaskComment = (ids: Array<string>) => {
  return flatRequest({
    url: '/api/plantask/comment/ali',
    method: 'delete',
    params: { ids }
  });
};

export const fetchRunRightNowAliPlanTaskComment = (id: string) => {
  return flatRequest({
    url: `/api/plantask/comment/ali/run/${id}`,
    method: 'put'
  });
};

export const fetchAliSonList = (input: Api.PlanTaskComment.PlanTaskCommentListInput) => {
  const { pageIndex, pageSize } = input;
  const data: Api.PlanTaskComment.PlanTaskCommentListInput = {
    pageIndex,
    pageSize
  };

  if (input.taskId) {
    data.taskId = input.taskId;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  return flatRequest({
    url: '/api/plantask/comment/ali/SonList',
    method: 'get',
    params: { ...data }
  });
};

// douyin
export const fetchGetDouyinPlanTaskCommentList = (input: Api.PlanTaskComment.SearchFilter) => {
  const { pageIndex, pageSize } = input;
  const data: Api.PlanTaskComment.SearchFilter = {
    pageIndex,
    pageSize
  };

  if (input.taskId) {
    data.taskId = input.taskId;
  }

  if (input.deviceId) {
    data.deviceId = input.deviceId;
  }

  if (input.orderId) {
    data.orderId = input.orderId;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  if (input.beginTime) {
    data.beginTime = input.beginTime;
  }

  if (input.endTime) {
    data.endTime = input.endTime;
  }

  if (input.tenantId) {
    data.tenantId = input.tenantId;
  }

  if (input.userId) {
    data.userId = input.userId;
  }

  return flatRequest({
    url: '/api/plantask/comment/douyin/list',
    params: { ...data }
  });
};

export const fetchDeleteDouyinPlanTaskComment = (ids: Array<string>) => {
  return flatRequest({
    url: '/api/plantask/comment/douyin',
    method: 'delete',
    params: { ids }
  });
};

export const fetchRunRightNowDouyinPlanTaskComment = (id: string) => {
  return flatRequest({
    url: `/api/plantask/comment/douyin/run/${id}`,
    method: 'put'
  });
};

export const fetchDouyinSonList = (input: Api.PlanTaskComment.PlanTaskCommentListInput) => {
  const { pageIndex, pageSize } = input;
  const data: Api.PlanTaskComment.PlanTaskCommentListInput = {
    pageIndex,
    pageSize
  };

  if (input.taskId) {
    data.taskId = input.taskId;
  }

  if (typeof input.status === 'number') {
    data.status = input.status;
  }

  return flatRequest({
    url: '/api/plantask/comment/douyin/SonList',
    method: 'get',
    params: { ...data }
  });
};
