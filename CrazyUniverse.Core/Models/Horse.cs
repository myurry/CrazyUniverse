using System;
using System.Resources;

namespace CrazyUniverse.Core.Models
{
    public class Horse : Animal
    {
        private static readonly Random _random = new();

        public Horse(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IAnimalRepository repository, Action<string> logger)
        {
            logger($"{Name} the Horse gallops fiercely and clones itself!");
            var clone = new Horse($"{Name}_Clone", EnergyLevel);
            repository.AddAnimal(clone);
        }

        public override void MakeSound(Action<string> logger)
        {
            string rawSounds = AnimalSounds.ResourceManager.GetString("HorseSounds") ?? string.Empty;
            string[] soundOptions = rawSounds.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (soundOptions.Length > 0)
            {
                string randomSound = soundOptions[_random.Next(soundOptions.Length)];
                logger($"{Name} says: {randomSound}");
            }
            else
            {
                logger($"{Name} is silent.");
            }
        }
    }
}