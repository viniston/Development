using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Collections;
using Newtonsoft.Json.Linq;
using Development.Core.Metadata;
using Development.Dal.Error.Model;
using Development.Dal.Common.Model;

namespace Development.Core.Interface.Managers
{
    public interface ICommonManager
    {

        #region Instance of Classes In ServiceLayer reference
        /// <summary>
        /// Returns File class.
        /// </summary>

        #endregion

        #region Methods

        string TestMethod();
        bool SaveError(ErrorDao _error);
        bool SaveStations();
        IList<WindStationsDao> GetStations();
        IList<WindSpeedDao> GetHistoricalData(int days);
        int CreateNewReading(WindSpeedDao speedDao);
        int GetPredictedSpeed(string stationCode);



        #endregion

    }
}
