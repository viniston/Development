using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Development.Dal.Base
{
  public  class MultiProperty
    {
        
            string _propertyName;
            object _propertyValue;

            public string propertyName
            {
                get
                {
                    return _propertyName;
                }
                set
                {
                    _propertyName = value;
                }
            }
            public object propertyValue
            {
                get
                {
                    return _propertyValue;
                }
                set
                {
                    _propertyValue = value;
                }
            }
    }
}
