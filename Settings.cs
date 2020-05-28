using System;
using System.Collections.Generic;
using System.Text;

namespace DeployCo
{
    public class Settings
    {
        public string VersionToInstall { get; set; }
        public List<Çlients> Çlients { get; set; }
    }

    public class Çlients
    {
        public bool Enabled { get; set; }
        public string BackupLocation { get; set; }
        public string ReleasePackageLocation { get; set; }
        public List<Services> Services { get; set; }

    }
    public class Services
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Location { get; set; }
        public string AppPool { get; set; }
    }
}
