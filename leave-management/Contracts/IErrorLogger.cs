using leave_management.Data;
using leave_management.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface IErrorLogger
    {
        void LogError(ErrorLog error);
    }
}
