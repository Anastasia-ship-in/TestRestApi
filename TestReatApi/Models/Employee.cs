namespace TestReatApi
{
	public class Employee
	{
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? ManagerId { get; set; }

        public bool Enable { get; set; }
    }
}