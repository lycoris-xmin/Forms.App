/* eslint-disable no-invalid-this */
/* eslint-disable func-names */
/* eslint-disable no-underscore-dangle */
/* eslint-disable max-params */
export type CanvasOperation = 'fill' | 'stroke' | 'clear' | 'beginPath' | 'closePath' | 'moveTo' | 'lineTo' | 'arc' | 'arcTo' | 'bezierCurveTo' | 'rect' | 'rotate' | 'scale' | 'translate' | 'clip';

export const PI = Math.PI;

export function sum(x: number, y: number): number {
  return x + y;
}

export function square(x: number): number {
  return x * x;
}

// 为了消除 `this` 的隐式类型问题，我们为函数添加类型注释
export function draw(ctx: CanvasRenderingContext2D, x: number, y: number, l: number, r: number, operation: CanvasOperation): void {
  ctx.beginPath();
  ctx.moveTo(x, y);
  ctx.arc(x + l / 2, y - r + 2, r, 0.72 * PI, 2.26 * PI);
  ctx.lineTo(x + l, y);
  ctx.arc(x + l + r - 2, y + l / 2, r, 1.21 * PI, 2.78 * PI);
  ctx.lineTo(x + l, y + l);
  ctx.lineTo(x, y + l);
  ctx.arc(x + r - 2, y + l / 2, r + 0.4, 2.76 * PI, 1.24 * PI, true);
  ctx.lineTo(x, y);
  ctx.lineWidth = 2;
  ctx.fillStyle = 'rgba(255, 255, 255, 0.7)';
  ctx.strokeStyle = 'rgba(255, 255, 255, 0.7)';

  if (operation === 'fill') {
    ctx.fill();
  } else if (operation === 'stroke') {
    ctx.stroke();
  } else if (operation === 'clip') {
    ctx.clip(); // 重点：加上 clip 操作
  } else if (operation === 'clear') {
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
  } else if (operation === 'beginPath') {
    ctx.beginPath();
  } else if (operation === 'closePath') {
    ctx.closePath();
  } else if (operation === 'moveTo') {
    ctx.moveTo(x, y);
  } else if (operation === 'lineTo') {
    ctx.lineTo(x, y);
  }

  ctx.globalCompositeOperation = 'destination-over';
}

export function createImg(imgs: string[], onload: () => void): HTMLImageElement {
  const img = document.createElement('img');
  img.crossOrigin = 'Anonymous';
  img.onload = onload;
  img.onerror = () => {
    img.src = getRandomImg(imgs);
  };
  img.src = getRandomImg(imgs);
  return img;
}

export function getRandomNumberByRange(start: number, end: number): number {
  return Math.round(Math.random() * (end - start) + start);
}

export function getRandomImg(imgs: string[]): string {
  const len = imgs.length;
  return len > 0 ? imgs[getRandomNumberByRange(0, len - 1)] : 'https://source.unsplash.com/300x150/?book,library';
}

interface ThrottleOptions {
  leading: boolean;
  trailing: boolean;
  resultCallback?: (result: any) => void;
}

interface ThrottleOptions {
  leading: boolean;
  trailing: boolean;
  resultCallback?: (result: any) => void;
}

// eslint-disable-next-line @typescript-eslint/no-unsafe-function-type
export function throttle(fn: Function, interval: number, options: ThrottleOptions = { leading: true, trailing: true }): Function {
  const { leading, trailing, resultCallback } = options;
  let lastTime = 0;
  let timer: NodeJS.Timeout | null = null;

  const _throttle = function (this: any, ...args: any[]): Promise<any> {
    // 为 this 明确添加类型
    return new Promise(resolve => {
      const nowTime = new Date().getTime();
      if (!lastTime && !leading) lastTime = nowTime;

      const remainTime = interval - (nowTime - lastTime);
      if (remainTime <= 0) {
        if (timer) {
          clearTimeout(timer);
          timer = null;
        }

        const result = fn.apply(this, args);
        if (resultCallback) resultCallback(result);
        resolve(result);
        lastTime = nowTime;
        return;
      }

      if (trailing && !timer) {
        timer = setTimeout(() => {
          timer = null;
          lastTime = !leading ? 0 : new Date().getTime();
          const result = fn.apply(this, args);
          if (resultCallback) resultCallback(result);
          resolve(result);
        }, remainTime);
      }
    });
  };

  _throttle.cancel = function () {
    if (timer) clearTimeout(timer);
    timer = null;
    lastTime = 0;
  };

  return _throttle;
}
