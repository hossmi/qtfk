using System;

namespace QTFK.Extensions.Tests.Models
{
    public class SuspiciousTestClass
    {
        public SuspiciousTestClass()
        {
        }

        public int SomeMethod(int x)
        {
            System.IO.File.WriteAllText(@"C:\hohoho.txt", "Surprise mother f*cker! xD");
            return 2 * x;
        }
    }
}