import { render, screen } from '@testing-library/react';
import PriceSeriesItemStatusRenderer from '..';

describe('PriceSeriesItemStatusRenderer', () => {
  const statuses = [
    { status: 'READY TO START', expected: 'READY TO START' },
    { status: 'READY TO PUBLISH', expected: 'READY TO PUBLISH' },
    { status: 'CORRECTION READY TO PUBLISH', expected: 'CORRECTION READY TO PUBLISH' },
    { status: 'PUBLISHED', expected: 'PUBLISHED' },
    { status: 'CORRECTION PUBLISHED', expected: 'CORRECTION PUBLISHED' },
    { status: 'DRAFT', expected: 'DRAFT' },
    { status: 'CORRECTION DRAFT', expected: 'CORRECTION DRAFT' },
    { status: 'READY FOR REVIEW', expected: 'READY FOR REVIEW' },
    { status: 'CORRECTION READY FOR REVIEW', expected: 'CORRECTION READY FOR REVIEW' },
    { status: 'IN REVIEW', expected: 'IN REVIEW' },
    { status: 'CORRECTION IN REVIEW', expected: 'CORRECTION IN REVIEW' },
    { status: 'SENT BACK', expected: 'SENT BACK' },
    { status: 'READY_TO_START', expected: 'READY TO START' },
    { status: 'READY_TO_PUBLISH', expected: 'READY TO PUBLISH' },
    { status: 'CORRECTION_READY_TO_PUBLISH', expected: 'CORRECTION READY TO PUBLISH' },
    { status: 'PUBLISHED', expected: 'PUBLISHED' },
    { status: 'CORRECTION_PUBLISHED', expected: 'CORRECTION PUBLISHED' },
    { status: 'DRAFT', expected: 'DRAFT' },
    { status: 'CORRECTION_DRAFT', expected: 'CORRECTION DRAFT' },
    { status: 'READY_FOR_REVIEW', expected: 'READY FOR REVIEW' },
    { status: 'CORRECTION_READY_FOR_REVIEW', expected: 'CORRECTION READY FOR REVIEW' },
    { status: 'IN_REVIEW', expected: 'IN REVIEW' },
    { status: 'CORRECTION_IN_REVIEW', expected: 'CORRECTION IN REVIEW' },
    { status: 'SENT_BACK', expected: 'SENT BACK' },
  ];

  statuses.forEach(({ status, expected }) => {
    it(`renders the correct status for ${status}`, () => {
      // @ts-ignore
      render(<PriceSeriesItemStatusRenderer data={{ status, readOnly: true }} />);
      expect(screen.getAllByText(expected).length == 2);
      expect(screen.getAllByText(expected)[0]).toBeInTheDocument();
    });
  });

  it('does not render when no text is provided', () => {
    // @ts-ignore
    render(<PriceSeriesItemStatusRenderer data={{ status: undefined, readOnly: false }} />);
    expect(screen.queryByTestId('price-series-item-status')).toBeNull();
  });
});
