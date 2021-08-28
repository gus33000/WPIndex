// Copyright (c) 2018, Gustave M. - gus33000.me - @gus33000
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace WPIndex
{
    public static class XmlPhoneFm
    {
        [XmlRoot(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class PackageFile
        {
            [XmlAttribute(AttributeName = "Path")]
            public string Path { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "Partition")]
            public string Partition { get; set; }
            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
            [XmlAttribute(AttributeName = "Version")]
            public string Version { get; set; }
            [XmlAttribute(AttributeName = "FeatureIdentifierPackage")]
            public string FeatureIdentifierPackage { get; set; }
            [XmlAttribute(AttributeName = "Resolution")]
            public string Resolution { get; set; }
            [XmlAttribute(AttributeName = "Language")]
            public string Language { get; set; }
            [XmlElement(ElementName = "FeatureIDs", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public FeatureIDs FeatureIDs { get; set; }
            [XmlAttribute(AttributeName = "ReleaseType")]
            public string ReleaseType { get; set; }
            [XmlAttribute(AttributeName = "Type")]
            public string Type { get; set; }
            [XmlAttribute(AttributeName = "CPUType")]
            public string CPUType { get; set; }
            [XmlAttribute(AttributeName = "SOC")]
            public string SOC { get; set; }
        }

        [XmlRoot(ElementName = "BasePackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class BasePackages
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<PackageFile> PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "BootUILanguagePackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class BootUILanguagePackageFile
        {
            [XmlAttribute(AttributeName = "Path")]
            public string Path { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "FeatureIdentifierPackage")]
            public string FeatureIdentifierPackage { get; set; }
            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
        }

        [XmlRoot(ElementName = "BootLocalePackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class BootLocalePackageFile
        {
            [XmlAttribute(AttributeName = "Path")]
            public string Path { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "FeatureIdentifierPackage")]
            public string FeatureIdentifierPackage { get; set; }
            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
        }

        [XmlRoot(ElementName = "FeatureIDs", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class FeatureIDs
        {
            [XmlElement(ElementName = "FeatureID", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<string> FeatureID { get; set; }
        }

        [XmlRoot(ElementName = "Microsoft", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class Microsoft
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<PackageFile> PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "FeatureGroup", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class FeatureGroup
        {
            [XmlElement(ElementName = "FeatureIDs", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public FeatureIDs FeatureIDs { get; set; }
            [XmlAttribute(AttributeName = "Name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "PublishingFeatureGroup")]
            public string PublishingFeatureGroup { get; set; }
            [XmlAttribute(AttributeName = "Constraint")]
            public string Constraint { get; set; }
            [XmlElement(ElementName = "SubGroups", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public SubGroups SubGroups { get; set; }
        }

        [XmlRoot(ElementName = "SubGroups", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class SubGroups
        {
            [XmlElement(ElementName = "FeatureGroup", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public FeatureGroup FeatureGroup { get; set; }
        }

        [XmlRoot(ElementName = "MSFeatureGroups", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class MSFeatureGroups
        {
            [XmlElement(ElementName = "FeatureGroup", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<FeatureGroup> FeatureGroup { get; set; }
        }

        [XmlRoot(ElementName = "Features", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class Features
        {
            [XmlElement(ElementName = "Microsoft", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public Microsoft Microsoft { get; set; }
            [XmlElement(ElementName = "MSFeatureGroups", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public MSFeatureGroups MSFeatureGroups { get; set; }
            [XmlElement(ElementName = "OEM", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string OEM { get; set; }
            [XmlElement(ElementName = "OEMFeatureGroups", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string OEMFeatureGroups { get; set; }
        }

        [XmlRoot(ElementName = "ReleasePackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class ReleasePackages
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<PackageFile> PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "PrereleasePackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class PrereleasePackages
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public PackageFile PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "DeviceLayoutPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class DeviceLayoutPackages
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<PackageFile> PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "SOCPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class SOCPackages
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<PackageFile> PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "SpeechPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class SpeechPackages
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public PackageFile PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "KeyboardPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class KeyboardPackages
        {
            [XmlElement(ElementName = "PackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public PackageFile PackageFile { get; set; }
        }

        [XmlRoot(ElementName = "FeatureManifest", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class FeatureManifest
        {
            [XmlElement(ElementName = "BasePackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public BasePackages BasePackages { get; set; }
            [XmlElement(ElementName = "BootUILanguagePackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public BootUILanguagePackageFile BootUILanguagePackageFile { get; set; }
            [XmlElement(ElementName = "BootLocalePackageFile", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public BootLocalePackageFile BootLocalePackageFile { get; set; }
            [XmlElement(ElementName = "Features", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public Features Features { get; set; }
            [XmlElement(ElementName = "ReleasePackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public ReleasePackages ReleasePackages { get; set; }
            [XmlElement(ElementName = "PrereleasePackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public PrereleasePackages PrereleasePackages { get; set; }
            [XmlElement(ElementName = "DeviceLayoutPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public DeviceLayoutPackages DeviceLayoutPackages { get; set; }
            [XmlElement(ElementName = "SOCPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public SOCPackages SOCPackages { get; set; }
            [XmlElement(ElementName = "SpeechPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public SpeechPackages SpeechPackages { get; set; }
            [XmlElement(ElementName = "KeyboardPackages", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public KeyboardPackages KeyboardPackages { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsd { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }
    }

    public static class XmlCix
    {
        [XmlRoot(ElementName = "Location", Namespace = "urn:ContainerIndex")]
        public class Location
        {
            [XmlAttribute(AttributeName = "path")]
            public string Path { get; set; }
            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }
        }

        [XmlRoot(ElementName = "DeltaBasisSearch", Namespace = "urn:ContainerIndex")]
        public class DeltaBasisSearch
        {
            [XmlElement(ElementName = "Location", Namespace = "urn:ContainerIndex")]
            public List<Location> Location { get; set; }
        }

        [XmlRoot(ElementName = "Hash", Namespace = "urn:ContainerIndex")]
        public class Hash
        {
            [XmlAttribute(AttributeName = "alg")]
            public string Alg { get; set; }
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
        }

        [XmlRoot(ElementName = "Source", Namespace = "urn:ContainerIndex")]
        public class Source
        {
            [XmlElement(ElementName = "Hash", Namespace = "urn:ContainerIndex")]
            public Hash Hash { get; set; }
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "Basis", Namespace = "urn:ContainerIndex")]
        public class Basis
        {
            [XmlElement(ElementName = "Hash", Namespace = "urn:ContainerIndex")]
            public Hash Hash { get; set; }
            [XmlAttribute(AttributeName = "loc")]
            public string Loc { get; set; }
        }

        [XmlRoot(ElementName = "Delta", Namespace = "urn:ContainerIndex")]
        public class Delta
        {
            [XmlElement(ElementName = "Source", Namespace = "urn:ContainerIndex")]
            public Source Source { get; set; }
            [XmlElement(ElementName = "Basis", Namespace = "urn:ContainerIndex")]
            public Basis Basis { get; set; }
        }

        [XmlRoot(ElementName = "File", Namespace = "urn:ContainerIndex")]
        public class File
        {
            [XmlElement(ElementName = "Hash", Namespace = "urn:ContainerIndex")]
            public Hash Hash { get; set; }
            [XmlElement(ElementName = "Delta", Namespace = "urn:ContainerIndex")]
            public Delta Delta { get; set; }
            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "length")]
            public string Length { get; set; }
            [XmlAttribute(AttributeName = "time")]
            public string Time { get; set; }
            [XmlAttribute(AttributeName = "attr")]
            public string Attr { get; set; }
        }

        [XmlRoot(ElementName = "Files", Namespace = "urn:ContainerIndex")]
        public class Files
        {
            [XmlElement(ElementName = "File", Namespace = "urn:ContainerIndex")]
            public List<File> File { get; set; }
        }

        [XmlRoot(ElementName = "Container", Namespace = "urn:ContainerIndex")]
        public class Container
        {
            [XmlElement(ElementName = "Description", Namespace = "urn:ContainerIndex")]
            public string Description { get; set; }
            [XmlElement(ElementName = "DeltaBasisSearch", Namespace = "urn:ContainerIndex")]
            public DeltaBasisSearch DeltaBasisSearch { get; set; }
            [XmlElement(ElementName = "Files", Namespace = "urn:ContainerIndex")]
            public Files Files { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
            [XmlAttribute(AttributeName = "length")]
            public string Length { get; set; }
            [XmlAttribute(AttributeName = "version")]
            public string Version { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }
    }

    public static class XmlDsmDiff
    {
        [XmlRoot(ElementName = "SourceVersion", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class SourceVersion
        {
            [XmlAttribute(AttributeName = "Major")]
            public string Major { get; set; }
            [XmlAttribute(AttributeName = "Minor")]
            public string Minor { get; set; }
            [XmlAttribute(AttributeName = "QFE")]
            public string QFE { get; set; }
            [XmlAttribute(AttributeName = "Build")]
            public string Build { get; set; }
        }

        [XmlRoot(ElementName = "TargetVersion", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class TargetVersion
        {
            [XmlAttribute(AttributeName = "Major")]
            public string Major { get; set; }
            [XmlAttribute(AttributeName = "Minor")]
            public string Minor { get; set; }
            [XmlAttribute(AttributeName = "QFE")]
            public string QFE { get; set; }
            [XmlAttribute(AttributeName = "Build")]
            public string Build { get; set; }
        }

        [XmlRoot(ElementName = "DiffFileEntry", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class DiffFileEntry
        {
            [XmlElement(ElementName = "FileType", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string FileType { get; set; }
            [XmlElement(ElementName = "DevicePath", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string DevicePath { get; set; }
            [XmlElement(ElementName = "CabPath", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string CabPath { get; set; }
            [XmlElement(ElementName = "DiffType", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string DiffType { get; set; }
        }

        [XmlRoot(ElementName = "Files", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class Files
        {
            [XmlElement(ElementName = "DiffFileEntry", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<DiffFileEntry> DiffFileEntry { get; set; }
        }

        [XmlRoot(ElementName = "DiffPackage", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class DiffPackage
        {
            [XmlElement(ElementName = "SourceVersion", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public SourceVersion SourceVersion { get; set; }
            [XmlElement(ElementName = "TargetVersion", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public TargetVersion TargetVersion { get; set; }
            [XmlElement(ElementName = "SourceHash", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string SourceHash { get; set; }
            [XmlElement(ElementName = "Name", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string Name { get; set; }
            [XmlElement(ElementName = "Files", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public Files Files { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }
    }

    public static class XmlMum
    {
        [XmlRoot(ElementName = "assemblyIdentity")]
        public class AssemblyIdentity
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "version")]
            public string Version { get; set; }
            [XmlAttribute(AttributeName = "language")]
            public string Language { get; set; }
            [XmlAttribute(AttributeName = "processorArchitecture")]
            public string ProcessorArchitecture { get; set; }
            [XmlAttribute(AttributeName = "publicKeyToken")]
            public string PublicKeyToken { get; set; }
            [XmlAttribute(AttributeName = "buildType")]
            public string BuildType { get; set; }
            [XmlAttribute(AttributeName = "versionScope")]
            public string VersionScope { get; set; }
        }

        [XmlRoot(ElementName = "phoneInformation")]
        public class PhoneInformation
        {
            [XmlAttribute(AttributeName = "phoneRelease")]
            public string PhoneRelease { get; set; }
            [XmlAttribute(AttributeName = "phoneOwnerType")]
            public string PhoneOwnerType { get; set; }
            [XmlAttribute(AttributeName = "phoneOwner")]
            public string PhoneOwner { get; set; }
            [XmlAttribute(AttributeName = "phoneComponent")]
            public string PhoneComponent { get; set; }
            [XmlAttribute(AttributeName = "phoneSubComponent")]
            public string PhoneSubComponent { get; set; }
            [XmlAttribute(AttributeName = "phoneGroupingKey")]
            public string PhoneGroupingKey { get; set; }
        }

        [XmlRoot(ElementName = "file")]
        public class File
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "size")]
            public string Size { get; set; }
            [XmlAttribute(AttributeName = "staged")]
            public string Staged { get; set; }
            [XmlAttribute(AttributeName = "compressed")]
            public string Compressed { get; set; }
            [XmlAttribute(AttributeName = "sourcePackage")]
            public string SourcePackage { get; set; }
            [XmlAttribute(AttributeName = "embeddedSign")]
            public string EmbeddedSign { get; set; }
            [XmlAttribute(AttributeName = "cabpath")]
            public string Cabpath { get; set; }
        }

        [XmlRoot(ElementName = "customInformation")]
        public class CustomInformation
        {
            [XmlElement(ElementName = "phoneInformation")]
            public PhoneInformation PhoneInformation { get; set; }
            [XmlElement(ElementName = "file")]
            public List<File> File { get; set; }
        }

        [XmlRoot(ElementName = "component")]
        public class Component
        {
            [XmlElement(ElementName = "assemblyIdentity")]
            public AssemblyIdentity AssemblyIdentity { get; set; }
        }

        [XmlRoot(ElementName = "update")]
        public class Update
        {
            [XmlElement(ElementName = "component")]
            public Component Component { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "package")]
        public class Package
        {
            [XmlElement(ElementName = "customInformation")]
            public CustomInformation CustomInformation { get; set; }
            [XmlElement(ElementName = "update")]
            public Update Update { get; set; }
            [XmlAttribute(AttributeName = "identifier")]
            public string Identifier { get; set; }
            [XmlAttribute(AttributeName = "releaseType")]
            public string ReleaseType { get; set; }
            [XmlAttribute(AttributeName = "restart")]
            public string Restart { get; set; }
            [XmlAttribute(AttributeName = "targetPartition")]
            public string TargetPartition { get; set; }
            [XmlAttribute(AttributeName = "binaryPartition")]
            public string BinaryPartition { get; set; }
        }

        [XmlRoot(ElementName = "assembly")]
        public class Assembly
        {
            [XmlElement(ElementName = "assemblyIdentity")]
            public AssemblyIdentity AssemblyIdentity { get; set; }

            [XmlArray(ElementName = "registryKeys")]
            [XmlArrayItem(ElementName = "registryKey")]
            public List<RegistryKey> RegistryKeys { get; set; }

            [XmlElement(ElementName = "package")]
            public Package Package { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlAttribute(AttributeName = "manifestVersion")]
            public string ManifestVersion { get; set; }
            [XmlAttribute(AttributeName = "displayName")]
            public string DisplayName { get; set; }
            [XmlAttribute(AttributeName = "company")]
            public string Company { get; set; }
            [XmlAttribute(AttributeName = "copyright")]
            public string Copyright { get; set; }

            //TODO: trustInfo
        }

        [XmlRoot(ElementName = "registryKey")]
        public class RegistryKey
        {

            [XmlElement(ElementName = "registryValue")]
            public List<RegistryValue> RegistryValues { get; set; }
            [XmlAttribute(AttributeName = "keyName")]
            public string KeyName { get; set; }
            [XmlElement(ElementName = "securityDescriptor")]
            public SecurityDescriptor SecurityDescriptor { get; set; }
        }

        [XmlRoot(ElementName = "securityDescriptor")]
        public class SecurityDescriptor
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "registryValue")]
        public class RegistryValue
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
            [XmlAttribute(AttributeName = "valueType")]
            public string ValueType { get; set; }
            [XmlAttribute(AttributeName = "mutable")]
            public string Mutable { get; set; }
            [XmlAttribute(AttributeName = "operationHint")]
            public string OperationHint { get; set; } //e.g: replace
        }
    }

    public static class XmlDsm
    {
        [XmlRoot(ElementName = "Version", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class Version
        {
            [XmlAttribute(AttributeName = "Major")]
            public string Major { get; set; }
            [XmlAttribute(AttributeName = "Minor")]
            public string Minor { get; set; }
            [XmlAttribute(AttributeName = "QFE")]
            public string QFE { get; set; }
            [XmlAttribute(AttributeName = "Build")]
            public string Build { get; set; }
        }

        [XmlRoot(ElementName = "Identity", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class Identity
        {
            [XmlElement(ElementName = "Owner", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string Owner { get; set; }
            [XmlElement(ElementName = "Component", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string Component { get; set; }
            [XmlElement(ElementName = "SubComponent", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string SubComponent { get; set; }
            [XmlElement(ElementName = "Version", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public Version Version { get; set; }
        }

        [XmlRoot(ElementName = "FileEntry", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class FileEntry
        {
            [XmlElement(ElementName = "FileType", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string FileType { get; set; }
            [XmlElement(ElementName = "DevicePath", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string DevicePath { get; set; }
            [XmlElement(ElementName = "CabPath", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string CabPath { get; set; }
            [XmlElement(ElementName = "Attributes", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string Attributes { get; set; }
            [XmlElement(ElementName = "SourcePackage", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string SourcePackage { get; set; }
            [XmlElement(ElementName = "FileSize", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string FileSize { get; set; }
            [XmlElement(ElementName = "CompressedFileSize", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string CompressedFileSize { get; set; }
            [XmlElement(ElementName = "StagedFileSize", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string StagedFileSize { get; set; }
        }

        [XmlRoot(ElementName = "Files", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class Files
        {
            [XmlElement(ElementName = "FileEntry", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public List<FileEntry> FileEntry { get; set; }
        }

        [XmlRoot(ElementName = "Package", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public class Package
        {
            [XmlElement(ElementName = "Identity", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public Identity Identity { get; set; }
            [XmlElement(ElementName = "ReleaseType", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string ReleaseType { get; set; }
            [XmlElement(ElementName = "OwnerType", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string OwnerType { get; set; }
            [XmlElement(ElementName = "BuildType", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string BuildType { get; set; }
            [XmlElement(ElementName = "CpuType", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string CpuType { get; set; }
            [XmlElement(ElementName = "Partition", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string Partition { get; set; }
            [XmlElement(ElementName = "GroupingKey", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string GroupingKey { get; set; }
            [XmlElement(ElementName = "IsRemoval", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public string IsRemoval { get; set; }
            [XmlElement(ElementName = "Files", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
            public Files Files { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }
    }
}