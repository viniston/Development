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
        [TestMethod]
        public void GetStations()
        {
            // Arrange
            Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
            IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);

            // Act
            IList<WindStationsDao> result = developmentManager.CommonManager.GetStations();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1215, result.Count());
            Assert.AreEqual("Andaman and Nicobar Islands", result.ElementAt(0));
            Assert.AreEqual("Andhra Pradesh", result.ElementAt(1));
        }

        [TestMethod]
        public void GetHistoricalData()
        {
            // Arrange
            Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
            IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);

            // Act
            IList<WindSpeedDao> result = developmentManager.CommonManager.GetHistoricalData(10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void CreateNewReading()
        {
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
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result);
        }

    }
}
