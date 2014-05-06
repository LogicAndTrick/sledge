using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Packages
{
    public class PackageException : Exception
    {
        public PackageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PackageException()
        {
        }

        public PackageException(string message) : base(message)
        {
        }
    }
}
