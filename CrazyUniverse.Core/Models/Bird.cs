using CrazyUniverse.Core.Interfaces;

namespace CrazyUniverse.Core.Models
{
    public class Bird : Animal
    {
        public Bird(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IRepository<Animal> repository, Action<string> logAction)
        {
            logAction($"{Name} has self-destructed!");
            repository.Remove(this);
        }
    }
}