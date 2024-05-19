using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.ThemeCreator.ElementResources;
internal class MessageBarResources
{
    public void CreateResources(IMainWindow main)
    {
        Application.Current.Resources.Add("Mb_bg", Brushes.Red);
        Application.Current.Resources.Add("Mb_bbg", Brushes.Red);
        Application.Current.Resources.Add("Mb_nfg", Brushes.Red);
        Application.Current.Resources.Add("Mb_cfg", Brushes.Red);
        Application.Current.Resources.Add("Mb_bbt", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Mb_bcr", new CornerRadius(0, 0, 0, 0));
        Application.Current.Resources.Add("Mb_o", 1f);
        Application.Current.Resources.Add("Mb_fs", 24f);

        this.ApplyResources(main);
    }

    public void ApplyResources(IMainWindow main)
    {
        VPet_Simulator.Core.MessageBar msgBar = (VPet_Simulator.Core.MessageBar)main.Main.MsgBar;
        Border mainBr = (msgBar.FindName("BorderMain") as Border);
        mainBr.Background = (Brush)Application.Current.Resources["Mb_bg"];
        mainBr.BorderBrush = (Brush)Application.Current.Resources["Mb_bbg"];
        mainBr.Opacity = (float)Application.Current.Resources["Mb_o"];
        mainBr.BorderThickness = (Thickness)Application.Current.Resources["Mb_bbt"];
        mainBr.CornerRadius = (CornerRadius)Application.Current.Resources["Mb_bcr"];

        Label lname = (mainBr.FindName("LName") as Label);
        TextBox ttext = (mainBr.FindName("TText") as TextBox);
        lname.Foreground = (Brush)Application.Current.Resources["Mb_nfg"];
        ttext.Foreground = (Brush)Application.Current.Resources["Mb_cfg"];

        lname.FontSize = (float)Application.Current.Resources["Mb_fs"] + 8;
        ttext.FontSize = (float)Application.Current.Resources["Mb_fs"];
    }
}
