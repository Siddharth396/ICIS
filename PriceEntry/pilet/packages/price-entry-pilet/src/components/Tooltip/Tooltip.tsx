import { TooltipText, TooltipWrapper } from './styled';

const TooltipComponent = ({ value }: any) => {
  return (
    <TooltipWrapper>
      <TooltipText>{value}</TooltipText>
    </TooltipWrapper>
  );
};

export default TooltipComponent;
