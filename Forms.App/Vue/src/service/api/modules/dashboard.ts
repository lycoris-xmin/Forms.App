import { flatRequest } from '../../request';

export function fetchDashboardHeaderData() {
  return flatRequest({
    url: '/api/dashboard/header/data'
  });
}

export function fetchDashboardCardData() {
  return flatRequest({
    url: '/api/dashboard/card/data'
  });
}

export function fetchDashboardLineChartData() {
  return flatRequest({
    url: '/api/dashboard/linechart/data'
  });
}

export function fetchDashboardPieChartData() {
  return flatRequest({
    url: '/api/dashboard/piechart/data'
  });
}

export const getStatisticsSummary = () => {
  return flatRequest({
    url: '/api/dashboard/statistics',
    method: 'get'
  });
};
