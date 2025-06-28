/**
 * Namespace Api
 *
 * All backend api type
 */
declare namespace Api {
  namespace Common {
    /** common params of paginating */
    interface PaginatingCommonParams {
      /** current page number */
      current: number;
      /** page size */
      size: number;
      /** total count */
      total: number;
    }

    /** common params of paginating query list data */
    interface PaginatingQueryRecord<T = any> extends PaginatingCommonParams {
      records: T[];
    }

    /** common search params of table */
    type CommonSearchParams = Pick<Common.PaginatingCommonParams, 'current' | 'size'>;

    /**
     * enable status
     *
     * - "1": enabled
     * - "2": disabled
     */
    type EnableStatus = '1' | '2';

    /** common record */
    type CommonRecord<T = any> = {
      /** record id */
      id: number;
      /** record creator */
      createBy: string;
      /** record create time */
      createTime: string;
      /** record updater */
      updateBy: string;
      /** record update time */
      updateTime: string;
      /** record status */
      status: EnableStatus;
    } & T;

    type PageFilter = {
      pageIndex: number;
      pageSize: number;
    };
  }

  /**
   * Namespace Auth
   *
   * Backend api module: "auth"
   */
  namespace Auth {
    interface LoginValidation {
      oathCode: string;
    }

    interface LoginToken {
      token: string;
      refreshToken: string;
    }

    interface Profile {
      id: string;
      email: string;
      nickName: string;
      avatar: string;
      gender: number;
      roleId: string;
      isAdmin: boolean;
      isVip: boolean;
      isLogin: boolean;
      isSettlement: boolean;
    }
  }

  /**
   * Namespace Route
   *
   * Backend api module: "route"
   */
  namespace Route {
    type ElegantConstRoute = import('@elegant-router/types').ElegantConstRoute;

    interface MenuRoute extends ElegantConstRoute {
      id: string;
    }

    interface UserRoute {
      routes: MenuRoute[];
      home: import('@elegant-router/types').LastLevelRouteKey;
    }
  }

  namespace UserProfile {
    type SaveProfile = {
      nickName: string;
      email: string;
      Wechat: string;
    };

    type ChangePassword = {
      oldPassword: string;
      password: string;
    };
  }

  namespace User {
    type ListSearchFilter = Common.PageFilter & {
      phone?: string | null;
      email?: string | null;
      roldId?: number | null;
      status?: number | null;
    };

    type Data = {
      id: string;
      phone: string;
      email: string;
      nickName: string;
      avatar: string;
      status: number;
      roleId: number;
      roleName?: string | null;
      createTime: string;
    };

    type UserCreate = {
      email: string;
      password: string;
      nickName: string;
      roleId: number;
    };

    type UserUpdate = {
      id: string;
      email?: string | null;
      pasword?: string | null;
      nickName?: string | null;
      roleId?: number | null;
      status?: number | null;
    };
  }

  namespace Enums {
    type Map<T = unknown> = {
      text: string;
      value: T;
    };

    type MapData<T = unknown, TData = unknown> = Map<T> & {
      data: TData;
    };
  }

  namespace Role {
    type Data = {
      id: string;
      roleName: string;
      count: string;
    };

    type UpdateRole = {
      id: string;
      roleName: string;
    };

    type RolePermissionChild = {
      key: string;
      title: string;
      selectable: boolean;
    };

    type RolePermission = {
      key: string;
      title: string;
      selectable: boolean;
      child: Array<RolePermissionChild>;
    };
  }

  namespace Dashboard {
    type HeaderData = {
      unSettlement: number;
      settlemented: number;
      settlementFailed: number;
    };

    type CardData = {
      totalDevice: number;
      onlineDevice: number;
      product: number;
      planTask: number;
    };

    type LineChart = {
      xAxis: Array<string>;
      total: Array<number>;
      success: Array<number>;
      failed: Array<number>;
    };

    type PieChart = {
      settlemented: number;
      settlementFailed: number;
      pairSettlementFailed: number;
      unSettlement: number;
      pairUnSettlement: number;
    };
  }

  namespace Configuration {
    type ListSearchFilter = Common.PageFilter & {
      configName?: string;
    };

    type Data = {
      id: string;
      configName: string;
      value: string;
      valueType: number;
      updator: string;
      updateTime: string;
    };
  }

  namespace Common {}

  namespace Slider {
    type Default = {
      path: string;
    };

    type BlockPuzzle = {
      path: string;
      width: number;
      height: number;
      l: number;
      r: number;
      timespan: string;
    };

    type BlockPuzzleData = {
      x: number;
      y: number;
      validate: string;
    };
  }

  namespace LoggerSystem {
    type ListFilter = Common.PageFilter & {
      level: number;
      beginTime: string;
      endTime: string;
    };

    type Data = {
      id: string;
      level: number;
      className: string;
      methodName: string;
      message: string;
      stackTrace: string;
      createTime: string;
    };
  }

  namespace LoggerRequest {
    type ListFilter = Common.PageFilter & {
      traceId: string;
      url: string;
      httpMethod: string;
      success: number;
      beginTime: string;
      endTime: string;
    };
  }

  namespace Redis {
    interface Monitor {
      server: MonitorServer;
      clients: MonitorClients;
      memory: MonitorMemory;
      stats: MonitorStats;
      keyspace: MonitorKeyspace;
      cpu: MonitorCpu;
    }

    interface MonitorCpu {
      /** Redis 系统级 CPU 使用时间（秒） */
      usedCpuSys: string;
      /** Redis 用户级 CPU 使用时间（秒） */
      usedCpuUser: string;
      /** 子进程的系统级 CPU 使用时间（秒） */
      usedCpuSysChildren: string;
      /** 子进程的用户级 CPU 使用时间（秒） */
      usedCpuUserChildren: string;
      /** 负载描述 */
      loadDescription: string;
    }

    interface MonitorKeyspace {
      /** 当前数据库中的键数量 */
      keys: string;
      /** 设置了过期时间的键数量 */
      expires: string;
      /** 平均存活时间（毫秒） */
      avgTtl: string;
    }

    /** 服务器相关信息 */
    interface MonitorServer {
      /** Redis 服务器版本号 */
      redisVersion?: string;

      /** 运行 Redis 的操作系统信息 */
      os?: string;

      /** Redis 服务器运行时间（秒） */
      uptimeInSeconds?: string;

      /** Redis 服务器运行时间（天） */
      uptimeInDays?: string;
    }

    /** 客户端相关信息 */
    interface MonitorClients {
      /** 当前连接的客户端数量 */
      connectedClients?: string;

      /** 连接负载 */
      loadDescription?: string;
    }

    /** 内存使用相关信息 */
    interface MonitorMemory {
      /** Redis 使用的内存量（字节） */
      usedMemory?: string;

      /** Redis 使用的内存量（人类可读格式） */
      usedMemoryHuman?: string;

      /** 内存碎片率 */
      memFragmentationRatio?: string;

      /** 最大内存 */
      maxMemory?: string;

      /** 最大内存 */
      maxMemHuman?: string;

      /** 内存占用率 */
      memoryUsage?: string;

      /** 内存负载 */
      loadDescription?: string;
    }

    /** 统计信息 */
    interface MonitorStats {
      /** 接收到的总连接数 */
      totalConnectionsReceived?: string;

      /** 已处理的总命令数 */
      totalCommandsProcessed?: string;

      /** 键命中次数 */
      keyspaceHits?: string;

      /** 键未命中次数 */
      keyspaceMisses?: string;

      /** 被拒绝的连接总数（达到 maxclients 限制时） */
      rejectedConnections?: string;

      /** 被驱逐的键总数（触发内存淘汰策略时） */
      evictedKeys?: string;

      /** 过期的键总数（因设置了过期时间） */
      expiredKeys?: string;

      /** 当前每秒处理命令数（QPS） */
      instantaneousOpsPerSec?: string;

      /** 缓存命中率 */
      loadDescription?: string;
    }
  }

  namespace Scheduler {
    type SearchFilter = Common.PageFilter & {
      jobGroup?: string;
      triggerType?: number;
      status?: number;
    };

    type RecordSearchFilter = Common.PageFilter & {
      jobKey: string;
      jobTraceId?: string;
      beginTime?: string;
      endTime?: string;
    };

    type Data = {
      id: string;
      jobKey: string;
      jobClass: string;
      jobName: string;
      jobGroup: string;
      triggerType: number;
      standby: boolean;
      cron?: string;
      intervalSecond: number;
      runTimes: number;
      status: number;
      beginTime: string;
      endTime?: string;
      runCount: number;
      previousFireTime?: string;
      nextFireTime?: string;
    };
  }
}
