using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models
{
    public class Bird : Animal, IFlyable
    {
        public Bird(string name, int energyLevel) : base(name, energyLevel) { }

        public override string MakeSound() => GetRandomSound("BirdSounds");

        public override string CrazyAction()
        {
            EnergyLevel = 0;
            return $"{Name} challenges gravity and initiates self-destruct sequence...";
        }

        public string Fly() => $"{Name} flies gracefully through the skies.";
    }
}