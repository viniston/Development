using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Development.Dal.Base
{
    public class DisposableClass : IDisposable
    {
        private XmlTextReader reader;
        private bool disposed = false; // to detect redundant calls

        public DisposableClass(string filename)
        {
            reader = new XmlTextReader(filename);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (reader != null)
                    {
                        ((IDisposable)reader).Dispose();
                        reader = null;
                    }
                }

                disposed = true;
            }
        }

        ~DisposableClass()
        {
            Dispose(false);
        }
    }
}
