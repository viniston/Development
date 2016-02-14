using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Core.Interface;
using Development.Core.Managers.Proxy;
using Newtonsoft.Json.Linq;
using Development.Dal.Base;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Web;
using System.Configuration;
using System.Collections;
using System.Threading.Tasks;
using Development.Core.Metadata;
using System.Xml.Serialization;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Development.Core.User.Interface;
using Development.Dal.Error.Model;
using System.Web.Script.Serialization;
using Development.Dal.Common;
using Development.Dal.Common.Model;

namespace Development.Core.Managers
{
    internal partial class CommonManager : IManager
    {

        /// <summary>
        /// The instance
        /// </summary>
        private static CommonManager instance = new CommonManager();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        internal static CommonManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Initializes the specified Development manager.
        /// </summary>
        /// <param name="DevelopmentManager">The Development manager.</param>
        void IManager.Initialize(IDevelopmentManager DevelopmentManager)
        {
            // Cache and initialize things here...
        }

        /// <summary>
        /// Commit all caches since the transaction has been commited.
        /// </summary>
        void IManager.CommitCaches()
        {
        }

        /// <summary>
        /// Rollback all caches since the transaction has been rollbacked.
        /// </summary>
        void IManager.RollbackCaches()
        {
        }

        #region TestMethod
        public string TestMethod(CommonManagerProxy proxy)
        {
            string response = "";
            try
            {

                using (ITransaction tx = proxy.DevelopmentManager.GetTransaction())
                {
                    //response = tx.PersistenceManager.UserRepository.ExecuteQuery("Query string"));
                    tx.Commit();
                }

                return response;

            }

            catch (DBConcurrencyException ex)
            {
                return response;
            }
            catch (Exception ex)
            {
                return response;
            }

        }

        #endregion

        #region SaveError
        public bool SaveError(CommonManagerProxy proxy, ErrorDao _error)
        {
            try
            {
                using (ITransaction tx = proxy.DevelopmentManager.GetTransaction())
                {
                    tx.PersistenceManager.UserRepository.Save<ErrorDao>(_error);
                    tx.Commit();
                }

                return true;
            }

            catch (DBConcurrencyException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion

        #region LogError
        public bool LogError(CommonManagerProxy proxy, Exception ex)
        {
            try
            {
                ErrorDao errDao = new ErrorDao();
                errDao.DateCreated = DateTime.Now;
                errDao.StackTrace = ex.StackTrace;
                errDao.Message = ex.Message.ToString();
                SaveError(proxy, errDao);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region GetStations
        public IList<WindStationsDao> GetStations(CommonManagerProxy proxy)
        {
            IList<WindStationsDao> iwind = new List<WindStationsDao>();
            try
            {
                using (ITransaction tx = proxy.DevelopmentManager.GetTransaction())
                {
                    iwind = tx.PersistenceManager.UserRepository.Query<WindStationsDao>().OrderBy(c => c.StateName.ToLower()).ToList<WindStationsDao>();
                    tx.Commit();
                }

                return iwind;
            }

            catch (DBConcurrencyException ex)
            {
                return iwind;
            }
            catch (Exception ex)
            {
                LogError(proxy, ex);
                return iwind;
            }

        }

        #endregion

        #region GetPredictedSpeed
        public int GetPredictedSpeed(CommonManagerProxy proxy, string stationCode)
        {
            int predictedSpeed = 0;
            try
            {
                using (ITransaction tx = proxy.DevelopmentManager.GetTransaction())
                {
                    predictedSpeed = Convert.ToInt32(tx.PersistenceManager.UserRepository.Query<WindStationsDao>().Where(a => a.StationID == stationCode).Select(a => a.PredictedSpeed).FirstOrDefault());
                    tx.Commit();
                }
            }

            catch (DBConcurrencyException ex)
            {
            }
            catch (Exception ex)
            {
                LogError(proxy, ex);
            }

            return predictedSpeed;

        }

        #endregion

        #region GetHistoricalData
        public IList<WindSpeedDao> GetHistoricalData(CommonManagerProxy proxy, int days)
        {
            IList<WindSpeedDao> iwind = new List<WindSpeedDao>();
            try
            {
                using (ITransaction tx = proxy.DevelopmentManager.GetTransaction())
                {

                    string dateInString = DateTime.Now.ToString("yyyy-MM-dd");
                    DateTime startDate = DateTime.Parse(dateInString);
                    DateTime expiryDate = startDate.AddDays(-days);
                    iwind = tx.PersistenceManager.UserRepository.Query<WindSpeedDao>().Where(a => a.Date >= expiryDate).OrderBy(c => c.Date).Select(a => new WindSpeedDao { Id = a.Id, State = a.State, StationCode = a.StationCode, City = a.City, Variance = a.Variance, ActualSpeed = a.ActualSpeed, Date = a.Date, PredictedSpeed = a.PredictedSpeed, DesiredDate = a.Date != null ? a.Date.ToString("MMMM dd,yyyy") : "" }).ToList<WindSpeedDao>();
                    tx.Commit();
                }

                return iwind;
            }

            catch (DBConcurrencyException ex)
            {
                return iwind;
            }
            catch (Exception ex)
            {
                LogError(proxy, ex);
                return iwind;
            }

        }



        #endregion

        #region SaveStations
        public bool SaveStations(CommonManagerProxy proxy)
        {
            try
            {
                using (ITransaction tx = proxy.DevelopmentManager.GetTransaction())
                {

                    string jsonInput = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "cities.json");
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    List<WindStationsDao> stations = js.Deserialize<List<WindStationsDao>>(jsonInput).ToList<WindStationsDao>();
                    IList<WindStationsDao> iwind = new List<WindStationsDao>();
                    foreach (var wind in stations)
                    {
                        iwind.Add(new WindStationsDao { Id = 0, StationID = wind.StationID, StateName = wind.StateName, CityName = wind.CityName });
                    }
                    tx.PersistenceManager.UserRepository.Save<WindStationsDao>(iwind);
                    tx.Commit();
                }

                return true;
            }

            catch (DBConcurrencyException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                LogError(proxy, ex);
                return false;
            }

        }

        #endregion

        #region CreateNewReading
        public int CreateNewReading(CommonManagerProxy proxy, WindSpeedDao speedDao)
        {
            try
            {
                using (ITransaction tx = proxy.DevelopmentManager.GetTransaction())
                {
                    tx.PersistenceManager.UserRepository.Save<WindSpeedDao>(speedDao);
                    tx.Commit();
                }

                return speedDao.Id;
            }

            catch (DBConcurrencyException ex)
            {
                return 0;
            }
            catch (Exception ex)
            {
                LogError(proxy, ex);
                return 0;
            }

        }

        #endregion


    }
}




