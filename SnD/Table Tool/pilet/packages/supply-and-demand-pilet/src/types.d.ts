export type Language = 'en' | 'zh';

export type AppConfig = {
  appName: string;
  version: string;
  bffVersion: string;
  bypassVersionCheck: boolean;
  bffUrl: string;
  authoringBffUrl: string;
  bffCallEnabled: boolean;
  alwaysRefresh: boolean;
  disableGWA: boolean;
};

export type Column = {
  key: string;
  label: string;
  width?: number;
  sticky?: boolean;
  align?: 'left' | 'right';
};

export type TableProps = {
  schema: Column[];
  data: any;
  noRecords: {
    title: string;
    subtitle: string;
  };
  testId: string;
};

export type ConfigType = {
  commodity: string;
  region: string;
};

export type RegionData = {
  regions: Region[];
};

type Region = {
  code: string;
  description: string;
  id: number;
};

export type ProductData = {
  products: Product[];
};

type Product = {
  code: string;
  description: string;
  id: number;
};