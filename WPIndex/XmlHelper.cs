﻿using System.IO;
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
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new XmlSerializer(typeof(XmlDsmDiff.DiffPackage));
            XmlDsmDiff.DiffPackage package = (XmlDsmDiff.DiffPackage)serializer.Deserialize(stream);
            return package;
        }

        public static XmlDsm.Package GetManDSMXml(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new XmlSerializer(typeof(XmlDsm.Package));
            XmlDsm.Package package = (XmlDsm.Package)serializer.Deserialize(stream);
            return package;
        }

        public static BuildInfo.Buildinformation GetBuildInfoXml(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new XmlSerializer(typeof(BuildInfo.Buildinformation));
            BuildInfo.Buildinformation package = (BuildInfo.Buildinformation)serializer.Deserialize(stream);
            return package;
        }

        public static Cabinet.CabinetFile GetFileLocationInCab(this Stream cab, string filename)
        {
            cab.Seek(0, SeekOrigin.Begin);
            System.Collections.Generic.IReadOnlyCollection<Cabinet.CabinetFile> files = Cabinet.CabinetExtractor.EnumCabinetFiles(cab);
            if (files.Any(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] updatemum = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new MemoryStream(updatemum);
                XmlMum.Assembly package = stream.GetUpdateMum();

                XmlMum.File entry = package.Package.CustomInformation.File.First(x => x.Name.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return files.First(x => x.FileName.Equals(entry.Cabpath, System.StringComparison.InvariantCultureIgnoreCase));
            }
            else if (files.Any(x => x.FileName.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] mandsmxml = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("man.dsm.xml", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new MemoryStream(mandsmxml);
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
            System.Collections.Generic.IReadOnlyCollection<Cabinet.CabinetFile> files = Cabinet.CabinetExtractor.EnumCabinetFiles(cab);
            /*if (files.Any(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] updatemum = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("update.mum", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new MemoryStream(updatemum);
                XmlMum.Assembly package = stream.GetUpdateMum();

                XmlMum.File entry = package.Package.CustomInformation.File.First(x => x.Name.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return files.First(x => x.FileName.Equals(entry.Cabpath, System.StringComparison.InvariantCultureIgnoreCase));
            }
            else*/ if (files.Any(x => x.FileName.EndsWith("dman.ddsm.xml", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                byte[] mandsmxml = Cabinet.CabinetExtractor.ExtractCabinetFile(cab, files.First(x => x.FileName.EndsWith("dman.ddsm.xml", System.StringComparison.InvariantCultureIgnoreCase)).FileName);

                using MemoryStream stream = new MemoryStream(mandsmxml);
                XmlDsmDiff.DiffPackage package = stream.GetDManDDSMXml();

                XmlDsmDiff.DiffFileEntry entry = package.Files.DiffFileEntry.First(x => x.DevicePath.EndsWith(filename, System.StringComparison.InvariantCultureIgnoreCase));
                return files.First(x => x.FileName.Equals(entry.CabPath, System.StringComparison.InvariantCultureIgnoreCase));
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
