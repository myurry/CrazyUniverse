using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyUniverse.Core.Models
{
    public class Horse : Animal
    {
        public Horse(string name, int energyLevel) : base(name, energyLevel) { }

        public override string MakeSound()
        {
            return $"{Name} the Horse neighs loudly: Neighhh!";
        }

        public override string CrazyAction()
        {
            EnergyLevel = Math.Min(100, EnergyLevel + 20);
            return $"{Name} duplicates itself 100 times and fills up the entire pasture with hooves!";
        }
    }
}