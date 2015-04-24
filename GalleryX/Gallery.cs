using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GalleryBusiness
{
    class Gallery
    {
        private string mName;
        private Dictionary<int, Artist> mArtists;

        /// <summary>
        /// Create a new instance of a Gallery loaded from file.
        /// </summary>
        /// <param name="inName">Name of the Gallery.</param>
        /// <param name="inArtists">List of Artists in the Gallery.</param>
        private Gallery(string inName, Dictionary<int, Artist> inArtists)
        {
            mName = inName;
            if (inArtists != null)
            {
                mArtists = inArtists;
            }
        }

        /// <summary>
        /// Create a new instance of a Gallery.
        /// </summary>
        /// <param name="inName">New name of of the Gallery.</param>
        public Gallery(string inName)
            : this(inName, new Dictionary<int, Artist>())
        { }

        public int GetNewArtworkID()
        {
            throw new Exception("you need to write this before you can use it.");
        }

        public int GetArtistID(Artist pArtist)
        {
            throw new Exception("you need to write this before you can use it.");
        }
    }
}
