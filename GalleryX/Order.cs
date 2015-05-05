using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GalleryBusiness
{
    /// <summary>
    /// An Order for an Artwork made by a Customer.
    /// </summary>
    class Order
    {
        private int mArtworkID;
        private DateTime mOrderDate;
        private Customer mCustomer;

        /// <summary>
        /// Create a new instance of an Order.
        /// </summary>
        /// <param name="inArtworkID">Unique ID of the ordered Artwork.</param>
        /// <param name="inOrderDate">Order date of the Order.</param>
        /// <param name="inCustomer">Reference to the Customer who ordered the Artwork.</param>
        public Order(int inArtworkID, DateTime inOrderDate, Customer inCustomer)
        {
            mArtworkID = inArtworkID;
            mOrderDate = inOrderDate;
            mCustomer = inCustomer;
        }

        /// <summary>
        /// The ID of the ordered Artwork.
        /// </summary>
        public int ArtworkID
        {
            get { return mArtworkID; }
            set { mArtworkID = value; }
        }

        /// <summary>
        /// Date of the order of the Artwork.
        /// </summary>
        public string OrderDate
        {
            get { return mOrderDate.ToString(); }
        }

        /// <summary>
        /// Write Order information to an Xml file stream.
        /// </summary>
        /// <param name="pXmlOut"><Xml stream to write to./param>
        /// <param name="pID">ID of the Order to write.</param>
        public void XmlSave(XmlTextWriter pXmlOut, int pID)
        {
            pXmlOut.WriteStartElement("Order");
            pXmlOut.WriteAttributeString("ID", pID.ToString());
            pXmlOut.WriteElementString("ArtworkID", mArtworkID.ToString());
            pXmlOut.WriteElementString("OrderDate", mOrderDate.ToString());
            pXmlOut.WriteEndElement();
        }

        /// <summary>
        /// Load an Order from an XmlElement.
        /// </summary>
        /// <param name="pOrderElement">XmlElement to extract information from.</param>
        /// <param name="pCustomer">Reference to the Customer who made the Order.</param>
        /// <returns>Returns a loaded Order.</returns>
        public static Order XmlLoad(XmlElement pOrderElement, Customer pCustomer)
        {
            int loadedArtworkID = int.Parse(pOrderElement["ArtworkID"].InnerText);
            DateTime loadedOrderDate = DateTime.Parse(pOrderElement["OrderDate"].InnerText);

            return new Order(loadedArtworkID, loadedOrderDate, pCustomer);
        }

        /// <summary>
        /// Gets the string value of the Order.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ID of Artwork: " + mArtworkID + ", Date of Order: " + OrderDate;
        }

        /// <summary>
        /// Checks if the passed object has the same content.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>Whether or not the two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            Order CompareOrder = (Order)obj;

            bool ArtworkID = (mArtworkID == CompareOrder.mArtworkID);
            bool OrderDate = (mOrderDate == CompareOrder.mOrderDate);

            if (ArtworkID && OrderDate)
            {
                return true;
            }
            return false;
        }
    }
}
