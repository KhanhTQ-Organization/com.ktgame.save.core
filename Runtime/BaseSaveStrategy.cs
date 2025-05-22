using UnityEngine;

namespace com.ktgame.save.core
{
	public abstract class BaseSaveStrategy<TSaveModel, TDataModel> : ISaveStrategy<TSaveModel, TDataModel> 
		where TDataModel : IObscuredModel
		where TSaveModel : ISaveModel
	{
		public ISaveModel Save(IObscuredModel dataModel)
		{
			if (dataModel is TDataModel typedDataModel)
			{
				return Save(typedDataModel);
			}
			else
			{
				Debug.LogError("Type mismatch: Cannot cast IObscuredModel to the expected type.");
				return null;
			}
		}

		public abstract TSaveModel Save(TDataModel dataModel);
	}
}
