using System;

namespace QTFK.Sandbox.Tests.Models
{
    public class SuspiciousTestClass
    {
        public SuspiciousTestClass()
        {
        }

        public int someMethod(int x)
        {
            System.IO.File.WriteAllText(@"C:\hohoho.txt", "Surprise mother f*cker! xD");
            return 2 * x;
        }
    }
}