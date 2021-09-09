namespace SqlManager.Data.Models {
    public class ConnectionData {
        public string ServerName         { get; set; }
        public string Login              { get; set; }
        public string Password           { get; set; }
        public bool   IntegratedSecurity { get; set; }
    }
}