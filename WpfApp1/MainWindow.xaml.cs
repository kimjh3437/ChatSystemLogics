using Microsoft.AspNetCore.SignalR.Client;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection hubConnection;

        public MainWindow()
        {
            InitializeComponent();

            var ip = "localhost";

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"https://{ip}:44312/chathub")
                .Build();
            //https://localhost:44312/
            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var finalMessage = $"{user} says {message}";
                Console.WriteLine(finalMessage);

                box.Items.Add(finalMessage);
                // Update the UI
            });
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await hubConnection.StartAsync();
                await hubConnection.InvokeAsync("JoinRoom", "MyRoom123");
                MessageBox.Show("Connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Something has gone wrong
            }
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await hubConnection.InvokeAsync("SendToRoom", "MyRoom123", txtName.Text, txtMessage.Text);
            }
            catch (Exception ex)
            {
                // send failed
            }
        }
    }
}
