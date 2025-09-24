import React from 'react';
import { Icon } from '@icis/ui-kit';
import {
  AddSectionButton,
  AddSectionButtonWrapper,
  AddSectionWrapper,
  HorizontalLine,
} from './styled';

interface IAddGridButton {
  onAddClick: () => void;
}

const AddGridButton: React.FC<IAddGridButton> = ({ onAddClick }) => (
  <AddSectionWrapper>
    <HorizontalLine>
      <AddSectionButtonWrapper>
        <AddSectionButton onClick={onAddClick}>
          <Icon icon='plus' size='xl' />
          <span>Add Grid</span>
        </AddSectionButton>
      </AddSectionButtonWrapper>
    </HorizontalLine>
  </AddSectionWrapper>
);

export default AddGridButton;
