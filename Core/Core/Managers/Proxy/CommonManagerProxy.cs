using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Core.Interface.Managers;
using Development.Core.Interface;
using System.Xml.Linq;
using System.IO;
using System.Collections;
using Newtonsoft.Json.Linq;
using Development.Core.Metadata;
using Development.Dal.Error.Model;
using Development.Dal.Common.Model;

namespace Development.Core.Managers.Proxy
{
    internal partial class CommonManagerProxy : ICommonManager, IManagerProxy
    {
        // Reference to the DevelopmentManager
        private DevelopmentManager _DevelopmentManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonManagerProxy"/> class.
        /// </summary>
        /// <param name="DevelopmentManager">The Development manager.</param>
        internal CommonManagerProxy(DevelopmentManager DevelopmentManager)
        {
            _DevelopmentManager = DevelopmentManager;

            // Do some initialization.... 
            // i.e. cache logged in user specific things (or maybe use lazy loading for that)
        }

        // Reference to the DevelopmentManager (only internal)
        /// <summary>
        /// Gets the Development manager.
        /// </summary>
        /// <value>
        /// The Development manager.
        /// </value>
        internal DevelopmentManager DevelopmentManager
        {
            get { return _DevelopmentManager; }
        }

        #region Test

        #region TestMethod

        public string TestMethod()
        {
            return CommonManager.Instance.TestMethod(this);
        }

        #endregion

        #region SaveError

        public bool SaveError(ErrorDao _error)
        {
            return CommonManager.Instance.SaveError(this, _error);
        }

        #endregion


        #region SaveStations
        public bool SaveStations()
        {
            return CommonManager.Instance.SaveStations(this);
        }
        #endregion

        #region CreateNewReading
        public int CreateNewReading(WindSpeedDao speedDao)
        {
            return CommonManager.Instance.CreateNewReading(this, speedDao);
        }
        #endregion

        #region GetStations
        public IList<WindStationsDao> GetStations()
        {
            return CommonManager.Instance.GetStations(this);
        }
        #endregion

        #region GetHistoricalData
        public IList<WindSpeedDao> GetHistoricalData(int days)
        {
            return CommonManager.Instance.GetHistoricalData(this, days);
        }
        #endregion

        #region GetPredictedSpeed

        public int GetPredictedSpeed(string stationCode)
        {
            return CommonManager.Instance.GetPredictedSpeed(this, stationCode);
        }

        #endregion

        #endregion


    }

}
