﻿using System.Collections.Generic;
using System.Linq;

namespace BusinessTrips.DataAccessLayer
{
    public class InMemoryStorage<T> : IStorage<T>
    {
        private List<T> storage=new List<T>();

        public void Add(T obj)
        {
            storage.Add(obj);
        }

        public T Get(T element)
        {
            return storage.First(e => e.Equals(element));
        }
    }
}