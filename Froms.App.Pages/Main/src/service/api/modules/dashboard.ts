import { flatRequest } from '../../request';

export function dashboardHeaderDataApi() {
  return flatRequest({
    url: '/api/dashboard/header/data'
  });
}

export function dashboardCardDataApi() {
  return flatRequest({
    url: '/api/dashboard/card/data'
  });
}

export function dashboardLineChartDataApi() {
  return flatRequest({
    url: '/api/dashboard/linechart/data'
  });
}

export function dashboardPieChartDataApi() {
  return flatRequest({
    url: '/api/dashboard/piechart/data'
  });
}
