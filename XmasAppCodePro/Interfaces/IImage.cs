using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmasAppCodePro.Interfaces
{
    public interface IImage
    {
        string Prefix { get; set; }
        ImageSource Implementation { get; set; }
        bool Extension { get; set; }
    }
}
