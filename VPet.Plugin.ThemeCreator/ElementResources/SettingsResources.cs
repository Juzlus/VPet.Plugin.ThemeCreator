using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using VPet_Simulator.Windows.Interface;
using System.Windows.Media;
using Panuon.WPF.UI;

namespace VPet.Plugin.ThemeCreator.ElementResources;
internal class SettingsResources
{
    public void CreateResources(ThemeCreator main)
    {
        for (int i = 1; i <= 6; i++)
            Application.Current.Resources.Add($"St_bg{i}", Brushes.Red);
        Application.Current.Resources.Add("St_fg1", Brushes.Red);
        Application.Current.Resources.Add("St_bt1", new Thickness(0));
        Application.Current.Resources.Add("St_sh1", Colors.Red);
        Application.Current.Resources.Add("St_o1", 1f);
        Application.Current.Resources.Add("St_ib1", null);
    }
}
