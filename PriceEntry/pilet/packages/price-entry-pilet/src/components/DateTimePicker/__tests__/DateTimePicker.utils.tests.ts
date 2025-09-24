import { getDaysInMonth } from 'date-fns';
import {
  generateYearOptions,
  generateMonthOptions,
  generateDayOptions,
  generateHalfHourSlots,
  MONTHS_SHORT,
} from '../DateTimePicker.utils';

describe('DateTimePicker.utils functions', () => {
  describe('generateYearOptions', () => {
    it('returns 7 years centered on current year', () => {
      const current = new Date().getFullYear();
      const options = generateYearOptions();
      expect(options).toHaveLength(7);
      expect(options[0].value).toBe((current - 3).toString());
      expect(options[6].value).toBe((current + 3).toString());
      options.forEach((opt) => {
        expect(opt.label).toBe(opt.value);
      });
    });
  });

  describe('generateMonthOptions', () => {
    it('maps locale "en" correctly', () => {
      const opts = generateMonthOptions('en');
      expect(opts).toHaveLength(12);
      opts.forEach((opt, idx) => {
        expect(opt.value).toBe(idx.toString());
        expect(opt.label).toBe(MONTHS_SHORT['en'][idx]);
      });
    });

    it('maps locale "zh" correctly', () => {
      const opts = generateMonthOptions('zh');
      expect(opts[0].label).toBe(MONTHS_SHORT['zh'][0]);
      expect(opts[11].label).toBe(MONTHS_SHORT['zh'][11]);
    });
  });

  describe('generateDayOptions', () => {
    it('generates correct count for February in a leap year', () => {
      const days = generateDayOptions('2020', '1'); // Feb 2020
      const expectedCount = getDaysInMonth(new Date(2020, 1));
      expect(days).toHaveLength(expectedCount);
      expect(days[0]).toEqual({ label: '01', value: '1' });
      expect(days[expectedCount - 1]).toEqual({
        label: expectedCount.toString().padStart(2, '0'),
        value: expectedCount.toString(),
      });
    });

    it('falls back to January of current year on invalid inputs', () => {
      const currentYear = new Date().getFullYear();
      const days = generateDayOptions('foo', 'bar');
      const expectedCount = getDaysInMonth(new Date(currentYear, 0));
      expect(days).toHaveLength(expectedCount);
      expect(days[0]).toEqual({ label: '01', value: '1' });
      expect(days[expectedCount - 1]).toEqual({
        label: expectedCount.toString().padStart(2, '0'),
        value: expectedCount.toString(),
      });
    });
  });

  describe('generateHalfHourSlots', () => {
    it('generates 48 half-hour slots', () => {
      const slots = generateHalfHourSlots();
      expect(slots).toHaveLength(48);
      expect(slots[0]).toBe('00:00');
      expect(slots[47]).toBe('23:30');
    });
  });
});
