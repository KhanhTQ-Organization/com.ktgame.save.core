using System;

namespace com.ktgame.save.core
{
	public interface IGameSave
	{
		ISaveModel GetSaveModel(Type saveModelType);
		
		TSaveModel GetSaveModel<TSaveModel>() where TSaveModel : ISaveModel;

		bool TryGetSaveModel(Type saveModelType, out ISaveModel saveModel);
		
		bool TryGetSaveModel<TSaveModel>(out TSaveModel saveModel) where TSaveModel : ISaveModel;
	}
}
