﻿using System.Windows;
using System.Windows.Input;
using TCC.Data.Pc;

namespace TCC.ResourceDictionaries
{
    public partial class DataTemplates
    {
        private void OnCharacterNameMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowManager.Dashboard.VM.SelectCharacter((sender as FrameworkElement)?.DataContext as Character);
        }
    }
}
