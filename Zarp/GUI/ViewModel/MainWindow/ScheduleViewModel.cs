using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
