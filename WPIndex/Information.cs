using System;
using System.IO;
using System.Linq;

namespace WPIndex
{

    /// <summary>
    /// TODO: CBSU/SPKU
    /// </summary>
    public class Information
    {
        private Cabinet.Cabinet cabFile;

        private string path = "";
        private string timestamp = "";
        private string type = "";
        private string wptargetversion = "";
        private string wpsourceversion = "";
        private string wpbuildstring = "";
        private string ntversion = "";
        private string ntbuildstring = "";
        private string architecture = "";

        private XmlMum.Assembly mum;
        private XmlDsm.Package dsm;
        private XmlDsmDiff.DiffPackage diffdsm;

        public XmlMum.Assembly MUM
        { 
            get
            {
                if (mum != null)
                    return mum;
                using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "update.mum"));
                mum = xmlfile.GetUpdateMum();
                return mum;
            } 
        }

        public XmlDsm.Package DSM
        {
            get
            {
                if (dsm != null)
                    return dsm;
                using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "man.dsm.xml"));
                dsm = xmlfile.GetManDSMXml();
                return dsm;
            }
        }

        public XmlDsmDiff.DiffPackage DiffDSM
        {
            get
            {
                if (diffdsm != null)
                    return diffdsm;
                using var xmlfile = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, "dman.ddsm.xml"));
                diffdsm = xmlfile.GetDManDDSMXml();
                return diffdsm;
            }
        }

        public string Path { get => path; set => path = value; }
        public string TimeStamp { get => timestamp; set => timestamp = value; }
        public string Type { get => type; set => type = value; }
        public string WPTargetVersion { get => wptargetversion; set => wptargetversion = value; }
        public string WPSourceVersion { get => wpsourceversion; set => wpsourceversion = value; }
        public string Architecture { get => architecture; set => architecture = value; }

        // Canonical only for now
        public string WPBuildString { get => wpbuildstring; set => wpbuildstring = value; }
        public string NTVersion { get => ntversion; set => ntversion = value; }
        public string NTBuildString { get => ntbuildstring; set => ntbuildstring = value; }

        public override string ToString()
        {
            return $"{Path},{TimeStamp},{Type},{WPTargetVersion},{WPSourceVersion},{WPBuildString},{NTVersion},{Architecture},{NTBuildString}";
        }

        private Information() { }

        public Information(string path)
        {
            this.path = path;
            GetTimeStamp();

            try
            {
                using var strm = File.OpenRead(path);
                cabFile = new(strm);

                GetCabType(); // slow
                GetWPTargetVersion();
                GetWPSourceVersion();
                GetArchitecture();


                if (Path.Contains("microsoft.mainos.production", StringComparison.InvariantCultureIgnoreCase))
                    GetBuildInfo(); // slow
                if (Path.Contains("microsoft.mobilecore.kernelbaseprod", StringComparison.InvariantCultureIgnoreCase) ||
                    Path.Contains("microsoft.mobilecore.prod.mainos", StringComparison.InvariantCultureIgnoreCase) ||
                    Path.Contains("microsoft.mobilecore.mainos", StringComparison.InvariantCultureIgnoreCase))
                    GetNTVersion();
            }
            catch { }

            diffdsm = null;
            dsm = null;
            mum = null;
            cabFile = null;
        }

        private void GetTimeStamp()
        {
            timestamp = new FileInfo(path).LastWriteTimeUtc.ToString("o");
        }

        private void GetWPTargetVersion()
        {
            switch (type)
            {
                case "CBS":
                case "CBSU":
                case "CBSR":
                    {
                        wptargetversion = MUM.AssemblyIdentity.Version;
                        break;
                    }
                case "SPKG":
                case "SPKU":
                case "SPKR":
                    {
                        wptargetversion = DSM.Identity.Version.Major + "." + DSM.Identity.Version.Minor + "." + DSM.Identity.Version.QFE + "." + DSM.Identity.Version.Build;
                        break;
                    }

            }
        }

        private void GetArchitecture()
        {
            switch (type)
            {
                case "CBS":
                case "CBSU":
                case "CBSR":
                    {
                        architecture = MUM.AssemblyIdentity.ProcessorArchitecture.ToLower();
                        break;
                    }
                case "SPKG":
                case "SPKU":
                case "SPKR":
                    {
                        architecture = DSM.CpuType.ToLower();
                        break;
                    }

            }
        }

        private void GetWPSourceVersion()
        {
            if (!string.IsNullOrEmpty(wpsourceversion))
                return;

            switch (type)
            {
                case "CBSU":
                case "SPKU":
                    {
                        wpsourceversion = DiffDSM.SourceVersion.Major + "." + DiffDSM.SourceVersion.Minor + "." + DiffDSM.SourceVersion.QFE + "." + DiffDSM.SourceVersion.Build;
                        break;
                    }

            }
        }

        private void GetCabType()
        {
            type = XmlHelper.GetCabinetType(cabFile).ToString();
            if (type.Equals("cbs", StringComparison.InvariantCultureIgnoreCase))
            {
                if (MUM.DisplayName.EndsWith("Recall", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = "CBSR";
                }
            }
            else if (type.Equals("spkg", StringComparison.InvariantCultureIgnoreCase))
            {
                if (DSM.IsRemoval.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = "SPKR";
                }
            }
        }

        private void FixCabType()
        {
            if (type.Equals("unknown", StringComparison.InvariantCultureIgnoreCase) || type.Equals("broken", StringComparison.InvariantCultureIgnoreCase))
            {
                type = XmlHelper.GetCabinetType(cabFile).ToString();

                GetWPTargetVersion();
                GetTimeStamp();
            }

            if (type.Equals("cbs", StringComparison.InvariantCultureIgnoreCase))
            {
                if (MUM.DisplayName.EndsWith("Recall", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = "CBSR";
                }
            }
            else if (type.Equals("spkg", StringComparison.InvariantCultureIgnoreCase))
            {
                if (DSM.IsRemoval.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = "SPKR";
                }
            }
        }

        // ntbuildstring, wpbuildstring, ntversion
        #region Canonical only
        private void GetNTVersion()
        {
            if (!string.IsNullOrEmpty(ntbuildstring))
                return;

            string buildstr = "";
            if (type.Equals("cbs", StringComparison.InvariantCultureIgnoreCase))
            {
                var pkg = MUM;

                if (pkg.Package.CustomInformation.File.Any(x => x.Name.Contains(@"\ntoskrnl.exe", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var entry = pkg.Package.CustomInformation.File.First(x => x.Name.Contains(@"\ntoskrnl.exe", StringComparison.InvariantCultureIgnoreCase));

                    buildstr = PEUtils.GetBuildNumberFromPE(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, entry.Cabpath));
                }
            }
            else if (type.Equals("spkg", StringComparison.InvariantCultureIgnoreCase))
            {
                var pkg = DSM;

                if (pkg.Files.FileEntry.Any(x => x.DevicePath.Contains(@"\ntoskrnl.exe", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var entry = pkg.Files.FileEntry.First(x => x.DevicePath.Contains(@"\ntoskrnl.exe", StringComparison.InvariantCultureIgnoreCase));

                    buildstr = PEUtils.GetBuildNumberFromPE(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, entry.CabPath));
                }
            }
            ntbuildstring = buildstr;
        }

        private void GetBuildInfo()
        {
            if (!string.IsNullOrEmpty(wpbuildstring))
                return;

            string buildstr = "";
            string ntver = "";
            if (type.Equals("cbs", StringComparison.InvariantCultureIgnoreCase))
            {
                var pkg = MUM;

                if (pkg.Package.CustomInformation.File.Any(x => x.Name.Contains(@"\buildinfo.xml", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var entry = pkg.Package.CustomInformation.File.First(x => x.Name.Contains(@"\buildinfo.xml", StringComparison.InvariantCultureIgnoreCase));

                    using var xmlstrm = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, entry.Cabpath));
                    var target = XmlHelper.GetBuildInfoXml(xmlstrm);
                    buildstr = $"{target.Majorversion}.{target.Minorversion}.{target.Qfelevel}.{pkg.AssemblyIdentity.Version.Split(".").Last()}.{target.Releaselabel}({target.Builder}).{target.Buildtime}";

                    if (!string.IsNullOrEmpty(target.Ntrazzlebuildnumber))
                    {
                        ntver = $"{target.Ntrazzlemajorversion}.{target.Ntrazzleminorversion}.{target.Ntrazzlebuildnumber}.{target.Ntrazzlerevisionnumber} ({target.Releaselabel.ToLower()}.{target.Buildtime[2..]})";
                    }
                }
            }
            else if (type.Equals("spkg", StringComparison.InvariantCultureIgnoreCase))
            {
                var pkg = DSM;

                if (pkg.Files.FileEntry.Any(x => x.DevicePath.Contains(@"\buildinfo.xml", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var entry = pkg.Files.FileEntry.First(x => x.DevicePath.Contains(@"\buildinfo.xml", StringComparison.InvariantCultureIgnoreCase));

                    using var xmlstrm = new MemoryStream(Cabinet.CabinetExtractor.ExtractCabinetFile(cabFile, entry.CabPath));
                    var target = XmlHelper.GetBuildInfoXml(xmlstrm);
                    buildstr = $"{target.Majorversion}.{target.Minorversion}.{target.Qfelevel}.{pkg.Identity.Version.Build}.{target.Releaselabel}({target.Builder}).{target.Buildtime}";

                    if (!string.IsNullOrEmpty(target.Ntrazzlebuildnumber))
                    {
                        ntver = $"{target.Ntrazzlemajorversion}.{target.Ntrazzleminorversion}.{target.Ntrazzlebuildnumber}.{target.Ntrazzlerevisionnumber} ({target.Releaselabel.ToLower()}.{target.Buildtime[2..]})";
                    }
                }
            }
            wpbuildstring = buildstr;
            ntversion = ntver;
        }
        #endregion

        public static Information FromString(string str)
        {
            Information info = new();
            var data = str.Split(',');

            if (data.Length <= 0)
            {
                throw new InvalidDataException();
            }

            info.Path = data[0];

            if (data.Length >= 2)
                info.TimeStamp = data[1];
            if (data.Length >= 3)
                info.Type = data[2];
            if (data.Length >= 4)
                info.WPTargetVersion = data[3];
            if (data.Length >= 5)
                info.WPSourceVersion = data[4];
            if (data.Length >= 6)
                info.WPBuildString = data[5];
            if (data.Length >= 7)
                info.NTVersion = data[6];
            if (data.Length >= 8)
                info.Architecture = data[7];
            if (data.Length >= 9)
                info.NTBuildString = data[8];

            /*if (string.IsNullOrEmpty(info.TimeStamp))
                info.GetTimeStamp();

            try
            {
                using var strm = File.OpenRead(info.Path);
                info.cabFile = new(strm);

                if (string.IsNullOrEmpty(info.WPTargetVersion))
                    info.GetWPTargetVersion();

                if (!string.IsNullOrEmpty(info.Type))
                    info.FixCabType();
                else
                    info.GetCabType();

                info.GetWPSourceVersion();
                info.GetArchitecture();
                if (info.Path.Contains("microsoft.mainos.production", StringComparison.InvariantCultureIgnoreCase))
                    info.GetBuildInfo();
                if (info.Path.Contains("microsoft.mobilecore.kernelbaseprod", StringComparison.InvariantCultureIgnoreCase) ||
                    info.Path.Contains("microsoft.mobilecore.prod.mainos", StringComparison.InvariantCultureIgnoreCase) ||
                    info.Path.Contains("microsoft.mobilecore.mainos", StringComparison.InvariantCultureIgnoreCase))
                    info.GetNTVersion();
            }
            catch { }*/

            info.diffdsm = null;
            info.dsm = null;
            info.mum = null;
            info.cabFile = null;
            return info;
        }
    }
}
