using System;
using System.Collections.Generic;

namespace ContainerApp
{

    internal static class ProductMinTemp
    {
        private static readonly Dictionary<ProductType, double> MinTempMap = new()
        {
            { ProductType.Bananas,     13.3 },
            { ProductType.Chocolate,   18.0 },
            { ProductType.Fish,        2.0  },
            { ProductType.Meat,       -15.0 },
            { ProductType.IceCream,   -18.0 },
            { ProductType.FrozenPizza,-30.0 },
            { ProductType.Cheese,      7.2  },
            { ProductType.Sausages,    5.0  },
            { ProductType.Butter,     20.5 },
            { ProductType.Eggs,       19.0 }
        };

        public static double GetMinTemperature(ProductType product)
        {
            if (MinTempMap.TryGetValue(product, out double minT))
                return minT;


            return 0.0;
        }
    }

    public class RefrigeratedContainer : Container
    {
        public ProductType Product { get; }
        
        public double Temperature { get; }

        public RefrigeratedContainer(double maxLoadKg, double ownWeightKg, double heightCm, double depthCm,
                                     ProductType product, double temperature)
            : base(maxLoadKg, ownWeightKg, heightCm, depthCm, 'C')
        {
            double minTemp = ProductMinTemp.GetMinTemperature(product);
            if (temperature < minTemp)
            {
                throw new ArgumentException(
                    $"Temperatura {temperature}°C jest za niska dla produktu {product}. " +
                    $"Minimalna wymagana to {minTemp}°C."
                );
            }

            Product = product;
            Temperature = temperature;
        }

        public override void Load(double mass)
        {
            if (CurrentLoadKg + mass > MaxLoadKg)
                throw new OverfillException($"Załadunek przekracza pojemność chłodni {SerialNumber}");

            base.Load(mass);
        }

        public override string ToString() =>
            base.ToString() + $" [Produkt: {Product}, Temp: {Temperature}°C]";
    }
}
