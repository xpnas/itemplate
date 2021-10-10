using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Data.Models.System
{
    [NPoco.TableName("Sys_SystemRole")]
    [NPoco.PrimaryKey("name", AutoIncrement = false)]
    public class SystemRole
    {
        [NPoco.Column("name")]
        public string Name { get; set; }

        [NPoco.Column("description")]
        public string? Description { get; set; }
    }
}
