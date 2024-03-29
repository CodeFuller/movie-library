using System;
using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models
{
	public class NewMovieViewModel
	{
		[Required(ErrorMessage = "Please enter movie URL")]
		public Uri MovieUri { get; set; }

		[Required(ErrorMessage = "Please enter movie reference")]
		public string Reference { get; set; }
	}
}
