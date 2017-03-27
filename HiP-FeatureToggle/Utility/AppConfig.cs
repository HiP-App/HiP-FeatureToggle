using System.Text;
using Microsoft.Extensions.Configuration;

namespace de.uni_paderborn.si_lab.hip.featuretoggle.utility
{
    public class AppConfig
    {
        public string DB_HOST { get; set; }
        public string DB_USERNAME { get; set; }
        public string DB_PASSWORD { get; set; }
        public string DB_NAME { get; set; }
        public string CLIENT_ID { get; set; }
        public string DOMAIN { get; set; }
        public string EMAIL_SERVICE { get; set; }
        public string ALLOW_HTTP { get; set; }
        public string ADMIN_EMAIL { get; set; }

        public string ConnectionString
        {
            get
            {
                var connectionString = new StringBuilder();

                connectionString.Append($"Host={DB_HOST};");
                connectionString.Append($"Username={DB_USERNAME};");
                connectionString.Append($"Password={DB_PASSWORD};");
                connectionString.Append($"Database={DB_NAME};");
                connectionString.Append($"Pooling=true;");

                return connectionString.ToString();
            }
        }
    }
}