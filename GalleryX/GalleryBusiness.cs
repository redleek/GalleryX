using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryBusiness
{
    /// <summary>
    /// A piece of Artwork created by an artist.
    /// </summary>
    class Artwork
    {
        /// <summary>
        /// The maximum number of days an Artwork can be displayed for.
        /// </summary>
        public const int MAX_DISPLAY_DAYS = 14;

        /// <summary>
        /// Stores the type of the Artwork.
        /// </summary>
        public enum ArtworkType
        {
            Painting,
            Sculpture
        }

        /// <summary>
        /// Stores the current state of an Artwork.
        /// </summary>
        public enum ArtworkState
        {
            InGallery,
            Sold,
            Returned
        }

        private string mDescription;
        private decimal mPrice;
        private DateTime mDisplayDate;
        private ArtworkType mType;
        private ArtworkState mState;
        private int mID;

        /// <summary>
        /// Create a new instance of an Artwork loaded from file.
        /// </summary>
        /// <param name="inDescription">A short description of the Artwork.</param>
        /// <param name="inPrice">The initial price of the Artwork.</param>
        /// <param name="inDisplayDate">The date at which the Artwork was put on display.</param>
        /// <param name="inType">The type of the Artwork. Either Painting or Sculpture.</param>
        /// <param name="inState">The state of the Artwork.</param>
        /// <param name="inID">Unique stock ID of the Artwork.</param>
        private Artwork(string inDescription, decimal inPrice, DateTime inDisplayDate, ArtworkType inType, ArtworkState inState, int inID)
        {
            mDescription = inDescription;
            mPrice = inPrice;
            mDisplayDate = inDisplayDate;
            mType = inType;
            mState = inState;
            mID = inID;
        }

        /// <summary>
        /// Create a new instance of an Artwork.
        /// </summary>
        /// <param name="inDescription">A short description of the Artwork.</param>
        /// <param name="inPrice">The initial price of the Artwork.</param>
        /// <param name="inDisplayDate">The date at which the Artwork was put on display.</param>
        /// <param name="inType">The type of the Artwork. Either Painting or Sculpture.</param>
        /// <param name="inID">Unique stock ID of the Artwork.</param>
        public Artwork(string inDescription, decimal inPrice, DateTime inDisplayDate, ArtworkType inType, int inID) 
            : this(inDescription, inPrice, inDisplayDate, inType, ArtworkState.InGallery, inID)
        { }

        /// <summary>
        /// Short description of the Artwork.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }

        /// <summary>
        /// The Price of the Artwork.
        /// </summary>
        public decimal Price
        {
            get { return mPrice; }
        }

        /// <summary>
        /// The date the item was put on display in the Gallery.
        /// </summary>
        public DateTime DisplayDate
        {
            get { return mDisplayDate; }
        }

        /// <summary>
        /// The type of the sculpture of type ArtworkType.
        /// </summary>
        public ArtworkType Type
        {
            get { return mType; }
        }

        /// <summary>
        /// The current state of the Artwork.
        /// </summary>
        public ArtworkState State
        {
            get { return mState; }
        }

        /// <summary>
        /// The stock identification number of the Artwork.
        /// </summary>
        public int ID
        {
            get { return mID; }
        }

        /// <summary>
        /// Sets the state of the Artwork to sold.
        /// </summary>
        public void Sell()
        {
            mState = ArtworkState.Sold;
        }

        /// <summary>
        /// Checks if the Artwork has been on display for longer than it should be.
        /// </summary>
        /// <param name="pCurrentDate">The current DateTime of the system.</param>
        /// <returns>Whether or not the Artwork has been in the gallery for more than two weeks.</returns>
        public bool GalleryTimeExpired(DateTime pCurrentDate)
        {
            if ((pCurrentDate - mDisplayDate).TotalDays > MAX_DISPLAY_DAYS)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the state of the Artwork to returned to the artist.
        /// </summary>
        public void ReturnToArtist()
        {
            mState = ArtworkState.Returned;
        }

        /// <summary>
        /// Write Artwork information to a TextWriter stream.
        /// </summary>
        /// <param name="pTextOut">Text stream to write to.</param>
        public void Save(System.IO.TextWriter pTextOut)
        {
            pTextOut.WriteLine(mDescription);
            pTextOut.WriteLine(mPrice);
            pTextOut.WriteLine(mDisplayDate);
            pTextOut.WriteLine(mType);
            pTextOut.WriteLine(mState);
            pTextOut.WriteLine(mID);
        }

        /// <summary>
        /// Write Artwork information to a file.
        /// </summary>
        /// <param name="pFileName">File to write to.</param>
        public void Save(string pFileName)
        {
            System.IO.TextWriter TextOut = null;
            try
            {
                TextOut = new System.IO.StreamWriter(pFileName);
                Save(TextOut);
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (TextOut != null)
                {
                    TextOut.Close();
                }
            }
        }

        /// <summary>
        /// Load Artwork information from a TextReader stream.
        /// </summary>
        /// <param name="pTextIn">Text stream to load from.</param>
        /// <returns>Returns a fully loaded Artwork class.</returns>
        public static Artwork Load(System.IO.TextReader pTextIn)
        {
            string loadDescription = pTextIn.ReadLine();
            decimal loadPrice = decimal.Parse(pTextIn.ReadLine());
            DateTime loadDisplayDate = DateTime.Parse(pTextIn.ReadLine());
            ArtworkType loadType = (ArtworkType)Enum.Parse(
                typeof(ArtworkType),
                pTextIn.ReadLine()
                );
            ArtworkState loadState = (ArtworkState)Enum.Parse(
                typeof(ArtworkState),
                pTextIn.ReadLine()
                );
            int loadID = int.Parse(pTextIn.ReadLine());
            return new Artwork(loadDescription, loadPrice, loadDisplayDate, loadType, loadState, loadID);
        }

        /// <summary>
        /// Load Artwork information form a file.
        /// </summary>
        /// <param name="pFileName">File to load from.</param>
        /// <returns>Returns a fully loaded Artwork class.</returns>
        public static Artwork Load(string pFileName)
        {
            Artwork outArtwork;
            System.IO.TextReader TextIn = null;
            try
            {
                TextIn = new System.IO.StreamReader(pFileName);
                outArtwork = Load(TextIn);
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (TextIn != null)
                {
                    TextIn.Close();
                }
            }
            return outArtwork;
        }

        /// <summary>
        /// Get a diagnostic of the Artwork to check it's validity when loaded from a file.
        /// </summary>
        /// <returns>Returns a string of all the data members of the Artwork.</returns>
        public override string ToString()
        {
            return "Description: " + mDescription + ", Price: £" + mPrice + ", Display date: " + mDisplayDate + ", Artwork type: " + mType + ", Artwork state: " + mState + ", Artwork ID: " + mID;
        }
    }

    /// <summary>
    /// An Artist who sells Artwork in a gallery.
    /// </summary>
    class Artist
    {
        /// <summary>
        /// The maximum number of Artworks an Artist can have in a gallery.
        /// </summary>
        public const int MAX_ARTWORKS = 5;

        private string mName;
        private List<Artwork> mStock;
        private int mID;

        /// <summary>
        /// Create a new instance of an Artist loaded from file.
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="inStock"></param>
        /// <param name="inID"></param>
        private Artist(string inName, List<Artwork> inStock, int inID)
        {
            mName = inName;
            mID = inID;
            mStock = inStock;
        }

        /// <summary>
        /// Create a new instance of an Artist.
        /// </summary>
        /// <param name="inName">The name of the Artist.</param>
        /// <param name="inID">The unique ID of the Artist.</param>
        public Artist(string inName, int inID) : this(inName, new List<Artwork>(), inID)
        { }

        // TODO: Add a base constructor for loading in the information from file.

        /// <summary>
        /// The name of the artist.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// The list of stock that the Artist has in the gallery.
        /// </summary>
        public List<Artwork> StockList
        {
            get { return mStock; }
        }

        /// <summary>
        /// The unique ID of the Artist.
        /// </summary>
        public int ID
        {
            get { return mID; }
        }

        /// <summary>
        /// Add an Artwork to the Artist's list of Artwork.
        /// </summary>
        /// <param name="pArtwork">Reference to the artwork to be added.</param>
        public void AddArtwork(Artwork pArtwork)
        {
            mStock.Add(pArtwork);
        }

        /// <summary>
        /// Find an Artwork in the list of the Artist's Artowrks.
        /// </summary>
        /// <param name="pID">The ID of the Artwork to find.</param>
        /// <returns>Returns the reference to the Artwork.</returns>
        public Artwork FindArtwork(int pID)
        {
            foreach (Artwork artwork in mStock)
            {
                if (artwork.ID == pID)
                {
                    return artwork;
                }
            }
            return null;
        }

        /// <summary>
        /// Remove a particular Artwork 
        /// </summary>
        /// <param name="pID">The ID of the Artwork to remove.</param>
        public bool RemoveArtwork(int pID)
        {
            Artwork artwork = FindArtwork(pID);
            if (artwork == null)
            {
                return false;
            }
            mStock.Remove(artwork);
            return true;
        }

        /// <summary>
        /// Write Artist information to a TextWriter stream.
        /// </summary>
        /// <param name="pTextOut">Text stream to write to.</param>
        /// <returns>Whether or not the file write succeeded or not.</returns>
        public void Save(System.IO.TextWriter pTextOut)
        {
            pTextOut.WriteLine(mName);
            pTextOut.WriteLine(mID);
            // Also save the amount of stock items the artist has.
            pTextOut.WriteLine(mStock.Count);
            foreach (Artwork artwork in mStock)
            {
                artwork.Save(pTextOut);
            }
        }

        /// <summary>
        /// Write Artist information to a file.
        /// </summary>
        /// <param name="pFileName">File to write to.</param>
        public void Save(string pFileName)
        {
            System.IO.TextWriter TextOut = null;
            try
            {
                TextOut = new System.IO.StreamWriter(pFileName);
                Save(TextOut);
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (TextOut != null)
                {
                    TextOut.Close();
                }
            }
        }

        /// <summary>
        /// Load Artist information from a TextReader stream.
        /// </summary>
        /// <param name="pTextIn">Text stream to load from.</param>
        /// <returns>Returns a fully loaded Artist class.</returns>
        public static Artist Load(System.IO.TextReader pTextIn)
        {
            string loadedName = pTextIn.ReadLine();
            int loadedID = int.Parse(pTextIn.ReadLine());
            int loadedStockCount = int.Parse(pTextIn.ReadLine());
            List<Artwork> loadedStock = new List<Artwork>();
            for (int stockItem = 0; stockItem < loadedStockCount; stockItem++)
            {
                loadedStock.Add(Artwork.Load(pTextIn));
            }
            return new Artist(loadedName, loadedStock, loadedID);
        }

        /// <summary>
        /// Load Artist information form a file.
        /// </summary>
        /// <param name="pFileName">File to load from.</param>
        /// <returns>Returns a fully loaded Artist class.</returns>
        public static Artist Load(string pFileName)
        {
            Artist outArtist;
            System.IO.TextReader TextIn = null;
            try
            {
                TextIn = new System.IO.StreamReader(pFileName);
                outArtist = Load(TextIn);
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (TextIn != null)
                {
                    TextIn.Close();
                }
            }
            return outArtist;
        }

        /// <summary>
        /// Get a diagnostic of the Artist to check it's validity when loaded from a file.
        /// </summary>
        /// <returns>Returns a string of all the data members of the Artist.</returns>
        public override string ToString()
        {
            return "Artist name: " + mName + ", Number of stock items: " + mStock.Count + ", Artist ID: " + mID;
        }
    }
}
