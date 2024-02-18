using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmasAppCodePro.Interfaces
{
    public interface IFile
    {
        Task<string> GetLocalPath(string path);
    }
}
