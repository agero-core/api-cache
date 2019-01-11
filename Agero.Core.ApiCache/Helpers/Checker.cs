using System;

namespace Agero.Core.ApiCache.Helpers
{
    internal static class Checker
    {
        public static void ArgumentIsNull<TArgument>(TArgument arg, string argumentName)
            where TArgument : class
        {
            if (arg == null)
                throw new ArgumentNullException(argumentName, $"Argument '{argumentName}' cannot be null.");
        }

        public static void ArgumentIsWhitespace(string arg, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(arg))
                throw new ArgumentException($"Argument '{argumentName}' cannot be null or empty string or contain only whitespaces.", argumentName);
        }

        public static void Argument(bool condition, string description)
        {
            if (!condition)
                throw new ArgumentException($"Argument does not meet the following '{description}' restriction.");
        }

        public static void Assert(bool condition, string description)
        {
            if (!condition)
                throw new InvalidOperationException(description);
        }
    }
}
