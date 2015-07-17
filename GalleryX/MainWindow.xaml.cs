/*   MainWindow.xaml.cs - Interaction file for MainWindow.xaml.
*    Copyright (C) 2015  Connor Blakey <connorblakey96@gmail.com>.
*
*    This program is free software; you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation; either version 2 of the License, or
*    (at your option) any later version.
*
*    This program is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.
*
*    You should have received a copy of the GNU General Public License along
*    with this program; if not, write to the Free Software Foundation, Inc.,
*    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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
using GalleryBusiness;

namespace GalleryX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Main Gallery object.
        /// </summary>
        Gallery gallery;

        string SaveFilename = "GalleryX.xml";

        public MainWindow()
        {
            InitializeComponent();
            string thisGalleryName = "GalleryX";
            Title = thisGalleryName;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                gallery = Gallery.XmlLoad(SaveFilename);
            }
            catch (GalleryExceptionMD5 E)
            {
                MessageBox.Show(E.Message);
                Environment.Exit(1);
            }
            catch (System.IO.FileNotFoundException E)
            {
                MessageBox.Show(E.Message, "File not found");
                MessageBox.Show("Creating new Gallery.", "New Gallery");
                gallery = new Gallery();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gallery.XmlSave(SaveFilename);
        }

        private void ArtistSearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ArtistSearchListBox.ItemsSource = gallery.FindArtist(ArtistSearchTextBox.Text);
            }
            catch (Exception E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void AddArtistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Artist newArtist = new Artist(ArtistNameTextBox.Text, gallery);
                gallery.AddArtist(newArtist);
            }
            catch (Exception E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void SearchArtworkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KeyValuePair<int, Artist> selectedArtist = (KeyValuePair<int, Artist>)ArtistSearchListBox.SelectedItem;
                SearchArtworkListBox.ItemsSource = selectedArtist.Value.FindArtwork(SearchArtworkTextBox.Text);
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Artist selected. Please select an Artist from the search box.";
            }
        }

	private Artwork.ArtworkState getArtworkState(string pSelectedState)
	{
	  switch (pSelectedState)
	    {
	    case "In Gallery":
	      return Artwork.ArtworkState.InGallery;
	    case "Awaiting Gallery Entry":
	      return Artwork.ArtworkState.AwaitingGalleryEntry;
	    case "Sold":
	      return Artwork.ArtworkState.Sold;
	    case "Returned To Artist":
	      return Artwork.ArtworkState.ReturnedToArtist;
	    }
	}

        private void AddArtworkButton_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<int, Artist> selectedArtist;
            try
            {
                selectedArtist = (KeyValuePair<int, Artist>)ArtistSearchListBox.SelectedItem;

                Artwork newArtwork = new Artwork(
                    DescriptionTextBox.Text,
                    decimal.Parse(PriceTextBox.Text),
                    DateTime.Now,
                    (Artwork.ArtworkType)Enum.Parse(
                        typeof(Artwork.ArtworkType),
                        ArtworkTypeComboBox.Text),
		    getArtworkState(ArtworkStateComboBox.Text)
                    /*(Artwork.ArtworkState)Enum.Parse(
                        typeof(Artwork.ArtworkState),
                        ArtworkStateComboBox.Text)*/,
                    selectedArtist.Value
                    );

                selectedArtist.Value.AddArtwork(newArtwork);
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Artist selected. Please select an Artist from the search box.";
            }
            catch (System.FormatException E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
            catch (ArtworkException E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
            catch (ArtistException E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void ArtistSearchListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KeyValuePair<int, Artist> selectedArtist = (KeyValuePair<int, Artist>)ArtistSearchListBox.SelectedItem;
            ArtistNameTextBox.Text = selectedArtist.Value.Name;
        }

        private void EditArtistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KeyValuePair<int, Artist> selectedArtist = (KeyValuePair<int, Artist>)ArtistSearchListBox.SelectedItem;
                selectedArtist.Value.UpdateName(ArtistNameTextBox.Text);
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Artist selected. Please select an Artist from the search box.";
            }
            catch (ArtistExceptionBadName E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void SearchArtworkListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KeyValuePair<int, Artwork> selectedArtwork = (KeyValuePair<int, Artwork>)SearchArtworkListBox.SelectedItem;
            DescriptionTextBox.Text = selectedArtwork.Value.Description;
            PriceTextBox.Text = selectedArtwork.Value.Price.ToString();
            ArtworkTypeComboBox.SelectedIndex = (int)selectedArtwork.Value.Type;
            ArtworkStateComboBox.SelectedIndex = (int)selectedArtwork.Value.State;
            DisplayDateLabel.Content = selectedArtwork.Value.MostRecentDisplayDateString;
            ArtworkIDTextBox.Text = selectedArtwork.Key.ToString();
        }

        private void EditArtworkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KeyValuePair<int, Artwork> selectedArtwork = (KeyValuePair<int, Artwork>)SearchArtworkListBox.SelectedItem;
                selectedArtwork.Value.UpdateDescription(DescriptionTextBox.Text);
                selectedArtwork.Value.UpdatePrice(
                    decimal.Parse(PriceTextBox.Text)
                    );
                gallery.ChangeArtworkState(
                    selectedArtwork.Key,
                    (Artwork.ArtworkState)Enum.Parse(
                        typeof(Artwork.ArtworkState),
                        ArtworkStateComboBox.Text),
                    DateTime.Now
                    );
                selectedArtwork.Value.Type = (Artwork.ArtworkType)Enum.Parse(
                    typeof(Artwork.ArtworkType),
                    ArtworkTypeComboBox.Text
                    );
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Artwork selected. Please select an Artwork from the search box.";
            }
            catch (ArtworkException E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void CheckArtworkExpired_Click(object sender, RoutedEventArgs e)
        {
            ExpiredArtworkListBox.ItemsSource = gallery.ArtworkGalleryTimeExpired();
        }

        private void CutomerSearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerSearchListBox.ItemsSource = gallery.FindCustomer(CustomerSearchTextBox.Text);
            }
            catch (Exception E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Customer newCustomer = new Customer(CustomerNameTextBox.Text, gallery);
                gallery.AddCustomer(newCustomer);
            }
            catch (Exception E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void CustomerSearchListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KeyValuePair<int, Customer> selectedCustomer = (KeyValuePair<int, Customer>)CustomerSearchListBox.SelectedItem;
            CustomerNameTextBox.Text = selectedCustomer.Value.Name;
        }

        private void EditCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KeyValuePair<int, Customer> selectedCustomer = (KeyValuePair<int, Customer>)CustomerSearchListBox.SelectedItem;
                selectedCustomer.Value.UpdateName(CustomerNameTextBox.Text);
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Cutomer selected. Please select a Customer from the search box.";
            }
            catch (CustomerExceptionBadName E)
            {
                ExceptionMessageAreaLabel.Content = E.Message;
            }
        }

        private void SearchOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KeyValuePair<int, Customer> selectedCustomer = (KeyValuePair<int, Customer>)CustomerSearchListBox.SelectedItem;
                OrderSearchListBox.ItemsSource = selectedCustomer.Value.FindOrderByArtworkID(int.Parse(SearchOrderTextBox.Text));
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Artist selected. Please select an Artist from the search box.";
            }
        }

        private void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KeyValuePair<int, Order> selectedOrder = (KeyValuePair<int, Order>)OrderSearchListBox.SelectedItem;
                selectedOrder.Value.ArtworkID = int.Parse(ArtworkIDTextBox.Text);
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Order selected. Please select an Order from the search box.";
            }
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<int, Customer> selectedCustomer;
            try
            {
                selectedCustomer = (KeyValuePair<int, Customer>)CustomerSearchListBox.SelectedItem;

                Order newOrder = new Order(
                    int.Parse(ArtworkIDTextBox.Text),
                    DateTime.Now,
                    selectedCustomer.Value
                    );

                selectedCustomer.Value.AddOrder(newOrder);
            }
            catch (NullReferenceException)
            {
                ExceptionMessageAreaLabel.Content = "No Customer selected. Please select a Customer from the search box.";
            }
        }

        private void OrderSearchListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KeyValuePair<int, Order> selectedOrder = (KeyValuePair<int, Order>)OrderSearchListBox.SelectedItem;
            ArtworkIDTextBox.Text = selectedOrder.Value.ArtworkID.ToString();
        }
    }
}