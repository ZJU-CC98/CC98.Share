using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace CC98.Sports
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Property)]
	public class BindExcludeAttribute : Attribute, IPropertyBindingPredicateProvider
	{
		public string[] Properties { get; }

		public BindExcludeAttribute(params string[] properties)
		{
			Properties = properties;
		}

		/// <summary>
		/// Gets a predicate which can determines which model properties should be bound by model binding.
		/// </summary>
		public Func<ModelBindingContext, string, bool> PropertyFilter => OnPropertyFilter;

		protected virtual bool OnPropertyFilter(ModelBindingContext context, string propertyName)
		{
			var realProperties =
				Properties.SelectMany(i => i.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

			return !realProperties.Contains(propertyName);

		}
	}
}
