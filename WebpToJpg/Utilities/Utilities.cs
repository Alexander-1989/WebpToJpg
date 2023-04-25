using System;

namespace WebpToJpg.UtilitiesServise
{
    internal static class Utilities
    {
        private static readonly Random random = new Random();

        public static string GetRandomName()
        {
            return GetRandomName(5, 20, null);
        }
        public static string GetRandomName(int max)
        {
            return GetRandomName(0, max, null);
        }

        public static string GetRandomName(int min, int max)
        {
            return GetRandomName(min, max, null);
        }

        public static string GetRandomName(string extension)
        {
            return GetRandomName(5, 20, extension);
        }

        public static string GetRandomName(int max, string extension)
        {
            return GetRandomName(0, max, extension);
        }

        public static string GetRandomName(int min, int max, string extension)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int minimum = min >= 0 ? min : 0;
            int maximum = max >= min ? max : min;
            int lineLength = random.Next(minimum, maximum);
            int extensionLength = extension?.Length ?? 0;
            char[] result = new char[lineLength + extensionLength];

            for (int i = 0; i < lineLength; i++)
            {
                result[i] = alphabet[random.Next(alphabet.Length)];
            }

            for (int j = 0; j < extensionLength; j++)
            {
                result[lineLength + j] = extension[j];
            }

            return new string(result);
        }
    }
}