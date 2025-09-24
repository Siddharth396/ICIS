// Copyright © 2021 LexisNexis Risk Solutions Group
import { useUser } from '@icis/app-shell-apis';
import getMessages from 'getMessages';

const useLocaleMessages = () => getMessages(useUser().locale);

export default useLocaleMessages;
