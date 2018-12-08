using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Attributes
{
    public abstract class DBAttribute : Attribute
    {
        public DBAttribute(string engine)
        {
            Asserts.stringIsNotEmpty(engine);
            Engine = engine;
        }

        public string Engine { get; }
    }
}
