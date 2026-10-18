namespace CrazyUniverse.Core.Models
{
    public class Horse : Animal
    {
        public Horse(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IRepository<Animal> repository, Action<string> logAction)
        {
            logAction($"{Name} the Horse gallops fiercely and clones itself!");
            // Clone logic handled polymorphically here or via repository callback
            var clone = new Horse($"{Name}_Clone", EnergyLevel);
            repository.Add(clone);
        }
    }
}