using Blazor.IndexedDB.Framework;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLastTime.Data
{
    // Represents the database
    public class ExampleDb : IndexedDb
    {
        public ExampleDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

        // These are like tables. Declare as many of them as you want.
        public IndexedSet<Person> People { get; set; }
    }
}
