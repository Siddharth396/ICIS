// istanbul ignore file
import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

export const HarnessWrapper = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  flex-wrap: nowrap;
  align-items: center;
  justify-content: space-between;
  gap: 25px;
  margin: auto;
  margin-top: -24px;
  padding: 16px 24px;
  width: 860px;
  height: 100%;
  min-height: 100vh;
  background: ${theme.colours.WHITE};
`;

export const Header = styled.div`
  width: 100%;
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  justify-content: space-between;
  align-items: center;
`;
export const Title = styled.h1``;
export const ButtonsContainer = styled.div`
  display: flex;
  flex-direction: row;
  gap: 14px;
`;

export const ContentSection = styled.span`
  display: block;
  margin-bottom: 10px;
`;

export const Container = styled.div`
  background-color: ${theme.colours.NEUTRALS_6};
  padding: 20px 20px 10px 20px;
  position: relative;
  left: -120px;
`;
