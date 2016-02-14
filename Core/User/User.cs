using System;
using System.Collections;
using System.Collections.Generic;
using Development.Core.User.Interface;
using System.Security.Cryptography;

namespace Development.Core.User
{

    /// <summary>
    /// User object for table 'UM_User'.
    /// </summary>
    public class User : IUser, ICloneable
    {

        #region Public Properties

        public int Id
        {
            get;
            set;

        }

        public string FirstName
        {
            get;
            set;

        }

        public string LastName
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;

        }


        #endregion


        #region ICloneable methods

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

    }

}
