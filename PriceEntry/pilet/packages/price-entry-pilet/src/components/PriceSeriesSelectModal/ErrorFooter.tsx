// istanbul ignore file
import React from 'react';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { ErrorText } from './PriceSeriesSelectModal.style';

interface ErrorFooterProps {
  errorCodes?: string[];
}

const ErrorFooter: React.FC<ErrorFooterProps> = ({ errorCodes = [] }) => {
  const messages: { Errors: { [key: string]: string } } = useLocaleMessages();

  const errorMessages = errorCodes
    .map((code) => messages.Errors[code])
    .filter((msg): msg is string => Boolean(msg));

  if (errorMessages.length === 0) {
    return <div />;
  }

  const displayText =
    errorMessages.length > 1
      ? errorMessages.map((msg, i) => `${i + 1}. ${msg}`).join('\n')
      : errorMessages[0];

  return <ErrorText data-testid='footer-error'>{displayText}</ErrorText>;
};

export default ErrorFooter;
