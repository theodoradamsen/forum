﻿using Forum.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Forum.Filters {
	public class UserContextActionFilter : IAsyncActionFilter {
		UserContextLoader UserContextLoader { get; }

		public UserContextActionFilter(
			UserContextLoader userContextLoader
		) {
			UserContextLoader = userContextLoader;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
			try {
				await UserContextLoader.Invoke();
			}
			catch (TaskCanceledException) { }
			catch (OperationCanceledException) { }

			await next();
		}
	}
}