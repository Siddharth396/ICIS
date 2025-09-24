
type NoRecords = {
  title: string;
  subtitle?: string;
};

export interface Props<T> {
  schema: Column[];
  data: T;
  noRecords: NoRecords;
  testId: string;
};

type Column = {
  key: string;
  label: string;
  width?: number;
  sticky?: boolean;
  align?: 'left' | 'right';
};