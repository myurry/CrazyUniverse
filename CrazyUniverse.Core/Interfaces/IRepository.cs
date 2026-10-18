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
}