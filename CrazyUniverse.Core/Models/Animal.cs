using System;

namespace CrazyUniverse.Core.Models
{
    public abstract class Animal
    {
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

        // Virtual method for standard behavior
        public virtual string MakeSound()
        {
            return $"{Name} makes a generic animal sound.";
        }

        // Abstract method that every subclass must implement uniquely
        public abstract string CrazyAction();

        public override string ToString()
        {
            return $"{GetType().Name}: {Name} (Energy: {EnergyLevel}%)";
        }
    }
}