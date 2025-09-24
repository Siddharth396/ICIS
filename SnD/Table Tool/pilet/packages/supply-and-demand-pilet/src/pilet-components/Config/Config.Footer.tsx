import { Button } from '@icis/ui-kit';
import { useUser } from '@icis/app-shell-apis';

import getMessages from 'constants/getMessages';

import { ModalFooter, SaveButton } from './Config.style';
import { FooterProps } from './Config.types';

const Footer = ({ saveDisabled, saveConfig, toggleModal }: FooterProps) => {
  const { locale } = useUser();
  const messages = getMessages(locale).config;

  return (
    <ModalFooter>
      <Button variant='Tertiary' onClick={toggleModal} testId='Close__Button'>{messages.cancel}</Button>
      <SaveButton onClick={saveConfig} disabled={saveDisabled} testId='Save__Button'>{messages.save}</SaveButton>
    </ModalFooter>
  );
};

export default Footer;
