using System;
using System.Resources;

namespace CrazyUniverse.Core.Models
{
    public class Monkey : Animal
    {
        private static readonly Random _random = new();

        public Monkey(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IAnimalRepository repository, Action<string> logger)
        {
            logger($"{Name} the Monkey swings around and successfully wipes all core activity logs clean!");
        }

        public override void MakeSound(Action<string> logger)
        {
            string rawSounds = AnimalSounds.ResourceManager.GetString("MonkeySounds") ?? string.Empty;
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