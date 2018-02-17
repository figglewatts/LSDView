using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSDView.model
{
    public abstract class AbstractDocument<T>
    {
        public abstract T Document { get; }
    }
}
