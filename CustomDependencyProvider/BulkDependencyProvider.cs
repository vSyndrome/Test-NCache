using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.Dependencies;

namespace CustomDependencyProvider
{
    class BulkDependencyProvider: BulkExtensibleDependency
    {
        public override bool Initialize()
        {
            throw new NotImplementedException();
        }

        public override void EvaluateBulk(IEnumerable<BulkExtensibleDependency> dependencies)
        {
            throw new NotImplementedException();
        }
    }
}
