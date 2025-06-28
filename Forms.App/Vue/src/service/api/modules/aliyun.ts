import { flatRequest } from '../../request';

export function getOssUploadPolicyApi({ dir, fileName }: { dir: string; fileName: string }) {
  return flatRequest({
    url: '',
    params: {
      dir,
      fileName
    }
  });
}
