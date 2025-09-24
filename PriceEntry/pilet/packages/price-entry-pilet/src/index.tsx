// istanbul ignore file
// This file defines the entrypoint for your pilet.
// All code that interacts directly with Piral or the App Shell should go here and only here.
// All components that are to be exported by your pilet should be lazy loaded as is being done for ExampleCard below.
// The size of this file should be kept as small as possible as it will be loaded on every page load no matter where
// you are on the site. Keep non lazy imports to a minimum.

import * as React from 'react';
import { PiletApi } from '@icis/app-shell';
import { init as initAppShellApis } from '@icis/app-shell-apis';

const PriceEntryCapabilityTestHarness = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-entry-PriceEntryCapabilityTestHarness"*/ 'pilet-components/PriceEntryCapability/PriceEntryCapabilityTestHarness'
    ),
);

const PriceDisplayTableCapabilityTestHarness = React.lazy(
  () =>
    import(
      /* webpackChunkName: "PriceDisplayTableCapabilityTestHarness-Subscriber"*/
      './pilet-components/PriceDisplayTableCapability/PriceDisplayTableCapabilityTestHarness'
    ),
);

const PriceDisplayTableContainerSubscriber = React.lazy(
  () =>
    import(
      /* webpackChunkName: "price-display-table-pilet"*/ './pilet-components/PriceDisplayTableCapability/PriceDisplayTableContainerSubscriber'
    ),
);

// This is the entrypoint that Piral calls when running your Pilet.
// The app parameter is a reference to the app shell. It contains functions defined by Piral
// as well as custom APIs defined within the app shell.
export function setup(app: PiletApi) {
  // This initializes the app-shell-apis with a reference to the app shell.
  // Once this is done you can interact with our Pilet API from anywhere in the app.
  // e.g. if you wanted to get access to the logger defined in the app-shell you could do the following:
  // import { logger } from '@icis/app-shell-apis';
  // const user = useUser();
  // @ts-ignore
  initAppShellApis(app);

  // MFE path, will remove the shared header
  app.registerPage('/mfe/price-entry', () => <PriceEntryCapabilityTestHarness />);

  app.registerPage('/mfe/price-display-table', ({ piral }) => (
    <PriceDisplayTableCapabilityTestHarness isAuthoring={false} piralApi={piral} />
  ));

  app.registerCapability({
    name: 'price-display-table-capability',
    Component: (props) => {
      return <PriceDisplayTableContainerSubscriber params={{ ...props.params, piralApi: app }} />;
    },
    metadata: {
      friendlyName: 'Price Display Table',
      description: 'Display prices for the selected commodities.',
      icon: 'table',
      categories: ['wide'],
    },
  });

  // The following would add a card to the homepage
  // app.registerTile('price-entry-charting-card', ExampleCard, { initialColumns: 8, customProperties: ['order:30'] });

  // The following would register an "extension" that this, or another pilet could render.
  // Extensions allow you to render a component that could be defined in another codebase by name.
  // app.registerExtension('example-card', ({ params }) => <ExampleCard />);
  // To render this Extension the code would be <app.Extension name="example-card" />
  // If you were to pass a params prop to <app.Extension> this would be passed to the extension
}
