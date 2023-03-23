using System;
using System.Reflection;
using AutoFixture.Kernel;
using Npa.Accounting.Application.CardTransactions;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Tests.Application.Cards.Fixtures
{
    public class CardInfoGenerator : ISpecimenBuilder
    {
        private readonly Random random;

        public CardInfoGenerator()
        {
            random = new Random();
        }

        private static bool IsCardType(Type? t) => t == typeof(Card) || t == typeof(CardViewModel);

        private static bool IsNumberField(string n) => n == nameof(Card.Number) || n == nameof(CardViewModel.Number);
        
        private static bool IsCvvField(string n) => n == nameof(Card.Cvv) || n == nameof(CardViewModel.Cvv);
        
        private static bool IsExpiration(string n) => n == nameof(Card.Expiration) || n == nameof(CardViewModel.Expiration);

        private static bool IsZipCode(string n) =>
            n == nameof(Card.Address.ZipCode) || n == nameof(CardViewModel.ZipCode);

        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo pi && IsCardType(pi.DeclaringType))
            {
                if (IsNumberField(pi.Name))
                {
                    return
                        $"{random.Next(1000, 9999)}{random.Next(1000, 9999)}{random.Next(1000, 9999)}{random.Next(1000, 9999)}";
                }

                if (IsCvvField(pi.Name))
                {
                    return random.Next(0, 999).ToString("D3");
                }

                if (IsExpiration(pi.Name))
                {
                    return $"{random.Next(1, 12).ToString("D2")}/{random.Next(15, 40).ToString("D2")}";
                }

                if (IsZipCode(pi.Name))
                {
                    return $"{random.Next(0, 99999).ToString("D5")}";
                }
            }

            return new NoSpecimen();
        }
    }
}