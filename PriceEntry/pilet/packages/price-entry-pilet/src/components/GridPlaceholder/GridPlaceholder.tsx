import { theme } from '@icis/ui-kit';
import useLocaleMessages from 'utils/hooks/useLocaleMessage';
import { PlaceholderIcon, PlaceholderWrapper, PlaceholderMessage } from './styled';

const GridPlaceholder = () => {
  const messages = useLocaleMessages();
  return (
    <PlaceholderWrapper>
      <PlaceholderIcon data-testid='grid-placeholder-icon'>
        <svg
          width='48'
          height='42'
          viewBox='0 0 48 42'
          fill='none'
          xmlns='http://www.w3.org/2000/svg'>
          <path
            d='M6 3C4.3125 3 3 4.40625 3 6V12H45V6C45 4.40625 43.5938 3 42 3H6ZM3 15V25.5H22.5V15H3ZM25.5 15V25.5H45V15H25.5ZM22.5 28.5H3V36C3 37.6875 4.3125 39 6 39H22.5V28.5ZM25.5 39H42C43.5938 39 45 37.6875 45 36V28.5H25.5V39ZM0 6C0 2.71875 2.625 0 6 0H42C45.2812 0 48 2.71875 48 6V36C48 39.375 45.2812 42 42 42H6C2.625 42 0 39.375 0 36V6Z'
            fill={theme.colours.PRIMARY_3}
          />
        </svg>
      </PlaceholderIcon>
      <PlaceholderMessage>{messages.Capabilty.PlaceholderTitle}</PlaceholderMessage>
    </PlaceholderWrapper>
  );
};

export default GridPlaceholder;
