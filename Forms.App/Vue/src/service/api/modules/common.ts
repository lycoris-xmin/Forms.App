import { flatRequest } from '../../request';

export function fetchSlider(input: Api.Common.Slider) {
  const { path, width, height, l, r, timespan } = input;
  return flatRequest({
    url: '/api/common/slider',
    params: {
      path,
      width,
      height,
      l,
      r,
      timespan
    }
  });
}
