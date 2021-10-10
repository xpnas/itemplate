using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Data.Models.System
{
    [NPoco.TableName("Sys_SystemFunction")]
    [NPoco.PrimaryKey("id")]
    public class SystemFunction
    {
        [NPoco.Column("id")]
        public string Id { get; set; }

        [NPoco.Column("name")]
        public string Name { get; set; }

        [NPoco.Column("description")]
        public string? Description { get; set; }
    }
}
