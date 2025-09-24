import { Button } from '@icis/ui-kit';
import { Wrapper } from './PriceSeriesSelectionCard.style';

type Props = {
  onButtonClick: () => void;
  testId: string;
  buttonText: string;
};

const PriceSeriesSelectionCard = ({ testId, buttonText, onButtonClick }: Props) => {
  return (
    <Wrapper>
      <Button variant='Primary' onClick={onButtonClick} testId={`${testId}-add-button`}>
        {buttonText}
      </Button>
    </Wrapper>
  );
};

export default PriceSeriesSelectionCard;
