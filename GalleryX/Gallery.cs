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
        /// <returns>If found, returns the KeyValuePair of the Artist.</returns>
        public KeyValuePair<int, Artist> FindArtist(int pID)
        {
            if (mArtists.ContainsKey(pID))
            {
                return new KeyValuePair<int, Artist>(pID, mArtists[pID]);
            }
            return new KeyValuePair<int, Artist>(-1, null);
        }

        /// <summary>
        /// Find an Artist by it's name.
        /// </summary>
        /// <param name="pName">Name of the Artist to search for.</param>
        /// <returns>Returns a list of all Artists whose names contain the given string. Throws exception if string is invalid.</returns>
        public List<KeyValuePair<int, Artist>> FindArtist(string pName)
        {
            pName = pName.Trim();
            if (pName.Length > 0 && pName.Length <= Artist.MAX_NAME_CHARS)
            {
                List<KeyValuePair<int, Artist>> FoundArtists = new List<KeyValuePair<int, Artist>>();
                foreach (KeyValuePair<int, Artist> KVPartist in mArtists)
                {
                    if (KVPartist.Value.Name.Contains(pName) ||
                        KVPartist.Value.Name.ToLower().Contains(pName.ToLower()))
                    {
                        FoundArtists.Add(KVPartist);
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
                    Artist artist = FindArtist(pArtistID).Value;
                    artist.AddArtwork(pNewArtwork);
                    return;
                }
                throw new ArtistDuplicateArtworksException("Artwork is a duplicate of artwork:" + artistDuplicateCheck);
            }
        }

        /// <summary>
        /// Find an Artwork by it's ID.
        /// </summary>
        /// <param name="pID">Unique ID of the artwork to look for.</param>
        /// <returns>Returns a reference to the found Artwork. Returns null if not found.</returns>
        public KeyValuePair<int, Artwork> FindArtwork(int pID)
        {
            KeyValuePair<int, Artwork> KVPartwork;
            foreach (Artist artist in mArtists.Values)
            {
                if ((KVPartwork = artist.FindArtwork(pID)).Key != -1)
                {
                    return KVPartwork;
                }
            }
            return new KeyValuePair<int, Artwork>(-1, null);
        }

        /// <summary>
        /// Find Artworks owned by an Artist by a description.
        /// </summary>
        /// <param name="pDescription">Description to search for.</param>
        /// <returns>Returns a list of found Artworks.</returns>
        public List<KeyValuePair<int, Artwork>> FindArtwork(string pDescription)
        {
            List<KeyValuePair<int, Artwork>> FoundArtworks = new List<KeyValuePair<int, Artwork>>();
            foreach (Artist artist in mArtists.Values)
            {
                List<KeyValuePair<int, Artwork>> thisArtistFoundArtworks = new List<KeyValuePair<int, Artwork>>();
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
        /// <param name="pNewState">New state to change the Artwork to.</param>
        /// <param name="pAddToGalleryDate">Date the Artwork was added to the Gallery.</param>
        public void ChangeArtworkState(int pID, Artwork.ArtworkState pNewState, DateTime pAddToGalleryDate)
        {
            KeyValuePair<int, Artwork> KVPartwork;
            if ((KVPartwork = FindArtwork(pID)).Key == -1)
            {
                throw new ArtworkExceptionDoesNotExist("Artwork not found.");
            }

            switch (pNewState)
            {
                case Artwork.ArtworkState.InGallery:
                    if (ArtworksInGallery != GALLERY_CAPACITY)
                    {
                        if (KVPartwork.Value.Owner.ArtworksInGalleryCount
                            != Artist.MAX_ARTWORKS_IN_GALLERY)
                        {
                            KVPartwork.Value.AddToGallery(DateTime.Now);
                            return;
                        }
                        throw new ArtistException("Artist has already reached maximum allowance of Artworks in Gallery: "
                            + Artist.MAX_ARTWORKS_IN_GALLERY);
                    }
                    throw new GalleryException("The Gallery is already at maximum capacity.");
                case Artwork.ArtworkState.ReturnedToArtist:
                    KVPartwork.Value.ReturnToArtist();
                    break;
                case Artwork.ArtworkState.Sold:
                    KVPartwork.Value.Sell();
                    break;
                case Artwork.ArtworkState.AwaitingGalleryEntry:
                    KVPartwork.Value.SendToWaitingList();
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
        /// <returns>Returns the KeyValuePair of the found Customer.</returns>
        public KeyValuePair<int, Customer> FindCustomer(int pID)
        {
            if (mCustomers.ContainsKey(pID))
            {
                return new KeyValuePair<int, Customer>(pID, mCustomers[pID]);
            }
            return new KeyValuePair<int, Customer>(-1, null);
        }

        /// <summary>
        /// Find Customers by name.
        /// </summary>
        /// <param name="pName">String to search for.</param>
        /// <returns>Returns a list of KeyValuePairs of any found Customers.</returns>
        public List<KeyValuePair<int, Customer>> FindCustomer(string pName)
        {
            pName = pName.Trim();
            if (pName.Length > 0 && pName.Length <= Customer.MAX_NAME_CHARS)
            {
                List<KeyValuePair<int, Customer>> FoundCustomers = new List<KeyValuePair<int, Customer>>();
                foreach (KeyValuePair<int, Customer> KVPcustomer in mCustomers)
                {
                    if (KVPcustomer.Value.Name.Contains(pName) ||
                        KVPcustomer.Value.Name.ToLower().Contains(pName.ToLower()))
                    {
                        FoundCustomers.Add(KVPcustomer);
                    }
                }
                return FoundCustomers;
            }
            throw new CustomerException("Entered string contains characters, limit: " + Customer.MAX_NAME_CHARS);
        }

        /// <summary>
        /// Add an Order to a Customer.
        /// </summary>
        /// <param name="pCustomerID">ID of the Customer to add to.</param>
        /// <param name="pNewOrder">Reference to the new Order.</param>
        public void AddOrder(int pCustomerID, Order pNewOrder)
        {
            foreach (Customer customerDuplicateCheck in mCustomers.Values)
            {
                if (!customerDuplicateCheck.CheckDuplicates(pNewOrder))
                {
                    Customer customer = FindCustomer(pCustomerID).Value;
                    customer.AddOrder(pNewOrder);
                }
            }
        }

        /// <summary>
        /// Find an Order by it's ID.
        /// </summary>
        /// <param name="pID">ID to search for.</param>
        /// <returns></returns>
        public KeyValuePair<int, Order> FindOrderByID(int pID)
        {
            KeyValuePair<int, Order> KVPorder;
            foreach (Customer customer in mCustomers.Values)
            {
                if ((KVPorder = customer.FindOrderByID(pID)).Key != -1)
                {
                    return KVPorder;
                }
            }
            return new KeyValuePair<int, Order>(-1, null);
        }

        /// <summary>
        /// Find an Order by the ID of the Artwork it includes.
        /// </summary>
        /// <param name="pArtworkID">Artwork ID to search for.</param>
        /// <returns>Returns the KeyValuePair of the found Order.</returns>
        public KeyValuePair<int, Order> FindOrderByArtworkID(int pArtworkID)
        {
            KeyValuePair<int, Order> KVPorder;
            foreach (Customer customer in mCustomers.Values)
            {
                if ((KVPorder = customer.FindOrderByArtworkID(pArtworkID)).Key != -1)
                {
                    return KVPorder;
                }
            }
            return new KeyValuePair<int, Order>(-1, null);
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

        /// <summary>
        /// Checks if the Gallery contains duplicate Customers.
        /// </summary>
        /// <returns>Retuns a list of duplicated Customers.</returns>
        public List<Customer> CheckDuplicateCustomers()
        {
            List<Customer> CustomerDuplicates = new List<Customer>();
            foreach (Customer customer in mCustomers.Values)
            {
                foreach (Customer customerCheck in mCustomers.Values)
                {
                    if (customer.Equals(customerCheck) && !CustomerDuplicates.Contains(customer))
                    {
                        CustomerDuplicates.Add(customer);
                    }
                }
            }
            return CustomerDuplicates;
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

        /// <summary>
        /// Save key Gallery information to a XmlText stream.
        /// </summary>
        /// <param name="pXmlOut">XmlText stream to write to.</param>
        public void XmlSave(XmlTextWriter pXmlOut)
        {
            pXmlOut.WriteStartDocument();
            pXmlOut.WriteStartElement("Gallery");
            pXmlOut.WriteElementString("ArtworkIDCount", mArtworkIDCount.ToString());
            pXmlOut.WriteElementString("ArtistIDCount", mArtistIDCount.ToString());
            pXmlOut.WriteElementString("OrderIDCount", mOrderIDCount.ToString());
            pXmlOut.WriteElementString("CustomerIDCount", mCustomerIDCount.ToString());

            foreach (KeyValuePair<int, Artist> artist in mArtists)
            {
                artist.Value.XmlSave(pXmlOut, artist.Key);
            }

            foreach (KeyValuePair<int, Customer> customer in mCustomers)
            {
                customer.Value.XmlSave(pXmlOut, customer.Key);
            }

            pXmlOut.WriteEndElement();
            pXmlOut.WriteEndDocument();
        }

        /// <summary>
        /// Save a Gallery to an Xml file.
        /// </summary>
        /// <param name="pFilename">Filename of the file to save to.</param>
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
        /// Load Gallery information from XmlElement.
        /// </summary>
        /// <param name="pGalleryElement">XmlElement to extract information from.</param>
        /// <returns>Returns a loaded Gallery.</returns>
        public static Gallery XmlLoad(XmlElement pGalleryElement)
        {
            int loadedArtworkIDCount = int.Parse(pGalleryElement["ArtworkIDCount"].InnerText);
            int loadedArtistIDCount = int.Parse(pGalleryElement["ArtistIDCount"].InnerText);
            int loadedOrderIDCount = int.Parse(pGalleryElement["OrderIDCount"].InnerText);
            int loadedCustomerIDCount = int.Parse(pGalleryElement["OrderIDCount"].InnerText);

            Dictionary<int, Artist> loadedArtists = new Dictionary<int, Artist>();
            Dictionary<int, Customer> loadedCustomers = new Dictionary<int, Customer>();

            Gallery loadedGallery = new Gallery(loadedArtists, loadedCustomers, loadedArtworkIDCount, loadedArtistIDCount, loadedOrderIDCount, loadedCustomerIDCount);

            XmlNodeList artistNodeList = pGalleryElement.GetElementsByTagName("Artist");
            foreach (XmlNode artistNode in artistNodeList)
            {
                XmlElement artistElement = (XmlElement)artistNode;
                int loadedArtistID = int.Parse(artistElement.GetAttribute("ID"));
                loadedArtists.Add(loadedArtistID, Artist.XmlLoad(artistElement, loadedGallery));
            }

            XmlNodeList customerNodeList = pGalleryElement.GetElementsByTagName("Customer");
            foreach (XmlNode customerNode in customerNodeList)
            {
                XmlElement customerElement = (XmlElement)customerNode;
                int loadedCustomerID = int.Parse(customerElement.GetAttribute("ID"));
                loadedCustomers.Add(loadedCustomerID, Customer.XmlLoad(customerElement, loadedGallery));
            }

            return loadedGallery;
        }

        /// <summary>
        /// Load a Gallery from Xml file.
        /// </summary>
        /// <param name="pFilename">Filename to load from.</param>
        /// <returns>A loaded Gallery.</returns>
        public static Gallery XmlLoad(string pFilename)
        {
            Gallery outGallery;
            XmlDocument loadedDocument = new XmlDocument();
            try
            {
                loadedDocument.Load(pFilename);
                outGallery = XmlLoad(loadedDocument.DocumentElement);
            }
            catch (Exception E)
            {
                throw E;
            }
            return outGallery;
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
