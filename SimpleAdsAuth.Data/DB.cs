using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SimpleAdsAuth.Data
{
    public class DB
    {
        private readonly string _connectionString;
        public DB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads(UserId, PhoneNumber, Description, Date) VALUES(@userId, @phoneNumber, @description, GETDATE()) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@desription", ad.Description);
            connection.Open();
            ad.Id = (int)(decimal)cmd.ExecuteScalar();
        }
        public List<Ad> GetAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT a.*, u.id FROM ads a
                                JOIN users u 
                                ON a.UserId = u.Id
                                ORDER BY a.Date desc";
            connection.Open();
            List<Ad> Ads = new List<Ad>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Ads.Add(new Ad
                {
                    Id = (int)reader["adId"],
                    UserId = (int)reader["userId"],
                    UserName = (string)reader["userName"],
                    PhoneNumber = (string)reader["phoneNumber"],
                    Description = (string)reader["description"],
                    Date = (DateTime)reader["date"]
                });
            }
            return Ads;
        }
        public List<Ad> GetAdsForUser(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT a.*, u.Name FROM Ads a " +
                                  "JOIN Users u on a.UserId = u.Id " +
                                  "WHERE a.UserId = @userId " +
                                  "ORDER BY a.Date DESC";
            command.Parameters.AddWithValue("@userid", userId);
            connection.Open();
            List<Ad> ads = new List<Ad>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["adId"],
                    UserId = (int)reader["userId"],
                    UserName = (string)reader["userName"],
                    PhoneNumber = (string)reader["phoneNumber"],
                    Description = (string)reader["description"],
                    Date = (DateTime)reader["date"]
                });
            }

            return ads;
        }

        public int GetUserIdForAd(int adId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserId FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", adId);
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }
        public void DeleteAd(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "delete from ads where id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #region UserDB
        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                              "VALUES (@name, @email, @passwordHash)";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;
        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
        public bool IsEmailAvailable(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            int count = (int)cmd.ExecuteScalar();
            return count == 0;
        }
        #endregion

        //public List<Ad> MyAccount(int id)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    using (SqlCommand cmd = connection.CreateCommand())
        //    {
        //        cmd.CommandText = @"select *, a.id as 'adId' from users u
        //                            join ads a
        //                            on a.UserId = u.Id
        //                            where u.id = @id
        //                            order by a.Date desc, a.id desc";
        //        cmd.Parameters.AddWithValue("@id", id);
        //        connection.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        List<Ad> ads = new List<Ad>();
        //        while (reader.Read())
        //        {
        //            ads.Add(new Ad
        //            {
        //                Id = (int)reader["adId"],
        //                UserId = (int)reader["userId"],
        //                UserName = (string)reader["name"],
        //                PhoneNumber = (string)reader["phoneNumber"],
        //                Description = (string)reader["details"],
        //                Date = (DateTime)reader["date"]
        //            });
        //        }
        //        return ads;
        //    }
        //}
    }
}
