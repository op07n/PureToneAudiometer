﻿namespace PureToneAudiometer
{
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows.Controls;
    using Audio;
    using Caliburn.Micro;
    using Caliburn.Micro.BindableAppBar;
    using Microsoft.Phone.Controls;
    using ViewModels;
    using ViewModels.Core;
    using ViewModels.Presets;
    using ViewModels.Results;
    using ViewModels.Start;
    using Views.Core;
    using Views.Presets;
    using Views.Results;
    using Windows.Storage;

    public class AppBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer container;

        protected override void Configure()
        {
            container = new PhoneContainer(RootFrame);
            container.Singleton<IEventAggregator, EventAggregator>();
            container.PerRequest<HelpPageViewModel>();
            container.PerRequest<PresetsPageViewModel>();
            container.PerRequest<PresetViewModel>();
            container.PerRequest<SavedFilesViewModel>();
            container.PerRequest<MainMenuPageViewModel>();
          
            container.PerRequest<MainPageViewModel>();
            container.Handler<ISettings>(simpleContainer => new Settings(IsolatedStorageSettings.ApplicationSettings));
            container.PerRequest<SettingsPageViewModel>();
            container.PerRequest<HearingTestViewModel>();
            container.PerRequest<HostPageViewModel>();
            container.PerRequest<HearingTestView>();
            container.PerRequest<RecentPageViewModel>();
            container.PerRequest<IAudiogramPlot, AudiogramPlot>();
            container.PerRequest<BrowserPageViewModel>();
            container.PerRequest<DataViewModel>();
            container.PerRequest<PlotViewModel>();
            container.Handler<IStorageFolder>(simpleContainer => ApplicationData.Current.LocalFolder);
            container.PerRequest<IAsyncXmlFileManager, AsyncXmlFileManager>();
            container.Handler<ISkyDriveUpload>(
                simpleContainer =>
                new SkyDriveUpload((IStorageFolder) simpleContainer.GetInstance(typeof (IStorageFolder), null),
                                   (ISettings) simpleContainer.GetInstance(typeof (ISettings), null)));
            container.PerRequest<ResultsPageViewModel>();
            container.RegisterPerRequest(typeof(AddItemViewModel), "AddItemViewModel", typeof(AddItemViewModel));
            container.RegisterPerRequest(typeof(SaveResultViewModel), "SaveResultViewModel", typeof(SaveResultViewModel));
            container.PerRequest<IDialogBuilder<AddItemView, AddItemViewModel>, DialogBuilder<AddItemView, AddItemViewModel>>();
            container.PerRequest<IDialogBuilder<SaveResultView, SaveResultViewModel>, DialogBuilder<SaveResultView, SaveResultViewModel>>();
            container.Handler<IOscillator>(simpleContainer => new SineOscillator(-95, 100));
            container.Handler<IPitchGenerator>(
                simpleContainer =>
                new PitchGenerator((IOscillator) simpleContainer.GetInstance(typeof (IOscillator), null)));
            container.RegisterPhoneServices();
            AddDefaultSettings();
            AddConventions();
        }

        private void AddDefaultSettings()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Any())
            {
                IsolatedStorageSettings.ApplicationSettings["MaxVolume"] = 100;
                IsolatedStorageSettings.ApplicationSettings["MaxRecentItems"] = 5;
            }
        }

        private void AddConventions()
        {
            ConventionManager.AddElementConvention<BindableAppBarButton>(Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(Control.IsEnabledProperty, "DataContext", "Click");
        }

        protected override object GetInstance(System.Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(System.Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void OnUnhandledException(object sender, System.Windows.ApplicationUnhandledExceptionEventArgs e)
        {
            
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new TransitionFrame();
        }
    }
}
