using System;
using System.Collections.Generic;
using System.Linq;

namespace FolderPoll.Tests.CoreTests
{
    public static class TestHelper
    {
        private static readonly Random Rnd = new Random();

        public static string GetRandomAlphaNumericFileExtension(int length)
        {
            var chars = 'a'.To('z').Concat('0'.To('9')).ToList();
            return new string(chars.Select(c => chars[Rnd.Next(chars.Count)]).Take(length).ToArray());
        }

        public static IEnumerable<char> To(this char start, char end)
        {
            if (end < start)
            {
                throw new ArgumentOutOfRangeException("the end char should be less than start char", innerException: null);
            }
            return Enumerable.Range(start, end - start + 1).Select(i => (char)i);
        }
    }
}
