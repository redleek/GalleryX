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
        const int MAX_ARTWORKS_IN_GALLERY = 5;

        /// <summary>
        /// Maximum number of characters allowed in a name.
        /// </summary>
        const int MAX_NAME_CHARS = 20;

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
        /// The ID of the artist in the Gallery.
        /// </summary>
        public int ID
        {
            get { return mSeller.GetArtistID(this); }
        }

        /// <summary>
        /// Add a new Artwork to the Artist.
        /// </summary>
        /// <param name="pNewArtwork">Reference to the new Artwork.</param>
        /// <param name="pNewID">Unique ID for the Artwork.</param>
        public void AddArtwork(int pNewID, Artwork pNewArtwork)
        {
            mStock.Add(pNewID, pNewArtwork);
        }

        /// <summary>
        /// Find an Artwork owned by an Artist by it's ID.
        /// </summary>
        /// <param name="pID">ID of the Artwork to find.</param>
        /// <returns>Returns a reference to the Artwork found. Null if not found.</returns>
        public Artwork FindArtwork(int pID)
        {
            foreach (KeyValuePair<int, Artwork> KVPartwork in mStock)
            {
                if (KVPartwork.Key == pID)
                {
                    return KVPartwork.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Find Artworks owned by an Artist by a description
        /// </summary>
        /// <param name="pDescription">Description to search for.</param>
        /// <returns>Returns a list of found Artworks or null if string is over max description characters or 0.</returns>
        public List<Artwork> FindArtwork(string pDescription)
        {
            pDescription = pDescription.Trim();
            if (pDescription.Length > 0 && pDescription.Length <= Artwork.MAX_DESCRIPTION_CHARS)
            {
                List<Artwork> FoundArtworks = new List<Artwork>();
                foreach (Artwork artwork in mStock.Values)
                {
                    if (artwork.Description.Contains(pDescription) ||
                        artwork.Description.ToLower().Contains(pDescription.ToLower()))
                    {
                        FoundArtworks.Add(artwork);
                    }
                }
                return FoundArtworks;
            }
            return null;
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
        /// Retrieve the unique ID of the Artwork.
        /// </summary>
        /// <param name="pArtwork">Artwork to look for.</param>
        /// <returns>Returns the ID of the Artwork.</returns>
        public int GetArtworkID(Artwork pArtwork)
        {
            foreach (KeyValuePair<int, Artwork> KVPartwork in mStock)
            {
                if (KVPartwork.Value.Equals(pArtwork))
                {
                    return KVPartwork.Key;
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if the Artist contains any duplicate Artworks.
        /// </summary>
        /// <returns>Returns a reference to a list of found duplicate Artworks.</returns>
        public List<Artwork> CheckDuplicates()
        {
            List<Artwork> ArtworkDuplicates = new List<Artwork>();
            foreach (Artwork artwork in mStock.Values)
            {
                foreach (Artwork artworkCheck in mStock.Values)
                {
                    if (artwork.Equals(artworkCheck) && !ArtworkDuplicates.Contains(artwork))
                    {
                        ArtworkDuplicates.Add(artwork);
                    }
                }
            }
            return ArtworkDuplicates;
        }

        public void XMLSave(XmlTextWriter pXMLOut)
        {
            pXMLOut.WriteStartDocument();
            pXMLOut.WriteStartElement("Artist");
            pXMLOut.WriteAttributeString("Name", mName);
            pXMLOut.WriteAttributeString("ID", "need to make ID property");
            foreach (Artwork artwork in mStock.Values)
            {
                artwork.XMLSave(pXMLOut);
            }
            pXMLOut.WriteEndElement();
            pXMLOut.WriteEndDocument();
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

        public void XMLSave(string pFilename)
        {
            XmlTextWriter XMLOut = null;
            System.IO.TextWriter MD5Out = null;
            try
            {
                XMLOut = new XmlTextWriter(pFilename, Encoding.ASCII);
                XMLOut.Formatting = Formatting.Indented;
                XMLSave(XMLOut);
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (XMLOut != null)
                {
                    XMLOut.Close();
                }
            }
            try
            {
                string MD5Filename = pFilename + "_md5.txt";
                byte[] MD5 = GetMD5(pFilename);
                MD5Out = new System.IO.StreamWriter(MD5Filename);
                foreach (byte b in MD5)
                {
                    MD5Out.WriteLine(b);
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (MD5Out != null)
                {
                    MD5Out.Close();
                }
            }
        }

        /// <summary>
        /// String format of the Artist.
        /// </summary>
        /// <returns>Returns a string of all the data members of the Artist.</returns>
        public override string ToString()
        {
            return "Name: " + mName + ", Number of stock items: " + mStock.Count + ", ID: " + ID;
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
