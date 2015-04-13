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
using System.Collections.ObjectModel;

namespace GalleryX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Test()
        {
            Artist artist = new Artist("Rob Miles", 32);
            Artwork artwork1, artwork2, artwork3;
            artist.AddArtwork(
                artwork1 = new Artwork(
                    "Hay",
                    319.99m,
                    DateTime.Now,
                    Artwork.ArtworkType.Painting,
                    Artwork.ArtworkState.InGallery)
                    );
            artist.AddArtwork(
                artwork2 = new Artwork(
                    "Way",
                    259.99m,
                    DateTime.Now,
                    Artwork.ArtworkType.Sculpture,
                    Artwork.ArtworkState.InGallery)
                    );
            artist.AddArtwork(
                artwork3 = new Artwork(
                    "Nay",
                    169.99m,
                    DateTime.Now,
                    Artwork.ArtworkType.Sculpture,
                    Artwork.ArtworkState.AwaitingGalleryEntry)
                );
            if (artwork3.Equals(artwork3))
            {
                Console.WriteLine("Equal");
            }
            else
            {
                Console.WriteLine("Not Equal");
            }
            artist.Save("test_save.txt");
        }

        public MainWindow()
        {
            InitializeComponent();
            string thisGalleryName = "GalleryX";
            Title = thisGalleryName;
            Test();
        }
    }
}
