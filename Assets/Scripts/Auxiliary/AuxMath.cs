using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceAce.Auxiliary
{
    public static class AuxMath
    {
        public const int SecondsPerMinute = 60;
        public const int MinutesPerHour = 60;

        public const int DegreesPerRevolution = 360;

        public static float Random => UnityEngine.Random.Range(0f, 1f);
        public static float RandomNormal => UnityEngine.Random.Range(-1f, 1f);
        public static float RandomSign => RandomNormal > 0f ? 1f : -1f;
        public static bool RandomBoolean => RandomSign > 0f;

        public static AnimationCurve EasingCurveIn { get; set; }
        public static AnimationCurve EasingCurveOut { get; set; }
        public static AnimationCurve EasingCurveInOut { get; set; }

        public static IEnumerable<int> GetRandomNumbersWithoutRepetition(int min, int max, int amount)
        {
            if (min >= max) throw new ArgumentOutOfRangeException($"{nameof(min)}, {nameof(max)}");
            if (amount > max - min) throw new ArgumentOutOfRangeException(nameof(amount));

            System.Random random = new();
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
            if (availableNumbers.Count() < amount) throw new ArgumentOutOfRangeException(nameof(amount));

            System.Random random = new();
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

        public static string GetFormattedTime(int time)
        {
            int minutes = time / SecondsPerMinute;
            int seconds = time % SecondsPerMinute;

            return $"{minutes:###0}:{seconds:00}";
        }

        public static string GetFormattedTime(float time)
        {
            int minutes = (int)time / SecondsPerMinute;
            int seconds = (int)time % SecondsPerMinute;

            return $"{minutes:###0}:{seconds:00}";
        }
    }
}