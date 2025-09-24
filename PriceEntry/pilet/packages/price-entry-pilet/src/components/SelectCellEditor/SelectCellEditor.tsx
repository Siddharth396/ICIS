import { memo, useEffect, useState } from 'react';
import { ICellEditorParams } from 'ag-grid-community';
import { CustomCellEditorProps } from 'ag-grid-react';
import Select from 'components/Select/Select';
import { useFocus } from 'utils/hooks/useFocus';

export interface SelectEditorParams extends ICellEditorParams {
  options: string[];
}

const SelectCellEditor = memo((props: CustomCellEditorProps & SelectEditorParams) => {
  const {
    value,
    eventKey,
    colDef: { cellEditorParams },
    stopEditing,
    onValueChange,
    cellStartedEdit,
  } = props;

  const options = cellEditorParams?.options?.map((o: string) => ({
    value: o,
    label: o,
  }));

  const [selectedValue, setSelectedValue] = useState({
    value: '',
    label: '',
  });

  const [refDiv] = useFocus(value, eventKey, cellStartedEdit);
  const [editing, setEditing] = useState(true);

  useEffect(() => {
    setSelectedValue(options?.find((o: { value: string }) => o.value === value));
    if (!editing) {
      stopEditing && stopEditing();
    }
  }, [value, editing]);

  const handleChange = (newValue: string) => {
    onValueChange && onValueChange(newValue);
    setEditing(false);
  };

  return (
    <div ref={refDiv as React.RefObject<HTMLDivElement>} style={{ width: '100%' }}>
      <Select
        value={selectedValue}
        options={options}
        styles={{
          valueContainer: () => ({
            height: '100%',
            display: 'flex',
            alignItems: 'center',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          }),
          menu: (baseStyles) => ({
            ...baseStyles,
            width: 'max-content',
            marginTop: 2,
          }),
          menuList: (baseStyles) => ({
            ...baseStyles,
            paddingTop: 0,
            paddingBottom: 0,
          }),

          option: (styles, state) => ({
            ...styles,
            backgroundColor: state.isSelected ? '#2684FF' : 'inherit',
            '&:hover': { backgroundColor: state.isSelected ? '#2684FF' : 'rgb(251, 251, 252)' },
          }),
        }}
        onChange={handleChange}
      />
    </div>
  );
});

export default SelectCellEditor;
