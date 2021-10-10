using Project.Data.Models.Project;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using NPoco;
using NPoco.Migrations;
using NPoco.Migrations.CurrentVersion;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Project.Data.Models.System
{
    public class SystemDBManager
    {
        private readonly string MigrationName = "Project";

        private readonly SqliteConnection m_dbConnection;

        private readonly Migrator m_migrator;

        private readonly string m_dataPath = "project_data";

        private readonly string m_jwtPath;

        private readonly string m_dbPath;

        private SystemJwtInfo m_JWT;

        public SystemJwtInfo JWT
        {
            get => m_JWT;
            set
            {
                m_JWT = value;
                File.WriteAllText(m_jwtPath, JsonConvert.SerializeObject(JWT));
            }
        }

        public readonly string project_data;

        public Database DBase { get; private set; }

        private static SystemDBManager? m_Instance;

        public static SystemDBManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new SystemDBManager();
                }

                return m_Instance;
            }
        }

        private SystemDBManager()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                project_data = Path.Combine(Directory.GetCurrentDirectory(), m_dataPath);
                if (!Directory.Exists(project_data))
                {
                    Directory.CreateDirectory(project_data);
                }

                m_jwtPath = Path.Combine(Directory.GetCurrentDirectory(), "/" + m_dataPath + "/jwt.json");
                m_dbPath = Path.Combine(Directory.GetCurrentDirectory(), "/" + m_dataPath + "/data.db");


            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                project_data = Path.Combine(Directory.GetCurrentDirectory(), m_dataPath);
                if (!Directory.Exists(project_data))
                {
                    Directory.CreateDirectory(project_data);
                }

                m_jwtPath = Path.Combine(Directory.GetCurrentDirectory(), m_dataPath + "/jwt.json");
                m_dbPath = Path.Combine(Directory.GetCurrentDirectory(), m_dataPath + "/data.db");
            }

            if (!File.Exists(m_jwtPath))
            {
                var bytes = new byte[128];
                var ranndom = new Random();
                ranndom.NextBytes(bytes);
                var key = Convert.ToBase64String(bytes);

                JWT = new SystemJwtInfo()
                {
                    ClockSkew = 10,
                    Audience = "Project",
                    Issuer = "Project",
                    IssuerSigningKey = key,
                    Expiration = 36000,
                };

                File.WriteAllText(m_jwtPath, JsonConvert.SerializeObject(JWT));
            }
            else
            {
                JWT = JsonConvert.DeserializeObject<SystemJwtInfo>(File.ReadAllText(m_jwtPath));
            }


            m_dbConnection = new SqliteConnection(string.Format("Data Source={0}", m_dbPath));

            if (m_dbConnection.State == ConnectionState.Closed)
            {
                m_dbConnection.Open();
            }

            DBase = new Database(m_dbConnection, DatabaseType.SQLite)
            {
                KeepConnectionAlive = true
            };
            m_migrator = new Migrator(DBase);
        }

        public bool IsUser(string userName)
        {
            return DBase.Query<ProjectUserInfo>().Any(e => e.UserName == userName);
        }

        public ProjectUserInfo GetUser(string userName)
        {
            return DBase.Query<ProjectUserInfo>().FirstOrDefault(e => e.UserName == userName);
        }

        public void Run()
        {

            var codeVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var versionProvider = new DatabaseCurrentVersionProvider(DBase);

            if (!m_migrator.TableExists<SystemConfig>())
            {
                var migrationBuilder = new MigrationBuilder(MigrationName, DBase);
                migrationBuilder.Append(new Version(codeVersion.ToString()), new ProjectMigration());
                migrationBuilder.Execute();
                versionProvider.SetMigrationVersion(MigrationName, new Version(codeVersion.ToString()));
            }
            else
            {
                if (versionProvider.GetMigrationVersion(MigrationName).ToString() == "0.0")
                {
                    versionProvider.SetMigrationVersion(MigrationName, new Version(1, 0, 0, 0));
                }

                var builder = new MigrationBuilder(MigrationName, DBase);
                builder.Execute();
            }
        }
    }
}
