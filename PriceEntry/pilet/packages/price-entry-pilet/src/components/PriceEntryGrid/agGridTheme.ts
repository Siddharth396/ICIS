import { iconOverrides, themeAlpine } from 'ag-grid-community';
import { theme } from '@icis/ui-kit';

const mySvgIcons = iconOverrides({
  type: 'image',
  mask: true,
  icons: {
    grip: {
      svg: `
          <svg width="32" height="32" viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg">
            <rect x="5" y="4" width="6" height="6" rx="2" fill="none" stroke="#5D6C7B" stroke-width="2"/>
            <rect x="5" y="13" width="6" height="6" rx="2" fill="none" stroke="#5D6C7B" stroke-width="2"/>
            <rect x="5" y="22" width="6" height="6" rx="2" fill="none" stroke="#5D6C7B" stroke-width="2"/>
            <rect x="15" y="4" width="6" height="6" rx="2" fill="none" stroke="#5D6C7B" stroke-width="2"/>
            <rect x="15" y="13" width="6" height="6" rx="2" fill="none" stroke="#5D6C7B" stroke-width="2"/>
            <rect x="15" y="22" width="6" height="6" rx="2" fill="none" stroke="#5D6C7B" stroke-width="2"/>
          </svg>
      `,
    },
  },
});

/**
 * Custom AG Grid theme parameters
 * (instead of importing the standard .css files)
 */
export const themeParams = themeAlpine.withPart(mySvgIcons).withParams({
  headerHeight: theme.spacing.BASE_6,
  rowHeight: theme.spacing.BASE_6,
  headerBackgroundColor: theme.colours.NEUTRALS_6,
  headerTextColor: theme.colours.PRIMARY_1,
  headerFontWeight: theme.fonts.weight.SEMI_BOLD,
  cellHorizontalPadding: theme.spacing.BASE_1,
  rowHoverColor: 'none',
  cellTextColor: theme.colours.NEUTRALS_9,
  inputFocusShadow: 'none',
  borderRadius: 'none',
  inputFocusBorder: {
    color: theme.colours.NEUTRALS_4,
    width: '1px',
    style: 'solid',
  },
  dataFontSize: theme.fonts.size.BASE,
  fontFamily: theme.fonts.graphikFonts,
  headerColumnBorder: {
    width: '1px',
  },
  borderColor: theme.colours.NEUTRALS_5,
});
