import { Icon, Text } from '@icis/ui-kit';

import { Props } from './Table.types';
import { TableWrapper, Cell, HeaderCell, HeaderRow, Row, StyledTable, StickyCell, StickyHeaderCell, NoRecordsWrapper, NoRecordsCell, NoRecordsIcon } from './Table.style';

let size = 0;

const Table = <T extends any[]>({ schema, data, noRecords, testId }: Props<T>) => {

  return (
    <TableWrapper>
      <StyledTable data-testid={testId}>
        <thead>
          <HeaderRow>
            {schema.map((column, index) => {
              // return column.sticky && column.sticky < index
              if(index === 0) size = 0;
              const returnItem = column.sticky
                ? (
                  <StickyHeaderCell key={column.key} size={size} width={column.width} align={column.align}>
                    {column.label} <Icon icon='chevron-down' className='sortIcon' />
                  </StickyHeaderCell>
                ) : (
                  <HeaderCell isFirst={index === 0} key={column.key} width={column.width} align={column.align}>
                    {column.label}
                    {/* <Icon icon='chevron-down' className='sortIcon' /> */}
                  </HeaderCell>
                );
              size += column.width ?? 100;
              return returnItem;
            })}
          </HeaderRow>
        </thead>
        <tbody>
          {!!data.length && data.map((item: any, key: any) => (
            <Row key={key}>
              {schema.map((column, index) => {
                const key = Object.keys(item).find((val: string) => val.toLowerCase() === column.key.toLowerCase());
                const value = key ? item[key] : '';
                // return column.sticky && column.sticky < index
                if(index === 0) size = 0;
                const returnItem = column.sticky
                  ? <StickyCell  isFirst={index === 0} key={column.key} width={column.width} align={column.align} size={size}>{value}</StickyCell>
                  : <Cell isFirst={index === 0} key={column.key} width={column.width} align={column.align}>{value}</Cell>;
                size += column.width ?? 100;
                return returnItem;
              })}
            </Row>
          ))}
          {!data.length && (
            <Row>
              <NoRecordsCell isFirst={true} colSpan={schema.length}>
                <NoRecordsWrapper>
                  <NoRecordsIcon icon='information' size='10x' />
                  <Text.Subtitle2>{noRecords.title}</Text.Subtitle2>
                  <Text.Body>{noRecords.subtitle}</Text.Body>
                </NoRecordsWrapper>
              </NoRecordsCell>
            </Row>
          )}
        </tbody>
      </StyledTable>
    </TableWrapper>
  );
};

export default Table;
