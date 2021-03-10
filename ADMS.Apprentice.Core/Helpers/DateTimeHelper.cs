using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ADMS.Apprentice.Core.Helpers
{
    public interface IDateTimeHelper
    {
        DateTime GetDateTimeNow();
    }

    public class DateTimeHelper : IDateTimeHelper
    {
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

    }
}
