using CrazyUniverse.Core.Models;
using System;
using System.Collections.Generic;

namespace CrazyUniverse.Core
{
    public interface IRepository<T> where T : class
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        T? Find(Func<T, bool> predicate);
    }

    public interface IAnimalRepository
    {
        IEnumerable<Animal> GetAllAnimals();
        void AddAnimal(Animal animal);
        void RemoveAnimal(Animal animal);
        void TriggerCrazyActionForAnimal(Animal animal);

        event EventHandler<Animal>? ItemAdded;
        event EventHandler<Animal>? ItemRemoved;
        event EventHandler<Animal>? CrazyActionExecuted;
    }

}