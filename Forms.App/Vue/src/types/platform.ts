export interface LevelConfig {
  level: number;
  dayMinLimit: number;
  dayMaxLimit: number;
  monthLimit: number;
  matchLevel?: number;
  ruleLevel?: number;
  modifyTime?: string;
  effectiveTime?: string;
  isEffective?: boolean;
  isFirstTimeConfig?: boolean;
}

export interface PlatformConfig {
  current: LevelConfig | null;
  last: LevelConfig | null;
  isUpgrade?: boolean;
}

export interface NetworkingConfiguration {
  TB: PlatformConfig;
  JD: PlatformConfig;
  DY: PlatformConfig;
  ALIBABA: PlatformConfig;
}

export type PlatformType = 'TB' | 'JD' | 'DY' | 'ALIBABA';

export interface SystemLevelConfig {
  tb: PlatformConfig;
  jd: PlatformConfig;
  dy: PlatformConfig;
  alibaba: PlatformConfig;
}
