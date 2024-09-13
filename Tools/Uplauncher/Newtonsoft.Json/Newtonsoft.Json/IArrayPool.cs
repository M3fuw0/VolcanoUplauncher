using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	/*[NullableContext(1)]*/
	public interface IArrayPool< T>
	{
		T[] Rent(int minimumLength);

		void Return(/*[Nullable(new byte[] { 2, 1 })]*/ T[] array);
	}
}
