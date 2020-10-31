using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Interfaces
{
	/// <summary>
	/// For use with classes that would be used to retrieve a specific set of data and return that item as a data class that other
	/// parts of the program can then use.
	/// </summary>
	/// <typeparam name="T">Type of data to return.</typeparam>
	public interface IDataReceiver<T>
	{
		public Task<T> GetDataAsync(params string[] args);

		public T GetData(params string[] args);
	}
}
