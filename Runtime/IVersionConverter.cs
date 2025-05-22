namespace com.ktgame.save.core
{
	public interface IVersionConverter<in TFrom, out TTo> where TFrom : ISaveModel where TTo : ISaveModel
	{
		int FromVersion { get; }
		
		int ToVersion { get; }

		TTo Convert(TFrom from);
	}
}
