﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleLibrary
{
    public interface IOtherSampleService : ISampleService
    {
        string getSomeOtherValue(int value1, int value2);
    }
}
