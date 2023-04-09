using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceAce.Auxiliary
{
    public static class AuxMath
    {
        private const float DegreesPerSecondPerRotationPerMinute = 6f;

        public static float RandomNormal => UnityEngine.Random.Range(-1f, 1f);
        public static float RandomSign => RandomNormal > 0f ? 1f : -1f;
        public static bool RandomBoolean => RandomSign > 0f;

        public static float RotationsPerMinuteToDegreesPerSecond(float value) => value * DegreesPerSecondPerRotationPerMinute;
        public static float DegreesPerSecondToRotationsPerMinute(float value) => value / DegreesPerSecondPerRotationPerMinute;

        public static IEnumerable<int> GetRandomNumbersWithoutRepetition(int min, int max, int amount)
        {
            if (min >= max)
            {
                throw new ArgumentOutOfRangeException($"{nameof(min)}, {nameof(max)}", "Range bounds cannot be inverted!");
            }

            if (amount > max - min)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount cannot be greater than total integers within range!");
            }

            Random random = new();
            List<int> availableNumbers = Enumerable.Range(min, max - min).ToList();
            List<int> generatedNumbers = new(amount);

            for (int i = 0; i < amount; i++)
            {
                int index = random.Next(0, availableNumbers.Count);

                generatedNumbers.Add(availableNumbers[index]);
                availableNumbers.RemoveAt(index);
            }

            return generatedNumbers;
        }

        public static IEnumerable<int> GetRandomNumbersWithoutRepetition(IEnumerable<int> availableNumbers, int amount)
        {
            if (availableNumbers.Count() < amount)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), $"Amount of random numbers to generate must not surpass the available numbers count!");
            }

            Random random = new();
            HashSet<int> numbersToUse = new(availableNumbers);
            List<int> generatedNumbers = new(amount);

            for (int i = 0; i < amount; i++)
            {
                int index = random.Next(0, numbersToUse.Count);
                int randomNumberFromRange = numbersToUse.ElementAt(index);

                generatedNumbers.Add(randomNumberFromRange);
                numbersToUse.Remove(index);
            }

            return generatedNumbers;
        }
    }
}