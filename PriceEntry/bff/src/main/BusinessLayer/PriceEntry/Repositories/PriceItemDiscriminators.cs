namespace BusinessLayer.PriceEntry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Attributes;

    public static class PriceItemDiscriminators
    {
        private static List<string>? knownTypeDiscriminators;

        public static IEnumerable<string> KnownTypeDiscriminators
        {
            get
            {
                knownTypeDiscriminators ??= GetKnownTypeDiscriminators<BasePriceItem>().ToList();
                return knownTypeDiscriminators;
            }
        }

        private static IEnumerable<string> GetKnownTypeDiscriminators<T>()
        {
            var baseType = typeof(T);

            if (baseType.GetCustomAttributes(typeof(BsonKnownTypesAttribute), false).FirstOrDefault() is
                BsonKnownTypesAttribute knownTypesAttr)
            {
                foreach (var type in knownTypesAttr.KnownTypes)
                {
                    var classMap = BsonClassMap.LookupClassMap(type);
                    yield return classMap.Discriminator;
                }
            }
        }
    }
}
