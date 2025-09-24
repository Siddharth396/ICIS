import { IOption } from '@icis/ui-kit';
import IContentBlockFilter from '../App/IContentBlockFilter';

export interface Props {
  setSelectedConfigTableFilters: (tableFilters: IContentBlockFilter) => void;
  saveToCanvas: (version : string) => void;
  contentBlockId: string;
};

export interface BodyProps {
  commodity: IOption;
  region: IOption;
  type: IOption | undefined;
  saveDisabled: boolean;
  handleSelectChange: (key: keyof ConfigState) => (value: string, label?: string) => void;
  setSaveDisabled: (isSaveDisabled: boolean) => void;
};

export interface FooterProps {
  saveDisabled: boolean;
  saveConfig: () => void;
  toggleModal: () => void;
};

export interface ConfigState {
  isOpen: boolean;
  saveDisabled: boolean;
  commodity: IOption;
  region: IOption;
  type?: IOption;
};
