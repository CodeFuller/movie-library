using System;
using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models
{
	public class NewMovieViewModel
	{
		[Required]
		public Uri MovieUri { get; set; }
	}
}
