using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TechNottingham.Common
{
    public interface IS3Client
    {
        Task<DateTime?> EventDataModifiedOn(string filename);
        Task SaveData(string filename, string content);
    }
}
