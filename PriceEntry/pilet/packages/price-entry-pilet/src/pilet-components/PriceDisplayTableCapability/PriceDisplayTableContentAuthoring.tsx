import { useState } from 'react';
import { GridApi } from 'ag-grid-community';
import {
  PriceDisplayContentBlockResponse,
  RowInputPriceDisplayTable,
  UserPreferenceColumnInput,
  SelectedFilterInput,
} from 'apollo/queries';
import { IconKeys } from '@icis/ui-kit';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { PriceSeriesSelectionCard } from 'components/PriceSeriesSelectionCard';
import {
  CapabilityContainer,
  HeaderWrapper,
  ShowHideButtonWrapper,
  ValidationMessageWrapper,
} from '../PriceEntryCapability/styled';
import PECapabilitySkeleton from '../PriceEntryCapability/PECapabilitySkeleton';
import EditColumns from '../PriceEntryCapability/EditColumns';
import PriceDisplaySelectModalContainer from '../../components/PriceSeriesSelectModal/PriceDisplaySelectModalContainer';
import PriceDisplayGrid from 'components/PriceDisplayGrid';
import { HeaderRow, RowLeft, RowRight, ValidationMessage, ValidationMessageIcon } from './styled';

interface IPriceDisplayContentAuthoring {
  loading: boolean;
  showPriceSeriesSelectionCard: boolean;
  data: PriceDisplayContentBlockResponse | undefined;
  selectedDate?: Date;
  showModal: boolean;
  setShowModal: React.Dispatch<React.SetStateAction<boolean>>;
  onStartEditing: () => void;
  onFinishEditing: () => void;
  handleUpdateUserPreference: (
    updatedColumnConfigs: UserPreferenceColumnInput[],
    selectedSeriesIDs?: string[],
  ) => Promise<void>;
  handleUpdateContentBlock: (
    selectedSeriesID?: RowInputPriceDisplayTable[],
    displayColumns?: UserPreferenceColumnInput[],
    selectedFilters?: SelectedFilterInput,
  ) => void;
  selectedFiltersData?: SelectedFilterInput;
  editMode: boolean;
}

const PriceDisplayContentAuthoring = ({
  loading,
  showPriceSeriesSelectionCard,
  data,
  showModal,
  setShowModal,
  onStartEditing,
  onFinishEditing,
  handleUpdateUserPreference,
  handleUpdateContentBlock,
  selectedFiltersData,
  editMode,
}: IPriceDisplayContentAuthoring) => {
  const messages = useLocaleMessages();
  const [gridApi, setGridApi] = useState<GridApi | null>(null);
  const [showValidationMessage, setShowValidationMessage] = useState<boolean>(false);

  // istanbul ignore next
  const renderValidationmessage = () => (
    <>
      <ValidationMessageIcon icon={IconKeys.warning} />
      <ValidationMessage>{messages.Workflow.GenericError}</ValidationMessage>
    </>
  );

  // istanbul ignore next
  const handleContentBlockUpdate = (
    selectedSeriesID?: RowInputPriceDisplayTable[],
    selectedFilters?: SelectedFilterInput,
  ) => {
    handleUpdateContentBlock(selectedSeriesID, [], selectedFilters);
    setShowValidationMessage(false);
  };

  return loading ? (
    /* istanbul ignore next */ <PECapabilitySkeleton contentHeight={400} />
  ) : (
    <>
      {showPriceSeriesSelectionCard && (
        <PriceSeriesSelectionCard
          testId='price-series-selection-card'
          buttonText={messages.Capabilty.SelectPriceSeries}
          onButtonClick={/* istanbul ignore next */ () => setShowModal(true)}
        />
      )}
      {
        /* istanbul ignore next */ !showPriceSeriesSelectionCard &&
          data?.contentBlockForDisplay &&
          data.contentBlockForDisplay?.priceSeries?.length > 0 && (
            <>
              <HeaderWrapper>
                <HeaderRow>
                  <RowLeft lg={4} md={4} sm={12}>
                    <ValidationMessageWrapper>
                      {
                        /* istanbul ignore next */ showValidationMessage &&
                          renderValidationmessage()
                      }
                    </ValidationMessageWrapper>
                  </RowLeft>
                  <RowRight lg={4} md={4} sm={12}>
                    <ShowHideButtonWrapper>
                      <EditColumns
                        isPriceDisplay={true}
                        columns={data?.contentBlockForDisplay?.gridConfiguration?.columns}
                        setGridApi={setGridApi}
                        // @ts-ignore
                        onUpdateUserPreference={handleUpdateUserPreference}
                      />
                    </ShowHideButtonWrapper>
                  </RowRight>
                </HeaderRow>
              </HeaderWrapper>
              <CapabilityContainer>
                <PriceDisplayGrid
                  data={data?.contentBlockForDisplay}
                  gridApi={gridApi}
                  setGridApi={setGridApi}
                  onUpdateUserPreference={handleUpdateUserPreference}
                  onStartEditing={onStartEditing}
                  onFinishEditing={onFinishEditing}
                  isAuthoring={true}
                />
              </CapabilityContainer>
            </>
          )
      }
      {showModal && (
        <PriceDisplaySelectModalContainer
          showModal={showModal}
          selectedPriceSeries={data?.contentBlockForDisplay?.rows ?? []}
          setShowModal={setShowModal}
          onUpdateContentBlock={handleContentBlockUpdate}
          selectedFiltersData={selectedFiltersData}
          editMode={editMode}
        />
      )}
    </>
  );
};

export default PriceDisplayContentAuthoring;
