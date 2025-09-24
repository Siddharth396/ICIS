import styled from 'styled-components';

export const HarnessContainer = styled.div`
  padding: 20px;
  color: #333;
`;

export const Header = styled.header`
  margin-bottom: 20px;
  h2 {
    margin: 0;
    font-size: 1.5rem;
    border-bottom: 1px solid #ccc;
    padding-bottom: 8px;
  }
`;

export const ToolSection = styled.section`
  background: #fafafa;
  padding: 16px;
  border: 1px solid #ddd;
  margin-bottom: 20px;
  border-radius: 4px;
`;

export const ConfigSection = styled.section`
  margin-bottom: 20px;
`;

export const InputSection = styled.div`
  margin-bottom: 16px;
`;

export const TextArea = styled.textarea`
  width: 100%;
  height: 120px;
  margin-bottom: 8px;
  font-family: monospace;
`;

export const Button = styled.button`
  margin-right: 8px;
  background: #0073e6;
  border: none;
  color: #fff;
  padding: 6px 12px;
  border-radius: 3px;
  cursor: pointer;

  &:hover {
    background: #005bb5;
  }
`;

export const InfoBox = styled.div`
  background: #f0f0f0;
  padding: 12px;
  margin-top: 8px;
  border-radius: 4px;
  font-size: 0.9rem;
`;

export const LogSection = styled.section`
  margin-top: 20px;
  background: #fafafa;
  padding: 16px;
  border: 1px solid #ddd;
  border-radius: 4px;
`;
