// <copyright file="Args.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CommonLib
{
    using System;

    public static class Args
    {
        public static void NotNull<T>(T argument, string argumentName)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void NotNullOrWhitespace(string stringArgument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(stringArgument))
            {
                throw new ArgumentException("Null or empty", argumentName);
            }
        }
    }
}
