using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Data.Models.System
{
    [NPoco.TableName("Sys_SystemRolePermission")]
    [NPoco.PrimaryKey(new string[] { "functionId", "roleId" })]
    public class SystemRolePermission
    {
        [NPoco.Column("functionId")]
        public string FunctionId { get; set; }

        [NPoco.Column("roleId")]
        public string RoleId { get; set; }

        [NPoco.Column("active")]
        public bool Active { get; set; }
    }
}
