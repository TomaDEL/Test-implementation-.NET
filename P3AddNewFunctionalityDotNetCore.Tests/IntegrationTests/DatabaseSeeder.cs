using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models.Entities;

namespace P3AddNewFunctionalityDotNetCore.Tests.IntegrationTests
{
    public static class DatabaseSeeder
    {
        public static void Seed(P3Referential db)
        {
            db.Product.AddRange(
                new Product
                {
                    Name = "Laptop",
                    Price = 1200.00,
                    Quantity = 10,
                    Description = "Ordinateur portable",
                    Details = "Intel i7"
                },
                new Product
                {
                    Name = "Mouse",
                    Price = 50.00,
                    Quantity = 20,
                    Description = "Souris sans fil",
                    Details = "USB"
                },
                new Product
                {
                    Name = "Keyboard",
                    Price = 100.00,
                    Quantity = 15,
                    Description = "Clavier mécanique",
                    Details = "AZERTY"
                }
            );
            db.SaveChanges();
        }
    }
}