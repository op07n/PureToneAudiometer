﻿namespace PureToneAudiometer.ViewModels.Start
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Core;

    public class RecentPageViewModel : ViewModelBase
    {
        public IObservableCollection<RecentItemViewModel> RecentItems { get; private set; }
       
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value == selectedIndex) return;
                selectedIndex = value;
                NotifyOfPropertyChange(() => SelectedIndex);
            }
        }

        private readonly IAsyncXmlFileManager recentManager;
        private int selectedIndex;

        public RecentPageViewModel(INavigationService navigationService, IAsyncXmlFileManager recentManager) : base(navigationService)
        {
            RecentItems = new BindableCollection<RecentItemViewModel>();
            this.recentManager = recentManager;
            this.recentManager.FileName = "recent.xml";
        }

        public async Task Initialize()
        {
            RecentItems.Clear();
            var result = await recentManager.GetCollection<RecentItemViewModel>();
            RecentItems.AddRange(result.OrderByDescending(x => x.LastUsedDate));
        }

        public void SelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var item = (RecentItemViewModel)e.AddedItems[0];
            NavigationService.UriFor<ChannelSelectionPageViewModel>().WithParam(x => x.PresetFileName, item.FilePath).Navigate();
            SelectedIndex = -1;
        }
    }
}