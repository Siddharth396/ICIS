import { Col, IOption, Row, Select } from '@icis/ui-kit';
import { useUser } from '@icis/app-shell-apis';
import { FETCH_REGIONS, FETCH_PRODUCT } from 'apollo/queries';

import {fetchData} from '../../Common/Helper/fetchData';
import getMessages from 'constants/getMessages';
import { Header, LeftColumn, ModalBody, RightColumn } from './Config.style';
import { BodyProps } from './Config.types';
import { RegionData, ProductData } from 'types';
import componentHelper from '../../Common/Helper/Component';
import ComponentEnum from '../../Common/Enum/Component';
import { capitalizeFirstLetter } from '../../Common/utils';

const typeOptions: IOption[] = [{ value: ComponentEnum.Capacity, label: 'Capacity changes' }, { value: ComponentEnum.Outages, label: 'Shutdowns' }];

const Body = ({ commodity, region, type, handleSelectChange, setSaveDisabled }: BodyProps) => {

  const { locale } = useUser();
  const messages = getMessages(locale).config;

  const isRightSideComponentMounted = (isComponentMounted: boolean) => {
    setSaveDisabled(!isComponentMounted);
  };

  // Fetch product data/commodity data
  const processRegionData = (data: RegionData) => data?.regions.map((region) => ({
    value: region.description,
    label: region.description,
  }));

  // Define how to process product data
  const processProductData = (data: ProductData) => data?.products.map((product) => ({
    value: product.code,
    label: capitalizeFirstLetter(product.description),
  }));

  const setRegionFilters = (commodity: IOption) => 
    (commodity.value === 'STYRENE') ? 
        (option: IOption) => option.value !== 'Asia-Pacific' : (option: IOption) => option.value !== 'Asia' && option.value !== 'China';

  // Fetch and process product data
  const { data: commodityDataOptions } =  fetchData<ProductData>(
    FETCH_PRODUCT,
    processProductData,
    () => true
  );

  // Fetch and process region data
  const { data: regionDataOptions } =  fetchData<RegionData>(
    FETCH_REGIONS,
    processRegionData,
    setRegionFilters(commodity)
  );

  const renderRightColumnContent = () => {

    const getSelectedComponent = () => {
      const component = componentHelper.getComponent(type?.value);
        component.Props = { commodities: commodity.value, regions: region.value, isInPreviewMode: true, isComponentMounted: isRightSideComponentMounted };
      return component;
    };

    const { Component, Props } = type?.value ? getSelectedComponent() : componentHelper.getBlankComponent();

    return <Component
      {...Props}
    />;
  };

  return (
    <ModalBody>
      <LeftColumn>
        <Header>{messages.tableHeader}</Header>
        <Row>
          <Col>{messages.commodity}</Col>
          <Col width='200px'><Select onChange={handleSelectChange('commodity')} options={commodityDataOptions?.map((option) => ({
            ...option,
            isDisabled: option.value === commodity?.value,
          }))} value={commodity} testId='Commodity__Dropdown' /></Col>
        </Row>
        <Row>
          <Col>{messages.region}</Col>
          <Col width='200px'><Select onChange={handleSelectChange('region')} options={regionDataOptions?.map((option) => ({
            ...option,
            isDisabled: option.value === region?.value,
          }))} value={region} testId='Region__Dropdown' /></Col>
        </Row>
        <Row>
          <Col>{messages.type}</Col>
          <Col width='200px'><Select onChange={handleSelectChange('type')} options={typeOptions.map((option) => ({
            ...option,
            isDisabled: option.value === type?.value,
          }))} value={type} testId='Type__Dropdown' /></Col>
        </Row>
      </LeftColumn>
      <RightColumn>
        {renderRightColumnContent()}
      </RightColumn>
    </ModalBody>
  );
};

export default Body;
