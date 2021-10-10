namespace Project.Data.Models.System
{

    [NPoco.TableName("Sys_SystemConfig")]
    [NPoco.PrimaryKey("key", AutoIncrement = false)]
    public class SystemConfig
    {
        [NPoco.Column("key")]
        public string key;

        [NPoco.Column("value")]
        public string Value;

        public SystemConfig()
        {

        }

        public SystemConfig(string key, string value)
        {
            this.key = key;
            Value = value;
        }
    }
}
