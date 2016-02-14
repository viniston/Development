using System;
using System.Collections;
using System.Collections.Generic;

namespace Development.Core.User.Interface
{
    /// <summary>
    /// IUser interface for table 'web_user'.
    /// </summary>
    public interface IWebUser
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

        byte[] Password
        {
            get;
            set;

        }

        string SaltPassword
        {
            get;
            set;

        }

        string Email
        {
            get;
            set;

        }

        string Image
        {
            get;
            set;

        }


        bool Gender
        {
            get;
            set;

        }




        string OldPassword
        {
            get;
            set;
        }

        int WebId
        {
            get;
            set;

        }

        DateTime RegistrationDate
        {
            get;
            set;

        }

        DateTime LastLogin
        {
            get;
            set;

        }
        bool IsPasswordReset
        {
            get;
            set;

        }

        bool IsActive
        {
            get;
            set;

        }

        int Currentwebid { get; set; }
        int UserType { get; set; }

        #endregion
    }
}
