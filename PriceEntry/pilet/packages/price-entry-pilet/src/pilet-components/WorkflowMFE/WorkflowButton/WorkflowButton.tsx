import { NextAction } from 'apollo/queries';
import { WORKFLOW_ACTIONS } from 'utils/constants';

import { WorkflowButtonWrapper, WorkflowButton } from './styled';

interface IWorkflowButton {
  disabled: boolean;
  action: NextAction;
  onConfirmation: () => void;
}

const WorkflowButtonComponent = ({ disabled, action, onConfirmation }: IWorkflowButton) => {
  const getVariant = () => {
    if (
      action.name === WORKFLOW_ACTIONS.CANCEL ||
      action.name === WORKFLOW_ACTIONS.INITIATE_CORRECTION
    )
      return 'Tertiary';
    return 'AuthoringPrimary';
  };

  return (
    <WorkflowButtonWrapper>
      <WorkflowButton
        disabled={!action.enabled || disabled}
        testId='workflow-button'
        variant={getVariant()}
        onClick={onConfirmation}
      >
        {action.displayValue}
      </WorkflowButton>
    </WorkflowButtonWrapper>
  );
};

export default WorkflowButtonComponent;
