using Project.Data.Models.System;
using Project.Utils;
using NPoco.Migrations;
using System;

namespace Project.Data.Models.Project
{

    public class ProjectMigration : Migration, IMigration
    {
        protected override void execute()
        {
            if (!Migrator.TableExists<SystemConfig>())
            {
                Migrator.CreateTable<SystemConfig>(true).Execute();
                Migrator.Database.Insert(new SystemConfig()
                {
                    key = "administrators",
                    Value = "admin"
                });
            }

            if (!Migrator.TableExists<ProjectUserInfo>())
            {
                Migrator.CreateTable<ProjectUserInfo>(true).Execute();
                ProjectUserInfo userInfo = new ProjectUserInfo()
                {
                    Token = Guid.NewGuid().ToString("N").ToUpper(),
                    UserName = "admin",
                    Email = "admin@qq.com",
                    CreateTime = DateTime.Now,
                    Active = true,
                    Password = "123456".ToMd5(),
                    Role = "admin",
                };
                Migrator.Database.Insert(userInfo);
            }

        }
    }
}
