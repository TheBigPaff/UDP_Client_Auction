using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace UDP_Client_Auction
{
    public class DB
    {
        private static string LoadConnectionString(string name = "MySQL")
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static User UserLogIn(string username, string password)
        {
            User user = null;

            string query = $"SELECT * FROM users WHERE username='{username}'";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if(reader.Read())
                {
                    user = new User();
                    user.Id = int.Parse(reader["id"].ToString());
                    user.Username = reader["username"].ToString();
                    user.Email = reader["email"].ToString();
                    user.Password = reader["password"].ToString();
                    user.ProPicPath = reader["propic_path"].ToString();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("DB error: " + ex.Message);
            }


            if (user != null)
            {
                /* Fetch the stored value */
                string savedPasswordHash = user.Password;
                /* Extract the bytes */
                byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                /* Get the salt */
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                /* Compute the hash on the password the user entered */
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
                byte[] hash = pbkdf2.GetBytes(20);
                /* Compare the results */
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        user = null; // unauthorized access
                    }
                }
            }
            return user;
        }

        public static User UserLogIn(string username)
        {
            User user = null;

            string query = $"SELECT * FROM users WHERE username='{username}'";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.Read())
                {
                    user = new User();
                    user.Id = int.Parse(reader["id"].ToString());
                    user.Username = reader["username"].ToString();
                    user.Email = reader["email"].ToString();
                    user.Password = reader["password"].ToString();
                    user.ProPicPath = reader["propic_path"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB error: " + ex.Message);
            }

            return user;
        }

        internal static List<Auction> GetItemsBought(User user)
        {
            List<Auction> auctions = new List<Auction>();
            string query = $"SELECT * FROM items WHERE bidder_id={user.Id}";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Auction auctionToAdd = new Auction();

                    auctionToAdd.Id = int.Parse(reader["id"].ToString());
                    auctionToAdd.Name = reader["name"].ToString();
                    auctionToAdd.Description = reader["description"].ToString();
                    auctionToAdd.ImgPath = reader["img_path"].ToString();
                    auctionToAdd.End = DateTime.Parse(reader["end"].ToString());
                    auctionToAdd.HighestBid = float.Parse(reader["highest_bid"].ToString());

                    auctions.Add(auctionToAdd);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB error: " + ex.Message);
            }

            return auctions;
        }

        internal static List<Auction> LoadAuctions()
        {
            List<Auction> auctions = new List<Auction>();

            string query = $"SELECT items.*, users.username, users.email, users.password, users.propic_path FROM items LEFT OUTER JOIN users ON items.bidder_id = users.id";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            dbConnection.Open();
            reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                Auction auctionToAdd = new Auction();

                auctionToAdd.Id = int.Parse(reader["id"].ToString());
                auctionToAdd.Name = reader["name"].ToString();
                auctionToAdd.Description = reader["description"].ToString();
                auctionToAdd.ImgPath = reader["img_path"].ToString();
                auctionToAdd.End = DateTime.Parse(reader["end"].ToString());
                if (reader["highest_bid"].ToString() != "")
                {
                    auctionToAdd.HighestBid = float.Parse(reader["highest_bid"].ToString());
                }
                else
                {
                    auctionToAdd.HighestBid = 0;
                }
                if (reader["bidder_id"].ToString() != "" && reader["bidder_id"] != null)
                {
                    auctionToAdd.CurrentUser.Id = int.Parse(reader["bidder_id"].ToString());
                    auctionToAdd.CurrentUser.Username = reader["username"].ToString();
                    auctionToAdd.CurrentUser.Email = reader["email"].ToString();
                    auctionToAdd.CurrentUser.Password = reader["password"].ToString();
                    auctionToAdd.CurrentUser.ProPicPath = reader["propic_path"].ToString();
                }
                else
                {
                    auctionToAdd.CurrentUser = null;
                }

                auctions.Add(auctionToAdd);
            }

            return auctions;
        }

        internal static bool TryGetUserEmail(string text, out string email)
        {
            throw new NotImplementedException();
        }

        public static bool RegisterUser(User user)
        {
            string query = $"INSERT users (username, email, password, propic_path) VALUES ('{user.Username}', '{user.Email}', '{user.Password}', '{user.ProPicPath}')";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB error: " + ex.Message);
                return false;
            }
        }

        internal static void DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        internal static void SaveResetTicket(int id, string tempPwd, DateTime expireDate)
        {
            string query = $"INSERT reset_tickets (token, expiration_date, token_used, user_id) VALUES ('{tempPwd}', '{expireDate.ToString("yyyy-MM-dd HH:mm:ss")}', '0' , '{id}')";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show("DB error: " + ex.Message);
            }
        }

        internal static void TicketWasUsed(int ticketId)
        {
            throw new NotImplementedException();
        }

        // very dumb you're doing this with the token and not the id, whatever man i'm tired
        internal static void SetTokenToUsed(string token)
        {
            string query = $"UPDATE reset_tickets SET token_used='1' WHERE token='{token}'";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
            }
        }

        internal static bool GetToken(string token, int userId)
        {
            bool authorized = false;
            string queryTokenId = $"SELECT * FROM reset_tickets WHERE token = '{token}' AND user_id = '{userId}' AND token_used = '0'";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(queryTokenId, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            dbConnection.Open();

            reader = dbCommand.ExecuteReader();

            if (reader.Read())
            {
                DateTime expirationDate = DateTime.Parse(reader["expiration_date"].ToString());
                if(expirationDate > DateTime.Now)
                {
                    authorized = true;
                }
            }

            return authorized;
        }

        internal static int GetUserId(string username)
        {
            int id = -1;
            string query = $"SELECT id FROM users WHERE lower(Username) = lower('{username}')";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            
            dbConnection.Open();
            reader = dbCommand.ExecuteReader();

            if (reader.Read())
            {
                id = int.Parse(reader["id"].ToString());
            }

            return id;
        }

        internal static bool DoesUsernameExist(string username)
        {
            string query = $"SELECT * FROM users WHERE lower(Username) = lower('{username}')";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;


            dbConnection.Open();
            reader = dbCommand.ExecuteReader();

            if (reader.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void UpdateUser(User user, bool updatePassword)
        {
            string query;

            if (updatePassword)
            {
                query = $"UPDATE users SET username='{user.Username}', email='{user.Email}', password='{user.Password}', propic_path='{user.ProPicPath}' WHERE id={user.Id}";
            }
            else
            {
                query = $"UPDATE users SET username='{user.Username}', email='{user.Email}', propic_path='{user.ProPicPath}' WHERE id={user.Id}";
            }
            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
            }
        }
    }
}
