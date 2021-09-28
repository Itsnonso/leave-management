using leave_management.Contracts;
using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Utilities
{
    public class ErrorLogger : IErrorLogger
    {
        private readonly ApplicationDbContext _db;
        public ErrorLog logError(Exception exMessage)
        {
            var error = new ErrorLog
            {
                ErrorMessage = exMessage.Message,
                ErrorSource = exMessage.Source,
                ErrorStackTrace = exMessage.StackTrace,
                ErrorDate = DateTime.Now
            };

            return error;
        }
        public async void LogError(ErrorLog error)
        {
            await _db.ErrorLogs.AddAsync(error);
            await _db.SaveChangesAsync();
        }

    }
}
