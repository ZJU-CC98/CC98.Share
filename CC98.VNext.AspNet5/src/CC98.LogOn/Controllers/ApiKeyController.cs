using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 为 API 密钥提供操作。
	/// </summary>
    public class ApiKeyController : Controller
    {
		/// <summary>
		/// 显示当前用户申请的所有 API 密钥。
		/// </summary>
		/// <returns>操作结果。</returns>
		[Authorize(Policies.RequireCC98)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
