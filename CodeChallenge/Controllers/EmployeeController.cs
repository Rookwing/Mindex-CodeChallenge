using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("reports/{id}", Name = "getEmployeeReports")]
        public IActionResult GetEmployeeReports(string id)
        {
            _logger.LogDebug($"Received employee direct report get request for '{id}'");

            //get employee by Id, return not found if empty
            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound();

            //Get reporting structure from service
            var reportingStructure = _employeeService.GetEmployeeReports(employee);

            return Ok(reportingStructure);
        }

        [HttpGet("compensation/{id}", Name = "getEmployeeCompensation")]
        public IActionResult GetEmployeeCompensation(string id)
        {
            _logger.LogDebug($"Received employee compensation get request for '{id}'");

            //get employee by Id, return not found if empty
            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound();

            //Get compensation from service
            List<Compensation> compensation = _employeeService.GetCompensation(employee);

            return Ok(compensation);
        }

        [HttpPost("compensation")]
        public IActionResult CreateEmployeeCompensation([FromBody]Compensation compensation)
        {
            _logger.LogDebug($"Received employee compensation create request for '{compensation.EmployeeId}'");

            //check for employee, return not found if empty
            var employee = _employeeService.GetById(compensation.EmployeeId);
            if (employee == null)
                return NotFound();

            //Create compensation with service
            _employeeService.CreateCompensation(compensation);

            //Return employee with new compensation added
            return GetEmployeeCompensation(compensation.EmployeeId);
        }
    }
}
