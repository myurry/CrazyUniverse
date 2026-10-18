namespace CrazyUniverse.Core.Models
{
    public class Monkey : Animal
    {
        public Monkey(string name, int energy) : base(name, energy) { }

        public override void PerformSpecialAction(IAnimalRepository repository, Action<string> logger)
        {
            logger($"{Name} the Monkey swings around and successfully wipes all core activity logs clean!");
        }
    }
}