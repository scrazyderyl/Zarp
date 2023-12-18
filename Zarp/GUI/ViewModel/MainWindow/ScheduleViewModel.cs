using System.Collections.ObjectModel;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.ViewModel.MainWindow
{
    internal class ScheduleViewModel
    {
        public ObservableCollection<FocusSessionPreset> FocusSessions { get; set; }

        public ScheduleViewModel()
        {
            FocusSessions = new ObservableCollection<FocusSessionPreset>(Core.Service.Zarp.FocusSessionPresetManager.GetPresets());
        }
    }
}
