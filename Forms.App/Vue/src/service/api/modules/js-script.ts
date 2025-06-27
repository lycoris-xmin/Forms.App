import { flatRequest } from '../../request';

export function fetchGetJsScriptList(input: Api.AutoJsScript.ListSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.name) {
    data.name = input.name;
  }

  if (input.version) {
    data.version = input.version;
  }

  return flatRequest({
    url: '/api/js/script/list',
    params: data
  });
}

export function fetchCreateJsScript(input: Api.AutoJsScript.CreateAutoJsScript) {
  const { name, version, filePath, size } = input;
  const data = {
    name,
    version,
    filePath,
    size,
    remark: ''
  };

  if (input.remark) {
    data.remark = input.remark;
  }

  return flatRequest({
    url: '/api/js/script',
    method: 'post',
    data
  });
}

export function fetchDeleteJsScript(ids: Array<string>) {
  return flatRequest({
    url: '/api/js/script',
    method: 'delete',
    params: { ids }
  });
}

export function fetchUploadJsScriptChunk(input: Api.AutoJsScript.UploadChunkRequest, onUploadProgress: () => void) {
  const formData = new FormData();
  Object.keys(input).forEach(x => {
    formData.append(x, input[x]);
  });

  return flatRequest({
    url: '/api/js/script/chunk/upload',
    method: 'post',
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    },
    onUploadProgress
  });
}

export function fetchMergeJsScriptChunk(input: Api.AutoJsScript.MergeChunkRequest) {
  const { name, version, totalChunks, chunkFolder } = input;
  return flatRequest({
    url: '/api/js/script/chunk/merge',
    method: 'post',
    data: {
      name,
      version,
      totalChunks,
      chunkFolder
    }
  });
}
