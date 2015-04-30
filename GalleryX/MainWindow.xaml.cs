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
        Gallery gallery;

        public MainWindow()
        {
            InitializeComponent();
            string thisGalleryName = "GalleryX";
            Title = thisGalleryName;
            try
            {
                gallery = new Gallery();//Gallery.XmlLoad("test.xml");
            }
            catch (GalleryException E)
            {
                //MessageBox.Show(E.Message, "File loading error.");
            }


            Artist artist;
            for (int i = 0; i < 1000; i++)
            {
                gallery.AddArtist(artist = new Artist("Rob Miles", gallery));
                for (int j = 0; j < 1000; j++)
                {
                    gallery.AddArtwork(0, new Artwork("Mona Lisa", 12.99m, DateTime.Now, Artwork.ArtworkType.Painting, Artwork.ArtworkState.AwaitingGalleryEntry, artist));
                }
            }
            gallery.XmlSave("test.xml");

        }
    }
}
