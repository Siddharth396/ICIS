import { theme } from '@icis/ui-kit';
import styled from 'styled-components';

type Props = {
  dayPicker?: boolean;
  dayPickerV2?: boolean;
  monthPicker?: boolean;
  showLoader?: boolean;
  borderColor?: string;
};

export const SelectBoxWrapper = styled.div`
  display: inline-block;
  width: 111px;
  position: relative;
`;

export const SelectBox = styled.div<{ isOpen: boolean }>`
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-weight: ${theme.fonts.weight.REGULAR};
  cursor: pointer;
  padding: ${theme.spacing.BASE_1};
  font-size: 13px;
  width: 112px;
  height: 32px;
  color: ${theme.colours.NEUTRALS_9};
  background: ${theme.colours.WHITE};
  border: 1px solid ${theme.colours.NEUTRALS_4};
  border-radius: 4px;
  &:hover {
    border-color: ${theme.colours.SECONDARY_4};
  }
  ${({ isOpen }) =>
    isOpen &&
    `
    border-color: ${theme.colours.SECONDARY_4};

    svg {
      color: ${theme.colours.SECONDARY_4};
    }
  `};
`;

export const PickerContent = styled.div<Props>`
  display: flex;
  flex-direction: row;
  background: ${theme.colours.WHITE};
  width: max-content;

  .rdp-tbody {
    height: max-content;
    transition: height 3000ms;
  }

  ${({ showLoader }) =>
    showLoader &&
    `
    pointer-events: none;
    .rdp-tbody {
      display: none;
    }
  `};

  .rdp-table {
    width: 100%;
    border-collapse: separate;
    margin: 0;
    border-spacing: 11px 12px;
    padding: 0 10px 8px;
    cursor: default;
  }

  .rdp-day {
    outline: none;
  }

  .rdp-day_disabled {
    color: ${theme.colours.NEUTRALS_4};
    cursor: default;
    opacity: 1 !important;
  }

  .rdp-day.rdp-day_outside {
    color: ${theme.colours.NEUTRALS_4};
    pointer-events: none;
  }

  .rdp-day:not(.rdp-day_outside):not(.rdp-day_disabled):not(.rdp-day_selected):not(.hoverRange) {
    color: #425971;
    border-radius: 3px;
  }

  .rdp {
    margin: 0;
  }

  .rdp-day_selected {
    border-radius: 3px;
    background: ${theme.colours.SECONDARY_1};
    color: ${theme.colours.WHITE} !important;

    &:hover {
      background: ${theme.colours.PRIMARY_1};
    }

    &.rdp-day_outside {
      background: none;
    }
  }

  .DayPicker-Caption {
    border-top: 1px solid ${theme.colours.NEUTRALS_4};
    display: flex;
    align-items: center;

    .NavButton {
      background-size: 30px 22px;
      width: 22px;
      height: 22px;
      background-repeat: no-repeat;
      background-position: center;

      &:hover {
        cursor: pointer;
      }

      &.disabled {
        opacity: 0.1;
        cursor: default;
      }
    }

    .NavButton--prev {
      background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGAAAABgCAYAAADimHc4AAAABmJLR0QA/wD/AP+gvaeTAAACyklEQVR4nO2Xv2tTURxHz4uxS8W1s4uLUDAZxEWhm5uTg0qxlc66CAXRviqCooPOxUQKOti56NTdIREKLi7OXcUOtkmuw0MQp4vg+/643/MP3E9ySO49EARBEARBEARBEARBEATFkap+PXja9qndtg/UyLn6/dwcw2GC68B6m2cXL2Dx+fZ89/DHDnBF4vyiBVx4srVwfDjZBfpSG4oVsPh468zkuPMROCu5o0gBvUdv+mk6202wIL2lIz2gbXr1YInZbK9S8OVDYQJ6m69vAB+A09JbflOMgP7m8A6p2gbmpLf8SQECmsBKKb1E4ed1fQn/FVgqcStAOrBycSlAQ2Dl4k6AlsDKxZUATYGVi7pXwb+iLbBycSFAY2DlYl6A1sDKxbAA3YGVi8lL2EJg5WJOgJXAysWUAEuBlYsZAdYCKxcTAiwGVi7qXw9WAysX1QIsB1YuagVYD6xcFArwEVi5qLqEPQVWLmoEeAusXFQI8BhYuYgL8BpYuYgK8BxYuYi9MrwHVi4iAkoIrFxaF1BKYOXiPnS007qA0cbKK6q0DBy1fbZGRH4B443bb2mC67vE+ZoQ+wsa16t7dDpLCQ6kNmhA9A4YP7w1mp6YXqxIXyV3SCJ+Ce8/WPvWPTm7BIykt0ggLgDg0/21g8l89zJNGxSFCgEA+/eWD39y6mqCd9Jb2kSNAIAv9bWjz/XKzQqeSW9pC1UCGqo0qlfXq6q6C8yk1/xvFApoKCXY1AqAMoJNtQDwH2zqBYDvYDMhAPwGmxkB4DPYTAkAf8FmTgD4CjaTAhp8BJthAQ3Wg828ALAdbC4EgN1gcyMAbAabKwFgL9jcCQBbweZSANgJNrcCwEawuRbQoDvYChDQoDXYihEAOoOtKAGgL9iKEwC6gq1IAaAn2IoVADqCrWgBYCfYCiBV5+vBC+kVQRAEQRAEQRAEQRAEgV9+AacgSTn7ZouvAAAAAElFTkSuQmCC);
      margin-left: 20px;
      margin-right: 6px;

      &:hover:not(.disabled) {
        background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGAAAABgCAYAAADimHc4AAAABmJLR0QA/wD/AP+gvaeTAAACr0lEQVR4nO3Xv2pTYRjH8e8JJQ5CJyFTE7q4ZOzkktQKQjdnFW9BRy9BwUGvQBDq4iwVwapZnDp2cSmJU8Ap4JCieR2OgovwgHieP+/zuYHztF/ac36QUkoppZRSSimllFJ9Gob7j7SPqNN43GdnesRwWrp+9FbXDzRncPMyq/UrGg41Hl93gN0bA36sXwN7WifUG2A02WXz/Q1wVfOMnubD1Qyv71GaTxTdXz7UGGA0OYDNCTDQPgVqCzCa3KE0x8C29im/1RNgOLlPaV4Afe1T/lRDgF8Dq3mKwZ839lfQeNxndeU5lNvap/xN3ADKA0sqZgADA0sqXgAjA0vK3EvpnxgaWFJxAhgbWFIxAhgcWFL+AxgdWFKeA5geWFI+v4IcDCwpfwGcDCwpXwEcDSwpPwGcDSwpHy8vhwNLyn4ApwNLynYAxwNLym4A5wNLymKAEANLytZXUKCBJWUnQLCBJWUjQMCBJaUfIOjAktJ9yQUeWFJ6AYIPLCmdABUMLKnuA1QysKTCDx3rug+wmD2jKfeAi86fbZDOX8B8dkRTDoGVyvMN0fsXNJ+dQO8AWKrdYIDuO2Dx/pSmXKPhs+odivRfwvPZOb2tCXCqfYoG/QAA5++WrC9NKRxrn9I1GwEAlm+/sf31FoWX2qd0yU4AgLOzC758vAvNY+1TumIrQKuw+PAQygNgo33M/2YxQKuSwWY3AFQx2GwHgPCDzX4ACD3YfASAsIPNTwAIOdh8BYBwg81fAAg12HwGaIUYbJ4DtJwPNv8BwPVgixEA3A62OAHA5WCLFQDcDbZ4AcDVYIsZANwMtrgBwMVgix2gZXqw1RCgZXSw1RMATA62ugKAucFWXwAwNdjqDABmBlu9AcDEYKs7ALgZbDVo2Nl/on1ESimllFJKKaWUUorrJwl+Bga1oNfXAAAAAElFTkSuQmCC);
      }
    }

    .NavButton--next {
      background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGAAAABgCAYAAADimHc4AAAABmJLR0QA/wD/AP+gvaeTAAACwElEQVR4nO2XLW9UQRhGn7nZLqIEW0kwGATZrmiwKBoUAgSQwvYHYAkh6U5JSCDBgCW0QAoChatrAoZUdJsKDIYgsQQE249B7JIgEK/pfT/mOX/gPntPZ3oPQAghhBBCCCGEEELIMdHLa0+Rc6O9wwqtv4gE3JnH6dfn8rtu28+2iNJfYrlxAj83F/LGKZ3n20HzKri4j/HWwsPnc4ob1NG+i/sH+51P5/Ors8o71NAWgIJypsHhx/kHL/vaWzRQFwAACZjD0dGH3uqLRe0tbWNCwJTZVNL7fl6/rj2kTSwJAIBuQdnora7f1R7SFtYEAEBKpTyqJdjM/sBags2sgAnxg824AADBg82DACBwsHkREDbY3AgAYgabKwFTQgWbRwFAoGDzKgAIEmxuh//Fe7C5FzDBb7AFEQDAabBFEgA4DLZoAtwFWzgBgK9gCylgiotgiywAcBBs0QUAxoPN3KDjwmqwVSNggr1gq0wAAGPBVqMAwFCw1SrATLBVKwCwEWxVC5iiGmwUMEEt2ChAmY72ACOME9JgNBy8bfvBFAD8KqlcHQ2XNzUeXrWAAnxPTXN5d+X2jtaGagUkpK+HaC7trdz6ormj1n/CO52Zgwt7WfflA3WegK0ZdK9s37/5Q3sIUN0JSG9+4+Tidrbx8oGKBBTg2Qjflj7na2PtLf9SwxVUSkr3doeDx9pD/kd0AWqBJSWyANXAkhJSgIXAkhJOgJXAkhLtK8hMYEmJdAJMBZaUICfAXmBJcS/AamBJ8XwFmQ4sKV4FmA8sKR4FuAgsKa4EeAosKW4EeAssKV6+gtwFlhQPJ8BlYEkxfgL8BpYUswK8B5YUi1dQiMCSYk1AmMCSYklAqMCSYkJAxMCSoi4gamBJ0f4KChtYUjRPQOjAkqJ0AuIHlll6ee0JUJL2DkIIIYQQQgghhBBC2uYPnec8ibt4VZUAAAAASUVORK5CYII=);
      margin-right: 20px;
      margin-left: 6px;

      &:hover:not(.disabled) {
        background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGAAAABgCAYAAADimHc4AAAABmJLR0QA/wD/AP+gvaeTAAACq0lEQVR4nO2XP0tVcRjHP8crl0DKJXCwrl2QFkeHEEStFqWpoaWm3kDRVC+hoaVeQqizg3A39V6CaLhvIIK4goPYVAgiXU9Di+Oz3PP8+T2fN3Aez8fvPXwgSZIkSZIkSZIkSZIkSSZFZ/0jMKV9hhVajT9x9l6P2e4id2b2OTsbN/58Yyj9J9Yv+HO7x+KDWzrPt4PmT8EjLm8c0H08p3iDOtq/xcuM/35lfvW+8h1qaAsA6NJqDeg8XNY+RAMLAgDm4KrPwvqW9iFNY0UAwAw1eyysPdc+pEksCQBoU1fbdNbeah/SFNYEAFRQvS8l2Cz/ga/obHxmaamtfcgksSyAEoLNuAAgeLB5EACBg82LAAgabJ4EQMBg8yYAggWbRwEQKNi8CoAgweb28Gu4DrYIAvAcbEEEAE6DLZIAcBhs0QSAs2CLKAAcBVtUAeAk2CILAAfBFl0AGA82cwdNEJPBVpIALAZbYQIAY8FWogAwFGylCgAjwVayADAQbKULAOVgSwH/UQu2FKDMtPYBRrikql8yGuw2/eAUAOdUPGM06Gk8vHQBpzD1hNHhUOuAkgX8ZDze5KT/XfOIUj/CQ1rTK5x8UX35UOYCDmhfPOXHt9/ah0BxC6h2uPlry8rLh7IW8InjozfAlfYh1ylhATXU7zjuv8bYy4f4C1ALLCmRBagGlpSoAtQDS0pEASYCS0q0j7CZwJISaQGmAktKkAXYCywpERZgMrCkeF6A6cCS4nUB5gNLikcBLgJLijcBbgJLiicBrgJLipePsLvAkuJhAS4DS4rxBfgNLCmWF+A6sKRYXECIwJJibQFhAkuKJQGhAkuKFQHhAkuKBQEhA0uK9kc4bGBJ0VxA6MCSorSA+IFll7sbH4BK+4wkSZIkSZIkSZIkSZIkaZp/Z/bvFeer0BwAAAAASUVORK5CYII=);
      }
    }
  }

  .rdp-cell,
  .rdp-day,
  .rdp-head_cell {
    font-size: 13px;
    font-weight: 600;
    height: 32px;
    width: 32px;
    padding: 0;
  }

  .rdp-head_cell {
    color: ${theme.colours.NEUTRALS_9};
  }

  .rdp-head_row > .rdp-head_cell:first-child {
    ${({ dayPicker }) =>
      dayPicker &&
      `
      display: table-cell;
    `}
  }

  .rdp-head_row {
    font-weight: 600;
  }

  .DayPicker-Caption {
    border-bottom: 1px solid ${theme.colours.NEUTRALS_4};
    padding: ${theme.spacing.BASE_1} 0;
    margin-bottom: ${theme.spacing.BASE_1};
    background-color: ${theme.colours.NEUTRALS_4};
  }

  .DayPicker-Caption > select {
    height: 35px;
    border-radius: 5px;
    border-color: #d1d6dd;
  }

  .select__control {
    cursor: pointer;
  }

  .DayPicker-Caption > select > option {
    font-family: ${theme.fonts.graphikFonts};
    font-size: ${theme.fonts.size.BASE};
    border: 1px solid black;
  }

  .DayPicker-Caption > div {
    border-radius: 5px;
    border-color: #d1d6dd;
    display: inline-block;
  }

  .DayPicker-Caption > div:nth-child(3) {
    margin-left: 8px;
  }

  .rdp-months:focus,
  .rdp-month:focus {
    outline: none;
  }

  .rdp-months:focus,
  .DayPicker-Caption:focus {
    outline: none !important;
  }

  .NavButton .disabled,
  .NavButton .disabled:hover,
  .NavButton .disabled:focus {
    display: block;
    opacity: 0.1;
    outline: none;
    cursor: default;
  }

  .NavButton--next:focus,
  .NavButton--prev:focus {
    outline: none !important;
  }

  .rdp:not(.rdp_interactionDisabled)
    .rdp-day:not(.rdp-day_disabled):not(.rdp-day_selected):not(.rdp-day_outside) {
    border-radius: 3px;

    &:hover {
      background-color: ${theme.colours.NEUTRALS_5};
    }

    &:active {
      color: ${theme.colours.WHITE};
      background-color: ${theme.colours.SECONDARY_1};
    }
  }

  .select__single-value {
    padding-bottom: 0;
  }

  .select__value-container,
  .select__menu {
    font-size: 13px;
  }

  .select__value-container {
    padding: 2px;
  }

  .select__menu {
    text-align: left;
  }

  .rdp-month {
    padding: 0;
    width: 328px;
  }

  .rdp:focus {
    outline: none !important;
  }

  ${({ monthPicker }) =>
    monthPicker &&
    `
    .MonthPicker-wrapper {
      display: flex;
      flex-wrap: wrap;
      width: 288px;
      justify-content: center;
      align-items: center;
    }

    .Month-wrapper {
      cursor: pointer;
      padding: 10px;
      margin: ${theme.spacing.BASE_1};
      background: ${theme.colours.SECONDARY_4};
      width: 76px;
      text-align: center;
      border-radius: 35px;
      color: ${theme.colours.WHITE};
      font-size: 11px;
      &:hover {
        background: ${theme.colours.PRIMARY_1};
      }
    }

    .rdp-tbody,
    .DayPicker-Caption {
      display: none;
    }

    .MonthPicker-Nav {
      display: flex;
      background: #FBFBFC;
      border-top: 1px solid #CCD2D9;
      border-bottom: 1px solid #CCD2D9;
      padding: 20px 0;
    }
  `};

  ${({ dayPickerV2 }) =>
    dayPickerV2 &&
    `
    border-radius: ${theme.borderRadius.default};
    .rdp-months {
      border: 1px solid ${theme.colours.NEUTRALS_4};
      border-radius: ${theme.borderRadius.default};
      ${theme.shadow.SOFT};
    };

    .DayPicker-Caption {
      background-color: ${theme.colours.NEUTRALS_7};
      border-top: none;
      border-top-left-radius: ${theme.borderRadius.default};
      border-top-right-radius: ${theme.borderRadius.default};

      .NavButton {
        background-size: 16px 13px;
        background-image: url("data:image/svg+xml;base64,PHN2ZyBhcmlhLWhpZGRlbj0idHJ1ZSIgZm9jdXNhYmxlPSJmYWxzZSIgZGF0YS1wcmVmaXg9ImZhbCIgZGF0YS1pY29uPSJjaGV2cm9uLWRvd24iIGNsYXNzPSJzdmctaW5saW5lLS1mYSBmYS1jaGV2cm9uLWRvd24gZmEtMXggIiByb2xlPSJpbWciIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgdmlld0JveD0iMCAwIDUxMiA1MTIiPjxwYXRoIGZpbGw9ImN1cnJlbnRDb2xvciIgZD0iTTI2Ny4zIDM5NS4zYy02LjIgNi4yLTE2LjQgNi4yLTIyLjYgMGwtMTkyLTE5MmMtNi4yLTYuMi02LjItMTYuNCAwLTIyLjZzMTYuNC02LjIgMjIuNiAwTDI1NiAzNjEuNCA0MzYuNyAxODAuN2M2LjItNi4yIDE2LjQtNi4yIDIyLjYgMHM2LjIgMTYuNCAwIDIyLjZsLTE5MiAxOTJ6Ij48L3BhdGg+PC9zdmc+");
        &:hover:not(.disabled) {
          background-image: url("data:image/svg+xml;base64,PHN2ZyBhcmlhLWhpZGRlbj0idHJ1ZSIgZm9jdXNhYmxlPSJmYWxzZSIgZGF0YS1wcmVmaXg9ImZhbCIgZGF0YS1pY29uPSJjaGV2cm9uLWRvd24iIGNsYXNzPSJzdmctaW5saW5lLS1mYSBmYS1jaGV2cm9uLWRvd24gZmEtMXggIiByb2xlPSJpbWciIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgdmlld0JveD0iMCAwIDUxMiA1MTIiPjxwYXRoIGZpbGw9ImN1cnJlbnRDb2xvciIgZD0iTTI2Ny4zIDM5NS4zYy02LjIgNi4yLTE2LjQgNi4yLTIyLjYgMGwtMTkyLTE5MmMtNi4yLTYuMi02LjItMTYuNCAwLTIyLjZzMTYuNC02LjIgMjIuNiAwTDI1NiAzNjEuNCA0MzYuNyAxODAuN2M2LjItNi4yIDE2LjQtNi4yIDIyLjYgMHM2LjIgMTYuNCAwIDIyLjZsLTE5MiAxOTJ6Ij48L3BhdGg+PC9zdmc+");
        }
      };
      .NavButton--prev {
        rotate: 90deg;
      };
      .NavButton--next {
        rotate: 270deg;
      };
    };
    .rdp-day:not(.rdp-day_outside):not(.rdp-day_disabled):not(.rdp-day_selected):not(.hoverRange) {
      background: none;
      color: ${theme.colours.NEUTRALS_2};
    };
    .rdp-day_today:not(.rdp-day_outside):not(.rdp-day_disabled):not(.rdp-day_selected):not(.hoverRange) {
      color: ${theme.colours.SECONDARY_4};
    };
    .rdp-day-latest-date {
      border: 1px solid ${theme.colours.SECONDARY_1};
      color: ${theme.colours.SECONDARY_1};
    };
  `}

  ${({ borderColor }) =>
    borderColor &&
    `
    .rdp-months {
      border-color: ${borderColor};
    }
  `}
`;

export const DropdownWrapper = styled.div`
  ${theme.shadow.SOFT_MEDIUM};
  max-height: 242px;
  position: absolute;
  top: 36px;
  border-radius: 4px;
  border: 1px solid ${theme.colours.SECONDARY_4};
  width: 111px;
  font-weight: ${theme.fonts.weight.REGULAR};
  overflow-y: auto;
  ::-webkit-scrollbar,
  ::-webkit-scrollbar-track {
    border-top-right-radius: 4px;
    border-bottom-right-radius: 4px;
  }
  z-index: 2;
`;

export const OptionWrapper = styled.div<{ selected: boolean; isLast: boolean }>`
  height: 40px;
  padding-left: ${theme.spacing.BASE_2};
  cursor: pointer;
  background: ${({ selected }) => (selected ? theme.colours.NEUTRALS_7 : theme.colours.WHITE)};
  font-size: 13px;
  color: ${theme.colours.NEUTRALS_1};
  display: flex;
  align-items: center;
  :hover {
    background: ${theme.colours.NEUTRALS_7};
  }
  ${({ isLast }) =>
    !isLast &&
    `
    border-bottom: 1px solid ${theme.colours.NEUTRALS_5};
  `};
`;
