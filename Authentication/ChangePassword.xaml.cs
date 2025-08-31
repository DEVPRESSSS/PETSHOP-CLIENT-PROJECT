﻿using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
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
using System.Windows.Shapes;

namespace PetShop.Authentication
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        private string userEmail;

        public ChangePassword(string email)
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

            userEmail = email;
        }

        private void PackIconMaterial_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

      

    

        private void ChangePass_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = FirstPassword.Text.Trim();
            string confirmPassword = ConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {

                     sqlConnection.Open();
                    string query = "UPDATE EmployeeTbl SET EmpPass = @password WHERE EmpEmail = @EmpEmail";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@password", newPassword); 
                        cmd.Parameters.AddWithValue("@EmpEmail", userEmail);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            var login = new Login();
                            login.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to change password. Email not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotPassword = new ForgotPassword();

            //call the show method to show the form
            forgotPassword.Show();

            //close the login form
            this.Close();

        }

        private void ConfirmPassword_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void ConfirmPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.InputValidator.Username(sender, e);
        }

        private void FirstPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.InputValidator.Username(sender, e);

        }
    }
}
