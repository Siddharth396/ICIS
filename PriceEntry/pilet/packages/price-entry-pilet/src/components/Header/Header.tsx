import React from 'react';
import { HeaderContainer } from './styled';

interface CustomHeaderProps {
  displayName: string;
}

const CustomHeader: React.FC<CustomHeaderProps> = (props) => {
  return <HeaderContainer>{props.displayName}</HeaderContainer>;
};

export default CustomHeader;
