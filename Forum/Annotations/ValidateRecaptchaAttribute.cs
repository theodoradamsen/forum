﻿using Forum.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Forum.Annotations {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class ValidateRecaptchaAttribute : Attribute, IFilterFactory, IOrderedFilter {
		public int Order { get; set; }
		public bool IsReusable => true;

		/// <summary>
		/// Returns an instance from the IOC container. Cleaner than using a ServiceFilterAttribute.
		/// </summary>
		public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<ValidateRecaptchaActionFilter>();
	}
}