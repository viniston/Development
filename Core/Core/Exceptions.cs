using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development.Core
{
    public class DevelopmentAccessDeniedException : Exception
    {
        public DevelopmentAccessDeniedException()
        {
        }

        public DevelopmentAccessDeniedException(string message)
            : base(message)
        {
        }

        public DevelopmentAccessDeniedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
