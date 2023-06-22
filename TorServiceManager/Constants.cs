using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorServiceManager
{
    public class Constants
    {
        public const string HiddenServiceDirReplaceConstant = "{HIDDEN_SERVICE_DIR_CONST}";
        public const string DefaultTorrc =
            $"""
            HiddenServiceDir {HiddenServiceDirReplaceConstant}
            HiddenServicePort 80 127.0.0.1:80
            HiddenServicePort 80 127.0.0.1:81
            """;
        public const char RandomStringFileSafeStart = 'A';
        public const char RandomStringFileSafeEnd = 'Z';
    }
}
