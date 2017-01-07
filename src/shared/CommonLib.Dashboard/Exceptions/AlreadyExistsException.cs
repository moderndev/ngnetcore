using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLib.Dashboard.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException()
        { }

        public AlreadyExistsException(string message) : base(message)
        { }

        public AlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}
