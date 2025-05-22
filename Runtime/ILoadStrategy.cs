namespace com.ktgame.save.core
{
	public interface ILoadStrategy
	{
		void Load(ISaveModel saveModel, IObscuredModel dataModel, bool firstLoad);
	}

	public interface ILoadStrategy<in TSaveModel, in TDataModel> : ILoadStrategy where TSaveModel : ISaveModel where TDataModel : IObscuredModel
	{
		void Load(TSaveModel saveModel, TDataModel dataModel, bool firstLoad);
	}
}
