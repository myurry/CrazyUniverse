using System;
using System.Collections.Generic;
using System.Linq;
using CrazyUniverse.Core.Models;

namespace CrazyUniverse.Core
{
    public class Repository<T> : IRepository<T>, IAnimalRepository where T : Animal
    {
        private readonly List<T> _items = new();

        // Events now use EventHandler<Animal> directly, which satisfies IAnimalRepository natively
        public event EventHandler<Animal>? ItemAdded;
        public event EventHandler<Animal>? ItemRemoved;
        public event EventHandler<Animal>? CrazyActionExecuted;

        public void Add(T item)
        {
            if (item == null || _items.Contains(item)) return;
            _items.Add(item);
            ItemAdded?.Invoke(this, item);
        }

        public void Remove(T item)
        {
            if (item == null || !_items.Contains(item)) return;
            ItemRemoved?.Invoke(this, item);
            _items.Remove(item);
            item.UnbindRepository();
        }

        public IEnumerable<T> GetAll() => _items.AsReadOnly();

        public T? Find(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);

        public void TriggerCrazyAction(T actor)
        {
            if (actor == null || !_items.Contains(actor)) return;
            CrazyActionExecuted?.Invoke(this, actor);
        }

        // --- IAnimalRepository Implementation ---

        IEnumerable<Animal> IAnimalRepository.GetAllAnimals() => _items;

        public void AddAnimal(Animal animal)
        {
            if (animal is T typedItem) Add(typedItem);
        }

        public void RemoveAnimal(Animal animal)
        {
            if (animal is T typedItem) Remove(typedItem);
        }

        public void TriggerCrazyActionForAnimal(Animal animal)
        {
            if (animal is T typedItem) TriggerCrazyAction(typedItem);
        }
    }
}