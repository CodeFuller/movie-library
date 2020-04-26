using System;
using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models
{
	public class NewMovieToGetViewModel
	{
		[Required]
		public Uri MovieUri { get; set; }
	}
}
