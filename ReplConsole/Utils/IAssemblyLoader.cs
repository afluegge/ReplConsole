namespace ReplConsole.Utils;


/// <summary>
/// This interface is used to abstract the loading of assemblies.
/// </summary>
public interface IAssemblyLoader
{
    /// <summary>
    /// Load an assembly from the given path.
    /// </summary>
    /// <param name="path">The path to the assembly to load.</param>
    /// <returns></returns>
    Assembly LoadFrom(string path);
}
