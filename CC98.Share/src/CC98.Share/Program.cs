using System.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

namespace CC98.Share
{
	/// <summary>
	///     应用程序的朱类型。
	/// </summary>
	public static class Program
	{
		/// <summary>
		///     应用程序的入口点。
		/// </summary>
		/// <param name="args">应用程序的启动参数。</param>
		[UsedImplicitly(ImplicitUseKindFlags.Access)]
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}