namespace BusinessLayer.PriceEntry.ValueObjects
{
    public record Price
    {
        private Price(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }

        public static bool TryCreate(decimal value, out Price? price)
        {
            if (value <= 0)
            {
                price = null;
                return false;
            }

            price = new Price(value);
            return true;
        }
    }
}
