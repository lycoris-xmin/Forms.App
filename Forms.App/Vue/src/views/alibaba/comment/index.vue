<script setup lang="tsx">
import { DeleteOutlined, EyeOutlined, StepForwardOutlined } from '@ant-design/icons-vue';
import { reactive, ref } from 'vue';
import { useTable, useTableOperate, useTableScroll } from '@/hooks/common/table';
import {
  apiUrl,
  fetchDeleteAliPlanTaskComment,
  fetchGetAliPlanTaskCommentList,
  fetchRunRightNowAliPlanTaskComment
} from '@/service/api';
import AppPermissions, { useRolePermission } from '@/hooks/custome/app.permissions';
import { planTaskCommentStatus as COMMENT_STATUS, platform as PLATFORM } from '@/views/shared/enums';
import { useAuthStore } from '@/store/modules/auth';
import { imgFallback } from '@/data/empty.json';
import { copyToClipboard } from '@/utils/helper';
import SearchFilter from './modules/search-filter.vue';
import VideoPreviewModal from './modules/video-preview-modal.vue';
import OperateDrawer from './modules/operate-drawer.vue';
import CheckDrawer from './modules/check-products.vue';

interface ProductItem {
  imageUrl?: string;
  productImage?: string;
  name?: string;
  productTitle?: string;
  sku?: string;
  spec?: string;
  quantity?: number;
}

interface TableRecord {
  id: string;
  platform: number;
  taskId?: string;
  orderId?: string;
  productImage?: string;
  productTitle?: string;
  text?: string;
  images: string[];
  videos: string[];
  status: number;
  creator?: string;
  createTime: string;
  beginTime: string;
  count?: number;
  products?: ProductItem[];
  roleId?: number;
  tenantUser?: string;
  count?: number; // 商品数量（关键字段）
  products?: ProductItem[]; // 多商品时的商品列表
}

const authStore = useAuthStore();
const rolePermission = useRolePermission();
const { tableWrapperRef, scrollConfig } = useTableScroll();
const drawerVisible = ref(false);
const operateType = ref<'create' | 'update'>('create');
const editRowData = ref<TableRecord>({} as TableRecord);

const model = reactive({
  videoVisible: false,
  video: ''
});

const expandedItems = ref(new Map<string, boolean>());

const {
  columns,
  columnChecks,
  data,
  getData,
  getDataByPage,
  loading,
  mobilePagination,
  searchParams,
  resetSearchParams
} = useTable<TableRecord>({
  apiFn: fetchGetAliPlanTaskCommentList,
  apiParams: {
    pageIndex: 1,
    pageSize: 10
  },
  columns: getColumns
});

const { checkedRowKeys, rowSelection, onBatchDeleted, onDeleted } = useTableOperate(data, getData);

async function batchDeleteHandler() {
  if (authStore.userInfo.isTenant) {
    const ids = checkedRowKeys.value;
    if (ids?.length) {
      const { data: res, error } = await fetchDeleteAliPlanTaskComment(ids);
      if (!error && res?.code === 0) {
        onBatchDeleted();
      }
    }
  }
}

async function deleteHandler(id: string) {
  const { data: res, error } = await fetchDeleteAliPlanTaskComment([id]);
  if (!error && res?.code === 0) {
    onDeleted();
  }
}

// function modifyButton(record: TableRecord) {
//   if (!rolePermission.hasOne(AppPermissions.Taobao.Comment.UPDATE)) {
//     return null;
//   }

//   return (
//     <ATooltip placement="top" title="修改评价">
//       <AButton type="link" size="small" onClick={() => handleModify(record.id)}>
//         <EditOutlined />
//       </AButton>
//     </ATooltip>
//   );
// }

// function handleModify(record: TableRecord) {
//   editRowData.value = { ...record };
//   operateType.value = 'edit';
//   drawerVisible.value = true;
// }

// function handleAdd() {
//   const defaultPlatform = PLATFORM.length > 0 ? PLATFORM[0].value : 0;
//   const defaultStatus = COMMENT_STATUS.length > 0 ? COMMENT_STATUS[0].value : 0;

//   editRowData.value = {
//     id: '',
//     platform: defaultPlatform,
//     taskId: '',
//     productTitle: '',
//     text: '',
//     images: [],
//     videos: [],
//     status: defaultStatus
//   } as TableRecord;

//   operateType.value = 'create';
//   drawerVisible.value = true;
// }

function getColumns() {
  const baseColumns = [
    {
      key: 'id',
      dataIndex: 'id',
      title: '订单详情',
      width: 400,
      ellipsis: true,
      customRender: ({ record }: { record: TableRecord }) => {
        const platform = PLATFORM.find(x => x.value === record.platform) || PLATFORM[0];
        const productImage =
          window.$isDebugger && record.productImage
            ? `${apiUrl}${record.productImage}`
            : record.productImage || imgFallback;

        const isMultiProduct = record.count > 1;
        const count = record.count || 1;
        const isExpanded = expandedItems.value.get(record.id) || false;

        // const toggleExpand = () => {
        //   expandedItems.value.set(record.id, !isExpanded);
        // };

        const renderProducts = () => {
          if (!isExpanded || !record.products?.length) return null;

          return (
            <div class="products-list mt-4 border-t border-gray-100 pt-4">
              {record.products.map((item, index) => {
                const itemImage = item.imageUrl || item.productImage || imgFallback;
                const itemName = item.name || item.productTitle || '未命名商品';
                const itemSku = item.sku || item.spec || '无规格';
                const itemQuantity = item.quantity || 1;

                return (
                  <div class="product-item flex items-center gap-3 py-2" key={index}>
                    <div class="product-thumbnail h-10 w-10 flex-shrink-0">
                      <AImage src={itemImage} width={40} height={40} fallback={imgFallback} />
                    </div>
                    <div class="product-info flex-grow-1 overflow-hidden">
                      <p class="product-name truncate text-sm font-medium">{itemName}</p>
                      <p class="product-sku text-xs text-gray-500">{itemSku}</p>
                      <p class="product-quantity text-xs text-gray-500">x{itemQuantity}</p>
                    </div>
                  </div>
                );
              })}
            </div>
          );
        };

        return (
          <div class="flex items-start gap-4">
            <div class="product-image-container h-20 w-20 flex-shrink-0">
              <AImage width={80} height={80} src={productImage} fallback={imgFallback} />
            </div>
            <div class="flex-1 overflow-hidden">
              <p class="title truncate text-gray-800 font-medium" title={record.productTitle}>
                <ATag color={platform.color}>{platform.label}联盟</ATag>
                {record.productTitle}
              </p>
              <p class="plantask-shop">店铺名称： {record.shopName}</p>
              <p
                class="task-id cursor-pointer text-sm text-gray-500 transition-colors hover:text-primary"
                onClick={() => copyToClipboard(record.taskId)}
              >
                任务编号：{record.taskId}
              </p>
              <p
                class="order-id cursor-pointer text-sm text-gray-500 transition-colors hover:text-primary"
                onClick={() => record.orderId && copyToClipboard(record.orderId)}
              >
                订单号：{record.orderId || '-'}
              </p>

              <div class="mt-2 flex items-center">
                <span class="inline-flex items-center rounded-full bg-blue-100 px-2 py-1 text-xs text-blue-800 font-medium">
                  {isMultiProduct ? `多商品 (${count}件)` : '单商品'}
                </span>
              </div>

              {renderProducts()}
            </div>
          </div>
        );
      }
    },
    {
      key: 'taskId',
      dataIndex: 'taskId',
      title: '评价详情',
      width: 400,
      ellipsis: true,
      customRender: ({ record }: { record: TableRecord }) => {
        const images = (record.images || []).map(x => (window.$isDebugger ? `${apiUrl}${x}` : x));
        // const videos = (record.videos || []).map(x => (window.$isDebugger ? `${apiUrl}${x}` : x));

        return (
          <div>
            <p class="comment-text mb-2 text-sm text-gray-700">{record.text || '无评论内容'}</p>
            <div class="media-container mt-2 flex flex-wrap gap-2">
              {images.map((x, i) => (
                <div class="image-container h-24 w-24 overflow-hidden rounded shadow-sm" key={`img-${i}`}>
                  <AImage width={100} height={100} src={x} fallback={imgFallback} />
                </div>
              ))}
              {/* {videos.map((x, i) => (
                <div
                  class="video-container relative h-24 w-24 cursor-pointer overflow-hidden rounded shadow-sm"
                  key={`vid-${i}`}
                  onClick={e => videoHandler(e, x)}
                >
                  <video width={100} height={100} controls src={x} class="object-cover" />
                  <div class="absolute inset-0 flex items-center justify-center bg-black/30 text-white">
                    <EyeOutlined />
                  </div>
                </div>
              ))} */}
            </div>
          </div>
        );
      }
    },
    {
      key: 'status',
      dataIndex: 'status',
      title: '状态',
      align: 'center',
      width: 100,
      customRender: ({ record }: { record: TableRecord }) => {
        const statusItem = COMMENT_STATUS.find(x => x.value === record.status) || COMMENT_STATUS[0];
        return <ATag color={statusItem.color}>{statusItem.label}</ATag>;
      }
    }
  ];

  if (!authStore.userInfo?.isTenant) {
    baseColumns.push({
      key: 'tenantUser',
      dataIndex: 'tenantUser',
      title: '所属商户',
      align: 'center',
      width: 200,
      ellipsis: true
    });
  } else if (!authStore.userInfo?.isTenantUser) {
    baseColumns.push({
      key: 'creator',
      dataIndex: 'creator',
      title: '创建员工',
      align: 'center',
      width: 200,
      ellipsis: true
    });
  }

  baseColumns.push(
    {
      key: 'createTime',
      dataIndex: 'createTime',
      title: '添加时间',
      align: 'center',
      width: 200
    },
    {
      key: 'beginTime',
      dataIndex: 'beginTime',
      title: '预计评价时间',
      align: 'center',
      width: 200
    },
    {
      key: 'operate',
      title: '操作',
      align: 'center',
      width: 100,
      customRender: ({ record }: { record: TableRecord }) => (
        <div class="flex justify-center gap-2">
          {runRightNowButton(record)}
          {deleteButton(record)}
          {/* {modifyButton(record)}隐藏修改组件 */}
          {record.count > 1 && <DetailButton record={record} />}
        </div>
      )
    }
  );

  return baseColumns;
}

function runRightNowButton(record: TableRecord) {
  if (rolePermission.hasOne(AppPermissions.Ali1688.Comment.CREATE) && authStore.userInfo.isTenant) {
    if (record.id && record.status !== 1) {
      return (
        <APopconfirm title="确认立即发布评价吗？" onConfirm={() => runRightNowHandler(record.id)}>
          <ATooltip placement="top" title="立即发布">
            <AButton size="small" type="link">
              <StepForwardOutlined />
            </AButton>
          </ATooltip>
        </APopconfirm>
      );
    }
  }
  return '';
}

function deleteButton(record: TableRecord) {
  if (rolePermission.hasOne(AppPermissions.Ali1688.Comment.DELETE) && authStore.userInfo.isTenant) {
    if (record.roleId !== 1000 && record.status !== 1) {
      return (
        <APopconfirm title="确认删除吗？" onConfirm={() => deleteHandler(record.id)}>
          <ATooltip placement="top" title="删除">
            <AButton danger size="small" type="link">
              <DeleteOutlined />
            </AButton>
          </ATooltip>
        </APopconfirm>
      );
    }
  }
  return '';
}

// function videoHandler(e: Event, src: string) {
//   e.stopPropagation();
//   e.preventDefault();
//   model.video = src;
//   model.videoVisible = true;
// }

function DetailButton({ record }) {
  if (authStore.userInfo.isTenant)
    return (
      <ATooltip placement="top" title="查看商品">
        <AButton type="link" size="small" onClick={() => showDetailDrawer(record.taskId)}>
          <EyeOutlined />
        </AButton>
      </ATooltip>
    );
}

function showDetailDrawer(taskId: string) {
  model.detailVisible = true;
  model.taskIdForCheck = taskId;
}

async function runRightNowHandler(commentId: string) {
  try {
    const { data: res, error } = await fetchRunRightNowAliPlanTaskComment(commentId);
    if (!error && res?.code === 0) {
      window.$message?.success('发布操作成功');
      getDataByPage();
    }
  } catch {}
}
</script>

<template>
  <div class="page min-h-500px flex-col-stretch gap-16px overflow-hidden lt-sm:overflow-auto">
    <SearchFilter
      v-model:model="searchParams"
      :user-info="authStore.userInfo"
      :status="COMMENT_STATUS"
      @reset="resetSearchParams"
      @search="getDataByPage(1)"
    />

    <ACard
      title="评价列表"
      :bordered="false"
      :body-style="{ flex: 1, overflow: 'hidden' }"
      class="plantaskcomment flex-col-stretch sm:flex-1-hidden card-wrapper"
    >
      <template #extra>
        <TableHeaderOperation
          v-model:columns="columnChecks"
          :disabled-delete="checkedRowKeys.length === 0"
          :loading="loading"
          :delete-btn="authStore.userInfo.isTenant && rolePermission.hasOne(AppPermissions.Ali1688.Task.DELETE)"
          @delete="batchDeleteHandler"
        >
          <!--
 <AButton
            v-if="rolePermission.hasOne(AppPermissions.Taobao.Comment.CREATE)"
            type="primary"
            size="small"
            @click="handleAdd"
          >
            <PlusOutlined />
            新增评价
          </AButton> 隐藏添加
-->
        </TableHeaderOperation>
      </template>

      <ATable
        ref="tableWrapperRef"
        :columns="columns"
        :data-source="data"
        size="small"
        :row-selection="rowSelection"
        :scroll="scrollConfig"
        :loading="loading"
        row-key="id"
        :pagination="mobilePagination"
        class="h-full"
      />

      <VideoPreviewModal v-model:visible="model.videoVisible" :video="model.video" />
    </ACard>
    <OperateDrawer
      v-model:visible="drawerVisible"
      :operate-type="operateType"
      :row-data="editRowData"
      @submitted="getDataByPage"
    />
    <CheckDrawer v-model:visible="model.detailVisible" :task-id="model.taskIdForCheck || ''" />
  </div>
</template>

<style lang="scss" scoped>
.plantaskcomment {
  :deep(.plantaskcomment-product) {
    width: 100%;
    overflow: hidden;

    > .flex {
      &:first-child {
        padding-bottom: 5px;
      }

      &:last-child {
        gap: 10px;
        overflow: hidden;
      }
    }

    .ant-tag {
      font-size: 12px;
      line-height: 16px;
    }

    .plantask-image {
      flex-shrink: 0;
      width: 120px;
      height: 120px;
      border-radius: 5px;
      overflow: hidden;
    }

    .plantask-info {
      height: 100%;
      padding: 5px 0;
      overflow: hidden;
      text-overflow: ellipsis;
      word-wrap: break-word;
      white-space: nowrap;
    }

    .plantask-title {
      padding-bottom: 5px;

      .ant-tag {
        &.tb {
          background-color: rgb(255, 80, 0);
        }
      }

      a {
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-word;
        white-space: nowrap;
        font-weight: 600;
        letter-spacing: 1.5px;
      }
    }

    .plantask-shop {
      font-size: 12px;
      padding-bottom: 5px;
    }

    .plantask-sku {
      font-size: 12px;
      padding-bottom: 5px;

      .ant-tag {
        font-size: 12px;
        line-height: 14px;
        overflow: hidden;
        text-overflow: ellipsis;
        word-break: break-all;
        white-space: nowrap;
      }
    }

    .f-12px {
      font-size: 12px;
    }

    .plantask-shop,
    .plantask-sku,
    .plantask-price,
    .plantask-keyword {
      .label {
        display: inline-block;
        width: 60px;
        text-align: right;
      }
    }
  }

  :deep(.screenshot) {
    padding: 0 10px;
    flex-wrap: nowrap;
    position: relative;
    // -webkit-mask-image: linear-gradient(to right, rgba(0, 0, 0, 1) 80%, rgba(0, 0, 0, 0));
    overflow-x: auto;

    .img-div {
      flex-shrink: 0;
      overflow: hidden;
      border-radius: 5px;
      height: 100px;
      width: 50px;
    }
  }

  :deep(.title) {
    font-weight: 600;
    margin-bottom: 10px;
    overflow: hidden;
    text-overflow: ellipsis;
    word-wrap: break-word;
    white-space: nowrap;
  }

  :deep(.task-id),
  :deep(.order-id) {
    color: var(--color-dark-light);
    overflow: hidden;
    text-overflow: ellipsis;
    word-wrap: break-word;
    white-space: nowrap;
    cursor: pointer;
    transition: all 0.3s ease-in-out;

    &:hover {
      color: var(--color-purple);
    }
  }

  :deep(.products-list) {
    margin-top: 8px;
    padding-top: 8px;
    border-top: 1px solid #f0f0f0;
  }

  :deep(.product-item) {
    display: flex;
    align-items: center;
    padding: 4px 0;
  }

  :deep(.product-thumbnail) {
    flex-shrink: 0;
  }

  :deep(.product-info) {
    flex-grow: 1;
    overflow: hidden;
  }

  :deep(.product-name) {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  :deep(.image-container) {
    height: 100px;
    width: 100px;
    border-radius: 5px;
    overflow: hidden;
  }

  :deep(.video-container) {
    position: relative;

    video {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .wrap {
      position: absolute;
      inset: 0;
      background-color: rgba(8, 8, 8, 0.3);
      opacity: 0;
      transition: all 0.3s ease-in-out;
      gap: 6px;

      .act {
        color: #fff;
      }
    }

    &:hover {
      cursor: pointer;

      .wrap {
        opacity: 1;
      }
    }
  }

  :deep(.comment-text) {
    word-break: break-all;
    white-space: normal;
    line-height: 24px;
  }
}
</style>
