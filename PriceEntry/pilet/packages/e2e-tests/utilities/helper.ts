export class Helpers {
  getRandomNumber(): [number, number] {
    const randomNumber: number = Math.floor(Math.random() * 91) + 10;
    const randomNumber1: number = parseFloat((Math.random() * 91 + 50).toFixed(2));
    return [randomNumber, randomNumber1];
}
  calc_Percentage(num1: number, num2: number) {
    if (num2 === 0) {
      return null;
    }
    const percentage = ((num1 - num2) / num2) * 100;
    const sign = percentage >= 0 ? "+" : "-";
    const formattedPercentage = Math.abs(percentage).toLocaleString("en-US", {
      maximumFractionDigits: 1,
    });
    if (!isNaN(percentage)) {
      return `${sign}${formattedPercentage}%`;
    } else {
      return null;
    }
  }
  calc_priceChange(num1: number, num2: number) {
    const sub = num1 - num2;
    const sign = sub >= 0 ? "+" : "-";
    const value = Math.abs(sub);
    return `${sign}${Number(value)}`;
  }
  calc_midPrice(num1: number, num2: number): number {
    return (num1 + num2) / 2;
  }

  getPreviousDate(): number {
    const today: Date = new Date();
    const previousDate: Date = new Date(today);

    // Attempt to set the date to the previous day
    previousDate.setDate(today.getDate() - 1);

    // Check if the resulting date is still in the same month
    if (previousDate.getMonth() === today.getMonth()) {
      return previousDate.getDate();
    }
    // If the previous date is not in the same month, return the last day of the previous month
    const lastDayOfPreviousMonth: Date = new Date(today.getFullYear(), today.getMonth(), 0);
    return lastDayOfPreviousMonth.getDate();
  }
  getNextDay(): number {
    const today: Date = new Date();
    const nextDay: Date = new Date(today);

    // Attempt to set the date to the previous day
    nextDay.setDate(today.getDate() + 1);

    // Check if the resulting date is still in the same month
    if (nextDay.getMonth() === today.getMonth()) {
      return nextDay.getDate();
    }
    // If the previous date is not in the same month, return the last day of the previous month
    const lastDayOfPreviousMonth: Date = new Date(today.getFullYear(), today.getMonth(), 0);
    return lastDayOfPreviousMonth.getDate();
  }
  getMonth(): string {
    const today: Date = new Date();
    const monthAsString: string = today.toLocaleString("default", { month: "long" });
    return monthAsString;
  }
  getPreviousMonth(): string {
    const today: Date = new Date();
    const lastMonthDate: Date = new Date(today.getFullYear(), today.getMonth() - 1, 1);
    const lastMonthAsString: string = lastMonthDate.toLocaleString("default", { month: "long" });
    return lastMonthAsString;
  }
  getPreviousMonthOnly(date: Date): string {
    const previousDate = new Date(date);
    previousDate.setDate(date.getDate() - 1);
    const month = previousDate.toLocaleString("default", { month: "long" });
    return `${month}`;
}
}
