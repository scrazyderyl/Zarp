using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zarp.Core;

namespace Zarp.ViewModel.MainWindow
{
    internal class ScheduleViewModel
    {
        public ObservableCollection<FocusSessionPreset> FocusSessions { get; set; }

        public ScheduleViewModel()
        {
            FocusSessions = new ObservableCollection<FocusSessionPreset>(Core.Zarp.FocusSessionPresetManager.GetPresets());
        }
    }
}
