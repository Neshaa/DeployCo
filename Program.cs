using Microsoft.Extensions.Configuration;
using Microsoft.Web.Administration;
using System;
using System.IO;
using System.Linq;

namespace DeployCo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("settings.json");

            var config = builder.Build();
            var settings = config.GetSection("Settings").Get<Settings>();

            DeployApp(settings);
        }


        static private void DeployApp(Settings settings)
        {
            var folerToCreate = settings.Çlients.Where(x => x.Enabled).FirstOrDefault().BackupLocation;
            string relativePath = string.Concat("pre_", settings.VersionToInstall, "_", DateTime.Now.ToString("dd_MM_YYYY"));
            var destBack = System.IO.Path.Combine(folerToCreate, relativePath);

            foreach (var client in settings.Çlients)
            {
                if (client.Enabled)
                {
                    foreach (var srv in client.Services.Where(x => x.Enabled))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Počinje deploy: {0}", srv.Name);
                        Console.WriteLine("-----------------------------------------------------------");
                        Console.ResetColor();

                        var sourcePath = srv.Location;
                        var destinationPath = System.IO.Path.Combine(destBack, srv.Name);
                        var releaseLocation = System.IO.Path.Combine(client.ReleasePackageLocation, "Intelisale." + srv.Name);

                        System.IO.Directory.CreateDirectory(destinationPath);//kreiranje foldera sa odrejdnim servisom

                        new ServerManager().ApplicationPools.Where(x => x.Name == srv.AppPool).FirstOrDefault().Stop(); // stopiranje pool-va

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Stopiran pool!");


                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*.*",
                            SearchOption.AllDirectories).Where(x => !x.EndsWith("Logs")).ToList())
                            Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

                        ////Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                            SearchOption.AllDirectories).Where(x => !x.Contains("Logs\\")).ToList())
                            File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);


                        Console.WriteLine("Završen bekap!");

                        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*.*",
                            SearchOption.TopDirectoryOnly).Where(x => !x.EndsWith("Logs")).ToList())
                            Directory.Delete(dirPath, true);

                        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                            SearchOption.TopDirectoryOnly).Where(x => !x.EndsWith("web.config")).ToList())
                            File.Delete(newPath);

                        Console.WriteLine("Završeo brisanje postojećih fallova!");

                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(releaseLocation, "*.*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(releaseLocation, sourcePath));

                        ////Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(releaseLocation, "*.*",
                            SearchOption.AllDirectories).Where(x => !x.EndsWith("web.config")).ToList())
                            File.Copy(newPath, newPath.Replace(releaseLocation, sourcePath), true);

                        Console.WriteLine("Završeno kopiranje!");

                        new ServerManager().ApplicationPools.Where(x => x.Name == srv.AppPool).FirstOrDefault().Start(); // stopiranje pool-va

                        Console.WriteLine("Startovan pool!");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Završen deploy: {0}", srv.Name);
                        Console.WriteLine("-----------------------------------------------------------");
                        Console.WriteLine("\n");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
