using Server.Data.Models;
using System;
using System.Collections.Generic;

namespace Server.Data.Services
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
}
