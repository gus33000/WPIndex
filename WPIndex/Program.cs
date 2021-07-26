using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WPIndex
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string filelist = @"W:\cablist.csv";

            Console.WriteLine("Reading all lines");
            var files = File.ReadAllLines(filelist);

            /*var newfilelist = files.Select(x =>
            {
                Console.Title = x;
                using var strm = File.OpenRead(x.Split(",")[1]);
                return $"{x.Split(",")[1]},{x.Split(",")[0]},{strm.GetCabinetType()}";
            });

            Console.WriteLine("Backing up data");
            File.WriteAllLines(@"W:\cablist.type.csv", newfilelist);*/

            await Parallel.ForEachAsync(files, (file, _) =>
            {
                Console.WriteLine(file);
                using var strm = File.OpenRead(file.Split(",")[1]);
                switch (strm.GetCabinetType())
                {
                    case XmlHelper.CabinetType.Broken:
                        Console.WriteLine("BROKEN");
                        break;
                    case XmlHelper.CabinetType.Unknown:
                        Console.WriteLine("UNKNOWN");
                        break;
                    case XmlHelper.CabinetType.CBS:
                    case XmlHelper.CabinetType.CBSU:
                        {
                            using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(strm, "update.mum"));
                            var pkg = xmlfile.GetUpdateMum();
                            try
                            {
                                Console.WriteLine("pkg.AssemblyIdentity.Version: " + pkg.AssemblyIdentity.Version);
                            }
                            catch { }
                            try
                            {
                                Console.WriteLine("pkg.ManifestVersion: " + pkg.ManifestVersion);
                            }
                            catch { }
                            try
                            {
                                Console.WriteLine("pkg.Package.Update.Component.AssemblyIdentity.Version: " + pkg.Package.Update.Component.AssemblyIdentity.Version);
                            }
                            catch { }
                            break;
                        }
                    case XmlHelper.CabinetType.SPKG:
                    case XmlHelper.CabinetType.SPKU:
                        {
                            try
                            {
                                using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(strm, "man.dsm.xml"));
                                var pkg = xmlfile.GetManDSMXml();
                                try
                                {
                                    Console.WriteLine("pkg.Identity.Version: " + pkg.Identity.Version.Major + "." + pkg.Identity.Version.Minor + "." + pkg.Identity.Version.QFE + "." + pkg.Identity.Version.Build);
                                }
                                catch { }
                            }
                            catch
                            {
                                Console.WriteLine("CRASH: " + file);
                                throw;
                            }
                            break;
                        }

                }

                return ValueTask.CompletedTask;
            });

            Console.WriteLine("The end");
        }
    }
}
