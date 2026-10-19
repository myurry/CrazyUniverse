using CrazyUniverse.Core.Interfaces;
using System.Resources;

namespace CrazyUniverse.Core.Models
{
    public class Bird : Animal, IFlyable
    {
        private static readonly Random _random = new();

        public Bird(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IAnimalRepository repository, Action<string> logger)
        {
            logger($"{Name} the Bird has self-destructed!");
            repository.RemoveAnimal(this);
        }

        public override void MakeSound(Action<string> logger)
        {
            string rawSounds = AnimalSounds.ResourceManager.GetString("BirdSounds") ?? string.Empty;
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

        public void Fly(Action<string> logger)
        {
            logger($"{Name} is flying high in the sky!");
        }
    }
}