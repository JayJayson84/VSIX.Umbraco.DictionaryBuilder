using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;

namespace Example.Composers
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class DependencyComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // Register the DictionaryService during application boot.
            composition.Register<IDictionaryService, DictionaryService>();
        }
    }
}