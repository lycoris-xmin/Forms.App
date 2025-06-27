/* eslint-disable no-console */
/* eslint-disable no-underscore-dangle */
import fs from 'node:fs';
import path from 'node:path';

// 移除指定目录下的 assets 文件夹及其内容
function removeAssetsFolder(targetDir) {
  const assetsDir = path.join(targetDir, 'assets');

  fs.rm(assetsDir, { recursive: true, force: true }, err => {
    if (err) {
      console.error(`Error removing assets directory: ${err}`);
    } else {
      console.log('Assets directory removed successfully.');
    }
  });
}

const __filename = new URL('', import.meta.url).pathname;
const __dirname = path.dirname(__filename);
const targetDirectory = path.join(__dirname, '../../../wwwroot');

removeAssetsFolder(targetDirectory.replace(/^\\+/, ''));
