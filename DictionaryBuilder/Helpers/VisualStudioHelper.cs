using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DictionaryBuilder
{
    /// <summary>
    /// Helper methods related to Visual Studio, user options and path data.
    /// </summary>
    internal sealed class VisualStudioHelper
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
        }

        #region " Properties "

        /// <summary>
        /// Returns the top-level object in the Visual Studio automation object model.
        /// </summary>
        private static DTE DTE
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return Package.GetGlobalService(typeof(DTE)) as DTE;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the Visual Studio instance contains a solution. Otherwise <see langword="false"/>.
        /// </summary>
        public static bool HasSolution
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                return DTE?.Solution?.IsOpen ?? false;
            }
        }

        #endregion

        #region " Public Methods "

        /// <summary>
        /// Gets a reference to the solution.
        /// </summary>
        /// <param name="project">The selected <see cref="Project"/>.</param>
        /// <returns>An <see cref="IVsSolution"/> if available. Otherwise <see langword="null"/>.</returns>
        public static IVsSolution GetSolution(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var provider = project?.DTE as IServiceProvider;
            var solution = provider?.QueryService<SVsSolution>() as IVsSolution;

            return solution;
        }

        /// <summary>
        /// Gets the project selected in the solution explorer window.
        /// </summary>
        /// <param name="dte">A reference to the top-level object in the Visual Studio automation object model.</param>
        /// <returns>The first <see cref="Project"/> in the solution explorer selection. Otherwise <see langword="null"/>.</returns>
        public static Project GetSelectedProject(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(dte?.ToolWindows?.SolutionExplorer?.SelectedItems is object[] solutionItems)) return null;

            return solutionItems
                .OfType<UIHierarchyItem>()
                .Where(x =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return x.Object is Project;
                })
                .FirstOrDefault()?
                .Object as Project;
        }

        /// <summary>
        /// Gets the user preferences.
        /// </summary>
        /// <returns>A <see cref="VisualStudioOptions"/> instance.</returns>
        public static VisualStudioOptions GetOptions()
        {
            return VisualStudioOptions.Instance.Reload();
        }

        /// <summary>
        /// Gets a dynamic namespace string.
        /// </summary>
        /// <param name="project">The selected <see cref="Project"/>.</param>
        /// <param name="namespace">The <see cref="VisualStudioOptions.ModelNamespace"/> or a static namespace string.</param>
        /// <param name="itemPath">The full path to the file of the item where this namespace will be used.</param>
        /// <returns>A namespace string. Invalid characters are removed and {project} and {path} placeholders are replaced.</returns>
        public static string ResolveNamespace(Project project, string @namespace, string itemPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            @namespace = @namespace ?? VisualStudioOptions.DEFAULT_NAMESPACE;

            var projectPath = Paths.GetProjectDirectory(project);

            itemPath = Path.GetDirectoryName(itemPath);

            var relativeItemPath = itemPath.StartsWith(projectPath)
                ? itemPath.Substring(projectPath.Length)
                : itemPath;

            @namespace = @namespace.Replace("{project}", project.Name);
            @namespace = @namespace.Replace("{path}", relativeItemPath);

            return Regex.Replace(@namespace, @"^[0-9]+|[^A-Z0-9.]|(\.)(?=\1)", string.Empty, RegexOptions.IgnoreCase);
        }

        #endregion

    }
}
