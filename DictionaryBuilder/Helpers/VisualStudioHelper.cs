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
    internal sealed partial class VisualStudioHelper
    {

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
            @namespace = Regex.Replace(@namespace, @"[\\\/]", ".");

            return Regex.Replace(@namespace, @"^[0-9]+|[^A-Z0-9.]|(\.)(?=\1)", string.Empty, RegexOptions.IgnoreCase);
        }

        #endregion

    }
}
