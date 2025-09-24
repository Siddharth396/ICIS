// istanbul ignore file
import * as React from 'react';
import { PiletApi } from '@icis/app-shell';

import { setup as customerFacingSetup } from './index';
import { setIsAuthoring } from 'constants/isAuthoring';
import { subPath } from 'constants/global';

const App = React.lazy(() => import(/* webpackChunkName: "ids-admin"*/ 'pilet-components/App'));
const AppTestHarness = React.lazy(() => import(/* webpackChunkName: "supply-and-demand"*/ 'pilet-components/App/TestHarness'));

setIsAuthoring(true);

export function setup(app: PiletApi) {
  customerFacingSetup(app);

  app.registerPage(`${subPath}/authoring`, () => <AppTestHarness />);
  
  app.registerCapability({
    name: 'snd-table-tool',
    Component: App,
    metadata: {
      friendlyName: 'SnD Table',
      icon: 'table',
      description: 'A tool with S&D data to show Outages and Capacity.',
      categories: ['wide', 'commodity-hub-pages'],
      availability: 'explicit-category',
    }
  });
}
