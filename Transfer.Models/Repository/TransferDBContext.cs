using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfer.Models.Repository
{
    // http://stackoverflow.com/questions/6967108/is-idisposable-dispose-called-automatically
    // http://dotnetspeak.com/2011/03/repository-pattern-with-entity-framework
    public class TransferDBContext : TransferDBEntities
    {
        // SqlFunctions

        ~TransferDBContext() { 
            Dispose(false); 
        }
    }
}
