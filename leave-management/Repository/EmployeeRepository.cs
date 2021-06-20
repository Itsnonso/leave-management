using leave_management.Contracts;
using leave_management.Data;
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
        public bool Create(Employee entity)
        {
            _db.Employees.Add(entity);
            
            return Save();
        }

        public bool Delete(Employee entity)
        {
            _db.Employees.Remove(entity);

            return Save();
        }

        public ICollection<Employee> FindAll()
        {
            var AllEmployees = _db.Employees.ToList();

            return AllEmployees;
        }

        public Employee FindById(int id)
        {
            var employee = _db.Employees.Find(id);

            return employee;
        }

        public bool isExists(int id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var changes = _db.SaveChanges();

            return changes > 0;
        }

        public bool Update(Employee entity)
        {
            _db.Employees.Update(entity);

            return Save();
        }
    }
}
