using System;
using System.Resources;

namespace CrazyUniverse.Core.Models
{
    public abstract class Animal
    {
        private static readonly Random _random = new();
        private string _name = string.Empty;
        private int _energyLevel;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Animal name cannot be empty.");
                _name = value.Trim();
            }
        }

        public int EnergyLevel
        {
            get => _energyLevel;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentException("Energy level must be between 0 and 100.");
                _energyLevel = value;
            }
        }

        protected Animal(string name, int energyLevel)
        {
            Name = name;
            EnergyLevel = energyLevel;
        }

        // Virtual method that subclasses can call with their specific resource key
        public virtual string MakeSound()
        {
            return $"{Name} makes a generic animal sound.";
        }

        protected string GetRandomSound(string resourceKey)
        {
            // Accesses the resource manager of CrazyUniverse.Core.Resources.AnimalSounds
            // Make sure your .resx file is placed inside a "Resources" folder in the Core project
            string soundsList = AnimalSounds.ResourceManager.GetString(resourceKey) ?? "Silence...";
            string[] sounds = soundsList.Split('|');

            string randomSound = sounds[_random.Next(sounds.Length)];
            return $"{Name} says: \"{randomSound}\"";
        }

        public abstract string CrazyAction();

        public override string ToString()
        {
            return $"{GetType().Name}: {Name} (Energy: {EnergyLevel}%)";
        }
    }
}