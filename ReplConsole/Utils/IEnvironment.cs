using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplConsole.Utils;

public interface IEnvironment
{
    void Exit(int exitCode);
}