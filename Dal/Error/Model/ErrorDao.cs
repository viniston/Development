using System;
using System.Collections;
using System.Collections.Generic;

namespace Development.Dal.Error.Model
{

    /// <summary>
    /// UserDao object for table 'Error'.
    /// </summary>
    public partial class ErrorDao : BaseDao
    {
        public virtual int Id { get; set; }
        public virtual string Message { get; set; }
        public virtual string StackTrace { get; set; }
        public virtual DateTime DateCreated { get; set; }

    }

}
