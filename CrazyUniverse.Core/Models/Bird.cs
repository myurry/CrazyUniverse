namespace CrazyUniverse.Core.Models
{
    public class Bird : Animal
    {
        public Bird(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IAnimalRepository repository, Action<string> logger)
        {
            logger($"{Name} the Bird has self-destructed!");
            repository.RemoveAnimal(this);
        }
    }
}