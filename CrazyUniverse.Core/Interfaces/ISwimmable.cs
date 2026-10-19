using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyUniverse.Core.Interfaces
{
    public interface ISwimmable
    {
        void Swim(Action<string> logger);
    }
}