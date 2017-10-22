﻿using Forum3.Helpers;
using Forum3.Models.DataModels;
using Forum3.Models.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Forum3.Services {
	public class ContextUserFactory {
		ApplicationDbContext DbContext { get; }
		UserManager<ApplicationUser> UserManager { get; }
		RoleManager<ApplicationRole> RoleManager { get; }
		SignInManager<ApplicationUser> SignInManager { get; }
		HttpContext HttpContext { get; }

		public ContextUserFactory(
			ApplicationDbContext dbContext,
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			SignInManager<ApplicationUser> signInManager,
			IHttpContextAccessor httpContextAccessor
		) {
			DbContext = dbContext;
			UserManager = userManager;
			RoleManager = roleManager;
			SignInManager = signInManager;

			HttpContext = httpContextAccessor.HttpContext;
		}

		public ContextUser GetContextUser() {
			HttpContext.Items.ThrowIfNull(nameof(HttpContext.Items));

			object value;

			if (HttpContext.Items.TryGetValue(typeof(ContextUser), out value) && value is ContextUser)
				return (ContextUser)value;

			var contextUser = ConstructContextUser();
			HttpContext.Items[typeof(ContextUser)] = contextUser;

			return contextUser;
		}

		ContextUser ConstructContextUser() {
			var currentPrincipal = HttpContext.User;

			var contextUser = new ContextUser();

			if (currentPrincipal.Identity.IsAuthenticated) {
				contextUser.IsAuthenticated = true;

				var userId = UserManager.GetUserId(currentPrincipal);
				contextUser.ApplicationUser = DbContext.Users.SingleOrDefault(u => u.Id == userId);

				var adminRole = DbContext.Roles.SingleOrDefault(r => r.Name == "Admin");

				var adminUsersQuery = from user in DbContext.Users
								 join userRole in DbContext.UserRoles on user.Id equals userRole.UserId
								 join role in DbContext.Roles on userRole.RoleId equals role.Id
								 where role.Name == "Admin"
								 select user;

				var adminUsers = adminUsersQuery.ToList();

				if (adminRole == null)
					contextUser.IsAdmin = true;
				else if (adminUsers.Count() == 0)
					contextUser.IsAdmin = true;
				else if (currentPrincipal.IsInRole("Admin")) {
					// Force logout if the user was removed from Admin, but their cookie still says they're in Admin.
					if (!adminUsersQuery.Any(u => u.Id == contextUser.ApplicationUser.Id))
						SignInManager.SignOutAsync().ConfigureAwait(false);
					else
						contextUser.IsAdmin = true;
				}

				contextUser.ApplicationUser.LastOnline = DateTime.Now;

				DbContext.Update(contextUser.ApplicationUser);
				DbContext.SaveChanges();
			}

			return contextUser;
		}
	}
}