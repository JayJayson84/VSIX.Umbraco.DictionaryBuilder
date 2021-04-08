using DictionaryBuilder.Extensions;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DictionaryBuilder
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class RemoveDictionary
    {

        #region " Instance Fields "

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("8224636b-c695-4bb5-b9db-dc6ad3b8c1a1");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        #endregion

        #region " Constants "

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x1022;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDictionary"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private RemoveDictionary(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in RemoveDictionary's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new RemoveDictionary(package, commandService);
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static RemoveDictionary Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "CodeQuality",
            "IDE0051:Remove unused private members",
            Justification = "Unused auto-generated declaration")]
        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        #endregion

        #region " Private Methods "

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.WriteFirstLog("DictionaryBuilder: Running operation...", true);

                var hasSolution = VisualStudioHelper.HasSolution;
                await TaskScheduler.Default;

                if (!hasSolution)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.WriteLastLog("DictionaryBuilder: Removal failed, solution not found.", true);
                    await TaskScheduler.Default;

                    return;
                }

                if (!(await package.GetServiceAsync(typeof(DTE)) is DTE2 dte))
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.WriteLastLog("DictionaryBuilder: Removal failed, solution not found.", true);
                    await TaskScheduler.Default;

                    return;
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var project = VisualStudioHelper.GetSelectedProject(dte);
                await TaskScheduler.Default;

                if (project == null)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.WriteLastLog("DictionaryBuilder: Removal failed, project not found.", true);
                    await TaskScheduler.Default;

                    return;
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow("DictionaryBuilder: Resolving options...");

                var projectName = project.Name;
                var options = VisualStudioHelper.GetOptions();
                var cultureModelPath = VisualStudioHelper.Paths.GetCultureModelFilePath(project, options);
                var dictionaryModelPath = VisualStudioHelper.Paths.GetDictionaryModelFilePath(project, options);
                var dictionaryKeyModelPath = VisualStudioHelper.Paths.GetDictionaryKeyModelFilePath(project, options);
                var servicePath = VisualStudioHelper.Paths.GetServiceFilePath(project, options);
                var iServicePath = VisualStudioHelper.Paths.GetIServiceFilePath(project, options);
                var serviceExtensionPath = VisualStudioHelper.Paths.GetServiceExtensionFilePath(project, options);

                await TaskScheduler.Default;

                int pathCount = 0;
                int totalPathCount = 6;

                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow("DictionaryBuilder: Removing files...", true);
                    LogHelper.LogToWindow(string.Empty, true);
                    await TaskScheduler.Default;

                    pathCount += await VisualStudioHelper.Paths.RemovePathAsync(options.CultureModelPath, cultureModelPath);
                    pathCount += await VisualStudioHelper.Paths.RemovePathAsync(options.DictionaryModelPath, dictionaryModelPath);
                    pathCount += await VisualStudioHelper.Paths.RemovePathAsync(options.DictionaryKeyModelPath, dictionaryKeyModelPath);
                    pathCount += await VisualStudioHelper.Paths.RemovePathAsync(options.ServicePath, servicePath);
                    pathCount += await VisualStudioHelper.Paths.RemovePathAsync(options.IServicePath, iServicePath);
                    pathCount += await VisualStudioHelper.Paths.RemovePathAsync(options.ServiceExtensionPath, serviceExtensionPath);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow(string.Empty, true);
                    LogHelper.WriteLastLog($"DictionaryBuilder: Removed {pathCount}/{totalPathCount} files successfully.", true);
                    await TaskScheduler.Default;
                }
                catch (AggregateException ae)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow(string.Empty, true);
                    LogHelper.LogToWindow(ae.AggregateMessage(), true);
                    LogHelper.WriteLastLog($"DictionaryBuilder: Removed {pathCount}/{totalPathCount} files with errors.", true);
                    await TaskScheduler.Default;
                }
                catch (Exception ex)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow(string.Empty, true);
                    LogHelper.LogToWindow($"DictionaryBuilder: {ex.Message}", true);
                    LogHelper.WriteLastLog($"DictionaryBuilder: Removed {pathCount}/{totalPathCount} files with errors.", true);
                    await TaskScheduler.Default;
                }
            });
        }

        #endregion

    }
}
