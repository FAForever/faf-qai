using System;
using System.Collections.Generic;

using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Faforever.Qai.Core.Models
{
	public partial class MapRequest
	{
		[JsonProperty("data")]
		public Datum[] Data { get; set; }

		[JsonProperty("meta")]
		public Meta Meta { get; set; }

		[JsonProperty("included")]
		public Included[] Included { get; set; }
	}

	public partial class Datum
	{
		[JsonProperty("type")]
		public TypeEnum Type { get; set; }

		[JsonProperty("id")]
		[JsonConverter(typeof(ParseStringConverter))]
		public long Id { get; set; }

		[JsonProperty("attributes")]
		public DatumAttributes Attributes { get; set; }

		[JsonProperty("relationships")]
		public Relationships Relationships { get; set; }
	}

	public partial class DatumAttributes
	{
		[JsonProperty("battleType")]
		public string BattleType { get; set; }

		[JsonProperty("createTime")]
		public DateTimeOffset CreateTime { get; set; }

		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("mapType")]
		public string MapType { get; set; }

		[JsonProperty("updateTime")]
		public DateTimeOffset UpdateTime { get; set; }
	}

	public partial class Relationships
	{
		[JsonProperty("author")]
		public Author Author { get; set; }

		[JsonProperty("latestVersion")]
		public Author LatestVersion { get; set; }

		[JsonProperty("reviewsSummary")]
		public Author ReviewsSummary { get; set; }

		[JsonProperty("statistics")]
		public Author Statistics { get; set; }

		[JsonProperty("versions")]
		public Author Versions { get; set; }
	}

	public partial class Author
	{
		[JsonProperty("data")]
		public Data Data { get; set; }
	}

	public partial class Dat
	{
		[JsonProperty("type")]
		public TypeEnum Type { get; set; }

		[JsonProperty("id")]
		[JsonConverter(typeof(ParseStringConverter))]
		public long Id { get; set; }
	}

	public partial class Included
	{
		[JsonProperty("type")]
		public TypeEnum Type { get; set; }

		[JsonProperty("id")]
		[JsonConverter(typeof(ParseStringConverter))]
		public long Id { get; set; }

		[JsonProperty("attributes")]
		public IncludedAttributes Attributes { get; set; }

		[JsonProperty("relationships")]
		public Dictionary<string, Author> Relationships { get; set; }
	}

	public partial class IncludedAttributes
	{
		[JsonProperty("createTime")]
		public DateTimeOffset CreateTime { get; set; }

		[JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
		public string Description { get; set; }

		[JsonProperty("downloadUrl", NullValueHandling = NullValueHandling.Ignore)]
		public Uri DownloadUrl { get; set; }

		[JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
		public string Filename { get; set; }

		[JsonProperty("folderName", NullValueHandling = NullValueHandling.Ignore)]
		public string FolderName { get; set; }

		[JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
		public long? Height { get; set; }

		[JsonProperty("hidden", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Hidden { get; set; }

		[JsonProperty("maxPlayers", NullValueHandling = NullValueHandling.Ignore)]
		public long? MaxPlayers { get; set; }

		[JsonProperty("ranked", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Ranked { get; set; }

		[JsonProperty("thumbnailUrlLarge", NullValueHandling = NullValueHandling.Ignore)]
		public Uri ThumbnailUrlLarge { get; set; }

		[JsonProperty("thumbnailUrlSmall", NullValueHandling = NullValueHandling.Ignore)]
		public Uri ThumbnailUrlSmall { get; set; }

		[JsonProperty("updateTime")]
		public DateTimeOffset UpdateTime { get; set; }

		[JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
		public long? Version { get; set; }

		[JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
		public long? Width { get; set; }

		[JsonProperty("login", NullValueHandling = NullValueHandling.Ignore)]
		public string Login { get; set; }

		[JsonProperty("userAgent", NullValueHandling = NullValueHandling.Ignore)]
		public string UserAgent { get; set; }
	}

	public partial class Meta
	{
		[JsonProperty("page")]
		public Page Page { get; set; }
	}

	public partial class Page
	{
		[JsonProperty("number")]
		public long Number { get; set; }

		[JsonProperty("limit")]
		public long Limit { get; set; }
	}

	public enum TypeEnum { Map, MapStatistics, MapVersion, MapVersionStatistics, NameRecord, Player, MapReviewSummary, Unknown };

	public partial struct Data
	{
		public Dat? Dat;
		public Dat[]? DatArray;

		public static implicit operator Data(Dat Dat) => new Data { Dat = Dat };
		public static implicit operator Data(Dat[] DatArray) => new Data { DatArray = DatArray };
		public bool IsNull => DatArray == null && Dat == null;
	}

	public partial class MapRequest
	{
		public static MapRequest FromJson(string json) => JsonConvert.DeserializeObject<MapRequest>(json, Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this MapRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
	}

	internal static class Converter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Converters = {
				DataConverter.Singleton,
				TypeEnumConverter.Singleton,
				new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
			},
		};
	}

	internal class ParseStringConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;
			var value = serializer.Deserialize<string>(reader);
			long l;
			if (Int64.TryParse(value, out l))
			{
				return l;
			}
			throw new Exception("Cannot unmarshal type long");
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}
			var value = (long)untypedValue;
			serializer.Serialize(writer, value.ToString());
			return;
		}

		public static readonly ParseStringConverter Singleton = new ParseStringConverter();
	}

	internal class DataConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(Data) || t == typeof(Data?);

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			switch (reader.TokenType)
			{
				case JsonToken.Null:
					return new Data { };
				case JsonToken.StartObject:
					var objectValue = serializer.Deserialize<Dat>(reader);
					return new Data { Dat = objectValue };
				case JsonToken.StartArray:
					var arrayValue = serializer.Deserialize<Dat[]>(reader);
					return new Data { DatArray = arrayValue };
			}
			throw new Exception("Cannot unmarshal type Data");
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			var value = (Data)untypedValue;
			if (value.IsNull)
			{
				serializer.Serialize(writer, null);
				return;
			}
			if (value.DatArray != null)
			{
				serializer.Serialize(writer, value.DatArray);
				return;
			}
			if (value.Dat != null)
			{
				serializer.Serialize(writer, value.Dat);
				return;
			}
			throw new Exception("Cannot marshal type Data");
		}

		public static readonly DataConverter Singleton = new DataConverter();
	}

	internal class TypeEnumConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;
			var value = serializer.Deserialize<string>(reader);
			switch (value)
			{
				case "map":
					return TypeEnum.Map;
				case "mapStatistics":
					return TypeEnum.MapStatistics;
				case "mapVersion":
					return TypeEnum.MapVersion;
				case "mapVersionStatistics":
					return TypeEnum.MapVersionStatistics;
				case "nameRecord":
					return TypeEnum.NameRecord;
				case "player":
					return TypeEnum.Player;
				default:
					return TypeEnum.Unknown;
			}
			throw new Exception("Cannot unmarshal type TypeEnum");
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}
			var value = (TypeEnum)untypedValue;
			switch (value)
			{
				case TypeEnum.Map:
					serializer.Serialize(writer, "map");
					return;
				case TypeEnum.MapStatistics:
					serializer.Serialize(writer, "mapStatistics");
					return;
				case TypeEnum.MapVersion:
					serializer.Serialize(writer, "mapVersion");
					return;
				case TypeEnum.MapVersionStatistics:
					serializer.Serialize(writer, "mapVersionStatistics");
					return;
				case TypeEnum.NameRecord:
					serializer.Serialize(writer, "nameRecord");
					return;
				case TypeEnum.Player:
					serializer.Serialize(writer, "player");
					return;
			}
			throw new Exception("Cannot marshal type TypeEnum");
		}

		public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
	}
}
