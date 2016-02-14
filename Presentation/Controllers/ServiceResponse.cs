using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Development.Web.Controllers
{
    public class ServiceResponse
    {
        private int IntCode;
        public int StatusCode
        {
            get { return IntCode; }
            set { IntCode = value; }
        }

        private dynamic DynResponse;

        public dynamic Response
        {
            get { return DynResponse; }
            set { DynResponse = value; }
        }

    }
}