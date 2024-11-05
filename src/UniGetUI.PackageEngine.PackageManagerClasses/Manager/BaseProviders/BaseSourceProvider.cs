using UniGetUI.Core.Classes;
using UniGetUI.PackageEngine.Enums;
using UniGetUI.PackageEngine.Interfaces;
using UniGetUI.PackageEngine.Interfaces.ManagerProviders;

namespace UniGetUI.PackageEngine.Classes.Manager.Providers
{
    public abstract class BaseSourceProvider<ManagerT> : ISourceProvider where ManagerT : IPackageManager
    {
        public ISourceFactory SourceFactory { get; }
        protected ManagerT Manager;

        public BaseSourceProvider(ManagerT manager)
        {
            Manager = manager;
            SourceFactory = new SourceFactory(manager);
        }

        public abstract string[] GetAddSourceParameters(IManagerSource source);
        public abstract string[] GetRemoveSourceParameters(IManagerSource source);
        protected abstract OperationVeredict _getAddSourceOperationVeredict(IManagerSource source, int ReturnCode, string[] Output);
        protected abstract OperationVeredict _getRemoveSourceOperationVeredict(IManagerSource source, int ReturnCode, string[] Output);

        public OperationVeredict GetAddSourceOperationVeredict(IManagerSource source, int ReturnCode, string[] Output)
        {
            TaskRecycler<IEnumerable<IManagerSource>>.RemoveFromCache(_getSources);
            return _getAddSourceOperationVeredict(source, ReturnCode, Output);
        }

        public OperationVeredict GetRemoveSourceOperationVeredict(IManagerSource source, int ReturnCode, string[] Output)
        {
            TaskRecycler<IEnumerable<IManagerSource>>.RemoveFromCache(_getSources);
            return _getRemoveSourceOperationVeredict(source, ReturnCode, Output);
        }



        /// <summary>
        /// Loads the sources for the manager. This method SHOULD NOT handle exceptions
        /// </summary>
        protected abstract IEnumerable<IManagerSource> GetSources_UnSafe();

        public virtual IEnumerable<IManagerSource> GetSources()
            => TaskRecycler<IEnumerable<IManagerSource>>.RunOrAttachOrCache(_getSources, 15);

        public virtual IEnumerable<IManagerSource> _getSources()
        {
            IEnumerable<IManagerSource> sources = GetSources_UnSafe();
            SourceFactory.Reset();

            foreach (IManagerSource source in sources)
                SourceFactory.AddSource(source);

            return sources;
        }
    }
}
