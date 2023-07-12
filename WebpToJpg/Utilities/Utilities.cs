using System;

namespace WebpToJpg.UtilitiesService
{
    internal static class Utilities
    {
        private const int minValue = 5;
        private const int maxValue = 20;
        private static readonly Random random = new Random();

        public static string GetRandomName()
        {
            return GetRandomName(minValue, maxValue, null);
        }
        public static string GetRandomName(int max)
        {
            return GetRandomName(minValue, max, null);
        }

        public static string GetRandomName(int min, int max)
        {
            return GetRandomName(min, max, null);
        }

        public static string GetRandomName(string extension)
        {
            return GetRandomName(minValue, maxValue, extension);
        }

        public static string GetRandomName(int max, string extension)
        {
            return GetRandomName(minValue, max, extension);
        }

        public static string GetRandomName(int min, int max, string extension)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int minimum = min < 0 ? 0 : min;
            int maximum = max < min ? min : max;
            int lineLength = random.Next(minimum, maximum);
            int extensionLength = extension?.Length ?? 0;
            char[] result = new char[lineLength + extensionLength];

            for (int i = 0; i < lineLength; i++)
            {
                int index = random.Next(alphabet.Length);
                result[i] = alphabet[index];
            }

            for (int j = 0; j < extensionLength; j++)
            {
                result[lineLength + j] = extension[j];
            }

            return new string(result);
        }
    }
}