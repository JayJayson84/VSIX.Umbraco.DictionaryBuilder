using System;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Example.Components
{
    public class EventComponent : IComponent
    {
        private static IUmbracoContextFactory _umbracoContextFactory;
        private static IDictionaryService _dictionaryService;

        private const string DisableDeleteAlias = "disableDelete";

        public EventComponent(
            IUmbracoContextFactory umbracoContextFactory,
            IDictionaryService dictionaryService)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _dictionaryService = dictionaryService;
        }

        public void Initialize()
        {
            ContentService.Trashing += ContentService_Trashing;
        }

        private static void ContentService_Trashing(global::Umbraco.Core.Services.IContentService sender, global::Umbraco.Core.Events.MoveEventArgs<global::Umbraco.Core.Models.IContent> e)
        {
            using (var reference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var currentUser = reference.UmbracoContext.Security.CurrentUser;
                if (currentUser.Id == -1) return;
            }

            var protectedAlias = new[] {
                "configuration"
            };

            foreach (var moveEventInfo in e.MoveInfoCollection)
            {
                var entity = moveEventInfo.Entity;

                if (!protectedAlias.Contains(entity.ContentType.Alias) ||
                    (entity.HasProperty(DisableDeleteAlias) &&
                    !entity.GetValue<bool>(DisableDeleteAlias))) continue;

                var messageA = _dictionaryService
                    // Get a DictionaryItem using the strongly typed Dictionary class members.
                    .GetDictionaryItemByKey(Dictionary.Events.System.DeleteActionDisabled)
                    // Use an extension method to return the value, for the default site culture,
					// with format items e.g. {0}, {1} etc. replaced by an array of format args.
                    .GetFormattedLocaleValue(new[] { entity.Name });

                var messageB = _dictionaryService
                    // Get a DictionaryItem using the strongly typed Dictionary class members.
                    .GetDictionaryItemByKey(Dictionary.Events.System.DeleteActionDisabled)
                    // Use an extension method to return the value for the default site culture.
                    .GetLocaleValue();

                var messageC = _dictionaryService
                    // Get a DictionaryItem using the strongly typed DictionaryKeys members.
                    .GetDictionaryItemByKey(DictionaryKeys.DeleteActionDisabled)
                    // Use an extension method to return the value, using a strongly typed UmbracoCulture,
					// with format items e.g. {0}, {1} etc. replaced by an array of format args.
                    .GetFormattedLocaleValue(new[] { entity.Name }, UmbracoCultures.EnglishGB);

                var messageD = _dictionaryService
					// Get a DictionaryItem using the strongly typed DictionaryKeys members.
                    .GetDictionaryItemByKey(DictionaryKeys.DeleteActionDisabled)
                    // Use an extension method to return the value using the default site culture.
                    .GetLocaleValue(UmbracoCulture.Default);

                var messageE = _dictionaryService
					// Get a DictionaryItem using the strongly typed DictionaryKeys members.
                    .GetDictionaryItemByKey(DictionaryKeys.DeleteActionDisabled)
                    // Use an extension method to return the value using a strongly typed UmbracoCultures member.
                    .GetLocaleValue(UmbracoCultures.EnglishGB);

                e.Cancel = true;
                e.Messages.Add(
                    new EventMessage("System", messageA, EventMessageType.Error)
                );
            }
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}