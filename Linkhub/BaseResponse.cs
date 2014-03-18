using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkhub
{

    public class BaseResponse
    {
        public long code;
        public String message;

        public void throwException()
        {
            throw new LinkhubException(code, message);
        }
    }
}
