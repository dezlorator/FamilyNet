﻿namespace FamilyNetServer.Configuration
{
    public class JWTConfiguration
    {
        public string Secret { get; set; }
        public int MinutesLife { get; set; }
    }
}