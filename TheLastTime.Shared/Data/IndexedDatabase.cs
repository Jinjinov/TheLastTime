using IndexedDB.Blazor;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public class IndexedDatabase : IndexedDb, IDatabase
    {
        public IndexedDatabase(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

        public IndexedSet<Category> Categories { get; set; } = null!;
        public IndexedSet<Habit> Habits { get; set; } = null!;
        public IndexedSet<Goal> Goals { get; set; } = null!;
        public IndexedSet<Settings> Settings { get; set; } = null!;
        public IndexedSet<Time> Times { get; set; } = null!;
        public IndexedSet<Tasky> Tasks { get; set; } = null!;

        ICollection<Category> IDatabase.Categories => Categories;
        ICollection<Habit> IDatabase.Habits => Habits;
        ICollection<Goal> IDatabase.Goals => Goals;
        ICollection<Settings> IDatabase.Settings => Settings;
        ICollection<Time> IDatabase.Times => Times;
        ICollection<Tasky> IDatabase.Tasks => Tasks;

        public ICollection<T> GetCollection<T>()
        {
            if(typeof(T) == typeof(Category)) return (ICollection<T>)Categories;
            if(typeof(T) == typeof(Habit)) return (ICollection<T>)Habits;
            if(typeof(T) == typeof(Goal)) return (ICollection<T>)Goals;
            if(typeof(T) == typeof(Settings)) return (ICollection<T>)Settings;
            if(typeof(T) == typeof(Time)) return (ICollection<T>)Times;
            if(typeof(T) == typeof(Tasky)) return (ICollection<T>)Tasks;

            throw new Exception();
        }
    }

    public class DatabaseAccess : IDatabaseAccess
    {
        private readonly IIndexedDbFactory _indexedDbFactory;

        public DatabaseAccess(IIndexedDbFactory indexedDbFactory)
        {
            _indexedDbFactory = indexedDbFactory;
        }

        public async Task<IDatabase> CreateDatabase() => await _indexedDbFactory.Create<IndexedDatabase>();
    }
}
