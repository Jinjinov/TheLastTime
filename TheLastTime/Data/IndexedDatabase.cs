using Blazor.IndexedDB.Framework;
using Microsoft.JSInterop;

namespace TheLastTime.Data
{
    public class IndexedDatabase : IndexedDb
    {
        public IndexedDatabase(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

        public IndexedSet<Category> Categories { get; set; } = null!;
        public IndexedSet<Habit> Habits { get; set; } = null!;
        public IndexedSet<Time> Times { get; set; } = null!;
    }
}
