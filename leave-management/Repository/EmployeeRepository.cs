using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _db;

        public EmployeeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Employee entity)
        {
           await _db.Employees.AddAsync(entity);
            
            return await Save();
        }

        public async Task<bool> Delete(Employee entity)
        {
            _db.Employees.Remove(entity);

            return await Save();
        }

        public async Task< ICollection<Employee>> FindAll()
        {
            var AllEmployees = await _db.Employees.ToListAsync();

            return AllEmployees;
        }

        public async Task<Employee> FindById(int id)
        {
            var employee = await _db.Employees.FindAsync(id);

            return employee;
        }

        public async Task<ICollection<Employee>> GetNewUserRegistration()
        {
            var AllNewEmployees = await _db.Employees.Where(q => q.DateJoined <= DateTime.Now && q.DateJoined >= DateTime.Now.AddDays(-1)).ToListAsync();

            return AllNewEmployees;
        }

        public async Task<bool> isExists(int id)
        {
            bool isExists = false;
            var employee = await _db.Employees.FindAsync(id);

            if (employee != null)
            {
                isExists = true;
            }
            return isExists;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> Update(Employee entity)
        {
            _db.Employees.Update(entity);

            return await Save();
        }
    }
}
