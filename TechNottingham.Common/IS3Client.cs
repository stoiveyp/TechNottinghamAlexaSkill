using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TechNottingham.Common
{
    public interface IS3Client
    {
        Task<DateTime?> EventDataModifiedOn(string filename);
        Task SaveData(string filename, string content);
        Task<MeetupEvent[]> GetEventData(string key);
    }
}
