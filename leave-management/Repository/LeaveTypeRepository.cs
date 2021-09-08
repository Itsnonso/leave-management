using leave_management.Contracts;
using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(LeaveType entity)
        {
            await _db.LeaveTypes.AddAsync(entity);
            return await Save ();
        }

        public async Task<bool> Delete(LeaveType entity)
        {
            _db.LeaveTypes.Remove(entity);
            return await Save();
        }

        public  async Task <ICollection<LeaveType>> FindAll()
        {
            var leavetype =await _db.LeaveTypes.ToListAsync();
            return leavetype;
        }

        public async Task <LeaveType> FindById(int id)
        {
            var leavetype = await _db.LeaveTypes.FindAsync(id);
            return leavetype;
        }

        public ICollection<LeaveType> GetEmployeesbyLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public async Task <bool> isExists(int id)
        {
            var leavetype =await _db.LeaveTypes.AnyAsync(x => x.ID == id);
            return leavetype;

        }

        public async Task <bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveType entity)
        {
             _db.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
