using Blazor.IndexedDB.Framework;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastTime.Data
{
    public class IndexedDatabase : IndexedDb
    {
        public IndexedDatabase(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

        public IndexedSet<Category> Categories { get; set; }
        public IndexedSet<Habit> Habits { get; set; }
        public IndexedSet<Time> Times { get; set; }
    }
}
