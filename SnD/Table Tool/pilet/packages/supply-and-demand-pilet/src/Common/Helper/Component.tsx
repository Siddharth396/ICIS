import IComponent from '../Model/IComponent';
import ComponentEnum from '../Enum/Component';
import Capacity from 'pilet-components/Capacities';
import Outages from 'pilet-components/Outages';
import { showErrorPage } from '@icis/app-shell-apis';
import { ErrorPageType } from '@icis/app-shell';
import ScreenError from '../Error/ScreenError';
import { Props } from '../Error/ScreenError.types';

const componentMap: Map<ComponentEnum, IComponent<any>> = new Map<ComponentEnum, IComponent<any>>([
  [ComponentEnum.Capacity, { Component: Capacity, Props: {} }],
  [ComponentEnum.Outages, { Component: Outages, Props: {} }]
]);

const getComponent = (selectedComponent: ComponentEnum): IComponent<any> => {
  return componentMap.get(selectedComponent) || getBlankComponent();
};

const getComponentWithProps = (selectedComponent: ComponentEnum, props: any): IComponent<any> => {
  const component = getComponent(selectedComponent);
  component.Props = props;
  return component;
};

const getBlankComponent = (): IComponent<any> => {
  const BlankComponent = () => null;
  return { Component: BlankComponent, Props: {} };
};

const getGlobalErrorPageComponent = (errorPageType?: ErrorPageType) => {
  return <>{showErrorPage(errorPageType || 0)}</>;
};

const getErrorPageComponent = (props?: Props) => {
  return <ScreenError {...props} />;
};

const componentHelper = {
  getComponent,
  getComponentWithProps,
  getBlankComponent,
  getGlobalErrorPageComponent,
  getErrorPageComponent
};

export default componentHelper;
