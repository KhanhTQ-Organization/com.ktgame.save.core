using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public interface IDataTransform
	{
		byte[] Apply(byte[] data);

		UniTask<byte[]> ApplyAsync(byte[] data);

		byte[] Reverse(byte[] data);

		UniTask<byte[]> ReverseAsync(byte[] data);
	}
}
