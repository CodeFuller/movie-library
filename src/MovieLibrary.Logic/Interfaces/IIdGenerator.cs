namespace MovieLibrary.Logic.Interfaces
{
	public interface IIdGenerator<out TId>
	{
		TId GenerateId();
	}
}
