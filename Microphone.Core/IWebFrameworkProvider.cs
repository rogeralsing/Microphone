using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microphone.Core
{
    public interface IWebFrameworkProvider
    {
        Uri Start(string serviceName, string version);
    }
}
