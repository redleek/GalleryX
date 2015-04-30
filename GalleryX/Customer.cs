using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GalleryBusiness
{
    class Customer
    {
        /// <summary>
        /// Basic Exception class for Customers.
        /// </summary>
        class CustomerException : System.Exception
        {
            public CustomerException(string message)
                : base(message)
            { }
        }

        /// <summary>
        /// Incorrect name Exception.
        /// </summary>
        class CustomerExceptionBadName : CustomerException
        {
            public CustomerExceptionBadName(string message)
                : base(message)
            { }
        }

        /// <summary>
        /// Maximum allowed characters in Name.
        /// </summary>
        public const int MAX_NAME_CHARS = 20;

        private string mName;
        private Dictionary<int, Order> mOrders;
        private Gallery mGallery;

        /// <summary>
        /// Create a new instance of a Customer loaded from file.
        /// </summary>
        /// <param name="inName">Name of the Customer.</param>
        /// <param name="inOrders">Dictionary of Orders of the Customer.</param>
        /// <param name="inGallery">Reference to the Gallery where the Orders are being made.</param>
        private Customer(string inName, Dictionary<int, Order> inOrders, Gallery inGallery)
        {
            if (!UpdateName(inName))
            {
                throw new CustomerExceptionBadName("Name is blank.");
            }
            mOrders = inOrders;
            mGallery = inGallery;
        }

        /// <summary>
        /// Create a new instance of a Customer.
        /// </summary>
        /// <param name="inName">Name of the Customer.</param>
        /// <param name="inGallery">Reference to the Gallery where the Orders are made.</param>
        public Customer(string inName, Gallery inGallery)
            : this(inName, new Dictionary<int, Order>(), inGallery)
        { }

        /// <summary>
        /// Update the name of the Customer.
        /// </summary>
        /// <param name="pNewName">New name of the Customer.</param>
        /// <returns>Returns whether or not the name is valid.</returns>
        private bool UpdateName(string pNewName)
        {
            pNewName = pNewName.Trim();
            if (pNewName != "")
            {
                if (pNewName.Length <= MAX_NAME_CHARS)
                {
                    mName = pNewName;
                    return true;
                }
                throw new CustomerExceptionBadName("Name length is too long by "
                    + (pNewName.Length - MAX_NAME_CHARS) + " characters.");
            }
            return false;
        }

        /// <summary>
        /// Name of the Customer.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Add a new order for the Customer.
        /// </summary>
        /// <param name="pNewOrder">Reference to the new order to add.</param>
        public void AddOrder(Order pNewOrder)
        {
            mOrders.Add(mGallery.GetNewOrderID(), pNewOrder);
        }

        /// <summary>
        /// Find an Order by it's ID.
        /// </summary>
        /// <param name="pID">ID of the order to search for.</param>
        /// <returns>Returns the KeyValuePair of the found order.</returns>
        public KeyValuePair<int, Order> FindOrderByID(int pID)
        {
            if (mOrders.ContainsKey(pID))
            {
                return new KeyValuePair<int,Order>(pID, mOrders[pID]);
            }
            return new KeyValuePair<int,Order>(-1, null);
        }

        /// <summary>
        /// Find an Order by the ID of the Artwork it includes.
        /// </summary>
        /// <param name="pArtworkID">ID of the Artwork in the Order to search for.</param>
        /// <returns>Returns the KeyValuePair of the found Artwork.</returns>
        public KeyValuePair<int, Order> FindOrderByArtworkID(int pArtworkID)
        {
            foreach (KeyValuePair<int, Order> KVPorder in mOrders)
            {
                if (KVPorder.Value.ArtworkID == pArtworkID)
                {
                    return KVPorder;
                }
            }
            return new KeyValuePair<int,Order>(-1, null);
        }
    }
}
