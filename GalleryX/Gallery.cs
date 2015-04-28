using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GalleryBusiness
{
    /// <summary>
    /// Basic Exception class for Gallery.
    /// </summary>
    class GalleryException : System.Exception
    {
        public GalleryException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Art Gallery which holds Customers, Orders, Artists and Artworks.
    /// </summary>
    class Gallery
    {
        /// <summary>
        /// The number to start a new set of IDs on for a new gallery.
        /// </summary>
        public const int START_ID_NO = 0;

        /// <summary>
        /// The maximum number of Artworks allowed in a Gallery at one time.
        /// </summary>
        public const int GALLERY_CAPACITY = 50;

        private Dictionary<int, Artist> mArtists;
        private Dictionary<int, Customer> mCustomers;
        private List<int> mArtworkWaitingList;
        private int mArtworkIDCount;
        private int mArtistIDCount;
        private int mOrderIDCount;
        private int mCustomerIDCount;

        /// <summary>
        /// Creates a new instance of a Gallery loaded from file.
        /// </summary>
        /// <param name="inArtists">Dictionary containing IDs and Artists.</param>
        /// <param name="inCustomers">Dictionary containing IDs and Customers.</param>
        /// <param name="inArtworkIDCount">ID counter of Artworks.</param>
        /// <param name="inArtistIDCount">ID counter of Artists.</param>
        /// <param name="inOrderIDCount">ID counter of Orders.</param>
        /// <param name="inCustomerIDCount">ID counter of Customers.</param>
        private Gallery(Dictionary<int, Artist> inArtists, Dictionary<int, Customer> inCustomers,
            int inArtworkIDCount, int inArtistIDCount, int inOrderIDCount, int inCustomerIDCount)
        {
            mArtists = inArtists;
            mCustomers = inCustomers;
            mArtworkIDCount = inArtworkIDCount;
            mArtistIDCount = inArtistIDCount;
            mOrderIDCount = inOrderIDCount;
            mCustomerIDCount = inCustomerIDCount;
        }

        /// <summary>
        /// Creates a new instance of a Gallery.
        /// </summary>
        public Gallery()
            : this(new Dictionary<int, Artist>(), new Dictionary<int, Customer>(),
                START_ID_NO, START_ID_NO, START_ID_NO, START_ID_NO)
        { }

        /// <summary>
        /// Gets the total amount of Artworks on display in the Gallery.
        /// </summary>
        public int ArtworksInGallery
        {
            get
            {
                int Total = 0;
                foreach (Artist artist in mArtists.Values)
                {
                    Total += artist.ArtworksInGalleryCount;
                }
                return Total;
            }
        }

        /// <summary>
        /// Add an Artist to the Gallery.
        /// </summary>
        /// <param name="pNewArtist">Reference to the Artist to add.</param>
        public void AddArtist(Artist pNewArtist)
        {
            mArtists.Add(mArtistIDCount++, pNewArtist);
        }

        /// <summary>
        /// Find an Artist by it's ID.
        /// </summary>
        /// <param name="pID">ID of the Artist to searh for.</param>
        /// <returns>If found, returns a refernece to the Artist.</returns>
        public Artist FindArtist(int pID)
        {
            if (mArtists.ContainsKey(pID))
            {
                return mArtists[pID];
            }
            return null;
        }

        /// <summary>
        /// Find an Artist by it's name.
        /// </summary>
        /// <param name="pName">Name of the Artist to search for.</param>
        /// <returns>Returns a list of all Artists whose names contain the given string. Throws exception if string is invalid.</returns>
        public List<Artist> FindArtist(string pName)
        {
            pName = pName.Trim();
            if (pName.Length > 0 && pName.Length <= Artist.MAX_NAME_CHARS)
            {
                List<Artist> FoundArtists = new List<Artist>();
                foreach (Artist artist in mArtists.Values)
                {
                    if (artist.Name.Contains(pName) ||
                        artist.Name.ToLower().Contains(pName.ToLower()))
                    {
                        FoundArtists.Add(artist);
                    }
                }
                return FoundArtists;
            }
            throw new ArtistException("Entered string contains too many characters, limit: " + Artist.MAX_NAME_CHARS);
        }

        /// <summary>
        /// Add an Artwork to the Gallery.
        /// </summary>
        /// <param name="pArtistID">Unique ID of th owning Artist.</param>
        /// <param name="pNewArtwork">Reference to the new Artwork to add.</param>
        public void AddArtwork(int pArtistID, Artwork pNewArtwork)
        {
            if (pNewArtwork.State == Artwork.ArtworkState.InGallery
                && ArtworksInGallery == GALLERY_CAPACITY)
            {
                throw new GalleryException("Gallery capacity already reached: " + GALLERY_CAPACITY);
            }

            foreach (Artist artistDuplicateCheck in mArtists.Values)
            {
                if (!artistDuplicateCheck.CheckDuplicates(pNewArtwork))
                {
                    Artist artist = FindArtist(pArtistID);
                    artist.AddArtwork(pNewArtwork);
                }
                throw new ArtistDuplicateArtworksException("Artwork is a duplicate of artwork:" + artistDuplicateCheck);
            }
        }

        /// <summary>
        /// Find an Artwork by it's ID.
        /// </summary>
        /// <param name="pID">Unique ID of the artwork to look for.</param>
        /// <returns>Returns a reference to the found Artwork. Returns null if not found.</returns>
        public Artwork FindArtwork(int pID)
        {
            Artwork artwork;
            foreach (Artist artist in mArtists.Values)
            {
                if ((artwork = artist.FindArtwork(pID)) != null)
                {
                    return artwork;
                }
            }
            return null;
        }

        /// <summary>
        /// Find Artworks owned by an Artist by a description.
        /// </summary>
        /// <param name="pDescription">Description to search for.</param>
        /// <returns>Returns a list of found Artworks.</returns>
        public List<Artwork> FindArtwork(string pDescription)
        {
            List<Artwork> FoundArtworks = new List<Artwork>();
            foreach (Artist artist in mArtists.Values)
            {
                List<Artwork> thisArtistFoundArtworks = new List<Artwork>();
                if ((thisArtistFoundArtworks = artist.FindArtwork(pDescription)).Count
                    != 0)
                {
                    FoundArtworks.AddRange(thisArtistFoundArtworks);
                }
            }
            return FoundArtworks;
        }

        /// <summary>
        /// Change the state of an artwork.
        /// </summary>
        /// <param name="pID">ID of the Artwork to change state.</param>
        /// <param name="pNewState"></param>
        /// <param name="pAddToGalleryDate"></param>
        public void ChangeArtworkState(int pID, Artwork.ArtworkState pNewState, DateTime pAddToGalleryDate)
        {
            Artwork artwork;
            if ((artwork = FindArtwork(pID)) == null)
            {
                throw new ArtworkExceptionDoesNotExist("Artwork not found.");
            }

            switch (pNewState)
            {
                case Artwork.ArtworkState.InGallery:
                    if (ArtworksInGallery != GALLERY_CAPACITY)
                    {
                        if (artwork.Owner.ArtworksInGalleryCount
                            != Artist.MAX_ARTWORKS_IN_GALLERY)
                        {
                            artwork.AddToGallery(DateTime.Now);
                            return;
                        }
                        throw new ArtistException("Artist has already reached maximum allowance of Artworks in Gallery: "
                            + Artist.MAX_ARTWORKS_IN_GALLERY);
                    }
                    throw new GalleryException("The Gallery is already at maximum capacity.");
                case Artwork.ArtworkState.ReturnedToArtist:
                    artwork.ReturnToArtist();
                    break;
                case Artwork.ArtworkState.Sold:
                    artwork.Sell();
                    break;
                case Artwork.ArtworkState.AwaitingGalleryEntry:
                    artwork.SendToWaitingList();
                    break;
            }
        }

        /// <summary>
        /// Add a Customer to the Gallery.
        /// </summary>
        /// <param name="pNewCustomer">Reference to the Customer to add.</param>
        public void AddCustomer(Customer pNewCustomer)
        {
            mCustomers.Add(mCustomerIDCount++, pNewCustomer);
        }

        /// <summary>
        /// Find a Customer by it's ID.
        /// </summary>
        /// <param name="pID">ID of the Customer to search for.</param>
        /// <returns>Returns a reference to the found Customer. Returns null if not found.</returns>
        public Customer FindCustomer(int pID)
        {
            if (mCustomers.ContainsKey(pID))
            {
                return mCustomers[pID];
            }
            return null;
        }

        public List<Customer> FindCustomer(string pName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gives a new Artwork a new ID.
        /// </summary>
        /// <returns>New ID for an Artwork.</returns>
        public int GetNewArtworkID()
        {
            return mArtworkIDCount++;
        }

        /// <summary>
        /// Gives a new Order a new ID.
        /// </summary>
        /// <returns>New ID for an Order.</returns>
        public int GetNewOrderID()
        {
            return mOrderIDCount++;
        }

        /// <summary>
        /// Retrieve the unique ID of the Artist.
        /// </summary>
        /// <param name="pArtist">Artist to look for.</param>
        /// <returns>Returns the ID of the Artist. Throws ArtistException if not found in Gallery.</returns>
        public int GetArtistID(Artist pArtist)
        {
            // START_ID must be 1 for this code.
            /*
            int ArtistID = mArtists.FirstOrDefault(artist => artist.Value.Equals(pArtist)).Key;
            if (!(ArtistID == 0))
            {
                return ArtistID;
            }
            throw new ArtistException("Artist not found in the Gallery.");
            */
            foreach (KeyValuePair<int, Artist> KVPartist in mArtists)
            {
                if (KVPartist.Value.Equals(pArtist))
                {
                    return KVPartist.Key;
                }
            }
            return -1;
        }

        public int GetCustomerID(Customer pCustomer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the Gallery contains duplicate Artists.
        /// </summary>
        /// <returns>Returns a list of duplicated Artists.</returns>
        public List<Artist> CheckDuplicateArtists()
        {
            List<Artist> ArtistDuplicates = new List<Artist>();
            foreach (Artist artist in mArtists.Values)
            {
                foreach (Artist artistCheck in mArtists.Values)
                {
                    if (artist.Equals(artistCheck) && !ArtistDuplicates.Contains(artist))
                    {
                        ArtistDuplicates.Add(artist);
                    }
                }
            }
            return ArtistDuplicates;
        }

        public List<Customer> CheckDuplicateCustomers()
        {
            List<Customer> CustomerDuplicates = new List<Customer>();
            foreach (Customer customer in mCustomers.Values)
            {
                foreach (Customer customerCheck in mCustomers.Values)
                {
                }
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the MD5 checksum for the saved file.
        /// </summary>
        /// <param name="pFilename">File name of the saved file.</param>
        /// <returns>Returns byte array of the checksum.</returns>
        public byte[] GetMD5(string pFilename)
        {
            using (var MD5 = System.Security.Cryptography.MD5.Create())
            {
                using (var Stream = System.IO.File.OpenRead(pFilename))
                {
                    return MD5.ComputeHash(Stream);
                }
            }
        }

        public void XmlSave(XmlTextWriter pXmlOut)
        {
            pXmlOut.WriteStartDocument();
            pXmlOut.WriteStartElement("Gallery");
            pXmlOut.WriteElementString("ArtworkIDCount", mArtworkIDCount.ToString());
            pXmlOut.WriteElementString("ArtistIDCount", mArtistIDCount.ToString());
            pXmlOut.WriteElementString("OrderIDCount", mOrderIDCount.ToString());
            pXmlOut.WriteElementString("CustomerIDCount", mCustomerIDCount.ToString());
            foreach (Artist artist in mArtists.Values)
            {
                artist.XmlSave(pXmlOut);
            }
            pXmlOut.WriteEndElement();
            pXmlOut.WriteEndDocument();
        }

        public void XmlSave(string pFilename)
        {
            XmlTextWriter XmlOut = null;
            try
            {
                XmlOut = new XmlTextWriter(pFilename, Encoding.ASCII);
                XmlOut.Formatting = Formatting.Indented;
                XmlSave(XmlOut);
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (XmlOut != null)
                {
                    XmlOut.Close();
                }
            }
        }

        /// <summary>
        /// Gets the string version of the Gallery.
        /// </summary>
        /// <returns>Returns a string detailing the number of items in the Gallery.</returns>
        public override string ToString()
        {
            return "No. of Artists: " + mArtists.Count + ", No. of Customers: " + mCustomers.Count;
        }
    }
}
