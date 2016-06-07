using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CC98.Identity;
using CC98.Share.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CC98.Share.Controllers
{
    /// <summary>
    ///     提供网站基本的上传和下载功能。
    /// </summary>
    public class FileController : Controller
    {
        public FileController(IHostingEnvironment environment, CC98ShareModel model,
            ExternalSignInManager externalSignInManager, IOptions<Setting> setting)
        {
            Environment = environment;
            Model = model;
            ExternalSignInManager = externalSignInManager;
            Setting = setting;
        }

        private ExternalSignInManager ExternalSignInManager { get; }

        private CC98ShareModel Model { get; }

        private IHostingEnvironment Environment { get; }

        private IOptions<Setting> Setting { get; }

        /// <summary>
        ///     显示上传界面。
        /// </summary>
        /// <returns>操作结果。</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }

        /// <summary>
        ///     显示下载界面。
        /// </summary>
        /// <returns>操作结果。</returns>
        /// <summary>
        ///     提供下载功能。
        /// </summary>
        /// <returns>操作结果。</returns>
        [HttpGet]
        [Route("Download/{id}")]
        public IActionResult Download(int id)
        {
            try
            {
                //将文件在数据库中的标识传入函数。
                //并在数据库中找到此文件，返回给用户
                var output = from i in Model.Items where i.Id == id select i;
                var result = output.SingleOrDefault();
                if (result != null && result.IsShared || result != null && result.UserName == User.Identity.Name)
                {
                    var addressName = Setting.Value.StoreFolder + result.Path;
                    return PhysicalFile(addressName, "application/octet-stream", result.Name);
                }
                return StatusCode(403);
            }
            catch
            {
                return StatusCode(404);
            }
        }

        /// <summary>
        ///     提供删除功能。
        /// </summary>
        /// <returns>操作结果。</returns>
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                //将文件在数据库中的标识传入函数。
                //并在数据库中找到此文件，删除
                var output = from i in Model.Items where i.Id == id select i;
                var result = output.SingleOrDefault();
                if (result != null && result.UserName == User.Identity.Name)
                {
                    Model.Items.Remove(result);
                    await Model.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                return StatusCode(403);
            }
            catch
            {
                return StatusCode(404);
            }
        }

		/// <summary>
		///     提供分享功能。
		/// </summary>
		/// <returns>操作结果。</returns>
		//[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> ShareFile(int id)
        {
            try
            {
                //将文件在数据库中的标识传入函数。
                //并在数据库中找到此文件，改为允许分享
                var output = from i in Model.Items where i.Id == id select i;
                var result = output.SingleOrDefault();
                if (result != null && result.UserName == User.Identity.Name)
                {
                    result.IsShared = true;
                    await Model.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                return StatusCode(403);
            }
            catch
            {
                return StatusCode(404);
            }
        }

		/// <summary>
		///     提供取消分享功能。
		/// </summary>
		/// <returns>操作结果。</returns>
		//[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> CancelShare(int id)
        {
            try
            {
                //将文件在数据库中的标识传入函数。
                //并在数据库中找到此文件，改为允许分享
                var output = from i in Model.Items where i.Id == id select i;
                var result = output.SingleOrDefault();
                if (result != null && result.UserName == User.Identity.Name)
                {
                    result.IsShared = false;
                    await Model.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                return StatusCode(403);
            }
            catch
            {
                return StatusCode(404);
            }
        }

        /// <summary>
        ///     获取ContentDisposition中的文件名称。
        /// </summary>
        /// <returns>操作结果。</returns>
        public string GetFileName(string contentDisposition)
        {
            //获取文件名称的起始位置。
            var startIndex = contentDisposition.IndexOf("filename=\"", StringComparison.OrdinalIgnoreCase) + 10;
            //获取文件名称的结尾位置。
            var endIndex = contentDisposition.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase);
            var length = endIndex - startIndex;
            //通过起始位置和结尾位置获得名称。
            if (length > 50) length = 50;
            var fileName = contentDisposition.Substring(startIndex, length);
            return fileName;
        }

        /// <summary>
        ///     显示上传界面。
        /// </summary>
        /// <returns>操作结果。</returns>
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int value)
        {
            //首先检测是否有文件传入函数。
            if (file == null)
            {
                return StatusCode(400);
            }
            //随机生成一个文件名字，并将此文件插入数据库。
            var fileNameRandom = Path.GetRandomFileName();
            var tm = new ShareItem();
            bool share;
            if (value == 1)
            {
                share = true;
            }
            else
            {
                share = false;
            }
            try
            {
                var s = GetFileName(file.ContentDisposition);
                tm.Path = "\\" + fileNameRandom;

                var saveFileName = Setting.Value.StoreFolder + "\\" + fileNameRandom;

                using (var stream = System.IO.File.OpenWrite(saveFileName))
                {
                    await file.CopyToAsync(stream);
                }
                tm.Size = file.Length;
                tm.TotalSize += tm.Size;
                if (tm.Size > Setting.Value.UserOnceSize)
                {
                    return StatusCode(403);
                }
                if (tm.TotalSize > Setting.Value.UserTotalSize)
                {
                    return StatusCode(403);
                }
                tm.Name = s;
                tm.UserName = ExternalSignInManager.GetUserName(User);
                tm.IsShared = share;
                Model.Items.Add(tm);
                await Model.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return StatusCode(404);
            }
        }
    }
}