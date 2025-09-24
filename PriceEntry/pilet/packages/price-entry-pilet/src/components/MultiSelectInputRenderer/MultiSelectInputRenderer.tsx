import { SelectMulti } from '@icis/ui-kit';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';

export interface IMultiOption {
  id: string;
  label: string;
  value: any;
}

type Props = {
  options: IMultiOption[];
  value: IMultiOption[] | [] | undefined;
  onChange: (value: IMultiOption[] | []) => void;
  placeholder?: string;
  testId: string;
  disabled?: boolean;
};

const MultiSelectInputRenderer = ({ options, value, testId, placeholder, disabled, onChange }: Props) => {
  const messages = useLocaleMessages();
  return (
    // istanbul ignore next
    <SelectMulti
      variant='canvas'
      testid={testId}
      value={value}
      options={options}
      // istanbul ignore next
      placeHolder={placeholder ? placeholder : messages.General.SelectPlaceholder}
      disabled={disabled}
      isSearchable
      onChange={(selectedOptions) => {
        // istanbul ignore next
        onChange(selectedOptions);
      }}
    />
  );
};

export default MultiSelectInputRenderer;
