namespace TestReatApi
{
	public class EmployeeDto
	{
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? ManagerId { get; set; }

        public bool Enable { get; set; }

        public List<EmployeeDto> EmployeeDtos { get; set; }
    }
}

