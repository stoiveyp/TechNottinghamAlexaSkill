using System;
using System.Collections.Generic;
using System.Text;

namespace TechNottingham.Common
{
    public class DefaultEnvironment:IEnvironment
    {
        public string Get(string key)
        {
            return System.Environment.GetEnvironmentVariable(key);
        }

        public DateTime CurrentTime => DateTime.Now;
    }
}
