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
      Artist NewArtist = new Artist("Rob Miles", 1);
      Artwork a = new Artwork("Mona Lisa", 49.99m, DateTime.Now, Artwork.ArtworkType.Painting, Artwork.ArtworkState.AwaitingGalleryEntry);
      NewArtist.AddArtwork(a);
      Console.WriteLine("New artwork: " + a);
      NewArtist.Save("save.txt");
      try
	{
	  a.AddToGallery(DateTime.Now);
	  Console.WriteLine("Artwork added to Gallery: " + a);
	}
      catch(ArtworkException e)
	{
	  Console.WriteLine(e.Message);
	}
      Console.WriteLine("New artist: " + NewArtist);
      //NewArtist.Save("save.txt");
      Artist loadedArtist = Artist.Load("save.txt");
      Console.WriteLine("Loaded artist: " + loadedArtist);
    }
  }
}