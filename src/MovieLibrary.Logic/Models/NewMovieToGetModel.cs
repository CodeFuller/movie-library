using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Logic.Models
{
	public class NewMovieToGetModel
	{
		[Required]
		public string Title { get; set; }
	}
}
