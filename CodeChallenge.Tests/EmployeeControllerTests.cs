
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        //Noticed missing test for not found employee
        [TestMethod]
        public void GetEmployeeById_Returns_NotFound()
        {
            // Arrange
            var employeeId = "Invalid_Id";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        }

        [TestMethod]
        public void GetEmployeeById_EmptyString_Returns_MethodNotAllowed()
        {
            // Arrange
            string employeeId = null;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);

        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetEmployeeReports_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reports/{employee.EmployeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [TestMethod]
        public void GetEmployeeReports_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedReports = 4;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reports/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reports = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, reports.Employee.FirstName);
            Assert.AreEqual(expectedLastName, reports.Employee.LastName);
            Assert.AreEqual(expectedReports, reports.NumberOfReports);
        }

        [TestMethod]
        public void CreateEmployeeCompensation_Returns_Ok()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 1234.56M,
                EffectiveDate = new DateTime(2025, 1, 1)
            };

            // Execute
            var requestContent = new JsonSerialization().ToJson(compensation);
            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(compensation.EmployeeId, compensations[0].EmployeeId);
            Assert.AreEqual(compensation.Salary, compensations[0].Salary);
            Assert.AreEqual(compensation.EffectiveDate, compensations[0].EffectiveDate);
        }

        [TestMethod]
        public void CreateEmployeeCompensation_Returns_NotFound()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "Invalid_Id",
                Salary = 1234.56M,
                EffectiveDate = new DateTime(2025, 1, 1)
            };

            // Execute
            var requestContent = new JsonSerialization().ToJson(compensation);
            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateEmployeeCompensation_Update_Returns_Ok()
        {
            // Arrange
            var compensation1 = new Compensation()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 1234.56M,
                EffectiveDate = new DateTime(2025, 1, 1)
            };
            var compensation2 = new Compensation()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 3456.78M,
                EffectiveDate = new DateTime(2025, 1, 1)
            };
            //Create initial compensation
            var requestContent = new JsonSerialization().ToJson(compensation1);
            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));

            // Execute
            //Update new compensation
            requestContent = new JsonSerialization().ToJson(compensation2);
            postRequestTask = _httpClient.PostAsync($"api/employee/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(1, compensations.Count);
            Assert.AreEqual(compensation2.EmployeeId, compensations[0].EmployeeId);
            Assert.AreEqual(compensation2.Salary, compensations[0].Salary);
            Assert.AreEqual(compensation2.EffectiveDate, compensations[0].EffectiveDate);
        }


        [TestMethod]
        public void GetEmployeeCompensation_Returns_Ok()
        {
            // Arrange
            var compensation = new Compensation()
            {
            EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
            Salary = 1234.56M,
            EffectiveDate = new DateTime(2025, 1, 1)
            };

            //use CreateEmployeeCompensation to generate record.
            //haven't worked with seeded data like this, theres probably a way to
            //generate a fake record without relying on another method, unsure how.
            var requestContent = new JsonSerialization().ToJson(compensation);
            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{compensation.EmployeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreNotEqual(0, compensations.Count);
            Assert.AreEqual(compensation.EmployeeId, compensations[0].EmployeeId);
            Assert.AreEqual(compensation.Salary, compensations[0].Salary);
            Assert.AreEqual(compensation.EffectiveDate, compensations[0].EffectiveDate);
        }

        [TestMethod]
        public void GetEmployeeCompensation_Returns_NotFound()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "Invalid_Id",
                Salary = 1234.56M,
                EffectiveDate = new DateTime(2025, 1, 1)
            };

            //use CreateEmployeeCompensation to generate record.
            //haven't worked with seeded data like this, theres probably a way to
            //generate a fake record without relying on another method, unsure how.
            var requestContent = new JsonSerialization().ToJson(compensation);
            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation/",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{compensation.EmployeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
