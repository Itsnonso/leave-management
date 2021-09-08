using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task <bool> CheckAllocation(int leavetypeid, string employeeid)
        { 
            var period = DateTime.Now.Year;
            var checkAllocation = await FindAll();
            return checkAllocation.Where(q => q.EmployeeId == employeeid && q.LeaveTypeId == leavetypeid && q.Period == period).Any();
        }

        public async Task <bool> Create(LeaveAllocation entity)
        {
            await _db.LeaveAllocations.AddAsync(entity);

            return  await Save();
        }

        public async Task< bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);

            return await Save();
        }

        public async Task <ICollection<LeaveAllocation>> FindAll()
        {
            var leaveallocations = await _db.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .ToListAsync();

            return leaveallocations;
        }

        public LeaveAllocation FindById(int id)
        {
            var leaveallocation = _db.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefault(q => q.ID == id);

            return leaveallocation;
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationbyEmployee(string id)
        {
            var period = DateTime.Now.Year;
            var GetLeaveAllocation = await FindAll();
            return GetLeaveAllocation.Where(q => q.EmployeeId == id && q.Period == period).ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationbyEmployeeandType(string employeeid, int leavetypeid)
        {
            var period = DateTime.Now.Year;
            var GetLeaveAllocation = await FindAll();
            return GetLeaveAllocation.FirstOrDefault(q => q.EmployeeId == employeeid && q.Period == period && q.LeaveTypeId == leavetypeid);
        }

        public async Task<bool> isExists(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);

            return await Save();
        }

        Task<LeaveAllocation> IRepositoryBase<LeaveAllocation>.FindById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
