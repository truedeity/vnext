using Microsoft.AspNet.FileSystems;
using Raven.Abstractions.FileSystem;
using Raven.Client.FileSystem;
using System;
using System.Collections.Generic;

namespace BrickPile.FileSystem
{
    /// <summary>
    /// Summary description for RavenDBFileSystem
    /// </summary>
    public class RavenDBFileSystem : IFileSystem
    {
        private readonly IFilesStore filesStore;

	    public RavenDBFileSystem(IFilesStore filesStore)
	    {
            this.filesStore = filesStore;
	    }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            try
            {
                if (subpath.StartsWith("/", StringComparison.Ordinal))
                {
                    subpath = subpath.Substring(1);
                }


                using(var session = filesStore.OpenAsyncSession())
                {
                    var files = session.Query().OnDirectory(subpath).ToListAsync();
                    files.Wait();

                    var virtualInfos = new List<IFileInfo>();

                    foreach (var fileHeader in files.Result)
                    {
                        var fileInfo = fileHeader as FileHeader;
                        if (fileInfo != null)
                        {                            
                            virtualInfos.Add(new RavenDBFileInfo(filesStore, fileInfo));
                        }
                    }

                    var directories = session.Commands.GetDirectoriesAsync(subpath);
                    directories.Wait();

                    foreach(var directory in directories.Result)
                    {
                        virtualInfos.Add(new RavenDBDirectoryInfo(filesStore, directory));
                    }

                    contents = virtualInfos;
                    return true;
                }


            }
            catch (ArgumentException)
            {
            }

            contents = null;
            return false;
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            try
            {
                if (subpath.StartsWith("/", StringComparison.Ordinal))
                {
                    subpath = subpath.Substring(1);
                }

                using (var session = filesStore.OpenAsyncSession())
                {
                    var info = session.LoadFileAsync(subpath);
                    info.Wait();
                    fileInfo = new RavenDBFileInfo(filesStore, info.Result);
                    return true;
                }
            }
            catch (ArgumentException)
            {
            }
            fileInfo = null;
            return false;
        }
    }
}