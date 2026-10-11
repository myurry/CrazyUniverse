namespace CrazyUniverse.Core.Models
{
    public class Cow : Animal
    {
        public Cow(string name, int energyLevel) : base(name, energyLevel) { }

        public override string MakeSound() => GetRandomSound("CowSounds");

        public override string CrazyAction()
        {
            EnergyLevel = Math.Max(0, EnergyLevel - 10);
            return $"{Name} attempts quantum teleportation and produces 500 liters of chocolate milk!";
        }
    }
}