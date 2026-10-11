using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyUniverse.Core.Models
{
    public class Cow : Animal
    {
        public Cow(string name, int energyLevel) : base(name, energyLevel) { }

        public override string MakeSound()
        {
            return $"{Name} the Cow moos deeply: Moooooo!";
        }

        public override string CrazyAction()
        {
            EnergyLevel = Math.Max(0, EnergyLevel - 10);
            return $"{Name} attempts quantum teleportation and accidentally produces 500 liters of chocolate milk!";
        }
    }
}