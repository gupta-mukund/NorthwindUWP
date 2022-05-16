using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace gestionaleDB
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MySqlConnection mysqlcon;
        public List<Customer> Customers;
        public MainPage()
        {
            this.InitializeComponent();
        }
        public MySqlConnection DB_Connect()
        {
            string db_server = "127.0.0.1";
            string user_id = "root";
            string password = "";
            string database = "northwind";
            string M_str_sqlcon = $"server={db_server};user id={user_id};password={password};database={database}";

            if (mysqlcon == null)
                mysqlcon = new MySqlConnection(M_str_sqlcon);
            if (mysqlcon.State == System.Data.ConnectionState.Closed)
                mysqlcon.Open();

            return mysqlcon;
        }

        public void GetCustomers()
        {
            DB_Connect();
            Customers = new List<Customer>();
            string sql = "select CustomerID, CompanyName, ContactName, ContactTitle, Country from customers";
            MySqlCommand mysqlcom = new MySqlCommand(sql, mysqlcon);
            using (MySqlDataReader mysqlread = mysqlcom.ExecuteReader())
            {
                while (mysqlread.Read())
                {
                    Customer customer = new Customer()
                    {
                        CustomerID = mysqlread.GetString(0),
                        CompanyName = mysqlread.GetString(1),
                        ContactName = mysqlread.GetString(2),
                        ContactTitle = mysqlread.GetString(3),
                        Country = SafeGetString(mysqlread, 4)
                    };
                    Customers.Add(customer);
                }
            }
        }
        public string SafeGetString(MySqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex)) return reader.GetString(colIndex);
            return string.Empty;

        }

        private void InitData()
        {
            GetCustomers();
            dataGridCustomers.ItemsSource = Customers;
        }

        private async void DeleteBtnClicked(object sender, RoutedEventArgs e)
        {
            if (dataGridCustomers.SelectedItems == null) return;
            Customer customerSelected = dataGridCustomers.SelectedItem as Customer;
            if (customerSelected == null) return;
            string sql = $"Delete from Customers where CustomerID = {customerSelected.CustomerID}";
            MySqlCommand cmd = new MySqlCommand(sql, mysqlcon);
            cmd.ExecuteNonQuery();
            await new MessageDialog("Deleted", "").ShowAsync();
            InitData();
        }

        private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rootPivot.SelectedIndex == 0)
            {
                InitData();
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var customerID = CustomerID.Text.Trim();
            var companyName = CompanyName.Text.Trim();
            var contactName = ContactName.Text.Trim();
            var contactTitle = ContactTitle.Text.Trim();
            var country = Country.Text.Trim();
            var titleMessage = "Data not valid";

            string msg = string.IsNullOrWhiteSpace(customerID) ? "Customer ID" :
                string.IsNullOrWhiteSpace(companyName) ? "Company Name" :
                string.IsNullOrWhiteSpace(contactName) ? "Contact Name" :
                string.IsNullOrWhiteSpace(contactTitle) ? "Contact Title" :
                string.IsNullOrWhiteSpace(country) ? "Country" : "";

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await new MessageDialog(titleMessage, $"{msg} is empty").ShowAsync();
                return;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CustomerID.Text = "";
            CompanyName.Text = "";
            ContactName.Text = "";
            ContactTitle.Text = "";
            Country.Text = "";
        }
    }
}
