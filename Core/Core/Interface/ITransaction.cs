using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Dal.Base;

namespace Development.Core.Interface
{
    public interface ITransaction : IDisposable
    {
        bool IsOpen { get; }

        void Commit();
        void Rollback();

        PersistenceManager PersistenceManager { get; }
    }
}
