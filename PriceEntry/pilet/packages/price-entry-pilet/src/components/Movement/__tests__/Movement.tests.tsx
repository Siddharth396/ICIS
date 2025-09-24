import React from 'react';
import { render, screen } from '@testing-library/react';
import { Language } from '@icis/ui-kit';
import { Movement } from '..';

describe('Movement Component', () => {
  const precision = 2;
  const locale: Language = 'en';

  it('renders fallback when price is undefined', () => {
    render(<Movement price={undefined} delta={10} locale={locale} />);
    expect(screen.getByText('(--)')).toBeInTheDocument();
  });

  it('renders fallback when price is 0', () => {
    render(<Movement price={0} delta={10} precision={precision} locale={locale} />);
    expect(screen.getByText('(--)')).toBeInTheDocument();
  });

  it('calculates percentage using previousPrice (positive delta)', () => {
    // previousPrice = 90, delta = 20 => ~22.22...% (rounded to 22.2 in theory)
    render(
      <Movement price={100} previousPrice={90} delta={20} precision={precision} locale={locale} />,
    );
    // We'll match partial text like "(+22.2" or "(+22.22...%)"
    expect(
      screen.getByText((content) => {
        const normalized = content.replace(/\s/g, '');
        // This regex means: "starts with '(+22.2' and ends with ')'"
        // allowing additional digits. e.g. "(+22.222222%...)"
        return /^\(\+22\.2[\d]*%\)$/.test(normalized);
      }),
    ).toBeInTheDocument();
  });

  it('calculates percentage using previousPrice (negative delta)', () => {
    render(
      <Movement price={100} previousPrice={90} delta={-20} precision={precision} locale={locale} />,
    );
    expect(
      screen.getByText((content) => {
        const normalized = content.replace(/\s/g, '');
        // e.g. "(-22.2%%%%)"
        return /^\(-22\.2[\d]*%\)$/.test(normalized);
      }),
    ).toBeInTheDocument();
  });

  it('calculates percentage using delta * 100 (positive delta)', () => {
    // No previousPrice: delta = 20 => 20×100 => 2000 => "2,000.0"
    render(<Movement price={100} delta={20} precision={precision} locale={locale} />);
    expect(
      screen.getByText((content) => {
        const normalized = content.replace(/\s/g, '');
        // matching something like "(+2,000.0%...)"
        return /^\(\+2,000\.?[\d]*%\)$/.test(normalized);
      }),
    ).toBeInTheDocument();
  });

  it('calculates percentage using delta * 100 (negative delta)', () => {
    render(<Movement price={100} delta={-20} precision={precision} locale={locale} />);
    expect(
      screen.getByText((content) => {
        const normalized = content.replace(/\s/g, '');
        return /^\(-2,000\.?[\d]*%\)$/.test(normalized);
      }),
    ).toBeInTheDocument();
  });

  it('renders (0%) when delta is 0', () => {
    render(<Movement price={100} delta={0} precision={precision} locale={locale} />);
    expect(
      screen.getByText((content) => content.replace(/\s/g, '') === '(0%)'),
    ).toBeInTheDocument();
  });
});
