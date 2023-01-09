// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace UserOwnsData
{
	using UserOwnsData.Services.Security;
	using Owin;
	using System.Web.Mvc;
	using System.Web.Routing;
	using UserOwnsData.Middleware;

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			IAppBuilder app2 = UseForwardedHeadersExtension.UseForwardedHeaders(app);

            // init ASP.NET MVC routes
            AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			// init OpenId Connect settings
			OwinOpenIdConnect.ConfigureAuth(app2);
		}
	}
}