import { Button } from '@icis/ui-kit';
import {
  ErrorFallbackTitle,
  ErrorFallbackWrapper,
  ReloadButtonWrapper,
  WarningIcon,
} from './styled';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';

type Props = {
  onReload: () => void;
};

const ErrorFallback = ({ onReload }: Props) => {
  const messages = useLocaleMessages();
  return (
    <ErrorFallbackWrapper>
      <WarningIcon icon='warning' testId='fallback-warning-icon' />
      <ErrorFallbackTitle>{messages.General.GenericErrorMessage}</ErrorFallbackTitle>
      <ReloadButtonWrapper>
        <Button onClick={onReload}>{messages.General.Reload}</Button>
      </ReloadButtonWrapper>
    </ErrorFallbackWrapper>
  );
};

export default ErrorFallback;
