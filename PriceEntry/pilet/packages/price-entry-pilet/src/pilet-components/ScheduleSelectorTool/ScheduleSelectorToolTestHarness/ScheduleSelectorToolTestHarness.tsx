import React, { useState } from 'react';
import { Text } from '@icis/ui-kit';
import ScheduleSelectorTool from '../ScheduleSelectorTool';
import {
  HarnessContainer,
  Header,
  ToolSection,
  ConfigSection,
  InputSection,
  TextArea,
  Button,
  InfoBox,
  LogSection,
} from './styled';
import { endOfMonth, format, startOfMonth } from 'date-fns';
import { DATE_YEAR_FORMAT } from 'components/DateSelector/DateSelector.constants';

interface TestConfig {
  scheduleId: string;
  startDate: string;
  endDate?: string;
  limit?: number;
}

const currentDate = new Date();

const ScheduleSelectorToolTestHarness: React.FC = () => {
  const [config, setConfig] = useState<TestConfig>({
    scheduleId: 'bb7c3ca8-a69c-44b7-8bb1-ed95ca9ba2f0',
    startDate: format(startOfMonth(currentDate), DATE_YEAR_FORMAT),
    endDate: format(endOfMonth(currentDate), DATE_YEAR_FORMAT),
  });
  const [rawJson, setRawJson] = useState(JSON.stringify(config, null, 2));
  const [selectedDate, setSelectedDate] = useState(new Date());

  const handleApplyConfig = () => {
    try {
      const parsed: TestConfig = JSON.parse(rawJson);

      if (!parsed.scheduleId || !parsed.startDate) {
        alert('Invalid config: "scheduleId" and "startDate" are required.');
        return;
      }

      // Check that at least one of endDate or limit is provided
      if (!parsed.endDate && (parsed.limit === undefined || parsed.limit === null)) {
        alert('Invalid config: Please provide either "endDate" or "limit".');
        return;
      }

      setConfig(parsed);
    } catch (error) {
      alert('Invalid JSON. Please correct the JSON and try again.');
    }
  };

  return (
    <HarnessContainer>
      <Header>
        <h2>Schedule Selector Tool Test Harness</h2>
      </Header>

      <ToolSection>
        <Text.Caption>
          The Schedule Selector Tool is displayed below. Update the configuration in JSON and click
          &quot;Apply Config&quot; to see changes.
        </Text.Caption>
        <Text.Caption>
          <strong>Required fields:</strong> <code>scheduleId</code>, <code>startDate</code>, and one
          of <code>endDate</code> or <code>limit</code>.
        </Text.Caption>

        <ScheduleSelectorTool
          scheduleId={config.scheduleId}
          // @ts-ignore
          startDate={config.startDate}
          endDate={config.endDate}
          limit={config.limit}
          selectedDate={selectedDate}
          // @ts-ignore
          onDateChange={setSelectedDate}
        />
      </ToolSection>

      <ConfigSection>
        <Text.H3>Configuration (JSON)</Text.H3>
        <InputSection>
          <Text.Label>Modify the configuration and click &quot;Apply Config&quot;:</Text.Label>
          <TextArea value={rawJson} onChange={(e) => setRawJson(e.target.value)} />
          <Button onClick={handleApplyConfig}>Apply Config</Button>
        </InputSection>
        <InfoBox>
          <strong>Current Config:</strong> {JSON.stringify(config)}
        </InfoBox>
      </ConfigSection>

      <LogSection>
        <Text.H3>Selected Date: {selectedDate.toString()}</Text.H3>
      </LogSection>
    </HarnessContainer>
  );
};

export default ScheduleSelectorToolTestHarness;
