using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Development.Core;
using Development.Core.Interface;
using Development.Dal.Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Development.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {

        void InitializeAllDevelopmentInstances()
        {

            //intialize all Development tenants and NHibernate mappings and all
            DevelopmentManagerFactory.InitializeSystem();

        }


        [TestMethod]
        public void GetStations()
        {

            InitializeAllDevelopmentInstances(); // initialize the persistance layer and nhibernate engine 

            // Arrange
            Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
            IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);

            // Act
            IList<WindStationsDao> result = developmentManager.CommonManager.GetStations();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1215, result.Count()); //the total stations in the system is 1215
            Assert.AreEqual("Andaman and Nicobar Islands", result.ElementAt(0).StateName);  // among 1215 stations the first one is Andaman and Nicobar Islands as per the statename order
            Assert.AreEqual("Andhra Pradesh", result.ElementAt(1).StateName); // among 1215 stations the second state one is Andaman and Nicobar Islands as per the statename order
        }

        [TestMethod]
        public void GetHistoricalData()
        {

            InitializeAllDevelopmentInstances(); // initialize the persistance layer and nhibernate engine 

            // Arrange
            Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
            IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);

            // Act
            IList<WindSpeedDao> result = developmentManager.CommonManager.GetHistoricalData(10);  //getting last 10 days hitsorical data

            // Assert
            Assert.IsNotNull(result);  // since this test case is depend upon the db data

            Assert.AreEqual(1, result.Count()); //matching the count is 1 or not
        }

        [TestMethod]
        public void GetPredictedSpeed()
        {

            InitializeAllDevelopmentInstances(); // initialize the persistance layer and nhibernate engine 

            // Arrange
            Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
            IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);

            // Act
            int result = developmentManager.CommonManager.GetPredictedSpeed("MH-MU-01"); //getting predicted speed of stationid : MH-MU-01

            // Assert
            Assert.IsNotNull(result);

            Assert.AreEqual(12, result); //the current predicted speed in the system is 12
        }

        [TestMethod]
        public void CreateNewReading()
        {

            InitializeAllDevelopmentInstances(); // initialize the persistance layer and nhibernate engine . This is very very must to do.

            // Arrange
            Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
            IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);

            WindSpeedDao speedDao = new WindSpeedDao
            {
                City = "Bengaluru",
                State = "Karnataka",
                StationCode = "KA-BE-03",
                ActualSpeed = 6,
                PredictedSpeed = 12,
                Date = DateTime.Now,
                Variance = -6
            };

            // Act
            int result = developmentManager.CommonManager.CreateNewReading(speedDao);

            // Assert
            Assert.IsNotNull(result);   //test case for make sure that data save successfully

            Assert.AreNotEqual(0, result); // test case to make sure that new id returned or not
        }

    }
}
