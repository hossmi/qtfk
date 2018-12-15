using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleLibrary
{
    public class ConcreteExtendedSampleService : IExtendedSampleService
    {
        public string getSomeValue(int value1, int value2)
        {
            return string.Format("{0}#{1}",value1, value2);
        }

        public int SomeMethod(decimal a, decimal b)
        {
            return (int)(a * b);
        }
    }
}
