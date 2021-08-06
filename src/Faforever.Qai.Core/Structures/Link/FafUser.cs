using System.Text.Json.Serialization;

namespace Faforever.Qai.Core.Structures.Link
{
	public class FafUser
	{
		[JsonPropertyName("data")]
		public FafUserData Data { get; internal set; }

		[JsonConstructor]
		public FafUser(FafUserData data)
		{
			Data = data;
		}
	}

	public class FafUserData
	{
		[JsonPropertyName("attributes")]
		public FafUserAttributes Attributes { get; internal set; }

		[JsonConstructor]
		public FafUserData(FafUserAttributes attributes)
		{
			Attributes = attributes;
		}
	}

	public class FafUserAttributes
	{
		[JsonPropertyName("userId")]
		public int UserId { get; internal set; }
		[JsonPropertyName("userName")]
		public string UserName { get; internal set; }

		[JsonConstructor]
		public FafUserAttributes(int userId, string username)
		{
			UserId = userId;
			UserName = username;
		}
	}
}
