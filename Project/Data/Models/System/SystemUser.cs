using System;

namespace Project.Data.Models
{
    [NPoco.TableName("Sys_SystemUser")]
    [NPoco.PrimaryKey("id")]
    public class SystemUser
    {
        public SystemUser()
        {
            Avatar = "";
        }

        [NPoco.Column("id")]
        public int Id { get; set; }

        [NPoco.Column("userName")]
        public string UserName { get; set; }

        [NPoco.Column("password")]
        public string? Password { get; set; }

        [NPoco.Column("avatar")]
        public string? Avatar { get; set; }

        [NPoco.Column("email")]
        public string? Email { get; set; }

        [NPoco.Column("active")]
        public bool Active { get; set; }

        [NPoco.Column("createTime")]
        public DateTime? CreateTime { get; set; }

        [NPoco.Column("role")]
        public string Role { get; set; }

        [NPoco.Column("token")]
        public string Token { get; set; }

    }
}
