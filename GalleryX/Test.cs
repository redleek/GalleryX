using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalleryBusiness;

namespace Test
{
  class Program
  {
    static void Main()
    {
      /*
      Artwork a;
      do
	{
	  try
	    {
	      a = new Artwork(
			      "The Mona Lisa",
			      49.99m,
			      DateTime.Now,
			      Artwork.ArtworkType.Painting,
			      Artwork.ArtworkState.InGallery
			      );
	    }
	  catch(OverflowException e)
	    {
	      Console.WriteLine(e.Message);
	      continue;
	    }
	  catch(ArtworkException e)
	    {
	      Console.WriteLine(e.Message);
	      continue;
	    }
	  break;
	} while (true);
      Console.WriteLine(a);
      a.ChangeDescription("An Expensive painting");
      Console.WriteLine(a);
      */
      Artist NewArtist = new Artist();
      Artwork a = new Artwork("Mona Lisa", 49.99m, DateTime.Now, Artwork.ArtworkType.Painting, Artwork.ArtworkState.InGallery, NewArtist);
      NewArtist.AddArtwork(a);
      int ID = NewArtist.FindArtworkID(a);
      Console.WriteLine(ID);
      try
	{
	  a.AddToGallery();
	}
      catch(ArtworkException e)
	{
	  Console.WriteLine(e.Message);
	}
      Console.WriteLine(a);
      a.Save("save.txt");
      Artwork b = Artwork.Load("save.txt");
      Console.WriteLine(b);
    }
  }
}