using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyUniverse.Core.Interfaces
{
    public interface IFlyable
    {
        void Fly(Action<string> logger);
    }
}