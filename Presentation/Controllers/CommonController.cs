using Development.Core;
using Development.Core.Interface;
using Development.Web.Controllers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Development.Dal.Common;
using System.IO;
using Development.Dal.Common.Model;
using Development.Web.Models;

namespace Development.Web.Controllers
{

    [RoutePrefix("api/Common")]
    public class CommonController : ApiController
    {

        ServiceResponse result;


        [AllowAnonymous]
        [HttpGet]
        [Route("GetCurrentTime/{id}")]
        public ServiceResponse GetCurrentTime(int id)
        {
            result = new ServiceResponse();
            try
            {
                result.StatusCode = (int)HttpStatusCode.OK;
                result.Response = DateTime.Now;
            }
            catch
            {
                result.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                result.Response = 0;
            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CreateBulkStations")]
        public ServiceResponse CreateBulkStations()
        {
            result = new ServiceResponse();
            try
            {
                Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
                IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);
                result.StatusCode = (int)HttpStatusCode.OK;
                result.Response = developmentManager.CommonManager.SaveStations();
            }
            catch
            {
                result.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                result.Response = 0;
            }
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetStations")]
        public ServiceResponse GetStations()
        {
            result = new ServiceResponse();
            try
            {
                Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
                IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);
                result.StatusCode = (int)HttpStatusCode.OK;
                result.Response = developmentManager.CommonManager.GetStations();
            }
            catch
            {
                result.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                result.Response = 0;
            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetPredictedSpeed")]
        public ServiceResponse GetPredictedSpeed([FromBody]JObject jobj)
        {
            result = new ServiceResponse();
            try
            {
                Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
                IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);
                result.StatusCode = (int)HttpStatusCode.OK;
                result.Response = developmentManager.CommonManager.GetPredictedSpeed((string)jobj["StationId"]);
            }
            catch
            {
                result.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                result.Response = 0;
            }
            return result;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("GetHistoricalData/{days}")]
        public ServiceResponse GetHistoricalData(int days)
        {
            result = new ServiceResponse();
            try
            {
                Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
                IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);
                result.StatusCode = (int)HttpStatusCode.OK;
                result.Response = developmentManager.CommonManager.GetHistoricalData(days);
            }
            catch
            {
                result.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                result.Response = 0;
            }
            return result;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("CreateNewReading")]
        public ServiceResponse CreateNewReading(Reading model)
        {
            result = new ServiceResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (HttpRequestMessageExtensions.GetCookie(Request, "Wnreg") == model.Captcha) // validate the CAPTCHA
                    {

                        WindSpeedDao speedDao = new WindSpeedDao
                        {
                            City = model.City,
                            State = model.State,
                            StationCode = model.StationCode,
                            ActualSpeed = model.ActualSpeed,
                            PredictedSpeed = model.PredictedSpeed,
                            Date = model.ReadingDate,
                            Variance = model.Variance
                        };
                        Guid systemSession = DevelopmentManagerFactory.GetSystemSession();
                        IDevelopmentManager developmentManager = DevelopmentManagerFactory.GetDevelopmentManager(systemSession);
                        result.StatusCode = (int)HttpStatusCode.OK;
                        result.Response = developmentManager.CommonManager.CreateNewReading(speedDao);

                    }
                    else
                    {
                        result.StatusCode = (int)HttpStatusCode.Forbidden;
                        result.Response = 0;
                    }
                }
                else
                {
                    result.StatusCode = (int)HttpStatusCode.BadRequest;
                    result.Response = 0;
                }
            }
            catch
            {
                result.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                result.Response = 0;
            }
            return result;
        }

    }
}