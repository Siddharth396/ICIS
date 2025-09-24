// istanbul ignore file
import * as React from 'react';
import { init as initAppShellApis } from '@icis/app-shell-apis';
import { PiletApi } from '@icis/app-shell';

import { subPath } from 'constants/global';

const App = React.lazy(
  () => import(/* webpackChunkName: "supply-and-demand"*/ 'pilet-components/App'),
);
const AppTestHarness = React.lazy(() => import(/* webpackChunkName: "supply-and-demand"*/ 'pilet-components/App/TestHarness'));

export function setup(app: PiletApi) {
  initAppShellApis(app);

  app.registerPage(subPath, () => <AppTestHarness />);
  
  app.registerCapability({
    name: 'snd-table-tool',
    Component: App,
    metadata: {
      friendlyName: 'SnD Table',
      icon: 'table',
      description: 'A tool with S&D data to show Outages and Capacity Developments',
      categories: ['wide'],
      hasDynamicData: true,
    }
  });
}
