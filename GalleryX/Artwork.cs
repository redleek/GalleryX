﻿/*   Artwork.cs - Class file for Artworks.
*    Copyright (C) 2015  Connor Blakey <connorblakey96@gmail.com>.
*
*    This program is free software; you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation; version 2 of the License, or
*    (at your option) any later version.
*
*    This program is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.
*
*    You should have received a copy of the GNU General Public License along
*    with this program; if not, write to the Free Software Foundation, Inc.,
*    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GalleryBusiness
{
    /// <summary>
    /// Basic Exception class for Artworks.
    /// </summary>
    class ArtworkException : System.Exception
    {
        public ArtworkException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Invalid Artwork Description Exception.
    /// </summary>
    class ArtworkExceptionBadDescription : ArtworkException
    {
        public ArtworkExceptionBadDescription(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Invalid Artwork Price Exception.
    /// </summary>
    class ArtworkExceptionBadPrice : ArtworkException
    {
        public ArtworkExceptionBadPrice(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Invaild Artwork Date difference from display date Exception.
    /// </summary>
    class ArtworkExceptionBadDate : ArtworkException
    {
        public ArtworkExceptionBadDate(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Invalid Artwork state transfer Exception.
    /// </summary>
    class ArtworkExceptionBadStateTransfer : ArtworkException
    {
        public ArtworkExceptionBadStateTransfer(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Artwork does not exist in Gallery Exception.
    /// </summary>
    class ArtworkExceptionDoesNotExist : ArtworkException
    {
        public ArtworkExceptionDoesNotExist(string message)
            : base(message)
        { }
    }

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
        /// The maximum price of an Artwork.
        /// </summary>
        public const int MAX_PRICE = 1000000;

        /// <summary>
        /// The minimum price of an Artwork.
        /// </summary>
        public const int MIN_PRICE = 0;

        /// <summary>
        /// The maximum difference of days for a new display date from the current date.
        /// </summary>
        public const int MAX_DISPLAYDAYS_DIFFERENCE = 3650;

        /// <summary>
        /// Limits the amount of characters allowed in a description.
        /// </summary>
        public const int MAX_DESCRIPTION_CHARS = 140;

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
            AwaitingGalleryEntry,
            Sold,
            ReturnedToArtist
        }

        private string mDescription;
        private decimal mPrice;
        private List<DateTime> mDisplayDates;
        private ArtworkType mType;
        private ArtworkState mState;
        private Artist mOwner;

        /// <summary>
        /// Create a new instance of an Artwork loaded from file.
        /// </summary>
        /// <param name="inDescription">A short description of the Artwork.</param>
        /// <param name="inPrice">The initial price of the Artwork.</param>
        /// <param name="inDisplayDates">The date at which the Artwork was put on display.</param>
        /// <param name="inType">The type of the Artwork. Either Painting or Sculpture.</param>
        /// <param name="inState">The state of the Artwork.</param>
        private Artwork(string inDescription, decimal inPrice, List<DateTime> inDisplayDates, ArtworkType inType,
                ArtworkState inState, Artist inOwner)
        {
            if (!UpdateDescription(inDescription))
            {
                throw new ArtworkExceptionBadDescription("Description is blank.");
            }
            UpdatePrice(inPrice);
            if (inDisplayDates != null)
            {
                mDisplayDates = inDisplayDates;
            }
            mType = inType;
            mState = inState;
            mOwner = inOwner;
        }

        /// <summary>
        /// Create a new instance of an Artwork.
        /// </summary>
        /// <param name="inDescription">A short description of the Artwork.</param>
        /// <param name="inPrice">The initial price of the Artwork.</param>
        /// <param name="inDisplayDate">The date at which the Artwork was put on display.</param>
        /// <param name="inType">The type of the Artwork. Either Painting or Sculpture.</param>
        /// <param name="inState">The current state of the Artwork.</param>
        public Artwork(string inDescription, decimal inPrice, DateTime inDisplayDate, ArtworkType inType,
            ArtworkState inState, Artist inOwner)
            : this(inDescription, inPrice, null, inType, inState, inOwner)
        {
            if (DateDifference(inDisplayDate))
            {
                throw new ArtworkExceptionBadDate("DateTime: " + inDisplayDate + " is too far back in the past. Exceeds max days of: "
                    + MAX_DISPLAYDAYS_DIFFERENCE + ".");
            }
            mDisplayDates = new List<DateTime>();
            if (inState == ArtworkState.InGallery)
            {
                mDisplayDates.Add(inDisplayDate);
            }
        }

        /// <summary>
        /// Checks if a given date exceeds over the time of the set max date difference from current time.
        /// </summary>
        /// <param name="pCheckDate">DateTime to check.</param>
        /// <returns>Whether or not the date exceeds the limit.</returns>
        private bool DateDifference(DateTime pCheckDate)
        {
            double dateDifference = (pCheckDate - DateTime.Now).TotalDays;
            if (dateDifference > MAX_DISPLAYDAYS_DIFFERENCE)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the description of the Artwork.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }

        /// <summary>
        /// Gets the price of the Artwork.
        /// </summary>
        public decimal Price
        {
            get { return mPrice; }
        }

        /// <summary>
        /// Gets the most recent display date of the Artwork.
        /// </summary>
        private DateTime MostRecentDisplayDate
        {
            get
            {
                if (mDisplayDates.Count == 0)
                {
                    throw new ArtworkException("The Artwork has not yet been placed in the Gallery to have a Display Date.");
                }
                return mDisplayDates.Last();
            }
        }

        /// <summary>
        /// Get the string version of the most recent display date of the Artwork.
        /// </summary>
        public string MostRecentDisplayDateString
        {
            get
            {
                return MostRecentDisplayDate.ToString();
            }
        }

        /// <summary>
        /// Gets the type of the Artwork.
        /// </summary>
        public ArtworkType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        /// <summary>
        /// Gets the state of the Artwork.
        /// </summary>
        public ArtworkState State
        {
            get { return mState; }
        }

        /// <summary>
        /// Gets the reference to the owner of the Artwork.
        /// </summary>
        public Artist Owner
        {
            get { return mOwner; }
        }

        /// <summary>
        /// Check if the Artwork's time in the Gallery has expired.
        /// </summary>
        /// <param name="pCurrentDateTime">The time to check against the display date of the Artwork.</param>
        /// <returns>Whether or not the gallery time has expired.</returns>
        public bool GalleryTimeExpired(DateTime pCurrentDateTime)
        {
            if (mState == ArtworkState.ReturnedToArtist && (pCurrentDateTime - MostRecentDisplayDate).TotalDays > MAX_DISPLAY_DAYS)
            {
                return true;
            }
            return false;
        }

        // Time since gallery time expired.
        /// <summary>
        /// The time since the gallery time has expired.
        /// </summary>
        /// <param name="pCurrentDateTime">The time to check against the display date of the Artwork.</param>
        /// <returns>The amount of time since the Artwork's gallery time expired. If there is no time since expired, returns -1.</returns>
        public double TimeSinceExpired(DateTime pCurrentDateTime)
        {
            if (mState == ArtworkState.ReturnedToArtist)
            {
                return (pCurrentDateTime - MostRecentDisplayDate).TotalDays - MAX_DISPLAY_DAYS;
            }
            return -1;
        }

        /// <summary>
        /// Update the price of the Artwork.
        /// </summary>
        /// <param name="pNewPrice">New price to change to.</param>
        /// <returns>If the change of price was successful. Throws exceptions if pNewPrice is not between limits.</returns>
        public bool UpdatePrice(decimal pNewPrice)
        {
            if (pNewPrice > MIN_PRICE)
            {
                if (pNewPrice <= MAX_PRICE)
                {
                    mPrice = pNewPrice;
                    return true;
                }
                throw new ArtworkExceptionBadPrice("Price: " + pNewPrice + ", is too high. Above " + MAX_PRICE);
            }
            throw new ArtworkExceptionBadPrice("Price: " + pNewPrice + ", is below 0.");
        }

        /// <summary>
        /// Update the description of the Artwork.
        /// </summary>
        /// <param name="pNewDescription">New decription of the Artwork.</param>
        /// <returns>Whether or not the change of description was successful.</returns>
        public bool UpdateDescription(string pNewDescription)
        {
            pNewDescription = pNewDescription.Trim();
            if (pNewDescription != "")
            {
                if (!(pNewDescription.Length > MAX_DESCRIPTION_CHARS))
                {
                    mDescription = pNewDescription;
                    return true;
                }
                throw new ArtworkExceptionBadDescription("Description length is too long by " + (pNewDescription.Length - MAX_DESCRIPTION_CHARS) + "characters.");
            }
            return false;
        }

        /// <summary>
        /// Change the state of the Artwork to in the gallery.
        /// </summary>
        /// <param name="pNewDisplayDate">The date the artwork was placed into the gallery.</param>
        public void AddToGallery(DateTime pNewDisplayDate)
        {
            switch (mState)
            {
                case ArtworkState.InGallery:
                    throw new ArtworkExceptionBadStateTransfer("This Artwork is already in the Gallery.");
                case ArtworkState.Sold:
                    throw new ArtworkExceptionBadStateTransfer("This Artwork has already been sold to a customer.");
                case ArtworkState.ReturnedToArtist:
                case ArtworkState.AwaitingGalleryEntry:
                    if (DateDifference(pNewDisplayDate))
                    {
                        throw new ArtworkExceptionBadDate("DateTime: " + pNewDisplayDate + " is too far back in the past. Exceeds max days of: "
                    + MAX_DISPLAYDAYS_DIFFERENCE + ".");
                    }
                    mDisplayDates.Add(pNewDisplayDate);
                    mState = ArtworkState.InGallery;
                    break;
            }
        }

        /// <summary>
        /// Change the state of the Artwork to sold.
        /// </summary>
        public void Sell()
        {
            switch (mState)
            {
                case ArtworkState.Sold:
                    throw new ArtworkExceptionBadStateTransfer("Artwork is already sold.");
                case ArtworkState.AwaitingGalleryEntry:
                    throw new ArtworkExceptionBadStateTransfer("Artwork has not been in the Gallery to sell.");
                case ArtworkState.ReturnedToArtist:
                case ArtworkState.InGallery:
                    mState = ArtworkState.Sold;
                    break;
            }
        }

        /// <summary>
        /// Change the state of the Artwork to returned to the Artist.
        /// </summary>
        public void ReturnToArtist()
        {
            switch (mState)
            {
                case ArtworkState.ReturnedToArtist:
                    throw new ArtworkExceptionBadStateTransfer("Artwork is already returned to Artist.");
                case ArtworkState.Sold:
                    throw new ArtworkExceptionBadStateTransfer("Artwork has been sold to a customer already.");
                case ArtworkState.AwaitingGalleryEntry:
                case ArtworkState.InGallery:
                    mState = ArtworkState.ReturnedToArtist;
                    break;
            }
        }

        public void SendToWaitingList()
        {
            switch (mState)
            {
                case ArtworkState.Sold:
                    throw new ArtworkExceptionBadStateTransfer("Artwork has been sold to a customer already.");
                case ArtworkState.AwaitingGalleryEntry:
                    throw new ArtworkExceptionBadStateTransfer("Artwork is already awaiting Gallery entry.");
                case ArtworkState.ReturnedToArtist:
                case ArtworkState.InGallery:
                    mState = ArtworkState.AwaitingGalleryEntry;
                    break;
            }
        }

        /// <summary>
        /// Write Artwork information to an Xml file stream.
        /// </summary>
        /// <param name="pXmlOut">XML stream to write to.</param>
        /// <param name="pID">ID of the Artwork to write.</param>
        public void XmlSave(XmlTextWriter pXmlOut, int pID)
        {
            pXmlOut.WriteStartElement("Artwork");
            pXmlOut.WriteAttributeString("ID", pID.ToString());
            pXmlOut.WriteElementString("Description", mDescription);
            pXmlOut.WriteElementString("Price", mPrice.ToString());

            if (mDisplayDates.Count > 0)
            {
                foreach (DateTime DisplayDate in mDisplayDates)
                {
                    pXmlOut.WriteStartElement("DisplayDate");
                    pXmlOut.WriteString(DisplayDate.ToString());
                    pXmlOut.WriteEndElement();
                }
            }

            pXmlOut.WriteElementString("Type", mType.ToString());
            pXmlOut.WriteElementString("State", mState.ToString());
            pXmlOut.WriteEndElement();
        }

        /// <summary>
        /// Load an Artwork from an XmlElement.
        /// </summary>
        /// <param name="pArtworkElement">XmlElement to extract information from.</param>
        /// <param name="pOwner">Refernece to the owner(Artist) of the Artwork.</param>
        /// <returns>Returns a loaded Artwork.</returns>
        public static Artwork XmlLoad(XmlElement pArtworkElement, Artist pOwner)
        {
            string loadedDescription = pArtworkElement["Description"].InnerText;
            decimal loadedPrice = decimal.Parse(pArtworkElement["Price"].InnerText);
            ArtworkType loadedType = (ArtworkType)Enum.Parse(
                typeof(ArtworkType),
                pArtworkElement["Type"].InnerText
                );
            ArtworkState loadedState = (ArtworkState)Enum.Parse(
                typeof(ArtworkState),
                pArtworkElement["State"].InnerText
                );

            List<DateTime> loadedDisplayDates = new List<DateTime>();
            Artwork loadedArtwork = new Artwork(
                loadedDescription,
                loadedPrice,
                loadedDisplayDates,
                loadedType,
                loadedState,
                pOwner
                );

            XmlNodeList displayDateNodeList = pArtworkElement.GetElementsByTagName("DisplayDate");
            foreach (XmlNode displayDateNode in displayDateNodeList)
            {
                loadedDisplayDates.Add(DateTime.Parse(displayDateNode.InnerText));
            }

            return loadedArtwork;
        }

        /// <summary>
        /// String format of the Artwork.
        /// </summary>
        /// <returns>Returns a string of all the data members of the Artwork.</returns>
        public override string ToString()
        {
            string InGalleryDisplayDate = "";
            if (mState == ArtworkState.InGallery)
            {
                InGalleryDisplayDate = ", Display Date: " + MostRecentDisplayDateString;
            }
            return "Description: " + mDescription + ", Price: £" + mPrice + InGalleryDisplayDate + ", Artwork type: " + mType + ", Artwork state: " + mState;
        }

        /// <summary>
        /// Checks to see if the passed Artwork has the same content as the current one.
        /// </summary>
        /// <param name="obj">The Artwork to check by comparison.</param>
        /// <returns>Whether or not the Artworks have equal content.</returns>
        public override bool Equals(object obj)
        {
            Artwork comparison = (Artwork)obj;

            bool Description = (mDescription == comparison.mDescription);
            bool Price = (mPrice == comparison.mPrice);
            bool Type = (mType == comparison.mType);
            bool State = (mState == comparison.mState);

            if (Description && Price && Type && State)
            {
                return true;
            }
            return false;
        }
    }
}
