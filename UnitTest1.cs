using System;
using System.Globalization;
using System.Management.Instrumentation;
using NUnit.Framework;

namespace CarPricer
{
    public class Car
    {
        public decimal PurchaseValue { get; set; }
        public int AgeInMonths { get; set; }
        public int NumberOfMiles { get; set; }
        public int NumberOfPreviousOwners { get; set; }
        public int NumberOfCollisions { get; set; }
        public string Make { get; set; }
    }
    public class PriceDeterminator
    {
        
        public decimal DetermineCarPrice(Car car)
        {
            var currentValue = car.PurchaseValue;

            //AGE
            currentValue -= car.AgeInMonths >= 120 ?
                120 * .005m * car.PurchaseValue : car.AgeInMonths * .005m * car.PurchaseValue;

            //MILES
            currentValue -= car.NumberOfMiles <= 150000 ?
                .002m * (car.NumberOfMiles / 1000) * currentValue : .002m * 150 * currentValue;

            //PREVIOUS OWNER -VE
            currentValue -= car.NumberOfPreviousOwners > 2 ?
                .25m * currentValue : 0;

            //COLLISION
            car.NumberOfCollisions = car.NumberOfCollisions > 5 ? 5 : car.NumberOfCollisions;
            currentValue -= .02m * car.NumberOfCollisions * currentValue;

            //MAKE
            TextInfo text = new CultureInfo("en-US", false).TextInfo;
            string make = car.Make != null ? text.ToTitleCase(car.Make): "";
            currentValue += 1.05m * currentValue <= .9m * car.PurchaseValue && "Toyota" == make  ?
                .05m * currentValue : 0;
            currentValue -= 500 + currentValue <= .9m * car.PurchaseValue && "Ford" == make ?
                500m : 0;

            //PREVIOUS OWNER +VE
            currentValue += 1.1m * currentValue <= .9m * car.PurchaseValue && car.NumberOfPreviousOwners == 0 ?
                .1m * currentValue : 0;

            return currentValue <= .9m * car.PurchaseValue ? currentValue : .9m * car.PurchaseValue;
        }
    }
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void CalculateCarValue()
        {
            AssertCarValue(24813.40m, 35000m, 3 * 12, 50000, 1, 1, "Ford");
            AssertCarValue(20672.61m, 35000m, 3 * 12, 150000, 1, 1, "Toyota");
            AssertCarValue(19688.20m, 35000m, 3 * 12, 250000, 1, 1, "Tesla");
            AssertCarValue(21094.5m, 35000m, 3 * 12, 250000, 1, 0, "toyota");
            AssertCarValue(21657.02m, 35000m, 3 * 12, 250000, 0, 1, "Acura");
            AssertCarValue(72000m, 80000m, 8, 10000, 0, 1, null);
        }
        private static void AssertCarValue(decimal expectValue, decimal purchaseValue,
        int ageInMonths, int numberOfMiles, int numberOfPreviousOwners, int
        numberOfCollisions, string make)
        {
            Car car = new Car
            {
                AgeInMonths = ageInMonths,
                NumberOfCollisions = numberOfCollisions,
                NumberOfMiles = numberOfMiles,
                NumberOfPreviousOwners = numberOfPreviousOwners,
                PurchaseValue = purchaseValue,
                Make = make
            };
            PriceDeterminator priceDeterminator = new PriceDeterminator();
            var carPrice = priceDeterminator.DetermineCarPrice(car);
            Assert.AreEqual(expectValue, carPrice);
        }
    }
}