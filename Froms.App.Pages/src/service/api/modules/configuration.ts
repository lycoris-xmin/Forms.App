import { flatRequest } from '../../request';

export function getConfigurationListApi(input: Api.Configuration.ListSearchFilter) {
  const { pageIndex, pageSize } = input;
  const data = {
    pageIndex,
    pageSize
  };

  if (input.configName) {
    data.configName = input.configName;
  }

  return flatRequest({
    url: '/api/configuration/list',
    params: data
  });
}

export function updateConfigurationListApi({ id, value }: { id: string; value: string }) {
  return flatRequest({
    url: '/api/configuration',
    method: 'put',
    data: {
      id,
      value
    }
  });
}
