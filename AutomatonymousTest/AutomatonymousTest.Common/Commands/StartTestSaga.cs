using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatonymousTest.Common.Commands
{
    public record StartTestSaga
    {
        public Guid Id { get; set; }
    }
}
