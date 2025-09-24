// istanbul ignore file
import styled from 'styled-components';
import Skeleton from 'react-loading-skeleton';
import { theme } from '@icis/ui-kit';

type Props = {
  contentHeight: number;
  bottomSpacing?: boolean;
};

const Title = styled(Skeleton)`
  height: ${theme.fonts.lineHeight.LARGE};
  width: ${(props) => props.width}% !important;
`;

const Box = styled(Skeleton)`
  margin-top: ${theme.spacing.BASE_3};
`;

const Container = styled.div<{ bottomSpacing?: boolean }>`
  margin-bottom: ${({ bottomSpacing }) => (bottomSpacing ? theme.spacing.BASE_3 : '0')};
`;

const PECapabilitySkeleton = ({ contentHeight, bottomSpacing }: Props) => (
  <Container bottomSpacing={bottomSpacing}>
    <Title width={40} />
    <Box height={contentHeight} />
  </Container>
);

export default PECapabilitySkeleton;
