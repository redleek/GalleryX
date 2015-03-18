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

namespace GalleryX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Test()
        {
            Artist Fred = new Artist("Fred", 0);
            //Artwork newArt = new Artwork("New artwork", 30, DateTime.Now, ArtworkState.InGallery, 0);
            Artwork newArt = new Artwork(
                "New artwork",
                30,
                DateTime.Now,
                Artwork.ArtworkType.Painting,
                0
                );
            Fred.AddArtwork(newArt);
            Fred.AddArtwork(new Artwork(
                "asd",
                34,
                DateTime.Now,
                Artwork.ArtworkType.Sculpture,
                1
                ));
            Fred.AddArtwork(new Artwork(
                "sdada",
                234,
                DateTime.Now,
                Artwork.ArtworkType.Painting,
                2
                ));
            //newArt.Save("newArt.txt");
            Fred.Save("GalleryX.txt");
            Console.WriteLine(newArt.ToString());

            Artist loadedArtist = Artist.Load("GalleryX.txt");
            //Artwork loadedArt = Artwork.Load("newArt.txt");
        }

        public MainWindow()
        {
            InitializeComponent();
            Test();
        }
    }
}
