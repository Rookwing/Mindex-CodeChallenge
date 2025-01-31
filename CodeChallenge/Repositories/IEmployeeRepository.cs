using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Task SaveAsync();
        List<Compensation> GetCompensation(Employee employee);
        Compensation CreateCompensation(Compensation compensation);
        Compensation UpdateCompensation(Compensation compensation);
    }
}