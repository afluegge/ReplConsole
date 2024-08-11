namespace ReplConsole.Utils;

[ExcludeFromCodeCoverage]
public class AssemblyLoader : IAssemblyLoader
{
#pragma warning disable S3885 // "Assembly.Load" should be used - but not in this case
    public Assembly LoadFrom(string path) => Assembly.LoadFrom(path);
#pragma warning restore S3885
}
