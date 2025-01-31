using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetEmployeeReports(Employee employee)
        {
            //define a reporting structure to be referenced by the loop
            ReportingStructure reportingStructure = new ReportingStructure();
            
            //employee shouldn't be null, but check to be sure
            if (employee != null)
            {
                //assign the original employee to the reporting structure
                reportingStructure.Employee = employee;
                //recursively go through direct reports, referencing the reporting structure
                ReportRecursion(employee, ref reportingStructure);
                return reportingStructure;
            }

            return null;
        }

        private static void ReportRecursion(Employee employee, ref ReportingStructure reports)
        {
            if(employee != null && employee.DirectReports != null)
            {
                //recursively go through direct reports, adding to the count in the reference
                foreach(Employee e in employee.DirectReports)
                {
                    reports.NumberOfReports++;
                    ReportRecursion(e, ref reports);
                }
            }
        }

        public List<Compensation> GetCompensation(Employee employee)
        {
            if (employee != null)
            {
                return _employeeRepository.GetCompensation(employee);
            }

            return null;
        }

        public Compensation CreateCompensation(Compensation compensation)
        {
            var compensations = _employeeRepository.GetCompensation(GetById(compensation.EmployeeId));

            //added ability to update salary for an existing Id + effective date because it just seemed nice
            foreach(Compensation c in compensations)
            {
                //check Id + effective date for existing compensations
                if (c.EmployeeId == compensation.EmployeeId && c.EffectiveDate == compensation.EffectiveDate)
                {
                    _employeeRepository.UpdateCompensation(compensation);
                    _employeeRepository.SaveAsync().Wait();
                    return compensation;
                }
            }

             _employeeRepository.CreateCompensation(compensation);
             _employeeRepository.SaveAsync().Wait();

            return compensation;
        }
    }
}
