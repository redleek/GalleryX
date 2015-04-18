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
#if DEBUG
        private void Test()
        {
            Artist artist1 = new Artist("Rob Miles");
            Artist artist2 = new Artist("Kevin Elner");
            Artwork testArtwork;
            artist2.AddArtwork(
                testArtwork = new Artwork(
                    "Just another Artwork",
                    12.99m,
                    DateTime.Now,
                    Artwork.ArtworkType.Sculpture,
                    Artwork.ArtworkState.Sold,
                    artist2)
                );
            Artwork artwork1, artwork2, artwork3, artwork4;
            artist1.AddArtwork(
                artwork1 = new Artwork(
                    "Hay",
                    319.99m,
                    DateTime.Now,
                    Artwork.ArtworkType.Painting,
                    Artwork.ArtworkState.InGallery,
                    artist1)
                    );
            artist1.AddArtwork(
                artwork2 = new Artwork(
                    "Way",
                    259.99m,
                    DateTime.Now,
                    Artwork.ArtworkType.Sculpture,
                    Artwork.ArtworkState.InGallery,
                    artist1)
                    );
            artist1.AddArtwork(
                artwork3 = new Artwork(
                    "Nay",
                    169.99m,
                    DateTime.Now,
                    Artwork.ArtworkType.Sculpture,
                    Artwork.ArtworkState.AwaitingGalleryEntry,
                    artist1)
                );
            artist1.AddArtwork(
                artwork4 = new Artwork(
                    "The Ten Commandments",
                    1000000,
                    DateTime.Now,
                    Artwork.ArtworkType.Sculpture,
                    Artwork.ArtworkState.AwaitingGalleryEntry,
                    artist1)
                );
            if (artwork4.Equals(testArtwork))
            {
                Console.WriteLine("Equal");
            }
            else
            {
                Console.WriteLine("Not Equal");
            }
            artist1.Save("test_save.txt");
            artist2.Save("test_save2.txt");
        }
#endif

        public MainWindow()
        {
            InitializeComponent();
            string thisGalleryName = "GalleryX";
            Title = thisGalleryName;
            //Test();
            Artist loadedArtist = Artist.Load("test_save.txt");
            Console.WriteLine(loadedArtist);
            loadedArtist = Artist.Load("test_save2.txt");
            Console.WriteLine(loadedArtist);
            foreach (Artwork artwork in loadedArtist.StockList)
            {
                Console.WriteLine(artwork.ID);
            }
        }
    }
}
