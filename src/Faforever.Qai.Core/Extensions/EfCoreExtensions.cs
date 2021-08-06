using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Faforever.Qai.Core.Extensions
{
	public static class EfCoreExtensions
	{
		public static PropertyBuilder<TProperty> Json<TProperty>(this PropertyBuilder<TProperty> builder) where TProperty : new()
		{
			var comparer = new ValueComparer<TProperty>(
				(l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
				v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
				v => JsonConvert.DeserializeObject<TProperty>(JsonConvert.SerializeObject(v)) ?? new()
			);


			builder = builder.HasConversion(
				v => JsonConvert.SerializeObject(v),
				v => JsonConvert.DeserializeObject<TProperty>(v) ?? new()
			);

			return builder;
		}
	}
}
