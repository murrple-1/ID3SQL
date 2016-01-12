using System;
using System.Runtime.Serialization;

namespace ID3SQL
{
    public class ID3SQLException : Exception
    {
        public ID3SQLException() :
            base()
        {

        }

        public ID3SQLException(string message) :
            base(message)
        {

        }

        public ID3SQLException(string message, Exception innerException) :
            base(message, innerException)
        {

        }

        public ID3SQLException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {

        }
    }
}
