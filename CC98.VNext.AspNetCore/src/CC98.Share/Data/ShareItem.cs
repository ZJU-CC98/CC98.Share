using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using System;

namespace CC98.Share.Data
{
    /// <summary>
    ///     表示用户上传的一个项目。
    /// </summary>
    public class ShareItem
    {
        /// <summary>
        ///     获取或设置该项目的标识。
        /// </summary>
        [Key]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public int Id { get; set; }

        /// <summary>
        ///     获取或设置该项目的名称。
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        ///     获取或设置该项目的描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     获取或设置项目的实际下载地址。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     获取或设置该项目的拥有者的用户名。
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        ///     获取或设置一个值，指示该项目是否被所有者共享。
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        ///     获取或设置该项目的下载次数。
        /// </summary>
        [Range(0, int.MaxValue)]
        public int DownloadCount { get; set; }

		/// <summary>
		/// 文件大小
		/// </summary>
		public long Size { get; set; }

        /// <summary>
		/// 用户文件总大小
		/// </summary>
		public long TotalSize { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }
    }
}