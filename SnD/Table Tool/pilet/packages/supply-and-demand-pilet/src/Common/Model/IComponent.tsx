interface IComponent<P> {
  Component: React.ComponentType<P>
  Props?: { [key: string]: any }
}


export default IComponent;
