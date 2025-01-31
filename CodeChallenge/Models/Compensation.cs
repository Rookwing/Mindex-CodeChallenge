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
        
        //Keys are EmployeeId + EffectiveDate because you can't have 2 different salaries with the same dates for the same employee
        [Key]
        public string EmployeeId { get; set; }
        

        public decimal Salary { get; set; }
        
        [Key]

        public DateTime EffectiveDate { get; set; }
    }
}
