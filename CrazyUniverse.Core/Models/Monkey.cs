using CrazyUniverse.Core.Interfaces;
using System;

namespace CrazyUniverse.Core.Models
{
    public class Monkey : Animal, IClimbable, ISwimmable
    {
        public class Monkey : Animal
        {
            public Monkey(string name, int energy) : base(name, energy) { }

            public override void PerformSpecialAction(IRepository<Animal> repository, Action<string> logAction)
            {
                logAction($"successfully wiped all core activity logs clean");
                // Monkey-specific special effect
            }
        }
    }
}