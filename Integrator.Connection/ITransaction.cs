using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Connection
{
    public interface ITransaction
    {
        void Commit();

        void Rollback();
    }
}
