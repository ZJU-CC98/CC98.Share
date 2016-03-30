using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CC98.Share.Controllers
{
	public class DownUploadController : Controller
	{
		public ActionResult IndexUpload()
		{
			return View();

		}
		public ActionResult IndexDownload()
		{
			return View();
		}
		public ActionResult Download(string URL)
		{
			CC98ShareModel Model = new CC98ShareModel();
			int num;
			try
			{
				num = Convert.ToInt32(URL);
			}
			catch
			{
				return Content("下载名为空！", "text/plain");
			}
			var Output = from i in Model.Items where i.Id == num select i;
			var result = Output.SingleOrDefault();
			string addressName = result.Path;
			try
			{
				return File(addressName, "application/octet-stream");
			}
			catch
			{
				return Content("下载异常！", "text/plain");
			}
		}
		public ActionResult Upload(ShareItem tm, HttpPostedFileBase file)
		{
			if (file == null)
			{
				return Content("没有文件！", "text/plain");
			}
			CC98ShareModel Model = new CC98ShareModel();
			var fileName = Path.Combine(Request.MapPath("~/Upload"), Path.GetFileName(file.FileName));
			try
			{
				file.SaveAs(fileName);
				tm.Path = "../upload/" + Path.GetFileName(file.FileName);
				tm.UserName = "Starve";
				tm.Name = "Starve";
				Model.Items.Add(tm);
				Model.SaveChanges();
				return Content("上传成功！", "text/plain");
			}
			catch
			{

				return Content("上传异常！", "text/plain");
			}
		}
	}
}