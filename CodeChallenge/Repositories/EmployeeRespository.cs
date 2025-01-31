using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Compensation CreateCompensation(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation UpdateCompensation(Compensation compensation)
        {
            //grab matching compensation and update salary
            var result = _employeeContext.Compensations.SingleOrDefault(e => e.EmployeeId == compensation.EmployeeId && e.EffectiveDate == compensation.EffectiveDate);
            result.Salary = compensation.Salary;
            return result;
        }

        public List<Compensation> GetCompensation(Employee employee)
        {
            List<Compensation> compensations = _employeeContext.Compensations.Where(e => e.EmployeeId == employee.EmployeeId).ToList();
            return compensations;
        }

        public Employee GetById(string id)
        {
            //had to enumerate employees due to weird bug not allowing direct hires to populate correctly.
            //this fixed the issue. otherwise they just came through as null unless I debugged and enumerated the results.
            List<Employee> employees = _employeeContext.Employees.ToList();
            Employee employee = employees.SingleOrDefault(e => e.EmployeeId == id);
            return employee;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
