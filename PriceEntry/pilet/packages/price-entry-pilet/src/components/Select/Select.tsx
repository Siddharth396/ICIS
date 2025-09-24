// istanbul ignore file
import React, { useState, useCallback } from 'react';
import SelectComponent, { MultiValue, SingleValue, StylesConfig, components } from 'react-select';
import { ReactSelect } from './Select.style';

export interface IOption {
  label: string;
  value: any;
}

export interface IProps {
  options: IOption[];
  value?: IOption;
  onChange: (value: IOption['value'], label?: IOption['label']) => void;
  error?: boolean;
  disabled?: boolean;
  placeholder?: string;
  isSearchable?: boolean;
  testId?: string;
  name?: string;
  locale?: string;
  styles?: StylesConfig<IOption>;
}

const Select: React.FC<IProps> = ({
  options,
  value,
  onChange,
  placeholder,
  isSearchable = true,
  testId,
  styles,
}) => {
  const [focusedOption, setFocusedOption] = useState<IOption | null>(null);

  const handleKeyDown = useCallback(
    (event: React.KeyboardEvent) => {
      if (event.key === 'Enter') {
        event.preventDefault();
        if (focusedOption) {
          onChange(focusedOption.value, focusedOption.label);
        }
      }
    },
    [focusedOption, onChange],
  );

  const customComponents = {
    ...components,
    Option: (optionProps: any) => (
      <components.Option
        {...optionProps}
        innerRef={() => {
          if (optionProps.isFocused) {
            setFocusedOption(optionProps.data);
          }
        }}
      />
    ),
  };

  const handleChange = useCallback(
    (newValue: MultiValue<IOption> | SingleValue<IOption>) => {
      const selectedOption = Array.isArray(newValue) ? newValue[0] : newValue;
      onChange(selectedOption.value, selectedOption.label);
    },
    [onChange],
  );

  return (
    <ReactSelect>
      <SelectComponent
        openMenuOnFocus
        menuPortalTarget={document.body}
        components={customComponents}
        classNamePrefix='select'
        options={options}
        defaultValue={value}
        placeholder={placeholder}
        value={value}
        isSearchable={isSearchable}
        data-testid={testId}
        styles={styles}
        onKeyDown={handleKeyDown}
        onChange={handleChange}
      />
    </ReactSelect>
  );
};

export default Select;
