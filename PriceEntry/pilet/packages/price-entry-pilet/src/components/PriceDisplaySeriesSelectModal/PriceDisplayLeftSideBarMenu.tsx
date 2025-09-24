// istanbul ignore file
import { useMemo } from 'react';
import { PriceSeriesForDisplayTool, SelectedFilterInput } from 'apollo/queries';
import MultiSelectInputRenderer from 'components/MultiSelectInputRenderer';
import {
  CheckboxContainer,
  Row,
  SelectWrapper,
  TextWrapper,
} from '../PriceSeriesSelectModal/PriceSeriesSelectModal.style';
import { IMultiOption } from '../MultiSelectInputRenderer/MultiSelectInputRenderer';
import { CheckboxItem } from 'pilet-components/PriceEntryCapability/EditColumns/styled';
import { CheckBox } from '@icis/ui-kit';
import { SectionContainer } from './styled';

type Props = {
  commodity: any;
  commodityData: any;
  priceCategories: IMultiOption[];
  regions: IMultiOption[];
  priceSettlementTypes: IMultiOption[];
  itemFrequencies: IMultiOption[];
  priceSeriesData: PriceSeriesForDisplayTool;
  inactivePriceSeriesFlag: boolean;
  setCommodity: (value: any) => void;
  setPriceCategories: (value: IMultiOption[]) => void;
  setRegions: (value: IMultiOption[]) => void;
  setPriceSettlementTypes: (value: IMultiOption[]) => void;
  setItemFrequencies: (value: IMultiOption[]) => void;
  setInactivePriceSeriesFlag: (value: boolean) => void;
  selectedFiltersData?: SelectedFilterInput;
};

type SelectFieldProps = {
  label: string;
  options: IMultiOption[];
  value: IMultiOption[] | [];
  onChange: (value: IMultiOption[] | []) => void;
  placeholder?: string;
  testId: string;
  disabled?: boolean;
};

function MultiSelectField({
  label,
  value,
  options,
  testId,
  onChange,
  placeholder,
}: SelectFieldProps) {
  return (
    <Row>
      <TextWrapper variant='SemiBold'>{label}</TextWrapper>
      <SelectWrapper>
        <MultiSelectInputRenderer
          testId={testId}
          value={value}
          options={options ?? []}
          onChange={onChange}
          disabled={!options?.length}
          placeholder={placeholder}
        />
      </SelectWrapper>
    </Row>
  );
}

function CheckBoxField({
  label,
  value,
  onChange,
  disabled,
  testId,
}: {
  label: string;
  value: boolean;
  onChange: (value: boolean) => void;
  disabled: boolean;
  testId: string;
}) {
  return (
    <CheckboxContainer data-testid={`${testId}-container`} disabled={disabled}>
      <CheckboxItem>
        <CheckBox
          data-testid={testId}
          checked={value}
          disabled={disabled}
          onChange={(e) => {
            onChange(e.target.checked);
          }}
        />
      </CheckboxItem>
      <TextWrapper variant='SemiBold'>{label}</TextWrapper>
    </CheckboxContainer>
  );
}

const PriceDisplayLeftSideBarMenu = ({
  commodity,
  commodityData,
  priceCategories,
  regions,
  priceSettlementTypes,
  itemFrequencies,
  inactivePriceSeriesFlag,
  priceSeriesData,
  setCommodity,
  setPriceCategories,
  setRegions,
  setPriceSettlementTypes,
  setItemFrequencies,
  setInactivePriceSeriesFlag,
}: Props) => {
  const {
    regionOptions,
    priceCategoryOptions,
    transactionTypesOptions,
    assessedFrequenciesOptions,
  } = useMemo(() => {
    const toOption = (item: { id: string; name: string }): IMultiOption => ({
      id: item.id,
      label: item.name,
      value: item.name,
    });
    return {
      regionOptions: priceSeriesData?.regions?.map(toOption) ?? [],
      priceCategoryOptions: priceSeriesData?.priceCategories?.map(toOption) ?? [],
      transactionTypesOptions: priceSeriesData?.transactionTypes?.map(toOption) ?? [],
      assessedFrequenciesOptions: priceSeriesData?.assessedFrequencies?.map(toOption) ?? [],
    };
  }, [priceSeriesData]);

  return (
    <SectionContainer>
      {MultiSelectField({
        label: 'Commodity',
        value: commodity,
        options: commodityData,
        onChange: setCommodity,
        placeholder: '--Select Commodity--',
        testId: 'commodity-multi-selectinput',
      })}
      {MultiSelectField({
        label: 'Region',
        value: regions,
        options: regionOptions,
        onChange: setRegions,
        testId: 'region-multi-selectinput',
      })}
      {MultiSelectField({
        label: 'Price Category',
        value: priceCategories,
        options: priceCategoryOptions,
        onChange: setPriceCategories,
        testId: 'price-category-multi-selectinput',
      })}
      {MultiSelectField({
        label: 'Transaction Type',
        value: priceSettlementTypes,
        options: transactionTypesOptions,
        onChange: setPriceSettlementTypes,
        testId: 'transaction-type-multi-selectinput',
      })}
      {MultiSelectField({
        label: 'Assessed Frequency',
        value: itemFrequencies,
        options: assessedFrequenciesOptions,
        onChange: setItemFrequencies,
        testId: 'frequency-multi-selectinput',
      })}
      {CheckBoxField({
        label: 'Include Inactive Series',
        value: inactivePriceSeriesFlag,
        onChange: setInactivePriceSeriesFlag,
        disabled: !commodity?.length,
        testId: 'include-inactive-priceseries-checkbox',
      })}
    </SectionContainer>
  );
};

export default PriceDisplayLeftSideBarMenu;
