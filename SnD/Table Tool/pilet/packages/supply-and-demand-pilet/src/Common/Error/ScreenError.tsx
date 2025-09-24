import { useUser } from '@icis/app-shell-apis';
import { ErrorContainer, TextBody } from './ScreenError.styled';
import getMessages from '../../constants/getMessages';
import { Props } from './ScreenError.types';

const ScreenError = (props?: Props) => {
  const { locale } = useUser();
  const messages = getMessages(locale);

  const bodyTextColour = props?.variant ?? 'BoldBlack';

  return (
    <ErrorContainer data-testid="error-container">
      <TextBody variant={bodyTextColour}>{messages.messages.Error}</TextBody>
    </ErrorContainer>
  )
}

export default ScreenError;
