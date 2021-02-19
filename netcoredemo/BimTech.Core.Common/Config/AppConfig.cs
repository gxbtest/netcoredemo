using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.Common.Config
{
    public class AppConfig
    {
        private static  SettingsOptions SettingsOptions = new SettingsOptions();
        public static SettingsOptions settingsOptions
        {
            get
            {
                return SettingsOptions;
            }
            set
            {
               settingsOptions = SettingsOptions;
            }
        }
}

    /// <summary>
    /// 配置静态类
    /// </summary>
    public class SettingsOptions
    {
        /// <summary>
        /// 一般配置项目
        /// </summary>
        public AppSetting appSetting { get; set; }
        /// <summary>
        /// 租户配置项
        /// </summary>
        public TenantConfig tenant { get; set; }
        /// <summary>
        /// redis配置项
        /// </summary>
        public CacheConfig cache { get; set; }

    }
    public class AppSetting
    {
        /// <summary>
        /// 监听ip地址
        /// </summary>
        public string[] Ip { get; set; }
        /// <summary>
        /// 数据接口
        /// </summary>
        public string[] DataInterfaceIp { get; set; }
        /// <summary>
        /// 监听端口好
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DbReadConnString { get; set; }
        /// <summary>
        /// 数据库类别
        /// MySql = 0,SqlServer = 1,Sqlite = 2,Oracle = 3,PostgreSQL = 4,
        /// </summary>
        public int DbType { get; set; }
        /// <summary>
        /// 是否是调试阶段
        /// </summary>
        public bool IsDebug { get; set; }
        /// <summary>
        /// 调试用户
        /// </summary>
        public UserInfo DebugUser { get; set; }
        /// <summary>
        /// 是否打印sql语句
        /// </summary>
        public bool IsLogSql { get; set; }
        /// <summary>
        /// 业务模块
        /// </summary>
        public List<string> AppModes { get; set; }
        /// <summary>
        /// 鉴权方式
        /// </summary>
        public AuthorizationType AuthorizationType { get; set; }
        /// <summary>
        /// 登录游戏时间单位秒
        /// </summary>
        public int AccessTokenExpireTimeSpan { get; set; }
        /// <summary>
        /// 日志记录地址 0 记录在租户平台中   1 记录在用户平台中
        /// </summary>
        public int LoggerAddress { get; set; }
        /// <summary>
        /// 鉴权加密 key
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 异地登录模式 0 允许异地登录  1 异地登录顶下当前登录  2 不允许异地登录
        /// </summary>
        public int RemoteLogin { get; set; }

    }
    public class TenantConfig
    {
        /// <summary>
        /// 是否为多租户
        /// </summary>
        public bool IsMulti { get; set; }
        /// <summary>
        /// 默认的数据库名称 如果是多租户的显示租户数据库名称，否则显示单用户数据库名称
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 模板数据库sql文件地址
        /// </summary>
        public string TempalteDataBasePath { get; set; }
    }
    public class CacheConfig
    {
        /// <summary>
        /// 0 MemoryCache  1 redis 数据量小先用内存缓存，数据量超10万用redis
        /// </summary>
        public int CacheType { get; set; }
        /// <summary>
        /// 缓存时间 
        /// </summary>
        public int Expire { get; set; }
        /// <summary>
        /// redis 连接字符串
        /// </summary>
        public string RedisConnString { get; set; }
    }
    /// <summary>
    /// 鉴权类型
    /// </summary>
    public enum AuthorizationType
    {
        JWT = 0,
        Cookie = 1,
    }

    /// <summary>
    /// 登录用户
    /// </summary>
    public sealed class UserInfo
    {
        #region 构造函数
        public UserInfo()
        {
        }
        #endregion

        #region 属性
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserAccount { get; set; }
        /// <summary>
        /// 用户别名中文
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 所属租户
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// 当前项目Id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 用户类别  1 web 用户, 2 app用户 
        /// </summary>
        public int? MenuType { get; set; }
        /// <summary>
        /// 当前用户令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 是否系统管理员属性
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 非业务字段，当允许异地多人登录时 在该字段添加 new guid
        /// </summary>
        public string RemoteLogin { get; set; }
        #endregion

        #region 常量

        /// <summary>
        /// 账号过期时间（分钟）
        /// </summary>
        public const int ACCOUNT_EXPIRATION_TIME = 60 * 12;

        #endregion


    }
}
