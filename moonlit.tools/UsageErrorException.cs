using System;
using System.Runtime.Serialization;

namespace Moonlit.Tools
{
    [Serializable]
    public class UsageErrorException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UsageErrorException()
        {
        }

        public UsageErrorException(string message) : base(message)
        {
        }

        public UsageErrorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UsageErrorException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}