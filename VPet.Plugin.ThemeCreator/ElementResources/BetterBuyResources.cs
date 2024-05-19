using System.Windows.Controls;
using System.Windows;
using VPet_Simulator.Windows.Interface;
using Panuon.WPF.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Windows.Media;
using ListBox = System.Windows.Controls.ListBox;

namespace VPet.Plugin.ThemeCreator.ElementResources;
internal class BetterBuyResources
{
    public void CreateResources(IMainWindow main)
    {
        for (int i = 1; i <= 19; i++)
            Application.Current.Resources.Add($"Bb_bg{i}", Brushes.Red);
        for (int i = 1; i <= 4; i++)
            Application.Current.Resources.Add($"Bb_cr{i}", new CornerRadius(0));

        Application.Current.Resources.Add("Bb_m1", new Thickness(0));
        Application.Current.Resources.Add("Bb_bt1", new Thickness(0));

        Application.Current.Resources.Add("Bb_fg1", Brushes.Red);
        Application.Current.Resources.Add("Bb_sh1", Colors.Red);
        Application.Current.Resources.Add("Bb_o1", 1f);
        Application.Current.Resources.Add("Bb_ib1", null);
    }
}
