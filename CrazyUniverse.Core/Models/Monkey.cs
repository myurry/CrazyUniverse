using CrazyUniverse.Core.Interfaces;
using System;

namespace CrazyUniverse.Core.Models
{
    public class Monkey : Animal, IClimbable, ISwimmable
    {
        private static readonly Random _random = new();

        public Monkey(string name, int energyLevel) : base(name, energyLevel) { }

        public override string MakeSound() => GetRandomSound("MonkeySounds");

        public override string CrazyAction()
        {
            EnergyLevel = Math.Max(0, EnergyLevel - 20);
            return $"{Name} is hacking into the core activity logs!";
        }

        // Implementation for IClimbable
        public string Climb() => $"{Name} deftly scales the sanctuary trees.";

        // Implementation for ISwimmable
        public string Swim() => $"{Name} takes a quick dip in the sanctuary pond.";

        public string PerformSpecialAction()
        {
            return _random.Next(2) == 0 ? Climb() : Swim();
        }
    }
}