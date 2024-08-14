public class EmailSettings
    {
        public static string Section  = "EmailSettings";
        public string FromAddress { get; set; }
        public string ToList { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Smtp { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }