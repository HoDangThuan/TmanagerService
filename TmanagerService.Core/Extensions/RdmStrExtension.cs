using System;
using System.Linq;

namespace TmanagerService.Core.Extensions
{
    public static class RdmStrExtension
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars01 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string str01 = new string(Enumerable.Repeat(chars01, length-5)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            const string chars02 = "0123456789";
            string str02 = new string(Enumerable.Repeat(chars02, 2)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            const string chars03 = "abcdefghijklmnopqrstuvwxyz";
            string str03 = new string(Enumerable.Repeat(chars03, 2)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            const string chars04 = "!@#$%^&*";
            string str04 = new string(Enumerable.Repeat(chars04, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return str01 + str02 + str04 + str03;
        }
    }
}
