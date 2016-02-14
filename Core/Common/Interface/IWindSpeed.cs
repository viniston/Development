using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development.Core.Common.Interface
{
    class IWindSpeed
    {
        int Id { get; set; }
        string State { get; set; }
        string City { get; set; }
        DateTime Date { get; set; }
        string StationCode { get; set; }
        int PredictedSpeed { get; set; }
        int ActualSpeed { get; set; }
        int Variance { get; set; }

    }
}
