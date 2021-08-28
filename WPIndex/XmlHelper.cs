using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static WPIndex.XmlDsm;

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
                XmlSerializer serializer = new(typeof(XmlMum.Assembly), "urn:schemas-microsoft-com:asm.v3");
                package = (XmlMum.Assembly)serializer.Deserialize(stream);
            }
            catch
            {
                //urn:schemas-microsoft-com:cbs
                stream.Seek(0, SeekOrigin.Begin);
                XmlSerializer serializer = new(typeof(XmlMum.Assembly), "urn:schemas-microsoft-com:cbs");
                package = (XmlMum.Assembly)serializer.Deserialize(stream);
            }
            return package;
        }

        public static XmlPhoneFm.FeatureManifest GetPhoneFmXml(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new(typeof(XmlPhoneFm.FeatureManifest));
            XmlPhoneFm.FeatureManifest package = (XmlPhoneFm.FeatureManifest)serializer.Deserialize(stream);
            return package;
        }

        public static XmlDsmDiff.DiffPackage GetDManDDSMXml(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new(typeof(XmlDsmDiff.DiffPackage));
            XmlDsmDiff.DiffPackage package = (XmlDsmDiff.DiffPackage)serializer.Deserialize(stream);
            return package;
        }

        public static XmlDsm.Package GetManDSMXml(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new(typeof(XmlDsm.Package));
            XmlDsm.Package package = (XmlDsm.Package)serializer.Deserialize(stream);
            return package;
        }

        public static XmlCix.Container GetCixXml(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new(typeof(XmlCix.Container));
            XmlCix.Container package = (XmlCix.Container)serializer.Deserialize(stream);
            return package;
        }

        public static BuildInfo.Buildinformation GetBuildInfoXml(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new(typeof(BuildInfo.Buildinformation));
            BuildInfo.Buildinformation package = (BuildInfo.Buildinformation)serializer.Deserialize(stream);
            return package;
        }

        public static Cabinet.CabinetFile GetFileLocationInCab(this Stream cab, string filename)
        {
            cab.Seek(0, SeekOrigin.Begin);
            return GetFileLocationInCab(new Cabinet.Cabinet(cab), filename);
        }

        public static Cabinet.CabinetFile GetFileLocationInCab(this Cabinet.Cabinet cab, string filename)
        {
            System.Collections.Generic.IReadOnlyCollection<Cabinet.CabinetFile> files = Cabinet.CabinetExtractor.EnumCabinetFiles(cab);
            if (files.Any(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] updatemum = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new(updatemum);
                XmlMum.Assembly package = stream.GetUpdateMum();

                XmlMum.File entry = package.Package.CustomInformation.File.First(x => x.Name.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return files.First(x => x.FileName.Equals(entry.Cabpath, System.StringComparison.InvariantCultureIgnoreCase));
            }
            else if (files.Any(x => x.FileName.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] mandsmxml = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new(mandsmxml);
                XmlDsm.Package package = stream.GetManDSMXml();

                XmlDsm.FileEntry entry = package.Files.FileEntry.First(x => x.DevicePath.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return files.First(x => x.FileName.Equals(entry.CabPath, System.StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        public static Cabinet.CabinetFile GetFileLocationInDiffCab(this Stream cab, string filename)
        {
            cab.Seek(0, SeekOrigin.Begin);
            return GetFileLocationInDiffCab(new Cabinet.Cabinet(cab), filename);
        }

        public static Cabinet.CabinetFile GetFileLocationInDiffCab(this Cabinet.Cabinet cab, string filename)
        {
            System.Collections.Generic.IReadOnlyCollection<Cabinet.CabinetFile> files = Cabinet.CabinetExtractor.EnumCabinetFiles(cab);
            /*if (files.Any(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] updatemum = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new MemoryStream(updatemum);
                XmlMum.Assembly package = stream.GetUpdateMum();

                XmlMum.File entry = package.Package.CustomInformation.File.First(x => x.Name.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return files.First(x => x.FileName.Equals(entry.Cabpath, System.StringComparison.InvariantCultureIgnoreCase));
            }
            else*/
            if (files.Any(x => x.FileName.EndsWith("dman.ddsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] mandsmxml = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("dman.ddsm.xml", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new(mandsmxml);
                XmlDsmDiff.DiffPackage package = stream.GetDManDDSMXml();

                if (files.Any(x => x.FileName.EndsWith("_manifest_.cix.xml", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    byte[] cix = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("_manifest_.cix.xml", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                    using MemoryStream stream2 = new(cix);
                    XmlCix.Container cix2 = stream2.GetCixXml();

                    XmlCix.File entry = cix2.Files.File.First(x => x.Name.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                    return files.First(x => x.FileName.EndsWith(entry.Delta.Source.Name) && x.FileName.Count(x => x == '\\') <= 1);
                }
                else
                {
                    XmlDsmDiff.DiffFileEntry entry = package.Files.DiffFileEntry.First(x => x.DevicePath.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                    return files.First(x => x.FileName.Equals(entry.CabPath, System.StringComparison.InvariantCultureIgnoreCase));
                }
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
            cab.Seek(0, SeekOrigin.Begin);
            return GetCabinetType(new Cabinet.Cabinet(cab));
        }

        public static CabinetType GetCabinetType(this Cabinet.Cabinet cab)
        {
            try
            {
                System.Collections.Generic.IReadOnlyCollection<Cabinet.CabinetFile> files = Cabinet.CabinetExtractor.EnumCabinetFiles(cab);
                if (files.Any(x => x.FileName.EndsWith("_manifest_.cix.xml", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CabinetType.CBSU;
                }
                else if (files.Any(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CabinetType.CBS;
                }
                else if (files.Any(x => x.FileName.EndsWith("dman.ddsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CabinetType.SPKU;
                }
                else if (files.Any(x => x.FileName.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
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