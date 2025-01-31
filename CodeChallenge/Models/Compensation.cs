using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        //Decided to seperate into its own "table" because you can always join by employeeId and with it tacked on the end of Employees,
        //it was pulling the direct reports compensations as well and the list was just messy.
        
        public string EmployeeId { get; set; }
        

        public decimal Salary { get; set; }
        

        public DateTime EffectiveDate { get; set; }
    }
}
