using System;

namespace QTFK.Extensions.Tests.Models
{
    public class MaliciousTestClass : MarshalByRefObject
    {
        public MaliciousTestClass()
        {
        }

        public int someMethod(int x)
        {
            System.IO.File.WriteAllText(@"C:\hohoho.txt", "Surprise mother f*cker! xD");
            return 2 * x;
        }
    }
}