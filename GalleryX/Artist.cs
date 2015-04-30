using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GalleryBusiness
{
    /// <summary>
    /// Basic Exception class for Artist.
    /// </summary>
    class ArtistException : System.Exception
    {
        public ArtistException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Artist invalid name Exception.
    /// </summary>
    class ArtistBadNameException : ArtistException
    {
        public ArtistBadNameException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Duplicate Artworks Exception.
    /// </summary>
    class ArtistDuplicateArtworksException : ArtistException
    {
        public ArtistDuplicateArtworksException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// An Artist who sells Artworks in a gallery.
    /// </summary>
    class Artist
    {
        /// <summary>
        /// Maximum number of Artworks owned by an Artist allowed to be in the Gallery at once.
        /// </summary>
        public const int MAX_ARTWORKS_IN_GALLERY = 5;

        /// <summary>
        /// Maximum number of characters allowed in a name.
        /// </summary>
        public const int MAX_NAME_CHARS = 20;

        private string mName;
        private Dictionary<int, Artwork> mStock;
        private Gallery mSeller;

        /// <summary>
        /// Create a new instance of an Artist loaded from file.
        /// </summary>
        /// <param name="inName">Loaded name of the Artist.</param>
        /// <param name="inStock">Loaded stock of the Artist.</param>
        private Artist(string inName, Dictionary<int, Artwork> inStock, Gallery inSeller)
        {
            if (!UpdateName(inName))
            {
                throw new ArtworkExceptionBadDescription("Name is blank.");
            }
            mStock = inStock;
            mSeller = inSeller;
        }

        /// <summary>
        /// Create a new instance of an Artist.
        /// </summary>
        /// <param name="inName">New name of the Artist.</param>
        /// <param name="inSeller">Reference to the Gallery that sells the Artist's Artworks.</param>
        public Artist(string inName, Gallery inSeller)
            : this(inName, new Dictionary<int, Artwork>(), inSeller)
        { }

        /// <summary>
        /// The name of the Artist.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// The amount of Artist's Artworks currently in the Gallery.
        /// </summary>
        public int ArtworksInGalleryCount
        {
            get
            {
                int inGalleryCount = 0;
                foreach (Artwork artwork in mStock.Values)
                {
                    if (artwork.State == Artwork.ArtworkState.InGallery)
                    {
                        inGalleryCount++;
                    }
                }
                return inGalleryCount;
            }
        }

        /// <summary>
        /// Add a new Artwork to the Artist.
        /// </summary>
        /// <param name="pNewArtwork">Reference to the new Artwork.</param>
        public void AddArtwork(Artwork pNewArtwork)
        {
            if (pNewArtwork.State != Artwork.ArtworkState.InGallery ||
                ArtworksInGalleryCount != 5)
            {
                mStock.Add(mSeller.GetNewArtworkID(), pNewArtwork);
            }
            else
            {
                throw new ArtistException("Artist has already reached maximum allowance of Artworks in Gallery: "
                    + Artist.MAX_ARTWORKS_IN_GALLERY);
            }
        }

        /// <summary>
        /// Find an Artwork owned by an Artist by it's ID.
        /// </summary>
        /// <param name="pID">ID of the Artwork to find.</param>
        /// <returns>Returns a KeyValuePair to the Artwork found and it's ID.</returns>
        public KeyValuePair<int, Artwork> FindArtwork(int pID)
        {
            if (mStock.ContainsKey(pID))
            {
                return new KeyValuePair<int, Artwork>(pID, mStock[pID]);
            }
            return new KeyValuePair<int, Artwork>(-1, null);
        }

        /// <summary>
        /// Find Artworks owned by an Artist by a description.
        /// </summary>
        /// <param name="pDescription">Description to search for.</param>
        /// <returns>Returns a list of found Keys and Artworks. Throws exception if string is invalid.</returns>
        public List<KeyValuePair<int, Artwork>> FindArtwork(string pDescription)
        {
            pDescription = pDescription.Trim();
            if (pDescription.Length > 0 && pDescription.Length <= Artwork.MAX_DESCRIPTION_CHARS)
            {
                List<KeyValuePair<int, Artwork>> FoundArtworks = new List<KeyValuePair<int, Artwork>>();
                foreach (KeyValuePair<int, Artwork> KVPartwork in mStock)
                {
                    if (KVPartwork.Value.Description.Contains(pDescription) ||
                        KVPartwork.Value.Description.ToLower().Contains(pDescription.ToLower()))
                    {
                        FoundArtworks.Add(KVPartwork);
                    }
                }
                return FoundArtworks;
            }
            throw new ArtworkException("Entered string contains too many characters, limit: " + Artwork.MAX_DESCRIPTION_CHARS);
        }

        /// <summary>
        /// Update the name of the Artist.
        /// </summary>
        /// <param name="pNewName">New name for the Artist.</param>
        /// <returns>Whether or not the name was changed.</returns>
        public bool UpdateName(string pNewName)
        {
            pNewName = pNewName.Trim();
            if (pNewName != "")
            {
                if (!(pNewName.Length > MAX_NAME_CHARS))
                {
                    mName = pNewName;
                    return true;
                }
                throw new ArtistBadNameException("Description length is too long by " + (pNewName.Length - MAX_NAME_CHARS) + "characters.");
            }
            return false;
        }

        /// <summary>
        /// Checks if an Artwork is already owned by an Artist.
        /// </summary>
        /// <param name="pArtwork">Reference to the Artwork to check.</param>
        /// <returns>Returns the KeyValuePair to the origional Artwork if a duplicate has been found.</returns>
        public KeyValuePair<int, Artwork> CheckDuplicates(Artwork pArtwork)
        {
            foreach (KeyValuePair<int, Artwork> KVPartwork in mStock)
            {
                if (KVPartwork.Value.Equals(pArtwork))
                {
                    return KVPartwork;
                }
            }
            return new KeyValuePair<int, Artwork>(-1, null);
        }

        /// <summary>
        /// Writes Artist information to an Xml file stream.
        /// </summary>
        /// <param name="pXmlOut">Xml stream to write to.</param>
        /// <param name="pID">ID of the Artist to write.</param>
        public void XmlSave(XmlTextWriter pXmlOut, int pID)
        {
            pXmlOut.WriteStartElement("Artist");
            pXmlOut.WriteAttributeString("ID", pID.ToString());
            pXmlOut.WriteElementString("Name", mName);
            foreach (KeyValuePair<int, Artwork> artwork in mStock)
            {
                artwork.Value.XmlSave(pXmlOut, artwork.Key);
            }
            pXmlOut.WriteEndElement();
        }

        /// <summary>
        /// Load an Artist from an XmlElement.
        /// </summary>
        /// <param name="pArtistElement">XmlElement to extract information from.</param>
        /// <param name="pSeller">Refernece to the Gallery.</param>
        /// <returns>Returns a loaded Artist.</returns>
        public static Artist XmlLoad(XmlElement pArtistElement, Gallery pSeller)
        {
            string loadedName = pArtistElement["Name"].InnerText;

            Dictionary<int, Artwork> loadedStock = new Dictionary<int, Artwork>();
            Artist loadedArtist = new Artist(loadedName, loadedStock, pSeller);

            XmlNodeList artworkNodeList = pArtistElement.GetElementsByTagName("Artwork");
            foreach (XmlNode artworkNode in artworkNodeList)
            {
                XmlElement artworkElement = (XmlElement)artworkNode;
                int loadedArtworkID = int.Parse(artworkElement.GetAttribute("ID"));
                loadedStock.Add(loadedArtworkID, Artwork.XmlLoad(artworkElement, loadedArtist));
            }

            return loadedArtist;
        }

        /// <summary>
        /// String format of the Artist.
        /// </summary>
        /// <returns>Returns a string of all the data members of the Artist.</returns>
        public override string ToString()
        {
            return "Name: " + mName + ", Number of stock items: " + mStock.Count;
        }

        /// <summary>
        /// Checks to see if the passed Artist has the same content as the current one.
        /// </summary>
        /// <param name="obj">The Artist to check by comparison.</param>
        /// <returns>Whether or not the Artists have equal content.</returns>
        public override bool Equals(object obj)
        {
            Artist comparison = (Artist)obj;

            bool Name = (mName == comparison.Name);

            if (Name)
            {
                return true;
            }
            return false;
        }
    }
}
