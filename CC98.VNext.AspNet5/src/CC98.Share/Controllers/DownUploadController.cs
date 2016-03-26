using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using CC98.Share.Models;
using System.IO;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.Net;

namespace CC98.Share.Controllers
{
	public class DownUploadController : Controller
	{
		public IActionResult IndexUpload()
		{
			return View();

		}
		public IActionResult IndexDownload()
		{
			return View();
		}
		[FromServices]
		public CC98ShareModel Model
		{
			get;
			set;
		}
		[FromServices]
		public IHostingEnvironment Address
		{
			get;
			set;
		}
		public IActionResult Download(int id)
		{
			try
			{
				var Output = from i in Model.Items where i.Id == id select i;
				var result = Output.SingleOrDefault();
				if (result != null)
				{
					string addressName = result.Path;
					return PhysicalFile(addressName, "application/octet-stream");
				}
				else
				{
					return Content(id.ToString(), "text/plain");
				}
			}
			catch
			{
				return new HttpStatusCodeResult(404);
			}
		}
		public IActionResult Upload(IFormFile file)
		{
			if(file==null)
			{
				return new HttpStatusCodeResult(404);
			}
			string fileNameRandom = Path.GetRandomFileName();
			var fileName = Path.Combine(Address.MapPath("Upload"), fileNameRandom);
			ShareItem tm = new ShareItem();
			try
			{
				file.SaveAs(fileName);
				tm.Path = fileName;
				tm.UserName = "Starve";
				tm.Name = "Starve";
				Model.Items.Add(tm);
				Model.SaveChanges();
				return new HttpStatusCodeResult(200);
			}
		    catch
			{
				return new HttpStatusCodeResult(404);
			}
		}
    }
}
