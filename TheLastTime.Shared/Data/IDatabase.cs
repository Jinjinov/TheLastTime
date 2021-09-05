using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheLastTime.Shared.Models;

namespace TheLastTime.Shared.Data
{
    public interface IDatabase : IDisposable
    {
        ICollection<Category> Categories { get; }
        ICollection<Group> Groups { get; }
        ICollection<Habit> Habits { get; }
        ICollection<Note> Notes { get; }
        ICollection<Settings> Settings { get; }
        ICollection<Time> Times { get; }
        ICollection<ToDo> ToDos { get; }

        Task SaveChanges();
    }

    public interface IDatabaseAccess
    {
        Task<IDatabase> CreateDatabase();
    }
}
