using Microsoft.AspNet.FileSystems;
using Raven.Abstractions.FileSystem;
using Raven.Client.FileSystem;
using System;
using System.IO;

namespace BrickPile.FileSystem
{
    /// <summary>
    /// Summary description for RavenDBFileInfo
    /// </summary>
    public class RavenDBFileInfo : IFileInfo
    {
        private readonly FileHeader fileHeader;
        private readonly IFilesStore filesStore;

        public RavenDBFileInfo(IFilesStore filesStore, FileHeader fileHeader)
        {
            this.filesStore = filesStore;
            this.fileHeader = fileHeader;
        }

        public bool IsDirectory
        {
            get
            {
                return false;
            }
        }

        public DateTime LastModified
        {
            get
            {
                return fileHeader.LastModified.DateTime;
            }
        }

        public long Length
        {
            get
            {
                return fileHeader.TotalSize.GetValueOrDefault();
            }
        }

        public string Name
        {
            get
            {
                return fileHeader.Name;
            }
        }

        public string PhysicalPath
        {
            get
            {
                return string.Join("/", filesStore.AsyncFilesCommands.UrlFor("BrickPile"), "files", fileHeader.Name.Trim(new[] { '/' }));
            }
        }

        public Stream CreateReadStream()
        {
            using (var session = filesStore.OpenAsyncSession())
            {
                var stream = session.DownloadAsync(fileHeader);
                stream.Wait();
                return stream.Result;
            }
        }
    }
}