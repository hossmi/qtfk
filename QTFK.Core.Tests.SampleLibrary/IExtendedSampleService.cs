﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleLibrary
{
    public interface IExtendedSampleService : ISampleService
    {
        string getSomeValue(int value1, int value2);
    }
}
