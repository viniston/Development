using System;
using System.Collections.Generic;

namespace Development.Dal.Common.Model
{

    /// <summary>
    /// AttributeDao object for table 'MM_Attribute'.
    /// </summary>

    public partial class WindSpeedDao : BaseDao, ICloneable
    {

        #region Public Properties

        public virtual int Id
        {
            get;
            set;

        }

        public virtual string City
        {
            get;
            set;

        }

        public virtual string State
        {
            get;
            set;

        }

        public virtual string StationCode
        {
            get;
            set;

        }

        public virtual DateTime Date
        {
            get;
            set;

        }


        public virtual int PredictedSpeed
        {
            get;
            set;

        }

        public virtual int ActualSpeed
        {
            get;
            set;
        }

        public virtual int Variance
        {
            get;
            set;
        }


        public virtual string DesiredDate
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
