using System;
using System.Collections;
using System.Collections.Generic;

namespace Development.Core.User.Interface
{
    /// <summary>
    /// IUser interface for table 'UM_User'.
    /// </summary>
    public interface IUser
    {
        #region Public Properties

        int Id
        {
            get;
            set;

        }

        string FirstName
        {
            get;
            set;

        }

        string LastName
        {
            get;
            set;

        }

        string UserName
        {
            get;
            set;

        }

      

        #endregion
    }
}
