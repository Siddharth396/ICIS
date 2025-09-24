import { Select, IOption, theme } from '@icis/ui-kit';
import InfoTooltip from 'components/InfoTooltip/InfoTooltip';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';

type Props = {
  options: IOption[];
  disabled?: boolean;
  value: IOption | undefined;
  placeholder?: string;
  testId: string;
  onChange: (value: IOption | null) => void;
};

const TOOLTIP_ID = 'disabled-select-tooltip';

const SelectInputRenderer = ({
  options,
  disabled,
  value,
  testId,
  placeholder,
  onChange,
}: Props) => {
  const messages = useLocaleMessages();

  const customStyles = {
    control: (base: any, state: any) => ({
      ...base,
      backgroundColor: state.isDisabled
        ? `${theme.colours.BUTTON_DISABLED} !important`
        : base.backgroundColor,
      borderColor: state.isDisabled ? `${theme.colours.NEUTRALS_4} !important` : base.borderColor,
      cursor: state.isDisabled ? 'not-allowed' : 'default',
    }),
    singleValue: (base: any, state: any) => ({
      ...base,
      color: /* istanbul ignore next */ state.isDisabled
        ? `${theme.colours.NEUTRALS_9} !important`
        : base.color,
    }),
    dropdownIndicator: (base: any, state: any) => ({
      ...base,
      color: state.isDisabled ? `${theme.colours.NEUTRALS_8} !important` : base.color,
    }),
  };

  return (
    <>
      <div
        data-tip={disabled ? true : undefined}
        data-for={disabled ? TOOLTIP_ID : undefined}
        style={{ width: '100%' }}>
        <div data-testid={testId}>
          <Select
            variant='canvas'
            testId={testId}
            disabled={disabled}
            value={value || undefined}
            options={options}
            styles={customStyles}
            placeholder={placeholder ? placeholder : messages.General.SelectPlaceholder}
            onChange={(selectedOption) => {
              const selectedFullOption =
                options.find((option) => option.value === selectedOption) ||
                /* istanbul ignore next */ null;
              onChange(selectedFullOption);
            }}
          />
        </div>
      </div>
      {disabled && (
        <InfoTooltip
          id={TOOLTIP_ID}
          place='bottom'
          message={messages.General.SelectTooltipMessage}
          maxWidth={240}
        />
      )}
    </>
  );
};

export default SelectInputRenderer;
