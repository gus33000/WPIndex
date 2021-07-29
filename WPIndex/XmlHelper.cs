using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace WPIndex
{
    public static class XmlHelper
    {
        public static XmlMum.Assembly GetUpdateMum(this Stream stream)
        {
            XmlMum.Assembly package;
            try
            {
                //urn:schemas-microsoft-com:asm.v3
                stream.Seek(0, SeekOrigin.Begin);
                XmlSerializer serializer = new XmlSerializer(typeof(XmlMum.Assembly), "urn:schemas-microsoft-com:asm.v3");
                package = (XmlMum.Assembly)serializer.Deserialize(stream);
            }
            catch
            {
                //urn:schemas-microsoft-com:cbs
                stream.Seek(0, SeekOrigin.Begin);
                XmlSerializer serializer = new XmlSerializer(typeof(XmlMum.Assembly), "urn:schemas-microsoft-com:cbs");
                package = (XmlMum.Assembly)serializer.Deserialize(stream);
            }
            return package;
        }

        public static XmlDsmDiff.DiffPackage GetDManDDSMXml(this Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlDsmDiff.DiffPackage));
            XmlDsmDiff.DiffPackage package = (XmlDsmDiff.DiffPackage)serializer.Deserialize(stream);
            return package;
        }

        public static XmlDsm.Package GetManDSMXml(this Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlDsm.Package));
            XmlDsm.Package package = (XmlDsm.Package)serializer.Deserialize(stream);
            return package;
        }

        public static BuildInfo.Buildinformation GetBuildInfoXml(this Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BuildInfo.Buildinformation));
            BuildInfo.Buildinformation package = (BuildInfo.Buildinformation)serializer.Deserialize(stream);
            return package;
        }

        public static string GetFileLocationInCab(this Stream cab, string filename)
        {
            var files = Cabinet.CabinetExtractor.EnumCabinetFiles(cab);
            if (files.Any(x => x.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                var updatemum = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)));

                using var stream = new MemoryStream(updatemum);
                var package = stream.GetUpdateMum();

                var entry = package.Package.CustomInformation.File.First(x => x.Name.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return entry.Cabpath;
            }
            else if (files.Any(x => x.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                var mandsmxml = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)));

                using var stream = new MemoryStream(mandsmxml);
                var package = stream.GetManDSMXml();

                var entry = package.Files.FileEntry.First(x => x.DevicePath.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return entry.CabPath;
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        public enum CabinetType
        {
            CBS,
            CBSU,
            SPKG,
            SPKU,
            Unknown,
            Broken
        }

        public static CabinetType GetCabinetType(this Stream cab)
        {
            try
            {
                var files = Cabinet.CabinetExtractor.EnumCabinetFiles(cab);
                if (files.Any(x => x.EndsWith("_manifest_.cix.xml", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CabinetType.CBSU;
                }
                else if (files.Any(x => x.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CabinetType.CBS;
                }
                else if (files.Any(x => x.EndsWith("dman.ddsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CabinetType.SPKU;
                }
                else if (files.Any(x => x.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CabinetType.SPKG;
                }
                else
                {
                    return CabinetType.Unknown;
                }
            }
            catch
            {
                return CabinetType.Broken;
            }
        }
    }
}
