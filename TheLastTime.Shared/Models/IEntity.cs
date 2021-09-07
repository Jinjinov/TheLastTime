using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLastTime.Shared.Models
{
    public interface IEntity<T>
    {
        long Id { get; set; }

        void CopyTo(T destination);
    }
}
