using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CC98.Share.Data
{
    /// <summary>
    ///     表示 CC98 Share 数据库模型对象。
    /// </summary>
    public class CC98ShareModel : DbContext
    {
        /// <summary>
        ///     获取或设置数据库中所有上传项目的集合。
        /// </summary>
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public virtual DbSet<ShareItem> Items { get; set; }
    }
}