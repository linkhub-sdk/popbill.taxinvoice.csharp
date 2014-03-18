using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkhub
{
    public class LinkhubException : ApplicationException
    {
        public LinkhubException(long Code, String Message) : base(Message)
        {
            this.code = Code;
        }

        private long code;

        public long Code
        {
            get { return code; }
            set { code = value; }
        }
     
    }
}
