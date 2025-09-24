// istanbul ignore file
import { useState } from 'react';
import { GridApi } from 'ag-grid-community';
import { PriceDisplayContentBlockResponse } from 'apollo/queries';
import PriceDisplayGrid from 'components/PriceDisplayGrid';
import { CapabilityContainer } from '../PriceEntryCapability/styled';
import PECapabilitySkeleton from '../PriceEntryCapability/PECapabilitySkeleton';

interface IPriceDisplayContentSubscriber {
  loading: boolean;
  data: PriceDisplayContentBlockResponse | undefined;
}

const PriceDisplayContentSubscriber = ({ loading, data }: IPriceDisplayContentSubscriber) => {
  const [gridApi, setGridApi] = useState<GridApi | null>(null);

  return loading ? (
    /* istanbul ignore next */ <PECapabilitySkeleton contentHeight={400} />
  ) : (
    <>
      {data?.contentBlockForDisplay && data.contentBlockForDisplay?.priceSeries?.length > 0 && (
        <CapabilityContainer>
          <PriceDisplayGrid
            data={data?.contentBlockForDisplay}
            gridApi={gridApi}
            setGridApi={setGridApi}
            isAuthoring={false}
          />
        </CapabilityContainer>
      )}
    </>
  );
};

export default PriceDisplayContentSubscriber;
