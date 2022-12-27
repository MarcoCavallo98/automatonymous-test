using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatonymousTest.Common.Events
{
    public record BDone
    {
        public Guid ItemId { get; }
    }
}
