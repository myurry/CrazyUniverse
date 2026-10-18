using System;

namespace CrazyUniverse.Core.Models
{
    public class Cow : Animal
    {
        public Cow(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IAnimalRepository repository, Action<string> logger)
        {
            logger($"{Name} the Cow peacefully grazes and restores energy!");
            ModifyEnergy(15);
        }
    }
}