import { Text } from '@icis/ui-kit';
import Skeleton from 'react-loading-skeleton';

import { LoaderContainer } from './styled';

interface Props {
  testId?: string | 'Loader';
  title?: string;
  height?: string | '100px';
}

const Loader = ({ testId, title, height }: Props) => {
  return (
    <LoaderContainer data-testid={testId}>
      {title && <Text.Body variant='SemiBold'>{title}</Text.Body>}
      <Skeleton height={height} />
    </LoaderContainer>
  );
};

export default Loader;
