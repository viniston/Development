using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Dal.Base;
using Development.Dal.Common.Model;
using NHibernate;

namespace Development.Dal.Common
{
    public class CommonRepository : GenericRepository
    {
        public CommonRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {

        }

    }
}
