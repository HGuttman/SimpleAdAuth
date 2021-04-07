using System;
using System.Data.SqlClient;

namespace SimpleAdsAuth.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
    public class Ad
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
    }
   
}
