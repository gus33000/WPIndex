using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WPIndex.XmlDsm;

namespace WPIndex
{
    internal class Program
    {
        private static readonly byte[] BaseBuildInfo = new byte[]
        {
            0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x3F, 0x3E, 0x20, 0x0D, 0x0A, 0x3C, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x2D, 0x69, 0x6E, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x72, 0x65, 0x6C, 0x65, 0x61, 0x73, 0x65, 0x2D, 0x6C, 0x61, 0x62, 0x65, 0x6C, 0x3E, 0x7B, 0x34, 0x7D, 0x3C, 0x2F, 0x72, 0x65, 0x6C, 0x65, 0x61, 0x73, 0x65, 0x2D, 0x6C, 0x61, 0x62, 0x65, 0x6C, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x2D, 0x74, 0x69, 0x6D, 0x65, 0x3E, 0x7B, 0x33, 0x7D, 0x3C, 0x2F, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x2D, 0x74, 0x69, 0x6D, 0x65, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x70, 0x61, 0x72, 0x65, 0x6E, 0x74, 0x2D, 0x62, 0x72, 0x61, 0x6E, 0x63, 0x68, 0x2D, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x3E, 0x7B, 0x32, 0x7D, 0x3C, 0x2F, 0x70, 0x61, 0x72, 0x65, 0x6E, 0x74, 0x2D, 0x62, 0x72, 0x61, 0x6E, 0x63, 0x68, 0x2D, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x65, 0x72, 0x3E, 0x77, 0x70, 0x62, 0x6C, 0x64, 0x6C, 0x61, 0x62, 0x3C, 0x2F, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x65, 0x72, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x6D, 0x61, 0x6A, 0x6F, 0x72, 0x2D, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3E, 0x7B, 0x30, 0x7D, 0x3C, 0x2F, 0x6D, 0x61, 0x6A, 0x6F, 0x72, 0x2D, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x6D, 0x69, 0x6E, 0x6F, 0x72, 0x2D, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3E, 0x7B, 0x31, 0x7D, 0x3C, 0x2F, 0x6D, 0x69, 0x6E, 0x6F, 0x72, 0x2D, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x71, 0x66, 0x65, 0x2D, 0x6C, 0x65, 0x76, 0x65, 0x6C, 0x3E, 0x7B, 0x32, 0x7D, 0x3C, 0x2F, 0x71, 0x66, 0x65, 0x2D, 0x6C, 0x65, 0x76, 0x65, 0x6C, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x2D, 0x74, 0x79, 0x70, 0x65, 0x3E, 0x66, 0x72, 0x65, 0x3C, 0x2F, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x2D, 0x74, 0x79, 0x70, 0x65, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x74, 0x61, 0x72, 0x67, 0x65, 0x74, 0x2D, 0x63, 0x70, 0x75, 0x3E, 0x41, 0x52, 0x4D, 0x3C, 0x2F, 0x74, 0x61, 0x72, 0x67, 0x65, 0x74, 0x2D, 0x63, 0x70, 0x75, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x74, 0x61, 0x72, 0x67, 0x65, 0x74, 0x2D, 0x6F, 0x73, 0x3E, 0x4D, 0x43, 0x3C, 0x2F, 0x74, 0x61, 0x72, 0x67, 0x65, 0x74, 0x2D, 0x6F, 0x73, 0x3E, 0x20, 0x0D, 0x0A, 0x20, 0x20, 0x3C, 0x77, 0x69, 0x6E, 0x70, 0x68, 0x6F, 0x6E, 0x65, 0x2D, 0x72, 0x6F, 0x6F, 0x74, 0x3E, 0x57, 0x3A, 0x5C, 0x57, 0x49, 0x4E, 0x43, 0x45, 0x52, 0x4F, 0x4F, 0x54, 0x3C, 0x2F, 0x77, 0x69, 0x6E, 0x70, 0x68, 0x6F, 0x6E, 0x65, 0x2D, 0x72, 0x6F, 0x6F, 0x74, 0x3E, 0x20, 0x0D, 0x0A, 0x3C, 0x2F, 0x62, 0x75, 0x69, 0x6C, 0x64, 0x2D, 0x69, 0x6E, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x3E, 0x20, 0x0D, 0x0A
        };

        private static byte[] ConstructBuildInfo(ulong major, ulong minor, ulong buildNumber, DateTime dateTime, string buildlab = "WPMAIN")
        {
            string baseBuildInfo = Encoding.UTF8.GetString(BaseBuildInfo);
            string constructedBuildInfo = string.Format(baseBuildInfo, major, minor.ToString("00"), buildNumber, dateTime.ToString("yyyyMMdd-HHmm"), buildlab);
            return Encoding.UTF8.GetBytes(constructedBuildInfo);
        }

        private static async Task Main(string[] args)
        {
            var cabs = Directory.EnumerateFiles(@"D:\WPSorting\deltas", "*.cab");
            var sortedCabs = cabs.OrderBy(x => new FileInfo(x).LastWriteTimeUtc);
            foreach (var cabinet in sortedCabs)
            {
                Console.WriteLine(cabinet);

                using var stream = File.OpenRead(cabinet);
                switch (stream.GetCabinetType())
                {
                    case XmlHelper.CabinetType.CBS:
                        {
                            break;
                        }
                    case XmlHelper.CabinetType.CBSU:
                        {
                            break;
                        }
                    case XmlHelper.CabinetType.SPKG:
                        {
                            break;
                        }
                    case XmlHelper.CabinetType.SPKU:
                        {
                            Cabinet.CabinetFile file = stream.GetFileLocationInDiffCab("buildinfo.xml");
                            byte[] buildInfoDelta = Cabinet.CabinetExtractor.ExtractCabinetFile(stream, file.FileName);
                            byte[] manifestDiff = Cabinet.CabinetExtractor.ExtractCabinetFile(stream, "dman.ddsm.xml");
                            using MemoryStream manifestDiffStream = new MemoryStream(manifestDiff);
                            XmlDsmDiff.DiffPackage man = manifestDiffStream.GetDManDDSMXml();

                            ulong buildNumberBase = ulong.Parse(man.SourceVersion.QFE);
                            ulong majorNumberBase = ulong.Parse(man.SourceVersion.Major);
                            ulong minorNumberBase = ulong.Parse(man.SourceVersion.Minor);

                            DateTime dateTime = file.TimeStamp.ToUniversalTime();//.Subtract(new TimeSpan(20, 0, 0));

                            bool succeeded = false;
                            byte[] finalBaseBuildInfo = Array.Empty<byte>();
                            byte[] finalTargetBuildInfo = Array.Empty<byte>();
                            while (!succeeded)
                            {
                                dateTime = dateTime.Subtract(new TimeSpan(0, 1, 0));
                                finalBaseBuildInfo = ConstructBuildInfo(majorNumberBase, minorNumberBase, buildNumberBase, dateTime, "WPMAIN");
                                try
                                {
                                    finalTargetBuildInfo = Delta.ApplyDelta(finalBaseBuildInfo, buildInfoDelta);
                                    succeeded = true;
                                }
                                catch { }
                            }

                            using MemoryStream sourceBuild = new MemoryStream(finalBaseBuildInfo);
                            using MemoryStream targetBuild = new MemoryStream(finalTargetBuildInfo);

                            BuildInfo.Buildinformation source = sourceBuild.GetBuildInfoXml();
                            BuildInfo.Buildinformation target = targetBuild.GetBuildInfoXml();

                            //Console.WriteLine($"Source: {source.Majorversion}.{source.Minorversion}.{source.Qfelevel}.{man.SourceVersion.Build}.{source.Releaselabel}({source.Builder}).{source.Buildtime}");
                            Console.WriteLine($"Target: {target.Majorversion}.{target.Minorversion}.{target.Qfelevel}.{man.TargetVersion.Build}.{target.Releaselabel}({target.Builder}).{target.Buildtime}");

                            break;
                        }
                }
            }
        }

        private static async Task Main2(string[] args)
        {
            int hour = 0;
            int min = 0;

            while (true)
            {
                try
                {
                    Console.WriteLine("Trying: {0}{1}", hour.ToString("00"), min.ToString("00"));
                    byte[] b9824 = GetBuildInfoTemplate(hour, min);
                    byte[] b9825Delta = File.ReadAllBytes(@"D:\WPSorting\352_buildin.xml");
                    byte[] b9825 = Delta.ApplyDelta(b9824, b9825Delta);
                    Console.WriteLine("OK: {0}{1}", hour.ToString("00"), min.ToString("00"));
                    File.WriteAllBytes(@"D:\WPSorting\buildinfo.9824.xml", b9824);
                    File.WriteAllBytes(@"D:\WPSorting\buildinfo.9824.patched.xml", b9825);
                    return;
                }
                catch { }
                min++;
                if (min % 60 == 0)
                {
                    min = 0;
                    hour++;
                }
            }
        }

        private static byte[] GetBuildInfoTemplate(int hour, int min)
        {
            string template = File.ReadAllText(@"D:\WPSorting\buildinfo.base.xml");
            return System.Text.Encoding.UTF8.GetBytes(string.Format(template, hour.ToString("00"), min.ToString("00")));
        }

        private static async Task Main3(string[] args)
        {
            string afilelist = @"W:\cablist.type.version.csv";

            Console.WriteLine("Reading all previous lines");
            string[] afiles = File.ReadAllLines(afilelist);

            string filelist = @"W:\cablist.type.csv";

            Console.WriteLine("Reading all lines");
            string[] files = File.ReadAllLines(filelist);

            // Path, TimeStamp, Type
            System.Collections.Generic.IEnumerable<(string Path, string Date, string Type)> basefiles = files.Select(x => (x.Split(",")[0], x.Split(",")[1], x.Split(",")[2]));

            // Path, TimeStamp, Type, Version
            System.Collections.Generic.IEnumerable<(string Path, string Date, string Type, string Version)> abasefiles = afiles.Select(x => (x.Split(",")[0], x.Split(",")[1], x.Split(",")[2], x.Split(",")[3]));

            Console.WriteLine(basefiles.Count());
            Console.WriteLine(abasefiles.Count());

            string beforefail = abasefiles.Last().Path;
            if (basefiles.ElementAt(abasefiles.Count() - 1).Path != beforefail)
            {
                throw new Exception("wut");
            }

            System.Collections.Generic.IEnumerable<(string Path, string Date, string Type)> remainingbasefiles = basefiles.Skip(abasefiles.Count());

            System.Collections.Generic.IEnumerable<string> newdata = remainingbasefiles.Select(((string Path, string Date, string Type) f) =>
            {
                Console.Title = f.Path;

                using FileStream strm = File.OpenRead(f.Path);
                switch (f.Type)
                {
                    case "Broken":
                        Console.WriteLine("BROKEN: " + f.Path);
                        break;
                    case "Unknown":
                        Console.WriteLine("UNKNOWN: " + f.Path);
                        break;
                    case "CBS":
                    case "CBSU":
                        {
                            try
                            {
                                using MemoryStream xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(strm, "update.mum"));
                                XmlMum.Assembly pkg = xmlfile.GetUpdateMum();
                                string version = pkg.AssemblyIdentity.Version;
                                return $"{f.Path},{f.Date},{f.Type},{version}";
                            }
                            catch
                            {
                                Console.WriteLine("CRASH/BROKEN: " + f.Path);
                                return $"{f.Path},{f.Date},Broken,";
                            }
                        }
                    case "SPKG":
                    case "SPKU":
                        {
                            try
                            {
                                using MemoryStream xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(strm, "man.dsm.xml"));
                                Package pkg = xmlfile.GetManDSMXml();
                                string version = pkg.Identity.Version.Major + "." + pkg.Identity.Version.Minor + "." + pkg.Identity.Version.QFE + "." + pkg.Identity.Version.Build;
                                return $"{f.Path},{f.Date},{f.Type},{version}";
                            }
                            catch
                            {
                                Console.WriteLine("CRASH/BROKEN: " + f.Path);
                                return $"{f.Path},{f.Date},Broken,";
                            }
                        }

                }
                return $"{f.Path},{f.Date},Unknown,";
            });

            File.AppendAllLines(afilelist, newdata);

            Console.WriteLine("The end");
        }
    }
}
