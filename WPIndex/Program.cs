using Cabinet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static WPIndex.XmlDsm;

namespace WPIndex
{
    public static class IEnumerableExtensions
    {
        public static IOrderedEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            int max = source
                .SelectMany(i => Regex.Matches(selector(i), @"\d+").Cast<Match>().Select(m => (int?)m.Value.Length))
                .Max() ?? 0;

            return source.OrderBy(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0')));
        }
    }

    class Program
    {
        private static byte[] GetKernel(string requestedKey)
        {
            foreach (var file in Directory.EnumerateFiles(@"W:\kernels", "*.exe", SearchOption.AllDirectories))
            {
                string key = file.Split("\\").Last().Replace(".ntoskrnl.exe", "");
                if (key == requestedKey)
                    return File.ReadAllBytes(file);
            }
            return null;
        }

        private static bool HasKernel(string requestedKey)
        {
            foreach (var file in Directory.EnumerateFiles(@"W:\kernels", "*.exe", SearchOption.AllDirectories))
            {
                string key = file.Split("\\").Last().Replace(".ntoskrnl.exe", "");
                if (key == requestedKey)
                    return true;
            }
            return false;
        }

        private static async Task Main7(string[] args)
        {
            string filelist = @"W:\cablist.type.version.csv";

            //Console.WriteLine("Reading all lines");
            int i = 0;
            var sortedCabs = File.ReadAllLines(filelist).Select(x => x.Split(",")[0]).Where(Path => Path.Contains("microsoft.mobilecore.prod.mainos.", StringComparison.InvariantCultureIgnoreCase));

            foreach (var cabinet in sortedCabs)
            {
                Console.WriteLine(cabinet);

                try
                {
                    using var stream = File.OpenRead(cabinet);
                    var cabFile = new Cabinet.Cabinet(stream);
                    var type = stream.GetCabinetType();
                    switch (type)
                    {
                        case XmlHelper.CabinetType.CBS:
                        case XmlHelper.CabinetType.SPKG:
                            {
                                string key = "";
                                switch (type.ToString())
                                {
                                    case "CBS":
                                    case "CBSU":
                                    case "CBSR":
                                        {
                                            using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "update.mum"));
                                            var MUM = xmlfile.GetUpdateMum();

                                            key = MUM.AssemblyIdentity.Version;
                                            break;
                                        }
                                    case "SPKG":
                                    case "SPKU":
                                    case "SPKR":
                                        {
                                            using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "man.dsm.xml"));
                                            var DSM = xmlfile.GetManDSMXml();

                                            key = DSM.Identity.Version.Major + "." + DSM.Identity.Version.Minor + "." + DSM.Identity.Version.QFE + "." + DSM.Identity.Version.Build;
                                            break;
                                        }

                                }

                                Console.WriteLine("-> " + key);

                                if (File.Exists(@$"W:\kernels\{key}.ntoskrnl.exe"))
                                    continue;

                                byte[] finalTargetBuildInfo = Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, XmlHelper.GetFileLocationInCab(cabFile, "ntoskrnl.exe").FileName);
                                Console.WriteLine($"{key}: {PEUtils.GetBuildNumberFromPE(finalTargetBuildInfo)}");
                                File.WriteAllBytes(@$"W:\kernels\{key}.ntoskrnl.exe", finalTargetBuildInfo);

                                break;
                            }
                        case XmlHelper.CabinetType.CBSU:
                        case XmlHelper.CabinetType.SPKU:
                            {
                                byte[] manifestDiff = Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "dman.ddsm.xml");
                                using MemoryStream manifestDiffStream = new MemoryStream(manifestDiff);
                                XmlDsmDiff.DiffPackage man = manifestDiffStream.GetDManDDSMXml();

                                string srckey = $"{man.SourceVersion.Major}.{man.SourceVersion.Minor}.{man.SourceVersion.QFE}.{man.SourceVersion.Build}";
                                string key = $"{man.TargetVersion.Major}.{man.TargetVersion.Minor}.{man.TargetVersion.QFE}.{man.TargetVersion.Build}";

                                Console.WriteLine(srckey + " -> " + key);

                                if (File.Exists(@$"W:\kernels\{key}.ntoskrnl.exe"))
                                    continue;

                                bool succeeded = false;
                                byte[] finalBaseBuildInfo = Array.Empty<byte>();
                                byte[] finalTargetBuildInfo = Array.Empty<byte>();

                                Cabinet.CabinetFile file = cabFile.GetFileLocationInDiffCab("ntoskrnl.exe");
                                byte[] buildInfoDelta = Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, file.FileName);

                                if (File.Exists(@$"W:\kernels\{srckey}.ntoskrnl.exe"))
                                {
                                    //Console.WriteLine("Attempting patching");
                                    try
                                    {
                                        finalBaseBuildInfo = GetKernel(srckey);
                                        finalTargetBuildInfo = Delta.ApplyDelta(finalBaseBuildInfo, buildInfoDelta);
                                        succeeded = true;
                                    }
                                    catch { /*Console.WriteLine("Patching failed");*/ }
                                }

                                if (!succeeded)
                                {
                                    /*ulong buildNumberBase = ulong.Parse(man.SourceVersion.QFE);
                                    ulong majorNumberBase = ulong.Parse(man.SourceVersion.Major);
                                    ulong minorNumberBase = ulong.Parse(man.SourceVersion.Minor);

                                    DateTime dateTime = file.TimeStamp.ToUniversalTime();//.Subtract(new TimeSpan(20, 0, 0));

                                    int c = 0;
                                    while (!succeeded && c <= 5000)
                                    {
                                        c++;
                                        dateTime = dateTime.Subtract(new TimeSpan(0, 1, 0));
                                        finalBaseBuildInfo = ConstructBuildInfo(majorNumberBase, minorNumberBase, buildNumberBase, dateTime, "WPMAIN");
                                        try
                                        {
                                            finalTargetBuildInfo = Delta.ApplyDelta(finalBaseBuildInfo, buildInfoDelta);
                                            succeeded = true;
                                        }
                                        catch { }
                                    }*/
                                }

                                if (!succeeded)
                                {
                                    //Console.WriteLine(":( " + key);
                                    continue;
                                }

                                Console.WriteLine($"{key}: {PEUtils.GetBuildNumberFromPE(finalBaseBuildInfo)}");
                                Console.WriteLine($"{key}: {PEUtils.GetBuildNumberFromPE(finalTargetBuildInfo)}");

                                File.WriteAllBytes(@$"W:\kernels\{key}.ntoskrnl.exe", finalTargetBuildInfo);

                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static string GetProperVersion(string version)
        {
            var maps = File.ReadAllLines(@"W:\currentmapping.csv").Select(x => x.Split(',')).Where(x => x.Any(y => y.Equals(version, StringComparison.InvariantCultureIgnoreCase)));
            
            foreach (var map in maps)
            {
                string selected = "";
                if (map.Any(x => x.Contains("8.10.") || x.Contains("8.00.") || x.Contains("9.0.")))
                {
                    if (map.Any(x => x.Contains(" ") && x.Contains(" (") && x.Count(x => x == '(') == 2))
                    {
                        selected = map.First(x => x.Contains(" ") && x.Contains(" (") && x.Count(x => x == '(') == 2);
                    }
                    else
                    {
                        selected = map.First();
                    }
                }
                else if (map.Any(x => x.Contains("8.15") || x.Contains("10.0") || x.Contains("8.2")))
                {
                    if (map.Any(x => x.Contains(" ") && x.Contains(" (") && x.Contains("10.0.") && x.Count(x => x == '(') == 1))
                    {
                        selected = map.Last(x => x.Contains(" ") && x.Contains(" (") && x.Contains("10.0.") && x.Count(x => x == '(') == 1);
                    }
                    else
                    {
                        selected = map.First();
                    }
                }

                return selected;
            }
            return version;
        }

        private static async Task Main(string[] args)
        {
            //int skip = File.ReadAllLines(@"W:\cablist.type.version.moreinfo.move.csv").Count();
            var infos = File.ReadAllLines(@"W:\cablist.type.version.moreinfo.csv").Select(x => Information.FromString(x));

            //int c = 0;
            foreach (var info in infos)
            {
                /*if (c < skip)
                {
                    c++;
                    Console.WriteLine("Skipping " + info.Path);
                    continue;
                }*/

                if (!File.Exists(info.Path))
                    continue;

                string dst = $@"W:\SortedBuilds\{GetProperVersion(info.WPTargetVersion)}";

                switch (info.Type)
                {
                    case "CBSU":
                    case "SPKU":
                        dst += $@"\Delta ({GetProperVersion(info.WPSourceVersion)})";
                        break;
                    case "CBSR":
                    case "SPKR":
                        dst += $@"\Removal";
                        break;
                    case "CBS":
                    case "SPKG":
                        dst += $@"\Canonical";
                        break;
                    default:
                        continue;
                }

                if (!Directory.Exists(dst))
                    Directory.CreateDirectory(dst);

                string fdst = Path.Combine(dst, Path.GetFileName(info.Path));

                int count = 0;

                while (count < 3)
                {
                    try
                    {
                        Console.Title = info.Path + " -> " + fdst;
                        File.Move(info.Path, fdst);
                        info.Path = fdst;
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Fail. (" + count + ")" + info.Path + " -> " + fdst);
                        count++;
                    }
                }

                if (count >= 3)
                {
                    Console.WriteLine("Fail. " + info.Path + " -> " + fdst);
                    return;
                }

                while (true)
                {
                    try
                    {
                        File.AppendAllLines(@"W:\cablist.type.version.moreinfo.move2.csv", new string[] { info.ToString() });
                        break;
                    }
                    catch { }
                }
            }
        }

        private static async Task Main0(string[] args)
        {
            /*string path = @"C:\Users\gus33000\Documents\EFUE.txt";
            string path2 = @"C:\Users\gus33000\Documents\EFUE.WP.txt";
            foreach (var element in File.ReadAllLines(path))
            {
                var name = element.Split("/").Last();
                if (name.Contains(".cbs_", StringComparison.InvariantCultureIgnoreCase) || name.Contains(".cbsu_", StringComparison.InvariantCultureIgnoreCase) || name.Contains(".spkg_", StringComparison.InvariantCultureIgnoreCase) || name.Contains(".spku_", StringComparison.InvariantCultureIgnoreCase) || name.Contains(".cbsr_", StringComparison.InvariantCultureIgnoreCase) || name.Contains(".spkr_", StringComparison.InvariantCultureIgnoreCase))
                {
                    File.AppendAllLines(path2, new string[] { element });
                }
            }*/

            /*string wpfiles = @"C:\Users\gus33000\Documents\EFUE.WP.txt";
            string dldfiles = @"W:\cablist.txt";

            var wpfilenames = File.ReadAllLines(wpfiles);
            var dldfilenames = File.ReadAllLines(dldfiles);
            foreach (var element in dldfilenames)
            {
                if (!wpfilenames.Select(x => x.Split("/").Last()).Contains(element.Split("\\").Last()))
                {
                    Console.WriteLine("Unique in DL: " + element);
                    File.AppendAllLines(@"C:\Users\gus33000\Documents\EFUE.WP.DLUNIQ.txt", new string[] { element });
                }
            }
            foreach (var element in wpfilenames)
            {
                if (!dldfilenames.Select(x => x.Split("\\").Last()).Contains(element.Split("/").Last()))
                {
                    Console.WriteLine("Unique in EFUE: " + element);
                    File.AppendAllLines(@"C:\Users\gus33000\Documents\EFUE.WP.EFUEUNIQ.txt", new string[] { element });
                }
            }*/

            string path = @"C:\Users\gus33000\Documents\EFUE.WP.txt";
            string destfile = @"C:\Users\gus33000\Documents\EFUE.WP.{0}.txt";
            foreach (var element in File.ReadAllLines(path))
            {
                var name = element.Split("/").Last();

                File.AppendAllLines(string.Format(destfile, name.Split("-")[0].Split(".")[0]), new string[] { element });
            }
        }

        private static async Task Main20(string[] args)
        {
            HashSet<string> versions = new();
            HashSet<string> dupes = new();
            string path = @"C:\Users\Gus\Documents\currentmapping.csv";
            foreach (var thing in File.ReadAllLines(path).SelectMany(x => x.Split(",")))
            {
                var old = versions.Count;
                versions.Add(thing);
                if (versions.Count == old)
                {
                    Console.WriteLine("dupe detected: " + thing);
                    dupes.Add(thing);
                }
            }

            string content = File.ReadAllText(path);
            foreach (var dupe in dupes)
            {
                content = content.Replace("\r\n" + dupe + ",", "\r\n").Replace("," + dupe + ",", ",").Replace("," + dupe + "\r\n", "\r\n");
            }
            File.WriteAllText(path, content);
        }

        private static async Task Main10(string[] args)
        {
            /*Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
            string dir = @"C:\Users\Gus\Documents\buildinfos";
            var infos = Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories).OrderByAlphaNumeric(x => x);
            foreach (var info in infos)
            {
                var key = Path.GetFileNameWithoutExtension(info).Replace(".buildinfo", "");

                if (!map.ContainsKey(key))
                    map.Add(key, new List<string>());

                using var xmlstrm = File.OpenRead(info);
                var target = XmlHelper.GetBuildInfoXml(xmlstrm);
                string buildstr = $"{target.Majorversion}.{target.Minorversion}.{target.Qfelevel}.{key.Split(".").Last()}.{target.Releaselabel}({target.Builder}).{target.Buildtime}";
                map[key].Add(buildstr);

                if (!string.IsNullOrEmpty(target.Ntrazzlebuildnumber))
                {
                    string ntver = $"{target.Ntrazzlemajorversion}.{target.Ntrazzleminorversion}.{target.Ntrazzlebuildnumber}.{target.Ntrazzlerevisionnumber} ({target.Releaselabel.ToLower()}.{target.Buildtime[2..]})";
                    map[key].Add(ntver);
                }
            }

            var maps = File.ReadAllLines(@"C:\Users\Gus\Documents\buildmap1.csv").Select(x => x.Split(","));
            foreach (var _map in maps)
            {
                string key = _map[0];
                if (!map.ContainsKey(key))
                    map.Add(key, new List<string>());

                map[key].Add(_map[1]);
            }

            foreach (var item in map.OrderByAlphaNumeric(x => x.Key))
            {
                Console.WriteLine($"{item.Key},{string.Join(",", item.Value)}");
            }*/

            /*string path = @"C:\Users\Gus\Documents\currentmapping.csv";
            var stuff = File.ReadAllLines(path).Select(x => x.Split(","));
            foreach (var thing in stuff)
            {
                var key = thing[0];
                var addlist = thing.Skip(1);
                var newaddlist = addlist.Distinct().OrderByAlphaNumeric(x => x);
                var win10list = thing.Distinct().OrderByAlphaNumeric(x => x).Where(x => x.StartsWith("10.0."));
                string s = string.Join(",", newaddlist);
                if (win10list.Count() >= 2 && win10list.First().Contains("10.0.00000."))
                {
                    var bad = win10list.First().Split(" ").First();
                    var @new = win10list.Last();
                    s = s.Replace(bad, @new);
                }
                if (s.Split(",").Any(x => x.Contains(" ")))
                {
                    var bldstr = s.Split(",").First(x => x.Contains(" "));
                    var buildnum = bldstr.Split(" ")[0].Split(".")[2];
                    var buildtag = bldstr.Replace("10.0.", "").Replace(" (", ".").Replace(")", "");
                    var betawikithing = "{{" +$"BLItem Confirmed|Windows 10 Mobile build {buildnum}|{buildtag}" + "}}";
                    //Console.WriteLine(betawikithing);
                }
                Console.WriteLine($"{key},{s}");
            }*/
            /*string kerneldir = @"C:\Users\Gus\Documents\kernels";
            string path = @"C:\Users\Gus\Documents\currentmapping.csv";
            var stuff = File.ReadAllLines(path).Select(x => x.Split(","));
            foreach (var thing in stuff)
            {
                if (thing.Any(x => File.Exists(Path.Combine(kerneldir, $"{x}.ntoskrnl.exe"))))
                {
                    var t = thing.First(x => File.Exists(Path.Combine(kerneldir, $"{x}.ntoskrnl.exe")));
                    string ver = PEUtils.GetBuildNumberFromPE(File.ReadAllBytes(Path.Combine(kerneldir, $"{t}.ntoskrnl.exe")));
                    Console.WriteLine(string.Join(",", thing) + "," + ver);
                }
                else
                {
                    Console.WriteLine(string.Join(",", thing));
                }
            }*/

            /*HashSet<(string, string)> DiffUpdates = new();

            var lns = File.ReadAllLines(@"W:\cablist.type.version.moreinfo - Copy.csv");
            foreach (var str in lns)
            {
                var data = str.Split(',');

                var Path = data[0];
                string TimeStamp = "";
                string Type = "";
                string WPTargetVersion = "";
                string WPSourceVersion = "";
                string WPBuildString = "";
                string NTVersion = "";
                string Architecture = "";
                string NTBuildString = "";

                if (data.Length >= 2)
                    TimeStamp = data[1];
                if (data.Length >= 3)
                    Type = data[2];
                if (data.Length >= 4)
                    WPTargetVersion = data[3];
                if (data.Length >= 5)
                    WPSourceVersion = data[4];
                if (data.Length >= 6)
                    WPBuildString = data[5];
                if (data.Length >= 7)
                    NTVersion = data[6];
                if (data.Length >= 8)
                    Architecture = data[7];
                if (data.Length >= 9)
                    NTBuildString = data[8];

                if (!string.IsNullOrEmpty(WPSourceVersion))
                {
                    DiffUpdates.Add((WPSourceVersion, WPTargetVersion));
                }
            }

            foreach (var el in DiffUpdates)
            {
                Console.WriteLine($"{el.Item1} -> {el.Item2}");
            }*/

            /*string path = @"C:\Users\Gus\Documents\currentmapping.csv";
            string wp8updatemappath = @"C:\Users\Gus\Documents\wp8updatemap.txt";
            var currentmapping = File.ReadAllLines(path).Select(x => x.Split(",").ToList()).ToList();
            var wp8updatemap = File.ReadAllLines(wp8updatemappath).Select(x => x.Split(",").ToList()).ToList();

            var considered = new List<List<string>>();

            while (wp8updatemap.Count() > 0)
            {
                Console.WriteLine("loop");
                Console.WriteLine(considered.Count);
                Console.WriteLine(wp8updatemap.Count);

                // update map: ver A -> ver B
                // currentmapping: version mapping

                // we need to build a list of source versions for each currentmapping entry.
                List<(List<List<string>> src, List<string> dst)> sourceToMapping = new();
                foreach (var mapping in currentmapping)
                {
                    List<List<string>> src = new();
                    foreach (var el in mapping)
                    {
                        if (wp8updatemap.Any(x => x[1] == el))
                        {
                            var mapp = wp8updatemap.First(x => x[1] == el);
                            src.Add(mapp);
                        }
                    }
                    sourceToMapping.Add((src, mapping));
                }

                // each mapping with more than one source need grouping
                foreach (var src2map in sourceToMapping.Where(x => x.src.Count >= 2))
                {
                    Console.WriteLine(src2map.dst[0] + ":");
                    foreach (var b in src2map.src)
                    {
                        Console.WriteLine(b[0]);
                    }
                    // add the info to existing current mapping

                    // find the map
                    var maps = currentmapping.Where(x => x.Union(src2map.src.Select(y => y[0])).Count() > 0);
                    foreach (var map in maps)
                        foreach (var e in src2map.src)
                            if (!map.Contains(e[0]))
                                map.Add(e[0]);

                    if (maps.Count() > 0)
                    {
                        considered.AddRange(src2map.src);
                        foreach (var e in src2map.src)
                            wp8updatemap.Remove(e);
                    }
                }

                foreach (var element in currentmapping)
                {
                    //Console.WriteLine(string.Join(",", element));
                }

                Console.ReadLine();
            }*/

            /*string filelist = @"W:\cablist.type.version.csv";
            HashSet<string> vers = new();
            foreach (var f in File.ReadAllLines(filelist))
            {
                vers.Add(f.Split(",").Last());
            }

            foreach (var e in vers.OrderByAlphaNumeric(x => x))
            {
                Console.WriteLine(e);
            }*/

            string kerneldir = @"W:\kernels";
            string path = @"W:\currentmapping.csv";
            string vers = @"W:\listofversions.txt";

            /*foreach (var l in File.ReadAllLines(path))
            {
                //string nl = l.Replace("8.15", "10.0").Replace("8.20", "10.0") + "," + l;
                string nl = l;
                if (l.Contains("rs1") && l.Contains(".1000 "))
                {
                    nl = l + "," + string.Join(",", l.Split(",").Where(x => x.StartsWith("10.0.") && x.EndsWith(".1000")).Select(x => x.Replace(".1000", ".0")));
                }

                nl = string.Join(",", nl.Split(",").Select(x =>
                {
                    if (x.Contains("(") && !x.Contains(" "))
                    {
                        var t = x.Split(".");
                        return $"{t[0]}.{t[1]}.{t[2]}.{t[3]} ({string.Join(".", t.Skip(4))})";
                    }
                    return x;
                }).Distinct().OrderByAlphaNumeric(x => x).OrderBy(x => x.Length));
                Console.WriteLine(nl);
            }
            return;*/
            var stuff = File.ReadAllLines(path).Select(x => x.Split(","));
            /*List<string> es = new();
            foreach (var thing in stuff)
            {
                var e = "";
                if (thing.Any(x => File.Exists(Path.Combine(kerneldir, $"{x}.ntoskrnl.exe"))))
                {
                    var t = thing.First(x => File.Exists(Path.Combine(kerneldir, $"{x}.ntoskrnl.exe")));
                    string ver = PEUtils.GetBuildNumberFromPE(File.ReadAllBytes(Path.Combine(kerneldir, $"{t}.ntoskrnl.exe")));
                    e = string.Join(",", thing) + "," + ver;
                }
                else
                {
                    e = string.Join(",", thing);
                }
                es.Add(e);
                Console.WriteLine(e);
            }

            Console.WriteLine("==============");
            Console.WriteLine();*/

            string filelist = @"W:\cablist.type.version.csv";
            foreach (var element in File.ReadAllLines(filelist))
            {
                var ver = element.Split(",").Last();
                var file = element.Split(",").First();

                if (stuff.Any(x => x.Contains(ver)))
                {
                    //Console.WriteLine(ver + " -> OK");
                }
                else
                {
                    //Console.WriteLine(ver + " -> NAK");
                    Console.WriteLine(element);
                }
            }
        }

        private static async Task Main4(string[] args)
        {
            string filelist = @"W:\cablist.type.version.csv";

            //Console.WriteLine("Reading all lines");
            int i = 0;
            var sortedCabs = File.ReadAllLines(filelist).Select(x => x.Split(",")[0]).Where(Path => Path.Contains("microsoft.phonefm.", StringComparison.InvariantCultureIgnoreCase));

            foreach (var phonefmspkg in sortedCabs)
            {
                try
                {
                    using var stream = File.OpenRead(phonefmspkg);
                    var cabFile = new Cabinet.Cabinet(stream);

                    using var phonefmxml = new MemoryStream(cabFile.ReadFile(cabFile.Files.First(x => x.FileName.EndsWith(".xml") && x.FileName.Contains("croso")).FileName));
                    var phonefm = phonefmxml.GetPhoneFmXml();
                    var versions = phonefm.BasePackages.PackageFile.Where(x => !string.IsNullOrEmpty(x.Version)).Select(x => x.Version).Distinct().Reverse();
                    Console.WriteLine(string.Join(",", versions));
                }
                catch { }// (Exception ex) { Console.WriteLine(phonefmspkg);  Console.WriteLine(ex.ToString()); }
            }
        }

        private static async Task Main3(string[] args)
        {
            string dir = @"C:\Users\Gus\Documents\buildinfos";
            var infos = Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories).OrderByAlphaNumeric(x => x);
            foreach (var info in infos)
            {
                var key = Path.GetFileNameWithoutExtension(info).Replace(".buildinfo", "");
                using var xmlstrm = File.OpenRead(info);
                var target = XmlHelper.GetBuildInfoXml(xmlstrm);
                string buildstr = $"{target.Majorversion}.{target.Minorversion}.{target.Qfelevel}.{key.Split(".").Last()}.{target.Releaselabel}({target.Builder}).{target.Buildtime}";
                Console.WriteLine($"WP ({key}): " + buildstr);

                if (!string.IsNullOrEmpty(target.Ntrazzlebuildnumber))
                {
                    string ntver = $"{target.Ntrazzlemajorversion}.{target.Ntrazzleminorversion}.{target.Ntrazzlebuildnumber}.{target.Ntrazzlerevisionnumber} ({target.Releaselabel.ToLower()}.{target.Buildtime[2..]})";
                    Console.WriteLine($"NT ({key}): " + ntver);
                }
            }
        }


        private static Dictionary<string, byte[]> buildInfoMapping = new();

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

        private static async Task Main1(string[] args)
        {
            string filelist = @"W:\cablist.type.version.csv";

            foreach (var file in Directory.EnumerateFiles(@"W:\buildinfos", "*.xml", SearchOption.AllDirectories))
            {
                string key = file.Split("\\").Last().Replace(".buildinfo.xml", "");
                Console.WriteLine("Loading " + key);
                buildInfoMapping.Add(key, File.ReadAllBytes(file));
            }

            //Console.WriteLine("Reading all lines");
            int i = 0;
            var sortedCabs = File.ReadAllLines(filelist).Select(x => x.Split(",")[0]).Where(Path => Path.Contains("microsoft.mainos.production.cbsu", StringComparison.InvariantCultureIgnoreCase));

            foreach (var cabinet in sortedCabs)
            {
                Console.WriteLine(cabinet);

                try
                {
                    using var stream = File.OpenRead(cabinet);
                    var cabFile = new Cabinet.Cabinet(stream);
                    var type = stream.GetCabinetType();
                    switch (type)
                    {
                        case XmlHelper.CabinetType.CBS:
                        case XmlHelper.CabinetType.SPKG:
                            {
                                string key = "";
                                switch (type.ToString())
                                {
                                    case "CBS":
                                    case "CBSU":
                                    case "CBSR":
                                        {
                                            using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "update.mum"));
                                            var MUM = xmlfile.GetUpdateMum();

                                            key = MUM.AssemblyIdentity.Version;
                                            break;
                                        }
                                    case "SPKG":
                                    case "SPKU":
                                    case "SPKR":
                                        {
                                            using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "man.dsm.xml"));
                                            var DSM = xmlfile.GetManDSMXml();

                                            key = DSM.Identity.Version.Major + "." + DSM.Identity.Version.Minor + "." + DSM.Identity.Version.QFE + "." + DSM.Identity.Version.Build;
                                            break;
                                        }

                                }

                                Console.WriteLine("-> " + key);

                                if (File.Exists(@$"W:\buildinfos\{key}.buildinfo.xml"))
                                    continue;

                                byte[] finalTargetBuildInfo = Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, XmlHelper.GetFileLocationInCab(cabFile, "buildinfo.xml").FileName);

                                Console.WriteLine(System.Text.Encoding.UTF8.GetString(finalTargetBuildInfo));
                                using MemoryStream targetBuild = new MemoryStream(finalTargetBuildInfo);

                                BuildInfo.Buildinformation target = targetBuild.GetBuildInfoXml();

                                //Console.WriteLine($"Source: {source.Majorversion}.{source.Minorversion}.{source.Qfelevel}.{man.SourceVersion.Build}.{source.Releaselabel}({source.Builder}).{source.Buildtime}");
                                Console.WriteLine($"{key}: {target.Majorversion}.{target.Minorversion}.{target.Qfelevel}.{key.Split(".").Last()}.{target.Releaselabel}({target.Builder}).{target.Buildtime}");

                                buildInfoMapping.Add(key, finalTargetBuildInfo);
                                File.WriteAllBytes(@$"W:\buildinfos\{key}.buildinfo.xml", finalTargetBuildInfo);

                                break;
                            }
                        case XmlHelper.CabinetType.CBSU:
                        case XmlHelper.CabinetType.SPKU:
                            {
                                byte[] manifestDiff = Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "dman.ddsm.xml");
                                using MemoryStream manifestDiffStream = new MemoryStream(manifestDiff);
                                XmlDsmDiff.DiffPackage man = manifestDiffStream.GetDManDDSMXml();

                                string srckey = $"{man.SourceVersion.Major}.{man.SourceVersion.Minor}.{man.SourceVersion.QFE}.{man.SourceVersion.Build}";
                                string key = $"{man.TargetVersion.Major}.{man.TargetVersion.Minor}.{man.TargetVersion.QFE}.{man.TargetVersion.Build}";

                                Console.WriteLine(srckey + " -> " + key);

                                if (buildInfoMapping.ContainsKey(key))
                                    continue;

                                bool succeeded = false;
                                byte[] finalBaseBuildInfo = Array.Empty<byte>();
                                byte[] finalTargetBuildInfo = Array.Empty<byte>();

                                Cabinet.CabinetFile file = cabFile.GetFileLocationInDiffCab("buildinfo.xml");
                                byte[] buildInfoDelta = Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, file.FileName);

                                if (buildInfoMapping.ContainsKey(srckey))
                                {
                                    //Console.WriteLine("Attempting patching");
                                    try
                                    {
                                        finalBaseBuildInfo = buildInfoMapping[srckey];
                                        finalTargetBuildInfo = Delta.ApplyDelta(finalBaseBuildInfo, buildInfoDelta);
                                        succeeded = true;
                                    }
                                    catch { /*Console.WriteLine("Patching failed");*/ }
                                }

                                if (!succeeded)
                                {
                                    /*ulong buildNumberBase = ulong.Parse(man.SourceVersion.QFE);
                                    ulong majorNumberBase = ulong.Parse(man.SourceVersion.Major);
                                    ulong minorNumberBase = ulong.Parse(man.SourceVersion.Minor);

                                    DateTime dateTime = file.TimeStamp.ToUniversalTime();//.Subtract(new TimeSpan(20, 0, 0));

                                    int c = 0;
                                    while (!succeeded && c <= 5000)
                                    {
                                        c++;
                                        dateTime = dateTime.Subtract(new TimeSpan(0, 1, 0));
                                        finalBaseBuildInfo = ConstructBuildInfo(majorNumberBase, minorNumberBase, buildNumberBase, dateTime, "WPMAIN");
                                        try
                                        {
                                            finalTargetBuildInfo = Delta.ApplyDelta(finalBaseBuildInfo, buildInfoDelta);
                                            succeeded = true;
                                        }
                                        catch { }
                                    }*/
                                }

                                if (!succeeded)
                                {
                                    //Console.WriteLine(":( " + key);
                                    continue;
                                }

                                Console.WriteLine(System.Text.Encoding.UTF8.GetString(finalBaseBuildInfo));
                                using MemoryStream sourceBuild = new MemoryStream(finalBaseBuildInfo);

                                Console.WriteLine(System.Text.Encoding.UTF8.GetString(finalTargetBuildInfo));
                                using MemoryStream targetBuild = new MemoryStream(finalTargetBuildInfo);

                                BuildInfo.Buildinformation source = sourceBuild.GetBuildInfoXml();
                                BuildInfo.Buildinformation target = targetBuild.GetBuildInfoXml();

                                //Console.WriteLine($"Source: {source.Majorversion}.{source.Minorversion}.{source.Qfelevel}.{man.SourceVersion.Build}.{source.Releaselabel}({source.Builder}).{source.Buildtime}");
                                Console.WriteLine($"{key}: {target.Majorversion}.{target.Minorversion}.{target.Qfelevel}.{man.TargetVersion.Build}.{target.Releaselabel}({target.Builder}).{target.Buildtime}");

                                buildInfoMapping.Add(key, finalTargetBuildInfo);
                                File.WriteAllBytes(@$"W:\buildinfos\{key}.buildinfo.xml", finalTargetBuildInfo);

                                break;
                            }
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


        static async Task BuildInformation(string filelist)
        {
            Console.WriteLine("Reading all lines");
            Information[] files = File.ReadAllLines(filelist).Select(x =>
            {
                Console.WriteLine(x);
                return Information.FromString(x);
            }).ToArray();
        }

        static async Task Main2(string[] args)
        {
            string filelist = @"W:\cablist.type.version.csv";

            Console.WriteLine("Reading all lines");
            int i = 0;
            var lines = File.ReadAllLines(filelist);
            int total = lines.Length;
            var newlines = lines.Select(x =>
            {
                Console.Title = ++i + "/" + total;
                return Information.FromString(x).ToString();
            }).AsParallel().AsOrdered();

            File.WriteAllLines(@"W:\cablist.type.version.moreinfo.csv", newlines);

            Console.WriteLine("The end");
        }
    }
}
