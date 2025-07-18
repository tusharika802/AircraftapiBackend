﻿namespace Aircraftapi
{
    public class EmailSettings
    {
        public string PrimaryDomain { get; set; }
        public int PrimaryPort { get; set; }
        public string SecondaryDomain { get; set; }
        public int SecondaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string CcEmail { get; set; }
    }
}
