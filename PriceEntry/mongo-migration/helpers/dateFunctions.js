function dateToString(date) {
  let month = `${date.getUTCMonth() + 1}`;
  let day = `${date.getUTCDate()}`;
  const year = date.getUTCFullYear();

  if (month.length < 2) { month = `0${month}`; }
  if (day.length < 2) { day = `0${day}`; }

  return [year, month, day].join('');
}

function dateRangeFromLabel(label) {
  const monthlyRegex = /^([A-Za-z]{3,})\s*(\d{4})$/;
  const quarterlyRegex = /^Q([1-4])\s*(\d{4})$/;

  let appliesFrom, appliesUntil;

  if (monthlyRegex.test(label)) {
    const [, monthStr, yearStr] = label.match(monthlyRegex);
    const monthIndex = new Date(`${monthStr} 1, 2000`).getMonth(); // 0-11
    const year = parseInt(yearStr);

    appliesFrom = new Date(Date.UTC(year, monthIndex , 1, 0, 0, 0)); // month
    appliesUntil = new Date(Date.UTC(year, monthIndex + 1, 0, 0, 0, 0)); // last day of month

  } else if (quarterlyRegex.test(label)) {
    const [, quarterStr, yearStr] = label.match(quarterlyRegex);
    const quarter = parseInt(quarterStr);
    const year = parseInt(yearStr);

    const startMonth = (quarter - 1) * 3;
    appliesFrom = new Date(Date.UTC(year, startMonth, 1, 0, 0, 0));
    appliesUntil = new Date(Date.UTC(year, startMonth + 3, 0, 0, 0, 0));
  }

  return { appliesFrom, appliesUntil };
}

module.exports = {
  dateToString,
  dateRangeFromLabel,
};
