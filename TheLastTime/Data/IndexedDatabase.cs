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
        public IndexedSet<Event> Events { get; set; }
        public IndexedSet<Instance> Instances { get; set; }
    }
}
