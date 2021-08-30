using IndexedDB.Blazor;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public class IndexedDatabase : IndexedDb, IDatabase
    {
        public IndexedDatabase(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

        public IndexedSet<Category> Categories { get; set; } = null!;
        public IndexedSet<Group> Groups { get; set; } = null!;
        public IndexedSet<Habit> Habits { get; set; } = null!;
        public IndexedSet<Note> Notes { get; set; } = null!;
        public IndexedSet<Settings> Settings { get; set; } = null!;
        public IndexedSet<Time> Times { get; set; } = null!;
        public IndexedSet<ToDo> ToDos { get; set; } = null!;

        ICollection<Category> IDatabase.Categories => Categories;
        ICollection<Group> IDatabase.Groups => Groups;
        ICollection<Habit> IDatabase.Habits => Habits;
        ICollection<Note> IDatabase.Notes => Notes;
        ICollection<Settings> IDatabase.Settings => Settings;
        ICollection<Time> IDatabase.Times => Times;
        ICollection<ToDo> IDatabase.ToDos => ToDos;
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
