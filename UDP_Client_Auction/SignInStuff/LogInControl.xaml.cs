using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for LogInControl.xaml
    /// </summary>
    public partial class LogInControl : UserControl
    {
        public LogInControl()
        {
            InitializeComponent();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnLogIn_Click(sender, e);
            }
        }
        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text) || string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                MessageBox.Show("Some fields are empty");
                return;
            }
            string username = tbUsername.Text.Trim();
            string password = pbPassword.Password.Trim();

            User user = DB.UserLogIn(username, password);

            if (user != null)
            {
                Session.LoggedUser = user;
                SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
                if (signWindow != null)
                {
                    signWindow.EnterApp(user.ProPicPath);
                }
            }
            else
            {
                //// does user exist? if so, he might have used a temp token
                //int userId = DB.GetUserId(username);
                //if(userId != -1)
                //{
                //    // check if he has a token available
                //    int ticketId = DB.TryGetToken(password, userId);
                //    if(ticketId > 0)
                //    {
                //        // set token as "Used" in table
                //        DB.TicketWasUsed(ticketId);

                //        // log user in
                //        user = DB.GetUser(userId);
                //        if(user != null)
                //        {
                //            Session.LoggedUser = user;
                //            SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
                //            if (signWindow != null)
                //            {
                //                signWindow.EnterApp(user.ProPicPath);
                //            }
                //        }
                //    }
                //    else if(ticketId == 0)
                //    {
                //        MessageBox.Show("You tried to use a temporary password that has already been used");
                //    }
                //    else if(ticketId == -1)
                //    {
                //        MessageBox.Show("You tried to use a temporary password that has expired");
                //    }
                //    else
                //    {
                //        MessageBox.Show("Wrong password");
                //    }
                //}
                //else
                //{
                //    MessageBox.Show("That username doesn't exist");
                //}


                // does username exist?
                int id = DB.GetUserId(tbUsername.Text);
                if (id != -1)
                {
                    if (DB.GetToken(pbPassword.Password, id))
                    {
                        MessageBox.Show("Token used!");
                        DB.SetTokenToUsed(pbPassword.Password);

                        Session.LoggedUser = DB.UserLogIn(tbUsername.Text);
                        SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
                        if (signWindow != null)
                        {
                            signWindow.EnterApp(Session.LoggedUser.ProPicPath);
                            return;
                        }
                    }
                }
                MessageBox.Show("Username or password wrong");
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
            if (signWindow != null)
            {
                signWindow.ShowSignUp();
            }
        }

        private void forgotPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
            if (signWindow != null)
            {
                signWindow.ShowForgotPassword();
            }
        }
    }
}
