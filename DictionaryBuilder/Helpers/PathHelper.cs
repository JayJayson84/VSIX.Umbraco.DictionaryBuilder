using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System.IO;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DictionaryBuilder
{
    internal sealed partial class VisualStudioHelper
    {
        /// <summary>
        /// Helper methods related to file and folder paths.
        /// </summary>
        internal sealed class Paths
        {

            /// <summary>
            /// Gets the full path of the solution file (.sln).
            /// </summary>
            public static string SolutionFilePath
            {
                get
                {
                    ThreadHelper.ThrowIfNotOnUIThread();

                    return DTE?.Solution?.FullName;
                }
            }

            /// <summary>
            /// Gets the full path of the directory containing the project file (.csproj).
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <returns>Returns the directory information for the specified project.</returns>
            public static string GetProjectDirectory(Project project)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return project?.Properties?.Item("LocalPath")?.Value as string;
            }

            /// <summary>
            /// Gets the project file name including extension (.csproj).
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <returns>Returns the file name and extension for the specified project.</returns>
            public static string GetProjectFileName(Project project)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return project?.Properties?.Item("FileName")?.Value as string;
            }

            /// <summary>
            /// Gets the full path of the project file (.csproj).
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <returns>Returns the directory information for the specified project.</returns>
            public static string GetProjectFilePath(Project project)
            {
                return Combine(GetProjectDirectory(project), GetProjectFileName(project));
            }

            /// <summary>
            /// Gets the full path of the output file where language cultures will be exported.
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <param name="options">The <see cref="VisualStudioOptions"/> containing user preferences.</param>
            /// <returns>Returns the directory information for the specified <see cref="VisualStudioOptions.CultureModelPath"/>, if available. Otherwise <see cref="VisualStudioOptions.DEFAULT_CULTURE_MODEL_PATH"/>.</returns>
            public static string GetCultureModelFilePath(Project project, VisualStudioOptions options = null)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return Combine(GetProjectDirectory(project), (options ?? GetOptions())?.CultureModelPath ?? VisualStudioOptions.DEFAULT_CULTURE_MODEL_PATH);
            }

            /// <summary>
            /// Gets the full path of the output file where dictionary models will be exported.
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <param name="options">The <see cref="VisualStudioOptions"/> containing user preferences.</param>
            /// <returns>Returns the directory information for the specified <see cref="VisualStudioOptions.DictionaryModelPath"/>, if available. Otherwise <see cref="VisualStudioOptions.DEFAULT_DICTIONARY_MODEL_PATH"/>.</returns>
            public static string GetDictionaryModelFilePath(Project project, VisualStudioOptions options = null)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return Combine(GetProjectDirectory(project), (options ?? GetOptions())?.DictionaryModelPath ?? VisualStudioOptions.DEFAULT_DICTIONARY_MODEL_PATH);
            }

            /// <summary>
            /// Gets the full path of the output file where dictionarykey models will be exported.
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <param name="options">The <see cref="VisualStudioOptions"/> containing user preferences.</param>
            /// <returns>Returns the directory information for the specified <see cref="VisualStudioOptions.DictionaryKeyModelPath"/>, if available. Otherwise <see cref="VisualStudioOptions.DEFAULT_DICTIONARYKEY_MODEL_PATH"/>.</returns>
            public static string GetDictionaryKeyModelFilePath(Project project, VisualStudioOptions options = null)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return Combine(GetProjectDirectory(project), (options ?? GetOptions())?.DictionaryKeyModelPath ?? VisualStudioOptions.DEFAULT_DICTIONARYKEY_MODEL_PATH);
            }

            /// <summary>
            /// Gets the full path of the output file where the dictionary service will be exported.
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <param name="options">The <see cref="VisualStudioOptions"/> containing user preferences.</param>
            /// <returns>Returns the directory information for the specified <see cref="VisualStudioOptions.ServicePath"/>, if available. Otherwise <see cref="VisualStudioOptions.DEFAULT_SERVICE_PATH"/>.</returns>
            public static string GetServiceFilePath(Project project, VisualStudioOptions options = null)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return Combine(GetProjectDirectory(project), (options ?? GetOptions())?.ServicePath ?? VisualStudioOptions.DEFAULT_SERVICE_PATH);
            }

            /// <summary>
            /// Gets the full path of the output file where the dictionary service interface will be exported.
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <param name="options">The <see cref="VisualStudioOptions"/> containing user preferences.</param>
            /// <returns>Returns the directory information for the specified <see cref="VisualStudioOptions.IServicePath"/>, if available. Otherwise <see cref="VisualStudioOptions.DEFAULT_ISERVICE_PATH"/>.</returns>
            public static string GetIServiceFilePath(Project project, VisualStudioOptions options = null)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return Combine(GetProjectDirectory(project), (options ?? GetOptions())?.IServicePath ?? VisualStudioOptions.DEFAULT_ISERVICE_PATH);
            }

            /// <summary>
            /// Gets the full path of the output file where dictionary extension method will be exported.
            /// </summary>
            /// <param name="project">The selected <see cref="Project"/>.</param>
            /// <param name="options">The <see cref="VisualStudioOptions"/> containing user preferences.</param>
            /// <returns>Returns the directory information for the specified <see cref="VisualStudioOptions.ServiceExtensionPath"/>, if available. Otherwise <see cref="VisualStudioOptions.DEFAULT_SERVICE_EXTENSION_PATH"/>.</returns>
            public static string GetServiceExtensionFilePath(Project project, VisualStudioOptions options = null)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return Combine(GetProjectDirectory(project), (options ?? GetOptions())?.ServiceExtensionPath ?? VisualStudioOptions.DEFAULT_SERVICE_EXTENSION_PATH);
            }

            /// <summary>
            /// Gets the full path of the options file where user preferences are saved.
            /// </summary>
            public static string OptionsFilePath
            {
                get
                {
                    var solutionPath = SolutionFilePath;
                    var optionsPath = Path.ChangeExtension(solutionPath, null);
                    var extension = ".UmbracoDictionaryBuilder.user";

                    return $"{optionsPath}{extension}";
                }
            }

            /// <summary>
            /// Combines two strings into a path.
            /// </summary>
            /// <param name="path1">The first path to combine.</param>
            /// <param name="path2">The second path to combine.</param>
            /// <returns>
            /// The combined paths. If one of the specified paths is a zero-length string, this method returns the other path.
            /// Converts <see cref="Path.AltDirectorySeparatorChar"/> to <see cref="Path.DirectorySeparatorChar"/>.
            /// </returns>
            /// <remarks>Trims leading <see cref="Path.DirectorySeparatorChar"/> and <see cref="Path.AltDirectorySeparatorChar"/> from <paramref name="path2"/>.</remarks>
            public static string Combine(string path1, string path2)
            {
                if (Path.IsPathRooted(path2))
                {
                    path2 = path2.TrimStart(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                }

                return Path.Combine(path1, path2)?.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            /// <summary>
            /// Removes a file from the filesystem and writes a status log to the output window.
            /// </summary>
            /// <param name="relativePath">The relative path to the file.</param>
            /// <param name="filePath">The fully qualified filesystem path to the file.</param>
            /// <returns>1 if the file was deleted. Otherwise 0.</returns>
            public static async Task<int> RemovePathAsync(string relativePath, string filePath)
            {
                return await Task.Run(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow($"DictionaryBuilder: Removing {relativePath}", true);
                    await TaskScheduler.Default;

                    if (!File.Exists(filePath))
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        LogHelper.LogToWindow("DictionaryBuilder: File not found", true);
                        await TaskScheduler.Default;

                        return 0;
                    }

                    File.Delete(filePath);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow("DictionaryBuilder: File removed", true);
                    await TaskScheduler.Default;

                    return 1;
                });
            }
        }
    }
}
