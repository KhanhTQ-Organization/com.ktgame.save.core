using Sirenix.OdinInspector;

namespace com.ktgame.save.core
{
	[HideReferenceObjectPicker]
	public interface ISaveModel
	{
		int Version { get; set; }
	}
}
