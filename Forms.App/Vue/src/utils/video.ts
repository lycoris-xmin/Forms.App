import { imgFallback } from '@/data/empty.json';

export interface VideoInfo {
  file: File | null;
  size: number | null;
  type: string;
  src: string;
  duration: number | null;
  durationText: string;
  sizeText: string;
  thumbnail: string;

  isFileSizeOver(limitMB: number): boolean;
  dispose(): void;
}

/** 获取视频缩略图（base64） */
function captureThumbnail(videoElement: HTMLVideoElement): Promise<string> {
  return new Promise(resolve => {
    const canvas = document.createElement('canvas');
    canvas.width = videoElement.videoWidth;
    canvas.height = videoElement.videoHeight;
    const ctx = canvas.getContext('2d');

    if (ctx) {
      ctx.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
      requestAnimationFrame(() => {
        resolve(canvas.toDataURL('image/jpeg', 0.8));
      });
    } else {
      resolve('');
    }
  });
}

/** 分析视频文件，生成 info 实例对象 */
export default async function videoInfo(input: File, seekTime?: number): Promise<VideoInfo> {
  if (!input.type.includes('video')) {
    throw new Error('请上传一个视频文件');
  }

  const src = URL.createObjectURL(input);

  return new Promise<VideoInfo>((resolve, reject) => {
    const video = document.createElement('video');
    video.preload = 'metadata';
    video.src = src;
    video.crossOrigin = 'anonymous';
    video.muted = true;

    video.onloadedmetadata = () => {
      const duration = video.duration;
      const durationText = `${Math.floor(duration / 60)} 分 ${Math.floor(duration % 60)} 秒`;

      let snapshotTime = 0;
      if (seekTime !== undefined && !Number.isNaN(seekTime) && seekTime <= duration) {
        snapshotTime = seekTime;
      } else if (duration > 1) {
        snapshotTime = duration / 2;
      }

      video.currentTime = snapshotTime;

      video.onseeked = async () => {
        let thumbnail = '';
        try {
          thumbnail = await captureThumbnail(video);
        } catch {
          thumbnail = imgFallback;
        }

        const size = input.size;
        let sizeText = '0';
        if (size < 1024 * 1024) {
          sizeText = `${(size / 1024).toFixed(2)} KB`;
        } else if (size < 1024 * 1024 * 1024) {
          sizeText = `${(size / 1024 / 1024).toFixed(2)} MB`;
        } else {
          sizeText = `${(size / 1024 / 1024 / 1024).toFixed(2)} GB`;
        }

        const info: VideoInfo = {
          file: input,
          size,
          type: input.type,
          src,
          duration,
          durationText,
          sizeText,
          thumbnail,

          isFileSizeOver(limitMB: number): boolean {
            return this.size !== null && this.size > limitMB * 1024 * 1024;
          },

          dispose(): void {
            if (this.src) {
              URL.revokeObjectURL(this.src);
            }
          }
        };

        resolve(info);
      };
    };

    video.onerror = () => {
      URL.revokeObjectURL(src);
      reject(new Error('视频加载失败'));
    };
  });
}
