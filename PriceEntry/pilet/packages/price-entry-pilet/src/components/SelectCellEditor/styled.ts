import { Icon, theme } from '@icis/ui-kit';
import styled from 'styled-components';

interface StyledDivProps {
  haserror?: boolean;
}

export const SelectRenderer = styled.div<StyledDivProps>`
  display: flex;
  align-items: center;
  border: 1px solid
    ${(props) => (props.haserror ? theme.colours.NEGATIVE : theme.colours.NEUTRALS_4)};
  height: 32px;
  min-height: 32px;
  width: 100%;
  padding-left: 10px;
  padding-right: 10px;
  border-radius: 4px;
  font-size: 13px;
  line-height: 18px;
  margin: 0;
`;

export const SelectRendererTitle = styled.div`
  overflow: hidden;
  text-overflow: ellipsis;
  margin-right: 8px;
`;

export const DropDownIndicator = styled(Icon)<StyledDivProps>`
  ${(props) =>
    props.haserror &&
    `
    color: ${theme.colours.NEGATIVE};`}
`;

export const InfoIcon = styled(Icon)`
  color: ${theme.colours.PRIMARY_2};
  font-size: 1.2em;
  margin-right: 8px;
`;

export const CustomTooltip = styled.div`
  z-index: 99999;
  padding: 10px;
`;
