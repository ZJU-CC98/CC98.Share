namespace CC98.Identity
{
    /// <summary>
    ///     表示 CC98 用户的基本信息。
    /// </summary>
    public class CC98User
    {
        /// <summary>
        ///     获取或设置 CC98 v1 版本标识。
        /// </summary>
        public int V1Id { get; set; }

        /// <summary>
        ///     获取或设置 CC98 v2 版本标识。
        /// </summary>
        public int V2Id { get; set; }

        /// <summary>
        ///     获取或设置用户名。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     获取或设置用户角色。
        /// </summary>
        public string[] Roles { get; set; }
    }
}