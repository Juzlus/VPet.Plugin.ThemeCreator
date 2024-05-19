using System.Windows.Media;
using System.Windows;
using Panuon.WPF.UI;
using System.Windows.Controls;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.ThemeCreator.ElementResources;
internal class ToolBarResources
{
    public void CreateResources(IMainWindow main)
    {
        Application.Current.Resources.Add("Tb_b1", Brushes.White);
        Application.Current.Resources.Add("Tb_b2", Brushes.White);
        Application.Current.Resources.Add("Tb_b3", Brushes.White);
        Application.Current.Resources.Add("Tb_b4", Brushes.White);
        Application.Current.Resources.Add("Tb_b5", Brushes.White);
        Application.Current.Resources.Add("Tb_b6", Brushes.White);
        Application.Current.Resources.Add("Tb_b7", Brushes.White);
        Application.Current.Resources.Add("Tb_b8", Brushes.White);
        Application.Current.Resources.Add("Tb_f1", Brushes.Black);
        Application.Current.Resources.Add("Tb_hf1", Brushes.Red);
        Application.Current.Resources.Add("Tb_hb1", Brushes.Red);
        Application.Current.Resources.Add("Tb_ob1", Brushes.Red);
        Application.Current.Resources.Add("Tb_of1", Brushes.Red);
        Application.Current.Resources.Add("Tb_hb2", Brushes.Red);
        Application.Current.Resources.Add("Tb_ob2", Brushes.Red);
        Application.Current.Resources.Add("Tb_sh2", Colors.Red);
        Application.Current.Resources.Add("Tb_sb", Brushes.Red);
        Application.Current.Resources.Add("Tb_spb", Brushes.Red);
        Application.Current.Resources.Add("Tb_sdc", Brushes.Red);

        Application.Current.Resources.Add("Tb_bt1", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_m1", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_m2", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_m3", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_m4", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_m5", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_bt2", new Thickness(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_bt3", new Thickness(0, 0, 0, 0));

        Application.Current.Resources.Add("Tb_cr1", new CornerRadius(0, 0, 0, 0));
        Application.Current.Resources.Add("Tb_cr2", new CornerRadius(0, 0, 0, 0));

        Application.Current.Resources.Add("Tb_sv1", Visibility.Visible);

        Application.Current.Resources.Add("Tb_fw1", FontWeights.Normal);

        Application.Current.Resources.Add("Tb_o1", 1f);
        Application.Current.Resources.Add("Tb_fs1", 24f);
        Application.Current.Resources.Add("Tb_h1", 55f);

        this.ApplyResources(main);
    }

    public void ApplyResources(IMainWindow main)
    {
        Menu parentMenu = (Menu)main.Main.ToolBar.MenuFeed.Parent;
        parentMenu.BorderThickness = (Thickness)Application.Current.Resources["Tb_bt1"];
        parentMenu.Background = (Brush)Application.Current.Resources["Tb_b1"];
        parentMenu.Foreground = (Brush)Application.Current.Resources["Tb_f1"];
        parentMenu.Opacity = (float)Application.Current.Resources["Tb_o1"];
        parentMenu.FontSize = (float)Application.Current.Resources["Tb_fs1"];
        parentMenu.FontWeight = (FontWeight)Application.Current.Resources["Tb_fw1"];
        parentMenu.Height = (float)Application.Current.Resources["Tb_h1"];

        parentMenu.SetValue(DropDownHelper.ShadowColorProperty, (Color)Application.Current.Resources["Tb_sh2"]);
        parentMenu.SetValue(MenuHelper.CornerRadiusProperty, (CornerRadius)Application.Current.Resources["Tb_cr1"]);
        parentMenu.SetValue(MenuHelper.TopLevelItemsHoverBackgroundProperty, (Brush)Application.Current.Resources["Tb_hb1"]);
        parentMenu.SetValue(MenuHelper.TopLevelItemsHoverForegroundProperty, (Brush)Application.Current.Resources["Tb_hf1"]);
        parentMenu.SetValue(MenuHelper.TopLevelItemsOpenedBackgroundProperty, (Brush)Application.Current.Resources["Tb_ob1"]);
        parentMenu.SetValue(MenuHelper.TopLevelItemsOpenedForegroundProperty, (Brush)Application.Current.Resources["Tb_of1"]);
        parentMenu.SetValue(MenuHelper.SubmenuItemsSeparatorVisibilityProperty, (Visibility)Application.Current.Resources["Tb_sv1"]);
        parentMenu.SetValue(DropDownHelper.BackgroundProperty, (Brush)Application.Current.Resources["Tb_b2"]);
        parentMenu.SetValue(DropDownHelper.BorderThicknessProperty, (Thickness)Application.Current.Resources["Tb_bt2"]);
        parentMenu.SetValue(MenuHelper.SubmenuItemsCornerRadiusProperty, (CornerRadius)Application.Current.Resources["Tb_cr2"]);
        parentMenu.SetValue(MenuHelper.SubmenuItemsBorderThicknessProperty, (Thickness)Application.Current.Resources["Tb_bt3"]);
        parentMenu.SetValue(MenuHelper.SubmenuItemsHoverBackgroundProperty, (Brush)Application.Current.Resources["Tb_hb2"]);
        parentMenu.SetValue(MenuHelper.SubmenuItemsBackgroundProperty, (Brush)Application.Current.Resources["Tb_b3"]);
        parentMenu.SetValue(MenuHelper.SubmenuItemsOpenedBackgroundProperty, (Brush)Application.Current.Resources["Tb_ob2"]);

        MenuItem menuFeed = main.Main.ToolBar.MenuFeed;
        menuFeed.Background = (Brush)Application.Current.Resources["Tb_b4"];
        menuFeed.Margin = (Thickness)Application.Current.Resources["Tb_m1"];

        MenuItem menuPanel = (MenuItem)(main.Main.ToolBar.FindName("MenuPanel"));
        menuPanel.Background = (Brush)Application.Current.Resources["Tb_b5"];
        menuPanel.Margin = (Thickness)Application.Current.Resources["Tb_m2"];

        MenuItem menuInteract = (MenuItem)(main.Main.ToolBar.FindName("MenuInteract"));
        menuInteract.Background = (Brush)Application.Current.Resources["Tb_b6"];
        menuInteract.Margin = (Thickness)Application.Current.Resources["Tb_m3"];

        MenuItem menuDIY = main.Main.ToolBar.MenuDIY;
        menuDIY.Background = (Brush)Application.Current.Resources["Tb_b7"];
        menuDIY.Margin = (Thickness)Application.Current.Resources["Tb_m4"];

        MenuItem menuSetting = main.Main.ToolBar.MenuSetting;
        menuSetting.Background = (Brush)Application.Current.Resources["Tb_b8"];
        menuSetting.Margin = (Thickness)Application.Current.Resources["Tb_m5"];

        Border brdPanel = main.Main.ToolBar.BdrPanel;
        brdPanel.Background = (Brush)Application.Current.Resources["Tb_sb"];
        (brdPanel.FindName("pExp") as ProgressBar).Background = (Brush)Application.Current.Resources["Tb_spb"];
        (brdPanel.FindName("pStrength") as ProgressBar).Background = (Brush)Application.Current.Resources["Tb_spb"];
        (brdPanel.FindName("pFeeling") as ProgressBar).Background = (Brush)Application.Current.Resources["Tb_spb"];
        (brdPanel.FindName("pStrengthFoodMax") as ProgressBar).Background = (Brush)Application.Current.Resources["Tb_spb"];
        (brdPanel.FindName("pStrengthDrinkMax") as ProgressBar).Background = (Brush)Application.Current.Resources["Tb_spb"];

        (brdPanel.FindName("tExp") as TextBlock).Foreground = (Brush)Application.Current.Resources["Tb_sdc"];
        (brdPanel.FindName("tStrength") as TextBlock).Foreground = (Brush)Application.Current.Resources["Tb_sdc"];
        (brdPanel.FindName("tFeeling") as TextBlock).Foreground = (Brush)Application.Current.Resources["Tb_sdc"];
        (brdPanel.FindName("tStrengthFood") as TextBlock).Foreground = (Brush)Application.Current.Resources["Tb_sdc"];
        (brdPanel.FindName("tStrengthDrink") as TextBlock).Foreground = (Brush)Application.Current.Resources["Tb_sdc"];
        (brdPanel.FindName("tMoney") as TextBlock).Foreground = (Brush)Application.Current.Resources["Tb_sdc"];
        (brdPanel.FindName("Tlv") as TextBlock).Foreground = (Brush)Application.Current.Resources["Tb_sdc"];
    }
}
