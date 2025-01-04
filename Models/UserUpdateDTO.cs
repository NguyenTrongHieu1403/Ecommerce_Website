namespace ecommerce_final.Models
{
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string? Sex { get; set; }
        public DateOnly? BirthOfDate { get; set; }
        public string? Avatar { get; set; }
        public bool Active { get; set; }
        public int RoleId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Telephone { get; set; }
        public bool IsDelete { get; set; }
    }


}
