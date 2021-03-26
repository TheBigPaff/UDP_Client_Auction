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

        internal static void SaveResetTicket(string text, string tempPwd, DateTime expireDate)
        {
            throw new NotImplementedException();
        }

        internal static void TicketWasUsed(int ticketId)
        {
            throw new NotImplementedException();
        }

        internal static User GetUser(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns token id if it exists, not expired and not been used already. 
        /// Returns 0 if it exists but has been used.
        /// Returns -1 if it has expired.
        /// Returns -2 if doesn't exist.
        /// </summary>
        internal static int TryGetToken(string token, int userId)
        {
            int id = -2;
            string queryExists = $"SELECT id FROM ResetTickets WHERE Token = '{token}' AND UserId = {userId}";
            string queryTokenId = $"SELECT id FROM ResetTickets WHERE Token = '{token}' AND UserId = {userId} AND TokenUsed = 0";
            string queryExpirationDate = $"SELECT ExpirationDate FROM ResetTickets WHERE Id = ";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(queryExists, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            //dbConnection.Open();

            //reader = dbCommand.ExecuteReader();

            //// check if ticket exists with that token and user exists
            //reader
            //if (reader.)
            //{
            //    id = 0;

            //    //check if a ticket with that token, that user and that has not been used exists.
            //    var resultToken = connection.Query<int>(queryTokenId).ToList();
            //    if (resultToken.Count > 0)
            //    {
            //        id = -1;

            //        int resultTokenId = resultToken.Single();

            //        // check if token has expired
            //        var resultDate = connection.Query<string>(queryExpirationDate + resultTokenId).ToList();

            //        DateTime expirationDate = DateTime.Parse(resultDate.Single());

            //        if (DateTime.Now < expirationDate)
            //        {
            //            // token is not expired!
            //            id = resultTokenId;
            //        }
            //    }
            //}
            

            return id;
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
