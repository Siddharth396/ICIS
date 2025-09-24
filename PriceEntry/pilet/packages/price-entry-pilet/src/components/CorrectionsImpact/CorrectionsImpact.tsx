// istanbul ignore file
import { GridApi } from 'ag-grid-community';
import { Modal, Text } from '@icis/ui-kit';
import { StyledButton, Wrapper, PopupContainer, CapabilityContainer } from './styled';
import { getStartOfDayUTC } from 'utils/date';
import {
  GET_IMPACTED_PRICES,
  GET_IMPACT_PRICE_DISPLAY_CONTENT_BLOCK,
  ImpactedPricesResponse,
  IImpactedPriceDisplayContentBlockResponse,
} from 'apollo/queries';
import React, { useEffect, useState } from 'react';
import PriceDisplayGrid from 'components/PriceDisplayGrid';
import { useLazyQuery } from '@apollo/client';
import withClient, { ApolloClientProps } from 'components/HOCs/withClient';
import PECapabilitySkeleton from 'pilet-components/PriceEntryCapability/PECapabilitySkeleton';

interface ICorrectionsImpact {
  data: any;
  snapshotDate: any;
}

const messages = {
  derivedPrices: 'Derived prices',
  referencePrices: 'Reference prices',
  calculatedPrices: 'Calculated prices',
  linkedPrices: 'Linked prices',
};

const CorrectionsImpact = ({
  data,
  snapshotDate,
  client,
}: ICorrectionsImpact & ApolloClientProps) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [gridApi, setGridApi] = useState<GridApi | null>(null);

  const selectedStartOfDayUTC = getStartOfDayUTC(snapshotDate);
  const handleButtonClick = () => {
    setIsModalOpen(true);
    getImpactedPrices({
      variables: {
        priceSeriesId: data?.id,
        assessedDateTime: selectedStartOfDayUTC,
      },
    });
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  const [getImpactedPrices, { data: impactedData }] = useLazyQuery<ImpactedPricesResponse>(
    GET_IMPACTED_PRICES,
    {
      client,
      fetchPolicy: 'no-cache',
    },
  );

  const [getDerivedPrices, { data: derivedData }] =
    useLazyQuery<IImpactedPriceDisplayContentBlockResponse>(
      GET_IMPACT_PRICE_DISPLAY_CONTENT_BLOCK,
      {
        client,
        fetchPolicy: 'no-cache',
      },
    );

  const [getReferencePrice, { data: referenceData }] =
    useLazyQuery<IImpactedPriceDisplayContentBlockResponse>(
      GET_IMPACT_PRICE_DISPLAY_CONTENT_BLOCK,
      {
        client,
        fetchPolicy: 'no-cache',
      },
    );

  const [getCalculatedPrice, { data: calculatedData }] =
    useLazyQuery<IImpactedPriceDisplayContentBlockResponse>(
      GET_IMPACT_PRICE_DISPLAY_CONTENT_BLOCK,
      {
        client,
        fetchPolicy: 'no-cache',
      },
    );

  const fetchPrices = (priceIds: string[], fetchFunction: any) => {
    if (priceIds?.length > 0) {
      fetchFunction({
        variables: {
          seriesIds: priceIds,
          assessedDateTime: selectedStartOfDayUTC,
        },
      });
    }
  };

  useEffect(() => {
    const impactedPrices = impactedData?.impactedPrices.impactedPrices;
    if (impactedPrices) {
      fetchPrices(impactedPrices.impactedDerivedPriceSeriesIds, getDerivedPrices);
      fetchPrices(impactedPrices.impactedReferencePriceSeriesIds, getReferencePrice);
      fetchPrices(impactedPrices.impactedCalculatedPriceSeriesIds, getCalculatedPrice);
    }
  }, [impactedData]);

  const renderPriceSection = (title: string, priceIds: string[], data: any, gridApiSetter: any) => {
    if (priceIds?.length > 0 && !data?.contentBlockFromInputParametersForDisplay) {
      return (
        <CapabilityContainer>
          <Text.Body variant='SemiBold'>{title}</Text.Body>
          <PECapabilitySkeleton contentHeight={400} />
        </CapabilityContainer>
      );
    }

    if (data?.contentBlockFromInputParametersForDisplay) {
      return (
        <CapabilityContainer>
          <Text.Body variant='SemiBold'>{title}</Text.Body>
          <PriceDisplayGrid
            data={{
              ...data.contentBlockFromInputParametersForDisplay,
              gridConfiguration:
                data.contentBlockFromInputParametersForDisplay.priceDisplayGridConfiguration,
              priceSeries: data.contentBlockFromInputParametersForDisplay.priceSeriesItemForDisplay,
            }}
            gridApi={gridApi}
            setGridApi={gridApiSetter}
            isAuthoring={false}
          />
        </CapabilityContainer>
      );
    }

    return null;
  };

  return (
    <Wrapper>
      <StyledButton
        data-testid={`impacted-button-${data.id}`}
        onClick={handleButtonClick}
        variant='Tertiary'
        disabled={!data.hasImpactedPrices}>
        {messages.linkedPrices}
      </StyledButton>
      {isModalOpen && data.hasImpactedPrices && (
        <Modal
          isOpen={isModalOpen}
          ariaLabel={messages.linkedPrices}
          variant='Large'
          testId='impacted-prices-modal'
          footer={{
            visible: false,
          }}
          header={{
            title: messages.linkedPrices,
            visible: true,
          }}
          onDismiss={handleCloseModal}>
          <PopupContainer>
            <Text.Body variant='SemiBold'>{data.priceSeriesName}</Text.Body>
            {renderPriceSection(
              messages.derivedPrices,
              impactedData?.impactedPrices.impactedPrices.impactedDerivedPriceSeriesIds ?? [],
              derivedData,
              setGridApi,
            )}
            {renderPriceSection(
              messages.referencePrices,
              impactedData?.impactedPrices.impactedPrices.impactedReferencePriceSeriesIds ?? [],
              referenceData,
              setGridApi,
            )}
            {renderPriceSection(
              messages.calculatedPrices,
              impactedData?.impactedPrices.impactedPrices.impactedCalculatedPriceSeriesIds ?? [],
              calculatedData,
              setGridApi,
            )}
          </PopupContainer>
        </Modal>
      )}
    </Wrapper>
  );
};

export default withClient(CorrectionsImpact);
