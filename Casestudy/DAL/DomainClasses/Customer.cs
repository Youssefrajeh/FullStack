namespace Casestudy.DAL.DomainClasses
{
    public class Customer
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Hash { get; set; }
        public string? Salt { get; set; }
    }

}
