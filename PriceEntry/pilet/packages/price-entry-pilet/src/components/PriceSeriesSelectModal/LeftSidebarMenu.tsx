// istanbul ignore file
import SelectInputRenderer from 'components/SelectInputRenderer';
import { Row, SelectWrapper, TableConfigHeader, TextWrapper } from './PriceSeriesSelectModal.style';
import { Text } from '@icis/ui-kit';
import { IOption } from 'utils/types';

type Props = {
  disabled: boolean;
  commodity: IOption | undefined;
  priceCategory: IOption | undefined;
  region: IOption | undefined;
  priceSettlementType: IOption | undefined;
  itemFrequency: IOption | undefined;
  setCommodity: (value: IOption | undefined) => void;
  setPriceCategory: (value: IOption | undefined) => void;
  setRegion: (value: IOption | undefined) => void;
  setPriceSettlementType: (value: IOption | undefined) => void;
  setItemFrequency: (value: IOption | undefined) => void;
  dropdownOptions: {
    commodities: IOption[];
    priceCategories: IOption[];
    regions: IOption[];
    priceSettlementTypes: IOption[];
    itemFrequencies: IOption[];
  };
};

const LeftSidebarMenu = ({
  disabled,
  commodity,
  priceCategory,
  region,
  priceSettlementType,
  itemFrequency,
  dropdownOptions,
  setCommodity,
  setPriceCategory,
  setRegion,
  setPriceSettlementType,
  setItemFrequency,
}: Props) => {
  const { commodities, priceCategories, regions, priceSettlementTypes, itemFrequencies } =
    dropdownOptions;

  return (
    <>
      <TableConfigHeader>
        <Text.H4>Table configuration</Text.H4>
      </TableConfigHeader>
      <Row>
        <TextWrapper variant='SemiBold'>Commodity</TextWrapper>
        <SelectWrapper data-testid='commodity-select'>
          <SelectInputRenderer
            disabled={disabled}
            testId='commodity-selectinput'
            value={commodity || undefined}
            options={commodities}
            onChange={(val) => setCommodity(val as IOption)}
          />
        </SelectWrapper>
      </Row>
      <Row>
        <TextWrapper variant='SemiBold'>Region</TextWrapper>
        <SelectWrapper data-testid='region-select'>
          <SelectInputRenderer
            testId='region-selectinput'
            disabled={disabled}
            value={region || undefined}
            options={regions}
            onChange={(val) => setRegion(val as IOption)}
          />
        </SelectWrapper>
      </Row>
      <Row>
        <TextWrapper variant='SemiBold'>Price Category</TextWrapper>
        <SelectWrapper data-testid='price-category-select'>
          <SelectInputRenderer
            testId='price-category-selectinput'
            disabled={disabled}
            value={priceCategory || undefined}
            options={priceCategories}
            onChange={(val) => setPriceCategory(val as IOption)}
          />
        </SelectWrapper>
      </Row>
      <Row>
        <TextWrapper variant='SemiBold'>Transaction Type</TextWrapper>
        <SelectWrapper data-testid='transaction-type-select'>
          <SelectInputRenderer
            testId='transaction-type-selectinput'
            disabled={disabled}
            value={priceSettlementType || undefined}
            options={priceSettlementTypes}
            onChange={(val) => setPriceSettlementType(val as IOption)}
          />
        </SelectWrapper>
      </Row>
      <Row>
        <TextWrapper variant='SemiBold'>Assessed Frequency</TextWrapper>
        <SelectWrapper data-testid='frequency-select'>
          <SelectInputRenderer
            testId='frequency-selectinput'
            disabled={disabled}
            value={itemFrequency || undefined}
            options={itemFrequencies}
            onChange={(val) => setItemFrequency(val as IOption)}
          />
        </SelectWrapper>
      </Row>
    </>
  );
};

export default LeftSidebarMenu;
