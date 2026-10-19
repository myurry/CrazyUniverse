using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyUniverse.Core.Interfaces
{
    public interface IClimbable
    {
        void Climb(Action<string> logger);
    }
}