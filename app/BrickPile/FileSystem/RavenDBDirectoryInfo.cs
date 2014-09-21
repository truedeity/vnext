using Microsoft.AspNet.FileSystems;
using Raven.Client.FileSystem;
using System;
using System.IO;

namespace BrickPile.FileSystem
{
    /// <summary>
    /// Summary description for RavenDBDirectoryInfo
    /// </summary>
    public class RavenDBDirectoryInfo : IFileInfo
    {
        private readonly IFilesStore filesStore;
        private readonly string directory;

        public RavenDBDirectoryInfo(IFilesStore filesStore, string directory)
        {
            this.filesStore = filesStore;
            this.directory = directory;
        }

        public bool IsDirectory
        {
            get
            {
                return true;
            }
        }

        public DateTime LastModified
        {
            get
            {
                return default(DateTime);
            }
        }

        public long Length
        {
            get
            {
                return -1;
            }
        }

        public string Name
        {
            get
            {
                return directory;
            }
        }

        public string PhysicalPath
        {
            get
            {
                return string.Join("/", filesStore.AsyncFilesCommands.UrlFor("BrickPile"), "files", directory.Trim(new[] { '/' }));
            }
        }

        public Stream CreateReadStream()
        {
            return null;
        }
    }
}