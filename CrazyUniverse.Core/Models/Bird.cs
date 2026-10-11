using System;
using System.Collections.Generic;
using System.Text;
using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models
{
    public class Bird : Animal, IFlyable
    {
        public Bird(string name, int energyLevel) : base(name, energyLevel) { }

        public override string MakeSound()
        {
            return $"{Name} the Bird chirps cheerfully: Tweet tweet!";
        }

        public override string CrazyAction()
        {
            EnergyLevel = 0;
            return $"{Name} challenges gravity and initiates self-destruct sequence... 4... 3... 2... 1... Just kidding! App still running!";
        }

        public string Fly()
        {
            return $"{Name} soars into the stratosphere, breaking the sound barrier.";
        }
    }
}