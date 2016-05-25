using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Share.Controllers
{
    /// <summary>
    ///     提供用户信息相关的操作。
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        ///     显示一个用户的详细信息。
        /// </summary>
        /// <param name="name">要显示详细信息的用户名。</param>
        /// <returns>操作结果。</returns>
        [Route("User/{name}")]
        [AllowAnonymous]
        public IActionResult Detail(string name)
        {
            // 参数检查
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            // 根据用户名生成跳转 URL，目前跳转到 CC98 论坛的个人信息页
            var uri = string.Format(CultureInfo.InvariantCulture, "http://www.cc98.org/dispuser.asp?name={0}",
                Uri.EscapeDataString(name));

            // 执行跳转
            return Redirect(uri);
        }
    }
}