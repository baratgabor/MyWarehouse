using MyWarehouse.Infrastructure.Authentication.External.Model;
using System;
using System.Collections.Generic;

namespace MyWarehouse.Infrastructure.Authentication.External.Exceptions
{
    public class ExternalAuthenticationInfoException : Exception
    {
        public readonly ExternalUserData ReceivedData;
        public readonly IEnumerable<string> MissingFields;

        public ExternalAuthenticationInfoException(IEnumerable<string> missingFields = null, ExternalUserData receivedData = null)
            : base("External authentication yielded insufficient information to allow local login.")
        {
            ReceivedData = receivedData;
            MissingFields = missingFields;
        }
    }
}
