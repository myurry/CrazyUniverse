using System;
using System.Collections.Generic;
using System.Linq;

namespace CrazyUniverse.Core
{
    public class Repository<T> : IRepository<T> where T : Models.Animal
    {
        private readonly List<T> _items = new List<T>();

        // Events to notify subscribers within the same repository
        public event EventHandler<T>? ItemAdded;
        public event EventHandler<T>? ItemRemoved;
        public event EventHandler<T>? CrazyActionExecuted;

        public void Add(T item)
        {
            if (item == null || _items.Contains(item)) return;

            _items.Add(item);

            // Trigger Addition event (existing items greet the new item)
            ItemAdded?.Invoke(this, item);
        }

        public void Remove(T item)
        {
            if (item == null || !_items.Contains(item)) return;

            // Trigger Removal event BEFORE removing so remaining items can say goodbye
            ItemRemoved?.Invoke(this, item);

            _items.Remove(item);
        }

        public IEnumerable<T> GetAll() => _items.AsReadOnly();

        public T? Find(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);

        public void TriggerCrazyAction(T actor)
        {
            if (actor == null || !_items.Contains(actor)) return;

            // Trigger CrazyAction event for neighbors
            CrazyActionExecuted?.Invoke(this, actor);
        }
    }
}