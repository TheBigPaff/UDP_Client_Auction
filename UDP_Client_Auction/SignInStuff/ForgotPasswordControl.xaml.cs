using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UDP_Client_Auction
{
    /// <summary>
    /// Interaction logic for ForgotPasswordControl.xaml
    /// </summary>
    public partial class ForgotPasswordControl : UserControl
    {
        public ForgotPasswordControl()
        {
            InitializeComponent();
        }

        private void sendEmailBtn_Click(object sender, RoutedEventArgs e)
        {
            // check if username is correct
            string email;
            bool usernameExists =  DB.TryGetUserEmail(tbUsername.Text, out email);

            if(!usernameExists)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("There is no user with that username! Try again.");
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("That username doesn't have an email. Contact support to get your account back.");
            }
            else 
            {
                // generate temporary password
                string tempPwd = RandomString(5);

                // send email
                string title = "Your temporary password - Deniso's Auctions";
                string msgBody = $"Hello {tbUsername.Text},\nThis is your temporary password which you will be able to use to access your account ONLY ONCE and will expire in 24 hours.\nTEMPORARY PASSWORD: {tempPwd}\n\nYou should change your password as soon as you access your account.";
                Helper.SendEmail(email, title, msgBody);


                // register to database ResetTickets table
                DateTime expireDate = DateTime.Now.AddDays(1); // email expire date

                DB.SaveResetTicket(tbUsername.Text, tempPwd, expireDate);

                // go back to LogInControl
                MessageBox.Show("Email sent! Check your inbox.");

                SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
                if(signWindow != null)
                {
                    signWindow.ShowLogIn();
                }
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                sendEmailBtn_Click(sender, e);
            }
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
