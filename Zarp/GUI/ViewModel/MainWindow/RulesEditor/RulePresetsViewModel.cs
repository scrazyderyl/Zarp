﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zarp.GUI.View;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Windows;
using Shell32;
using System.Diagnostics;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;

namespace Zarp.GUI.ViewModel.MainWindow.RulesEditor
{
    internal class RulePresetsViewModel : ObservableObject
    {
        public RelayCommand CreatePresetCommand { get; set; }
        public RelayCommand RemovePresetCommand { get; set; }
        public RelayCommand RenamePresetCommand { get; set; }
        public RelayCommand DuplicatePresetCommand { get; set; }
        public RelayCommand ImportPresetCommand { get; set; }
        public RelayCommand ExportPresetCommand { get; set; }
        public RelayCommand OpenApplicationSelectorCommand { get; set; }

        public ObservableCollection<RulePreset> RulePresets { get; set; }
        private int _SelectedPresetIndex;
        public int SelectedPresetIndex
        {
            get { return _SelectedPresetIndex; }
            set
            {
                _SelectedPresetIndex = value;
                OnPresetSelectionChanged();
            }
        }

        public Visibility EditorVisibility { get; set; }
        public string RulesetPolicy { get; set; }
        public ObservableCollection<ApplicationInfo> Rules { get; set; }
        private int _SelectedRuleIndex;
        public int SelectedRuleIndex
        {
            get { return _SelectedRuleIndex; }
            set
            {
                RulePresets[_SelectedPresetIndex].ApplicationRules.RemoveRule(Rules[value].Id);
                Rules.RemoveAt(value);
            }
        }

        public RulePresetsViewModel()
        {
            CreatePresetCommand = new RelayCommand(CreatePreset);
            RemovePresetCommand = new RelayCommand(RemovePreset);
            RenamePresetCommand = new RelayCommand(RenamePreset);
            DuplicatePresetCommand = new RelayCommand(DuplicatePreset);
            ImportPresetCommand = new RelayCommand(ImportPreset);
            ExportPresetCommand = new RelayCommand(ExportPreset);
            OpenApplicationSelectorCommand = new RelayCommand(OpenApplicationSelector);

            RulePresets = new ObservableCollection<RulePreset>(Core.Service.Zarp.RulePresetManager.GetPresets());
            _SelectedPresetIndex = -1;

            EditorVisibility = Visibility.Hidden;
            Rules = new ObservableCollection<ApplicationInfo>();
            _SelectedRuleIndex = -1;
            RulesetPolicy = string.Empty;
        }

        private void OnPresetSelectionChanged()
        {
            if (SelectedPresetIndex == -1)
            {
                Rules = new ObservableCollection<ApplicationInfo>();
                RulesetPolicy = string.Empty;
                EditorVisibility = Visibility.Hidden;
            }
            else
            {
                RulePreset SelectedPreset = RulePresets[SelectedPresetIndex];
                Rules = new ObservableCollection<ApplicationInfo>(SelectedPreset.ApplicationRules.GetRules());
                RulesetPolicy = SelectedPreset.ApplicationRules.IsWhitelist ? "Block all except" : "Allow all except";
                EditorVisibility = Visibility.Visible;
            }

            OnPropertyChanged("Rules");
            OnPropertyChanged("RulesetPolicy");
            OnPropertyChanged("EditorVisibility");
        }

        public void CreatePreset(object? parameter)
        {
            Core.Service.Zarp.DialogReturnValue = null;
            new CreateRulePresetView().ShowDialog();
            RulePreset? newPreset = (RulePreset?)Core.Service.Zarp.DialogReturnValue;

            if (newPreset == null)
            {
                return;
            }

            RulePresets = new ObservableCollection<RulePreset>(Core.Service.Zarp.RulePresetManager.GetPresets());
            SelectedPresetIndex = RulePresets.Count - 1;
            OnPropertyChanged("RulePresets");
            OnPropertyChanged("SelectedPresetIndex");
        }

        public void RemovePreset(object? parameter)
        {
            RulePreset SelectedPreset = RulePresets[SelectedPresetIndex];
            Core.Service.Zarp.RulePresetManager.Remove(SelectedPreset.Name);
            int newIndex = SelectedPresetIndex - 1;
            RulePresets = new ObservableCollection<RulePreset>(Core.Service.Zarp.RulePresetManager.GetPresets());
            OnPropertyChanged("RulePresets");
            SelectedPresetIndex = newIndex == -1 && RulePresets.Count > 0 ? 0 : newIndex;
            OnPropertyChanged("SelectedPresetIndex");
        }

        public void RenamePreset(object? parmaeter)
        {

        }

        public void DuplicatePreset(object? parameter)
        {

        }

        public void ImportPreset(object? parameter)
        {

        }

        public void ExportPreset(object? parameter)
        {

        }

        public void OpenApplicationSelector(object? parameter)
        {
            Core.Service.Zarp.DialogReturnValue = null;
            new ApplicationSelectorView().ShowDialog();
            List<ApplicationInfo>? newRules = (List<ApplicationInfo>?)Core.Service.Zarp.DialogReturnValue;

            if (newRules == null)
            {
                return;
            }

            RulePreset SelectedPreset = RulePresets[SelectedPresetIndex];
            SelectedPreset.ApplicationRules.AddRules(newRules);
            Rules = new ObservableCollection<ApplicationInfo>(SelectedPreset.ApplicationRules.GetRules());

            OnPropertyChanged("Rules");
        }
    }
}