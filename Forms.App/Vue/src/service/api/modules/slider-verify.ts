import { flatRequest } from '../../request';

export function sliderDefaultApi(input: Api.Slider.Default) {
  const { path } = input;
  return flatRequest({
    url: '/api/slider/verify/default',
    params: {
      path
    }
  });
}

export function sliderBlockPuzzleApi(input: Api.Slider.BlockPuzzle) {
  const { path, width, height, l, r, timespan } = input;
  return flatRequest({
    url: '/api/slider/verify/blockpuzzle',
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
