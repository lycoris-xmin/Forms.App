import { flatRequest } from '../../request';

export function fetchGetEmailConfiguration() {
  return flatRequest({
    url: '/api/configuration/email'
  });
}

export function fetchSaveEmailConfiguration(input: Api.Configuration.EmailConfiguration) {
  return flatRequest({
    url: '/api/configuration/email',
    method: 'put',
    data: {
      ...input
    }
  });
}

export function fetchEmailTest(email: string) {
  return flatRequest({
    url: '/api/configuration/email/test',
    method: 'post',
    data: {
      email
    }
  });
}

export function fetchGetConfigurationList(input: Api.Common.PageFilter) {
  const { pageIndex, pageSize } = input;
  return flatRequest({
    url: '/api/configuration/list',
    params: {
      pageIndex,
      pageSize
    }
  });
}

export function fetchUpdateConfiguration(input: Api.Configuration.UpdateConfiguration) {
  const { id, value } = input;
  return flatRequest({
    url: '/api/configuration',
    method: 'put',
    data: {
      id,
      value
    }
  });
}
