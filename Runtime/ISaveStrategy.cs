namespace com.ktgame.save.core
{
	public interface ISaveStrategy
	{
		ISaveModel Save(IObscuredModel dataModel);
	}

	public interface ISaveStrategy<out TSaveModel, in TDataModel> : ISaveStrategy where TDataModel : IObscuredModel where TSaveModel : ISaveModel
	{
		TSaveModel Save(TDataModel dataModel);
	}
}
