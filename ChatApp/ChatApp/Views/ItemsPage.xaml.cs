using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ChatApp.Models;
using ChatApp.Views;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        HubConnection hubConnection;


        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();

            var ip = "localhost";
            if (Device.RuntimePlatform == Device.Android)
                ip = "10.0.2.2";

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"https://{ip}:44312/chathub")
                .Build();
            //https://localhost:44312/
            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var finalMessage = $"{user} says {message}";
                Console.WriteLine(finalMessage);
                // Update the UI
            });
        }

        async Task Connect()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Something has gone wrong
            }
        }

        async Task SendMessage(string user, string message)
        {
            try
            {
                await hubConnection.InvokeAsync("SendMessage", user, message);
            }
            catch (Exception ex)
            {
                // send failed
            }
        }



        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Connect();
            //await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        private async void Send_Clicked(object sender, EventArgs e)
        {
            await SendMessage("John", txtMessage.Text);
        }
    }
}