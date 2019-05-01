/*********************************************************
* 
*  Authors:        Adam Sobieski
*
*********************************************************/

using System;

namespace Logic.Prolog.Xsb.Exceptions
{
    [Serializable]
    public class XsbException : Exception
    {
        public XsbException(string message)
            : base(message) { }

        public XsbException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}