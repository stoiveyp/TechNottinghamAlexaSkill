using System;
using System.Collections.Generic;
using System.Text;

namespace TechNottingham.Common
{
    public interface IEnvironment
    {
        string Get(string key);
    }
}
