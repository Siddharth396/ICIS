namespace SpecificKafkaProducer.Test
{
    using Avro;
    using Avro.Specific;

    public class PriceSeries : ISpecificRecord
    {
        public int Id { get; set; }

        public Schema Schema => throw new System.NotImplementedException();

        public object Get(int fieldPos)
        {
            throw new System.NotImplementedException();
        }

        public void Put(int fieldPos, object fieldValue)
        {
            throw new System.NotImplementedException();
        }
    }
}