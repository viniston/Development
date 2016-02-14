using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Development.Web.Controllers;
using System;


namespace Development.Web.Controllers
{
    public class AuthorizationController : ApiController
    {
        ServiceResponse result;
      
        [Route("api/Authorization/GeneratingCaptchaCookie")]
        [HttpPost]
        public ServiceResponse GeneratingCaptchaCookie(CaptchaModel jobj)
        {
            result = new ServiceResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    result.StatusCode = (int)HttpStatusCode.OK;
                    GenerateCaptchaCookie(jobj.Location);
                    result.Response = true;
                }
            }
            catch
            {
                result.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                result.Response = 0;
            }
            return result;
        }

        public bool GenerateCaptchaCookie(string cookieName)
        {
            try
            {
                Random random = new Random();
                string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                StringBuilder captcha = new StringBuilder();
                for (int i = 0; i < 6; i++)
                    captcha.Append(combination[random.Next(combination.Length)]);
                var csrfCookie = new HttpCookie(cookieName, captcha.ToString())
                {
                    HttpOnly = true,
                    Path = "/",
                    Domain = null,
                    Expires = DateTime.Now.AddDays(1)
                };
                HttpContext.Current.Response.Cookies.Add(csrfCookie);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public class CaptchaModel
        {
            [Required(ErrorMessage = "Provide Captcha place")]
            [StringLength(5)]
            public string Location { get; set; }
        }
      
    }


}