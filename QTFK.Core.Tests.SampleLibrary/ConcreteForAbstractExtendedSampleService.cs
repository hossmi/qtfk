using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleLibrary
{
    public class ConcreteForAbstractExtendedSampleService : AbstractSampleService
    {
        public override int SomeMethod(decimal a, decimal b)
        {
            return (int)(a * b);
        }
    }
}
