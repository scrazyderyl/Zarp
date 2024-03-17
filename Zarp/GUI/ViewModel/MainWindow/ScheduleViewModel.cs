using System.Collections.ObjectModel;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.ViewModel.MainWindow
{
    internal class ScheduleViewModel
    {
        public ObservableCollection<FocusSession> FocusSessions { get; set; }

        public ScheduleViewModel()
        {
            FocusSessions = new ObservableCollection<FocusSession>();
        }
    }
}
