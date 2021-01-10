using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    public interface IFileDataService
    {
        List<FileData> GetAllFilesData();
        FileData GetFileData(Guid id);
        FileData GetFileData(string name);
        bool AddFileData(FileData fileData);
        bool DeleteFileData(Guid id);
        bool UpdateFileData(FileData newFileData);
    }

    public class FileDataService : IFileDataService, IDisposable
    {
        private Repository repos; 

        public FileDataService()
        {
            repos = Repository.GetRepository();
        }
        
        public List<FileData> GetAllFilesData()
        {
            return repos.FileDatas.ToList();
        }

        public FileData GetFileData(Guid id)
        {
            return repos.FileDatas.SingleOrDefault(u => u.FileDataId == id);
        }

        public FileData GetFileData(string name)
        {
            return repos.FileDatas.SingleOrDefault(u => u.Path == name);
        }

        public bool AddFileData(FileData fileData)
        {
            repos.FileDatas.Add(fileData);
            int created = repos.SaveChanges();
            return created > 0;
        }

        public bool DeleteFileData(Guid id)
        {
            FileData fileData = GetFileData(id);
            if (fileData == null)
                return false;
            
            repos.FileDatas.Remove(fileData);
            int deleted = repos.SaveChanges();
            return deleted > 0;
        }

        public bool UpdateFileData(FileData newFileData)
        {
            repos.FileDatas.Update(newFileData);
            var updated = repos.SaveChanges();
            return updated > 0;
        }

        public void Dispose()
        {
            repos.Dispose();
        }
    }
}