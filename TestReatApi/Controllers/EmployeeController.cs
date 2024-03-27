using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace TestReatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private string _connectionString;

        public EmployeesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpGet("GetEmployeeByID/{id}")]
        public EmployeeDto GetEmployeeById(int id)
        {
            var employees = new List<EmployeeDto>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = $"SELECT * FROM Employees WHERE ID = {id} OR ManagerID = {id}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var idRead = (int)reader["ID"];
                    int? managerId = reader["ManagerID"] == DBNull.Value ? null : (int?)reader["ManagerID"];

                    var employee = new EmployeeDto
                    {
                        Id = idRead,
                        Name = reader["Name"].ToString(),
                        ManagerId = managerId,
                        Enable = (bool)reader["Enable"],
                        EmployeeDtos= new List<EmployeeDto>()
                    };


                    employees.Add(employee);
                }
            }

            var employeeById = employees.FirstOrDefault(emp => emp.Id == id);
            employeeById?.EmployeeDtos.AddRange(employees.Where(e => (e.ManagerId ?? default) == id).ToList());
            

            return employeeById;
        }


        [HttpPut("EnableEmployee/{id}")]
        public IActionResult EnableEmployee(int id, bool enable)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "UPDATE Employees SET Enable = @Enable WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Enable", enable);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok($"Enable flag for employee with ID {id} updated successfully.");
                        }
                        else
                        {
                            return NotFound($"Employee with ID {id} not found.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }
}

