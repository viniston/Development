using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Development.Core.Common.Interface;

namespace Development.Core.Common
{
    class WindSpeed : IWindSpeed
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public string StationCode { get; set; }
        public int PredictedSpeed { get; set; }
        public int ActualSpeed { get; set; }
        public int Variance { get; set; }
    }
}
