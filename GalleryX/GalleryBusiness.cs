using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace GalleryBusiness
{
  class ArtworkException : System.Exception
  {
    public ArtworkException(string message) : base(message)
      { }
  }

  class ArtworkExceptionBadPrice : ArtworkException
  {
    public ArtworkExceptionBadPrice(string message) : base(message)
      { }
  }

  class ArtworkExceptionBadDate : ArtworkException
  {
    public ArtworkExceptionBadDate(string message) : base(message)
      { }
  }

  class ArtworkExceptionBadStateTransfer : ArtworkException
  {
    public ArtworkExceptionBadStateTransfer(string message) : base(message)
      { }
  }

  class ArtworkExceptionNotInGallery : ArtworkException
  {
    public ArtworkExceptionNotInGallery(string message) : base(message)
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

    public const int MAX_PRICE = 1000000;

    public const int MIN_PRICE = 0;

    public const int MAX_DISPLAYDAYS_DIFFERENCE = 3650;
    
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
		    ArtworkState inState)
    {
      mDescription = inDescription;
      if (inPrice > MIN_PRICE)
	{
	  if (inPrice < MAX_PRICE)
	    {
	      mPrice = inPrice;
	    }
	  else
	    {
	      throw new ArtworkExceptionBadPrice("Price: " + inPrice + ", is too high. Above " + MAX_PRICE);
	    }
	}
      else
	{
	  throw new ArtworkExceptionBadPrice("Price: " + inPrice + ", is below " + MIN_PRICE);
	}
      if (inDisplayDates != null)
	{
	  mDisplayDates = inDisplayDates;
	}
      mType = inType;
      mState = inState;
    }
    
    /// <summary>
    /// Create a new instance of an Artwork.
    /// </summary>
    /// <param name="inDescription">A short description of the Artwork.</param>
    /// <param name="inPrice">The initial price of the Artwork.</param>
    /// <param name="inDisplayDate">The date at which the Artwork was put on display.</param>
    /// <param name="inType">The type of the Artwork. Either Painting or Sculpture.</param>
    /// <param name="inState">The current state of the Artwork.</param>
    public Artwork(string inDescription, decimal inPrice, DateTime inDisplayDate, ArtworkType inType, ArtworkState inState, Artist inOwner)
    : this(inDescription, inPrice, null, inType, inState)
    {
      double dateDifference = (inDisplayDate - DateTime.Now).TotalDays;
      if (dateDifference > MAX_DISPLAYDAYS_DIFFERENCE)
	{
	  throw new ArtworkExceptionBadDate("DateTime: " + inDisplayDate + " is too far back in the past. Date is "
						   + dateDifference + " days ago.");
	}
      mDisplayDates = new List<DateTime>();
      mDisplayDates.Add(inDisplayDate);
      mOwner = inOwner;
    }

    public string Description
    {
      get { return mDescription; }
    }

    public decimal Price
    {
      get { return mPrice; }
    }

    public List<DateTime> DisplayDates
    {
      get { return mDisplayDates; }
    }

    public DateTime MostRecentDisplayDate
    {
      get { return mDisplayDates.Last(); }
    }

    public ArtworkType Type
    {
      get { return mType; }
    }

    public ArtworkState State
    {
      get { return mState; }
    }

    public int ID
    {
      get { return mOwner.FindArtworkID(this); }
    }

    public bool GalleryTimeExpired(DateTime pCurrentDateTime)
    {
      if (mState == ArtworkState.ReturnedToArtist && (pCurrentDateTime - MostRecentDisplayDate).TotalDays > MAX_DISPLAY_DAYS)
	{
	  return true;
	}
      return false;
    }

    // Time since gallery time expired.
    public double TimeSinceExpired(DateTime pCurrentDateTime)
    {
      if (mState == ArtworkState.ReturnedToArtist)
	{
	  return (pCurrentDateTime - MostRecentDisplayDate).TotalDays - MAX_DISPLAY_DAYS;
	}
      throw new ArtworkException("Artwork \"" + mDescription + "\" has not been returned to the artist to have an expiratory date.");
    }

    public void ChangePrice(decimal pNewPrice)
    {
      if (pNewPrice > MIN_PRICE)
	{
	  if (pNewPrice < MAX_PRICE)
	    {
	      mPrice = pNewPrice;
	      return;
	    }
	  throw new ArtworkExceptionBadPrice("Price: " + pNewPrice + ", is too high. Above " + MAX_PRICE);
	}
      throw new ArtworkExceptionBadPrice("Price: " + pNewPrice + ", is below 0.");
    }

    public void ChangeDescription(string pNewDescription)
    {
      mDescription = pNewDescription;
    }

    public void AddToGallery()
    {
      if (mState != ArtworkState.InGallery)
	{
	  
	}
      throw new ArtworkException("Artwork: " + ID + ", is already in the Gallery.");
    }

    public void Save(System.IO.TextWriter pTextOut)
    {
      pTextOut.WriteLine(mDescription);
      pTextOut.WriteLine(mPrice);
      // Writes the list of display dates to file.
      pTextOut.WriteLine(mDisplayDates.Count);
      foreach (DateTime displaydate in mDisplayDates)
	{
	  pTextOut.WriteLine(displaydate);
	}
      pTextOut.WriteLine(mType);
      pTextOut.WriteLine(mState);
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
      int loadDisplayDatesCount = int.Parse(Console.ReadLine());
      List<DateTime> loadDisplayDates = new List<DateTime>();
      // Load each display date into new display dates list.
      for (int loadCount = 0; loadCount < loadDisplayDatesCount; loadCount++)
	{
	  loadDisplayDates.Add(DateTime.Parse(Console.ReadLine()));
	}
      ArtworkType loadType = (ArtworkType)Enum.Parse(
						     typeof(ArtworkType),
						     pTextIn.ReadLine()
						     );
      ArtworkState loadState = (ArtworkState)Enum.Parse(
							typeof(ArtworkState),
							pTextIn.ReadLine()
							);
      return new Artwork(loadDescription, loadPrice, loadDisplayDates, loadType, loadState);
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
    /// String format of the Artwork.
    /// </summary>
    /// <returns>Returns a string of all the data members of the Artwork.</returns>
      public override string ToString()
      {
	return "Description: " + mDescription + ", Price: £" + mPrice + ", Display date: " + MostRecentDisplayDate + ", Artwork type: " + mType + ", Artwork state: " + mState;
    }
  }
  
  /// <summary>
  /// An Artist who sells Artwork in a gallery.
  /// </summary>
  class Artist
  {
    private Dictionary<int, Artwork> mStock = new Dictionary<int, Artwork>();
    private int mStockID = 1;

    public Artist()
    { }

    public int FindArtworkID(Artwork pArtworkRef)
    {
      foreach (KeyValuePair<int, Artwork> kvp in mStock)
	{
	  if (kvp.Value == pArtworkRef)
	    {
	      return kvp.Key;
	    }
	}
      return 0;
    }

    public void AddArtwork(Artwork pNewArtwork)
    {
      mStock.Add(mStockID, pNewArtwork);
      mStockID++;
    }
  }
  /*
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
  */
}
