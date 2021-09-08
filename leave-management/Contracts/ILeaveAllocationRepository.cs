using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        Task <bool> CheckAllocation(int leavetypeid, string employeeid);

        Task <ICollection<LeaveAllocation>> GetLeaveAllocationbyEmployee(string id);
       Task < LeaveAllocation> GetLeaveAllocationbyEmployeeandType(string employeeid, int leavetypeid);
    }
}
