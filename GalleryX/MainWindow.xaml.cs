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
        /*
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
        */

        public MainWindow()
        {
            InitializeComponent();
            string thisGalleryName = "GalleryX";
            Title = thisGalleryName;
            Gallery a = new Gallery(thisGalleryName);
            Artist b = new Artist("Rob Miles", a);
            Artwork c = new Artwork(
                "Mona Lisa",
                12000.00m,
                DateTime.Now,
                Artwork.ArtworkType.Painting,
                Artwork.ArtworkState.InGallery,
                b
                );
            Artwork d = new Artwork(
                "Wall poster of the Mona Lisa",
                13.99m,
                DateTime.Now,
                Artwork.ArtworkType.Sculpture,
                Artwork.ArtworkState.InGallery,
                b
                );
            c.ReturnToArtist();
            c.AddToGallery(DateTime.Now);
            b.AddArtwork(10, c);
            b.AddArtwork(11, d);
            List<Artwork> query = b.FindArtwork("mona lisa");
            foreach (Artwork artwork in query)
            {
                Console.WriteLine(artwork);
            }
            b.XMLSave("test-xml.xml");
            Artist newB = new Artist("Rob Miles", a);
            if (newB.Equals(b))
            {
                Console.WriteLine("Equal");
            }
            else
            {
                Console.WriteLine("Not Equal");
            }
            //Test();
            /*
            Artist loadedArtist = Artist.Load("test_save.txt");
            Console.WriteLine(loadedArtist);
            loadedArtist = Artist.Load("test_save2.txt");
            Console.WriteLine(loadedArtist);
            /*
            foreach (Artwork artwork in loadedArtist.StockList)
            {
                Console.WriteLine(artwork.ID);
            }
             * */
            //Console.WriteLine(loadedArtist.GetHashCode());
        }
    }
}
