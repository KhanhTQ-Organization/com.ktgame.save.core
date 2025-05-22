using System;
using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public class CombinedDataTransform : IDataTransform
	{
		private readonly IDataTransform _firstTransform;
		private readonly IDataTransform _secondTransform;

		public CombinedDataTransform(IDataTransform firstTransform, IDataTransform secondTransform)
		{
			_firstTransform = firstTransform ?? throw new ArgumentNullException(nameof(firstTransform));
			_secondTransform = secondTransform ?? throw new ArgumentNullException(nameof(secondTransform));
		}

		public byte[] Apply(byte[] data)
		{
			return _secondTransform.Apply(_firstTransform.Apply(data));
		}

		public UniTask<byte[]> ApplyAsync(byte[] data)
		{
			return _firstTransform.ApplyAsync(data).ContinueWith(bytes => _secondTransform.ApplyAsync(bytes));
		}

		public byte[] Reverse(byte[] data)
		{
			return _firstTransform.Reverse(_secondTransform.Reverse(data));
		}

		public UniTask<byte[]> ReverseAsync(byte[] data)
		{
			return _secondTransform.ReverseAsync(data).ContinueWith(bytes => _firstTransform.ReverseAsync(bytes));
		}
	}
}
