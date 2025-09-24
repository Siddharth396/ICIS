// istanbul ignore file
import * as React from 'react';
import { PiletApi } from '@icis/app-shell';
import { setIsAuthoring } from 'constants/isAuthoring';
import { setup as customerFacingSetup } from './index';
import PiletType from 'constants/piletType';

setIsAuthoring(true);

const PriceEntryCapabilityTestHarness = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-entry-pilet"*/ 'pilet-components/PriceEntryCapability/PriceEntryCapabilityTestHarness'
    ),
);

const PriceDisplayTableCapabilityTestHarness = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-display-table-pilet-harness"*/ 'pilet-components/PriceDisplayTableCapability/PriceDisplayTableCapabilityTestHarness'
    ),
);

const PriceEntryContainerAuthoring = React.lazy(
  () => import(/* webpackChunkName: "price-entry-pilet"*/ 'pilet-components/PriceEntryCapability'),
);

const PriceDisplayTableContainerAuthoring = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-display-table-pilet"*/ 'pilet-components/PriceDisplayTableCapability/PriceDisplayTableContainerAuthoring'
    ),
);

const ScheduleSelectorToolTestHarness = React.lazy(
  () =>
    import(
      /* webpackChunkName: "schedule-selector-tool"*/ 'pilet-components/ScheduleSelectorTool/ScheduleSelectorToolTestHarness'
    ),
);

export function setup(app: PiletApi) {
  customerFacingSetup(app);

  // this page is only available on the authoring site.
  app.registerPage('/mfe/price-entry/authoring', ({ piral }) => (
    <PriceEntryCapabilityTestHarness piralApi={piral} />
  ));

  app.registerPage('/mfe/price-display-table/authoring', ({ piral }) => (
    <PriceDisplayTableCapabilityTestHarness piralApi={piral} isAuthoring={true} />
  ));

  app.registerPage('/mfe/schedule-selector-tool/authoring', () => (
    <ScheduleSelectorToolTestHarness />
  ));

  app.registerCapability({
    name: PiletType.PriceEntry,
    Component: (props) => {
      return <PriceEntryContainerAuthoring params={{ ...props.params, piralApi: app }} />;
    },
    metadata: {
      friendlyName: 'Price Entry',
      description: 'Enables you to input prices for the commodities.',
      icon: 'table',
      categories: ['wide', 'price-authoring'],
      availability: 'explicit-category',
    },
  });

  app.registerCapability({
    name: PiletType.PriceDisplayTable,
    Component: (props) => {
      return <PriceDisplayTableContainerAuthoring params={{ ...props.params, piralApi: app }} />;
    },
    metadata: {
      friendlyName: 'Price Display Table',
      description: 'Display prices for the selected commodities.',
      icon: 'table',
      categories: ['wide'],
    },
  });
}
