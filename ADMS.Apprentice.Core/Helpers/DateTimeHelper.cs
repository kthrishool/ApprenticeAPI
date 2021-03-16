using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Helpers
{
    [RegisterWithIocContainer]
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
