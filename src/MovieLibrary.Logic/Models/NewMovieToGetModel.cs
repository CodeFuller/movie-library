using System;
using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Logic.Models
{
	public class NewMovieToGetModel
	{
		[Required]
		public Uri MovieUri { get; set; }
	}
}
