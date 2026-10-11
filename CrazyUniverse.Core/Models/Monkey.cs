using System;
using System.Collections.Generic;
using System.Text;
using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models
{
    public class Monkey : Animal, IClimbable
    {
        public Monkey(string name, int energyLevel) : base(name, energyLevel) { }

        public override string MakeSound()
        {
            return $"{Name} the Monkey screeches: Ooh ooh ah ah!";
        }

        public override string CrazyAction()
        {
            EnergyLevel = Math.Max(0, EnergyLevel - 25);
            return $"{Name} hacks the mainframe and starts deleting system logs while eating a banana.";
        }

        public string Climb()
        {
            return $"{Name} scales the nearest skyscraper in 3 seconds flat.";
        }
    }
}