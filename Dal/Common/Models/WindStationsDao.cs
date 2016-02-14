using System;
using System.Collections.Generic;

namespace Development.Dal.Common.Model
{

    /// <summary>
    /// AttributeDao object for table 'MM_Attribute'.
    /// </summary>

    public partial class WindStationsDao : BaseDao, ICloneable
    {

        #region Public Properties

        public virtual int Id
        {
            get;
            set;

        }

        public virtual string StateName
        {
            get;
            set;

        }

        public virtual string CityName
        {
            get;
            set;

        }

        public virtual string StationID
        {
            get;
            set;

        }

        public virtual int PredictedSpeed
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
