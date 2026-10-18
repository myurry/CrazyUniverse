namespace CrazyUniverse.Core.Models
{
    public class Horse : Animal
    {
        public Horse(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IAnimalRepository repository, Action<string> logger)
        {
            logger($"{Name} the Horse gallops fiercely and clones itself!");
            var clone = new Horse($"{Name}_Clone", EnergyLevel);
            repository.AddAnimal(clone);
        }
    }
}