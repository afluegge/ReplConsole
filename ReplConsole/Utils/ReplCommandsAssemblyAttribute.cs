namespace ReplConsole.Utils;

/// <summary>
/// This attribute is used to mark an assembly as containing REPL commands.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class ReplCommandsAssemblyAttribute : Attribute
{
}
