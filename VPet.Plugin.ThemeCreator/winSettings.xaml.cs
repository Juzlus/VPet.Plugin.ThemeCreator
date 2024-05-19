using LinePutScript.Localization.WPF;
using Panuon.WPF.UI;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.Forms.MessageBox;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using Orientation = System.Windows.Controls.Orientation;
using Point = System.Windows.Point;

class ThemeElement
{
    public string ThemeNameWPF { get; set; }
    public string ThemeName { get; set; }
    public string ThemeDescription { get; set; }
    public string ModVersion { get; set; }
    public float ThemeOpacity { get; set; }
    public ThemeElement(string themeName, string themeDescription, string modVersion, float themeOpacity)
    {
        ThemeNameWPF = $"ThemeName_{themeName}";
        ThemeName = themeName;
        ThemeDescription = themeDescription;
        ModVersion = modVersion;
        ThemeOpacity = themeOpacity;
    }
}

namespace VPet.Plugin.ThemeCreator
{
    public partial class winSettings : WindowX
    {
        private string path;
        private bool disableTriggers = true;
        private bool hasChange = false;
        ThemeCreator main;
        private Point mousePosition;

        private Button lastElement;
        private Brush lastColor;
        private bool showCancel = false;

        public winSettings(ThemeCreator main, string path)
        {
            this.InitializeComponent();
            this.main = main;
            this.path = path;
            this.ThemeName.Text = $"{"Theme".Translate()}-{((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()}";
            this.ThemeDescription.Text = "Simple theme template".Translate();
            this.CreateSettings();
            this.UpdateThemeList();
        }

        private void ColorValueUpdate(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            System.Windows.Application.Current.Resources[lastElement.Name] = this.lastElement.Tag.ToString() == "Brushes" ? new SolidColorBrush((Color)e.NewValue) : (Color)e.NewValue;
            this.lastElement.Background = new SolidColorBrush((Color)e.NewValue);
            this.main.ApplyResources("All");
            this.hasChange = true;

            if (!this.showCancel)
            {
                this.cancelColorPicker.Visibility = Visibility.Visible;
                this.showCancel = true;
            }
        }

        private void CancelColorPicker(object sender, EventArgs e)
        {
            this.showCancel = false;
            this.cancelColorPicker.Visibility = Visibility.Collapsed;
            Color c = (Color)ColorConverter.ConvertFromString(((SolidColorBrush)this.lastColor).Color.ToString());
            this.lastElement.Background = this.lastColor;
            System.Windows.Application.Current.Resources[this.lastElement.Name] = this.lastElement.Tag.ToString() == "Brushes" ? new SolidColorBrush(c) : c;
            this.main.ApplyResources("All");
        }

        private void WClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.hasChange)
            {
                MessageBoxResult result = (MessageBoxResult)MessageBox.Show("You have unsaved changes. Exit without saving?".Translate(), "Unsaved".Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            this.main.ChangeTheme(this.main.activeTheme);
            this.main.ApplyResources("All");
        }

        private async Task<string> ShowInputDialog()
        {
            var inputDialog = new InputDialog();
            bool? result = inputDialog.ShowDialog();
            return inputDialog.Code;
        }

        private async void ImportFromPastebin(object sender, EventArgs e)
        {
            string code = await ShowInputDialog();
            if (code is null) return;

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                var response = new WebClient().DownloadString($"https://pastebin.com/raw/{code}");
                xmlDoc.LoadXml(response);
                XmlElement name = xmlDoc.SelectSingleNode($"//Name") as XmlElement;
                XmlElement desc = xmlDoc.SelectSingleNode($"//Desc") as XmlElement;
                XmlElement modVersion = xmlDoc.SelectSingleNode($"//ModVersion") as XmlElement;
                if (name is null || desc is null || modVersion is null)
                {
                    MessageBox.Show("This code is invalid!".Translate());
                    return;
                }
                File.WriteAllText($"{this.path}//theme//{name.InnerText}.cfg", response);
                this.UpdateThemeList();

            }
            catch
            {
                MessageBox.Show("This code is invalid!".Translate());
            }
        }

        private void UpdateThemeList()
        {
            this.themesList.Items.Clear();
            string[] files = Directory.GetFiles($"{this.path}//theme", "*.cfg");

            foreach (string file in files)
            {
                string name = this.main.GetFromConfig(file, "Name");
                string desc = this.main.GetFromConfig(file, "Desc");
                string modVersion = this.main.GetFromConfig(file, "ModVersion");
                if (name is null || desc is null || modVersion is null) continue;
                this.themesList.Items.Add(new ThemeElement(name, desc.Translate(), modVersion, this.main.activeTheme != name ? 0.7f : 1f));
            }
        }

        private void CopyTheme(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string themeName = button.Tag.ToString().Substring(10);
            try
            {
                if (themeName.Length == 50)
                    return;
                if (File.Exists($"{this.path}//theme//{themeName}.cfg"))
                {
                    File.Copy($"{this.path}//theme//{themeName}.cfg", $"{this.path}//theme//_{themeName}.cfg");
                    this.main.activeTheme = $"_{themeName}";
                    this.main.SaveToConfig("Name", -1, $"_{themeName}");
                    this.main.activeTheme = themeName;
                }
                this.UpdateThemeList();
            }
            catch { }
        }

        private void DeleteTheme(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string themeName = button.Tag.ToString().Substring(10);

            if (themeName == "Orginal")
            {
                MessageBox.Show("You cannot delete the Original theme!".Translate());
                return;
            }

            try
            {
                if (File.Exists($"{this.path}//theme//{themeName}.cfg"))
                    File.Delete($"{this.path}//theme//{themeName}.cfg");
                this.UpdateThemeList();
            }
            catch { }
        }

        private void TextInputForFile(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[a-zA-Z0-9 _-]+");
            if (!regex.IsMatch(e.Text))
                e.Handled = true;
        }

        private void ChangeBrush(object sender, EventArgs e)
        {
            if (this.disableTriggers) return;
            Button btn = sender as Button;

            this.lastElement = btn;
            this.lastColor = btn.Background;

            this.colorPicker.Margin = new Thickness(this.mousePosition.X, this.mousePosition.Y - 40, 0, 0);
            this.colorPicker.Visibility = Visibility.Visible;
            this.colorPicker.SelectedColor = btn.Tag.ToString() == "Color" ?
                (Color)System.Windows.Application.Current.Resources[btn.Name]
                : ((SolidColorBrush)System.Windows.Application.Current.Resources[btn.Name]).Color;
            this.colorPicker.IsOpen = true;
        }

        private void ChangeThickness(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.disableTriggers) return;
            Slider slider = (sender as Slider);
            string xName = slider.Name;

            Slider s1 = (Slider)this.mainTabControl.FindName($"{xName.Substring(0, xName.Length - 2)}_1");
            Slider s2 = (Slider)this.mainTabControl.FindName($"{xName.Substring(0, xName.Length - 2)}_2");
            Slider s3 = (Slider)this.mainTabControl.FindName($"{xName.Substring(0, xName.Length - 2)}_3");
            Slider s4 = (Slider)this.mainTabControl.FindName($"{xName.Substring(0, xName.Length - 2)}_4");

            try
            {
                float v1 = (float)s1.Value;
                float v2 = (float)s2.Value;
                float v3 = (float)s3.Value;
                float v4 = (float)s4.Value;
                if (xName.Contains("cr"))
                    System.Windows.Application.Current.Resources[xName.Substring(0, xName.Length - 2)] = new CornerRadius(v1, v2, v3, v4);
                else
                    System.Windows.Application.Current.Resources[xName.Substring(0, xName.Length - 2)] = new Thickness(v1, v2, v3, v4);
                this.main.ApplyResources(xName);
                this.hasChange = true;
            }
            catch { }
        }

        private void ChangeSwitchVisibility(object sender, RoutedEventArgs e)
        {
            if (this.disableTriggers) return;
            Switch textBox = (sender as Switch);
            System.Windows.Application.Current.Resources[textBox.Name] = (bool)textBox.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            this.main.ApplyResources(textBox.Name);
            this.hasChange = true;
        }

        private void ChangeSwitchBold(object sender, RoutedEventArgs e)
        {
            if (this.disableTriggers) return;
            Switch textBox = (sender as Switch);
            System.Windows.Application.Current.Resources[textBox.Name] = (bool)textBox.IsChecked ? FontWeights.Bold : FontWeights.Normal;
            this.main.ApplyResources(textBox.Name);
            this.hasChange = true;
        }

        private void ChangeSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.disableTriggers) return;
            Slider slider = (sender as Slider);
            if (System.Windows.Application.Current.FindResource(slider.Name) is not null)
                System.Windows.Application.Current.Resources[slider.Name] = (float)slider.Value;
            this.main.ApplyResources(slider.Name);
            this.hasChange = true;
        }

        private void SayTestMessage(object sender, RoutedEventArgs e)
        {
            this.main.MW.Main.Say("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", null, true);
        }

        private void SaveTheme(object sender, RoutedEventArgs e)
        {
            if (this.ThemeName.Text == "Orginal")
            {
                MessageBox.Show("You cannot overwrite the Original theme".Translate());
                return;
            }

            string configPath = $"{this.path}//theme//{this.ThemeName.Text}.cfg";
            if (File.Exists(configPath))
            {
                MessageBoxResult result = (MessageBoxResult)MessageBox.Show("Are you sure you want to overwrite the theme?".Translate(), "Overwrite".Translate(), MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (result == MessageBoxResult.No)
                    return;
            }

            this.main.activeTheme = this.ThemeName.Text;
            File.WriteAllText($"{this.path}//config.lps", this.main.activeTheme);

            this.main.SaveToConfig("Name", -1, this.main.activeTheme);
            this.main.SaveToConfig("Desc", -1, this.ThemeDescription.Text);
            this.main.SaveToConfig("ModVersion", -1, this.main.version);

            this.main.SaveToConfig("Tb_b1", 0);
            this.main.SaveToConfig("Tb_b1", 0);
            this.main.SaveToConfig("Tb_b3", 0);
            this.main.SaveToConfig("Tb_b4", 0);
            this.main.SaveToConfig("Tb_b5", 0);
            this.main.SaveToConfig("Tb_b6", 0);
            this.main.SaveToConfig("Tb_b7", 0);
            this.main.SaveToConfig("Tb_b8", 0);
            this.main.SaveToConfig("Tb_f1", 0);
            this.main.SaveToConfig("Tb_hb1", 0);
            this.main.SaveToConfig("Tb_hf1", 0);
            this.main.SaveToConfig("Tb_of1", 0);
            this.main.SaveToConfig("Tb_ob1", 0);
            this.main.SaveToConfig("Tb_hb2", 0);
            this.main.SaveToConfig("Tb_ob2", 0);
            this.main.SaveToConfig("Tb_sb", 0);
            this.main.SaveToConfig("Tb_spb", 0);
            this.main.SaveToConfig("Tb_sdc", 0);
            this.main.SaveToConfig("Tb_b2", 0);
            this.main.SaveToConfig("Tb_sh2", 1);
            this.main.SaveToConfig("Tb_o1", 1);
            this.main.SaveToConfig("Tb_fs1", 1);
            this.main.SaveToConfig("Tb_h1", 1);
            this.main.SaveToConfig("Tb_sv1", 2);
            this.main.SaveToConfig("Tb_fw1", 3);
            this.main.SaveToConfig("Tb_bt1", 4);
            this.main.SaveToConfig("Tb_m1", 4);
            this.main.SaveToConfig("Tb_m2", 4);
            this.main.SaveToConfig("Tb_m3", 4);
            this.main.SaveToConfig("Tb_m4", 4);
            this.main.SaveToConfig("Tb_m5", 4);
            this.main.SaveToConfig("Tb_bt2", 4);
            this.main.SaveToConfig("Tb_bt3", 4);
            this.main.SaveToConfig("Tb_cr1", 5);
            this.main.SaveToConfig("Tb_cr2", 5);

            this.main.SaveToConfig("Mb_bg", 0);
            this.main.SaveToConfig("Mb_bbg", 0);
            this.main.SaveToConfig("Mb_nfg", 0);
            this.main.SaveToConfig("Mb_cfg", 0);
            this.main.SaveToConfig("Mb_o", 1);
            this.main.SaveToConfig("Mb_fs", 1);
            this.main.SaveToConfig("Mb_bbt", 4);
            this.main.SaveToConfig("Mb_bcr", 5);

            this.main.SaveToConfig("Primary", 0);
            this.main.SaveToConfig("PrimaryTrans", 0);
            this.main.SaveToConfig("PrimaryTrans4", 0);
            this.main.SaveToConfig("PrimaryTransA", 0);
            this.main.SaveToConfig("PrimaryTransE", 0);
            this.main.SaveToConfig("PrimaryLight", 0);
            this.main.SaveToConfig("PrimaryLighter", 0);
            this.main.SaveToConfig("PrimaryDark", 0);
            this.main.SaveToConfig("PrimaryDarker", 0);
            this.main.SaveToConfig("PrimaryText", 0);
            this.main.SaveToConfig("Secondary", 0);
            this.main.SaveToConfig("SecondaryTrans", 0);
            this.main.SaveToConfig("SecondaryTrans4", 0);
            this.main.SaveToConfig("SecondaryTransA", 0);
            this.main.SaveToConfig("SecondaryTransE", 0);
            this.main.SaveToConfig("SecondaryLight", 0);
            this.main.SaveToConfig("SecondaryLighter", 0);
            this.main.SaveToConfig("SecondaryDark", 0);
            this.main.SaveToConfig("SecondaryDarker", 0);
            this.main.SaveToConfig("SecondaryText", 0);
            this.main.SaveToConfig("DARKPrimary", 0);
            this.main.SaveToConfig("DARKPrimaryTrans", 0);
            this.main.SaveToConfig("DARKPrimaryTrans4", 0);
            this.main.SaveToConfig("DARKPrimaryTransA", 0);
            this.main.SaveToConfig("DARKPrimaryTransE", 0);
            this.main.SaveToConfig("DARKPrimaryLight", 0);
            this.main.SaveToConfig("DARKPrimaryLighter", 0);
            this.main.SaveToConfig("DARKPrimaryDark", 0);
            this.main.SaveToConfig("DARKPrimaryDarker", 0);
            this.main.SaveToConfig("DARKPrimaryText", 0);
            this.main.SaveToConfig("ShadowColor", 1);

            this.main.SaveToConfig("Bb_bg1", 0);
            this.main.SaveToConfig("Bb_bg2", 0);
            this.main.SaveToConfig("Bb_bg3", 0);
            this.main.SaveToConfig("Bb_bg4", 0);
            this.main.SaveToConfig("Bb_bg5", 0);
            this.main.SaveToConfig("Bb_bg6", 0);
            this.main.SaveToConfig("Bb_bg7", 0);
            this.main.SaveToConfig("Bb_bg8", 0);
            this.main.SaveToConfig("Bb_bg9", 0);
            this.main.SaveToConfig("Bb_bg10", 0);
            this.main.SaveToConfig("Bb_bg11", 0);
            this.main.SaveToConfig("Bb_bg12", 0);
            this.main.SaveToConfig("Bb_bg13", 0);
            this.main.SaveToConfig("Bb_bg14", 0);
            this.main.SaveToConfig("Bb_bg15", 0);
            this.main.SaveToConfig("Bb_bg16", 0);
            this.main.SaveToConfig("Bb_bg17", 0);
            this.main.SaveToConfig("Bb_bg18", 0);
            this.main.SaveToConfig("Bb_bg19", 0);
            this.main.SaveToConfig("Bb_fg1", 0);

            this.main.SaveToConfig("Bb_sh1", 1);
            this.main.SaveToConfig("Bb_o1", 1);
            this.main.SaveToConfig("Bb_m1", 4);
            this.main.SaveToConfig("Bb_bt1", 4);
            this.main.SaveToConfig("Bb_cr1", 5);
            this.main.SaveToConfig("Bb_cr2", 5);
            this.main.SaveToConfig("Bb_cr3", 5);
            this.main.SaveToConfig("Bb_cr4", 5);
            this.main.SaveToConfig("Bb_ib1", 6);

            this.main.SaveToConfig("St_bg1", 0);
            this.main.SaveToConfig("St_bg2", 0);
            this.main.SaveToConfig("St_bg3", 0);
            this.main.SaveToConfig("St_bg4", 0);
            this.main.SaveToConfig("St_bg5", 0);
            this.main.SaveToConfig("St_bg6", 0);
            this.main.SaveToConfig("St_fg1", 0);
            this.main.SaveToConfig("St_sh1", 1);
            this.main.SaveToConfig("St_o1", 1);
            this.main.SaveToConfig("St_bt1", 4);
            this.main.SaveToConfig("St_ib1", 6);

            this.UpdateThemeList();
            this.hasChange = false;
        }

        private void CreateSettings()
        {
            this.disableTriggers = true;
            // ToolBarGrid
            this.CreateElement(this.ToolBarGrid, 1, 0, 1, "Background", "Tb_b1");
            this.CreateElement(this.ToolBarGrid, 2, 0, 1, "Dropdown", "Tb_b2");
            this.CreateElement(this.ToolBarGrid, 3, 0, 1, "Submenu", "Tb_b3");
            this.CreateElement(this.ToolBarGrid, 4, 0, 1, "MenuFeed", "Tb_b4");
            this.CreateElement(this.ToolBarGrid, 5, 0, 1, "MenuPanel", "Tb_b5");
            this.CreateElement(this.ToolBarGrid, 6, 0, 1, "MenuInteract", "Tb_b6");
            this.CreateElement(this.ToolBarGrid, 7, 0, 1, "MenuDIY", "Tb_b7");
            this.CreateElement(this.ToolBarGrid, 8, 0, 1, "MenuSetting", "Tb_b8");
            this.CreateElement(this.ToolBarGrid, 9, 0, 1, "Foreground", "Tb_f1");
            this.CreateElement(this.ToolBarGrid, 10, 0, 1, "Top hover", "Tb_hb1");
            this.CreateElement(this.ToolBarGrid, 11, 0, 1, "Top hover text", "Tb_hf1");
            this.CreateElement(this.ToolBarGrid, 12, 0, 1, "Top opened", "Tb_ob1");
            this.CreateElement(this.ToolBarGrid, 13, 0, 1, "Top opened text", "Tb_of1");
            this.CreateElement(this.ToolBarGrid, 14, 0, 1, "Submenu hover", "Tb_hb2");
            this.CreateElement(this.ToolBarGrid, 15, 0, 1, "Submenu opened", "Tb_ob2");
            this.CreateElement(this.ToolBarGrid, 16, 0, 2, "Submenu shadow", "Tb_sh2");
            this.CreateElement(this.ToolBarGrid, 17, 0, 1, "Statistics", "Tb_sb");
            this.CreateElement(this.ToolBarGrid, 18, 0, 1, "Statistics progressbar", "Tb_spb");
            this.CreateElement(this.ToolBarGrid, 19, 0, 1, "Statistics text", "Tb_sdc");

            this.CreateElement(this.ToolBarGrid, 1, 3, 7, "Border radius", "Tb_bt1");
            this.CreateElement(this.ToolBarGrid, 2, 3, 7, "MenuFeed margin", "Tb_m1");
            this.CreateElement(this.ToolBarGrid, 3, 3, 7, "MenuPanel margin", "Tb_m2");
            this.CreateElement(this.ToolBarGrid, 4, 3, 7, "MenuInteract margin", "Tb_m3");
            this.CreateElement(this.ToolBarGrid, 5, 3, 7, "MenuDIY margin", "Tb_m4");
            this.CreateElement(this.ToolBarGrid, 6, 3, 7, "MenuSetting margin", "Tb_m5");
            this.CreateElement(this.ToolBarGrid, 7, 3, 7, "DropDown border", "Tb_bt2");
            this.CreateElement(this.ToolBarGrid, 8, 3, 7, "Submenu border", "Tb_bt3");
            this.CreateElement(this.ToolBarGrid, 9, 3, 7, "Corner", "Tb_cr1");
            this.CreateElement(this.ToolBarGrid, 10, 3, 7, "Submenu corner", "Tb_cr2");
            this.CreateElement(this.ToolBarGrid, 11, 3, 4, "Submenu separator", "Tb_sv1");
            this.CreateElement(this.ToolBarGrid, 12, 3, 5, "Text bolted", "Tb_fw1");
            this.CreateElement(this.ToolBarGrid, 13, 3, 3, "Opacity", "Tb_o1");
            this.CreateElement(this.ToolBarGrid, 14, 3, 3, "Font size", "Tb_fs1", 100);
            this.CreateElement(this.ToolBarGrid, 15, 3, 3, "Height", "Tb_h1", 500);

            // MessageBarGrid
            this.CreateElement(this.MessageBarGrid, 1, 0, 1, "Background", "Mb_bg");
            this.CreateElement(this.MessageBarGrid, 2, 0, 1, "Border", "Mb_bbg");
            this.CreateElement(this.MessageBarGrid, 3, 0, 1, "Pet name text", "Mb_nfg");
            this.CreateElement(this.MessageBarGrid, 4, 0, 1, "Content text", "Mb_cfg");
            this.CreateElement(this.MessageBarGrid, 5, 0, 6, "Say test message", null);

            this.CreateElement(this.MessageBarGrid, 1, 3, 7, "Border thickness", "Mb_bbt");
            this.CreateElement(this.MessageBarGrid, 2, 3, 7, "Border radius", "Mb_bcr");
            this.CreateElement(this.MessageBarGrid, 3, 3, 3, "Opacity", "Mb_o");
            this.CreateElement(this.MessageBarGrid, 4, 3, 3, "Font size", "Mb_fs", 100);


            // OtherGrid
            this.CreateElement(this.OtherGrid, 1, 0, 1, "Primary", "Primary");
            this.CreateElement(this.OtherGrid, 2, 0, 1, "Primary trans", "PrimaryTrans");
            this.CreateElement(this.OtherGrid, 3, 0, 1, "Primary trans 4", "PrimaryTrans4");
            this.CreateElement(this.OtherGrid, 4, 0, 1, "Primary trans A", "PrimaryTransA");
            this.CreateElement(this.OtherGrid, 5, 0, 1, "Primary trans E", "PrimaryTransE");
            this.CreateElement(this.OtherGrid, 6, 0, 1, "Primary light", "PrimaryLight");
            this.CreateElement(this.OtherGrid, 7, 0, 1, "Primary lighter", "PrimaryLighter");
            this.CreateElement(this.OtherGrid, 8, 0, 1, "Primary dark", "PrimaryDark");
            this.CreateElement(this.OtherGrid, 9, 0, 1, "Primary darker", "PrimaryDarker");
            this.CreateElement(this.OtherGrid, 10, 0, 1, "Primary text", "PrimaryText");
            this.CreateElement(this.OtherGrid, 11, 0, 1, "Secondary", "Secondary");
            this.CreateElement(this.OtherGrid, 12, 0, 1, "Secondary trans", "SecondaryTrans");
            this.CreateElement(this.OtherGrid, 13, 0, 1, "Secondary trans 4", "SecondaryTrans4");
            this.CreateElement(this.OtherGrid, 14, 0, 1, "Secondary trans A", "SecondaryTransA");
            this.CreateElement(this.OtherGrid, 15, 0, 1, "Secondary trans E", "SecondaryTransE");
            this.CreateElement(this.OtherGrid, 16, 0, 1, "Secondary light", "SecondaryLight");
            this.CreateElement(this.OtherGrid, 17, 0, 1, "Secondary lighter", "SecondaryLighter");

            this.CreateElement(this.OtherGrid, 1, 3, 1, "Secondary dark", "SecondaryDark");
            this.CreateElement(this.OtherGrid, 2, 3, 1, "Secondary darker", "SecondaryDarker");
            this.CreateElement(this.OtherGrid, 3, 3, 1, "Secondary text", "SecondaryText");
            this.CreateElement(this.OtherGrid, 4, 3, 1, "Dark pimary", "DARKPrimary");
            this.CreateElement(this.OtherGrid, 5, 3, 1, "Dark primary trans", "DARKPrimaryTrans");
            this.CreateElement(this.OtherGrid, 6, 3, 1, "Dark primary trans 4", "DARKPrimaryTrans4");
            this.CreateElement(this.OtherGrid, 7, 3, 1, "Dark primary trans A", "DARKPrimaryTransA");
            this.CreateElement(this.OtherGrid, 8, 3, 1, "Dark primary trans E", "DARKPrimaryTransE");
            this.CreateElement(this.OtherGrid, 9, 3, 1, "Dark primary light", "DARKPrimaryLight");
            this.CreateElement(this.OtherGrid, 10, 3, 1, "Dark primary lighter", "DARKPrimaryLighter");
            this.CreateElement(this.OtherGrid, 11, 3, 1, "Dark primary dark", "DARKPrimaryDark");
            this.CreateElement(this.OtherGrid, 12, 3, 1, "Dark primary darker", "DARKPrimaryDarker");
            this.CreateElement(this.OtherGrid, 13, 3, 1, "Dark primary text", "DARKPrimaryText");
            this.CreateElement(this.OtherGrid, 14, 3, 2, "Shadow color", "ShadowColor");

            // BetterBuyGrin
            this.CreateElement(this.BetterBuyGrid, 1, 0, 1, "Background", "Bb_bg1");
            this.CreateElement(this.BetterBuyGrid, 2, 0, 1, "Header", "Bb_bg2");
            this.CreateElement(this.BetterBuyGrid, 3, 0, 1, "Header text", "Bb_fg1");
            this.CreateElement(this.BetterBuyGrid, 4, 0, 1, "Border", "Bb_bg3");
            this.CreateElement(this.BetterBuyGrid, 5, 0, 1, "Product menu", "Bb_bg4");
            this.CreateElement(this.BetterBuyGrid, 6, 0, 1, "Product menu selected", "Bb_bg5");
            this.CreateElement(this.BetterBuyGrid, 7, 0, 1, "Product menu selected text", "Bb_bg6");
            this.CreateElement(this.BetterBuyGrid, 8, 0, 1, "Sort menu 1", "Bb_bg7");
            this.CreateElement(this.BetterBuyGrid, 9, 0, 1, "Sort menu 1 selected", "Bb_bg8");
            this.CreateElement(this.BetterBuyGrid, 10, 0, 1, "Sort menu 1 selected text", "Bb_bg9");
            this.CreateElement(this.BetterBuyGrid, 11, 0, 1, "Sort menu 2", "Bb_bg10");
            this.CreateElement(this.BetterBuyGrid, 12, 0, 1, "Sort menu 2 selected", "Bb_bg11");
            this.CreateElement(this.BetterBuyGrid, 13, 0, 1, "Sort menu 2 selected text", "Bb_bg12");
            this.CreateElement(this.BetterBuyGrid, 14, 0, 1, "Products panel", "Bb_bg13");
            this.CreateElement(this.BetterBuyGrid, 15, 0, 1, "Products", "Bb_bg14");

            this.CreateElement(this.BetterBuyGrid, 1, 3, 1, "Product menu hover", "Bb_bg15");
            this.CreateElement(this.BetterBuyGrid, 2, 3, 1, "Sort menu 1 hover", "Bb_bg16");
            this.CreateElement(this.BetterBuyGrid, 3, 3, 1, "Sort menu 2 hover", "Bb_bg17");
            this.CreateElement(this.BetterBuyGrid, 4, 3, 1, "Menu text", "Bb_bg18");
            this.CreateElement(this.BetterBuyGrid, 5, 3, 1, "Left aside", "Bb_bg19");
            this.CreateElement(this.BetterBuyGrid, 6, 3, 2, "Shadow", "Bb_sh1");
            this.CreateElement(this.BetterBuyGrid, 7, 3, 8, "Background image", "Bb_ib1");

            this.CreateElement(this.BetterBuyGrid, 8, 3, 7, "Product menu selected", "Bb_cr1");
            this.CreateElement(this.BetterBuyGrid, 9, 3, 7, "Sort 1 menu selected", "Bb_cr2");
            this.CreateElement(this.BetterBuyGrid, 10, 3, 7, "Sort 2 menu selected", "Bb_cr3");
            this.CreateElement(this.BetterBuyGrid, 11, 3, 7, "Products corner radius", "Bb_cr4");

            this.CreateElement(this.BetterBuyGrid, 12, 3, 7, "Product menu margin", "Bb_m1");
            this.CreateElement(this.BetterBuyGrid, 13, 3, 7, "Border", "Bb_bt1");

            this.CreateElement(this.BetterBuyGrid, 14, 3, 3, "Brightness", "Bb_o1");

            // SettingsGrid
            this.CreateElement(this.SettingsGrid, 1, 0, 1, "Background", "St_bg1");
            this.CreateElement(this.SettingsGrid, 2, 0, 1, "Header", "St_bg2");
            this.CreateElement(this.SettingsGrid, 3, 0, 1, "Border", "St_bg3");
            this.CreateElement(this.SettingsGrid, 4, 0, 1, "List menu", "St_bg4");
            this.CreateElement(this.SettingsGrid, 5, 0, 1, "List menu hover", "St_bg5");
            this.CreateElement(this.SettingsGrid, 6, 0, 1, "List menu text", "St_fg1");
            this.CreateElement(this.SettingsGrid, 7, 0, 1, "Tab panel", "St_bg6");

            this.CreateElement(this.SettingsGrid, 1, 3, 7, "Border", "St_bt1");
            this.CreateElement(this.SettingsGrid, 2, 3, 2, "Shadow", "St_sh1");
            this.CreateElement(this.SettingsGrid, 3, 3, 3, "Brightness", "St_o1");
            this.CreateElement(this.SettingsGrid, 4, 3, 8, "Background image", "St_ib1");

            this.disableTriggers = false;
        }

        private void CreateElement(Grid grid, int row, int col, int type, string name, string id, double sliderMax = 1)
        {
            if (col == 0)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

            TextBlock text = new TextBlock();
            text.Text = name.Translate();
            text.Style = (Style)Application.Current.Resources["ThemeTextBlock"];
            text.Foreground = (SolidColorBrush)Application.Current.Resources["PrimaryText"];
            text.VerticalAlignment = VerticalAlignment.Center;

            Grid.SetRow(text, row);
            Grid.SetColumn(text, col + 0);
            grid.Children.Add(text);

            switch (type)
            {
                case 1:  // Brushes
                case 2:  // Colors
                    Button colorPickerButton = new Button();
                    colorPickerButton.Name = id;
                    colorPickerButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    colorPickerButton.Background = Brushes.Black;
                    colorPickerButton.Width = 40;
                    colorPickerButton.Height = 20;
                    colorPickerButton.Margin = new Thickness(0, 5, 0, 5);
                    colorPickerButton.Tag = type == 1 ? "Brushes" : "Color";
                    colorPickerButton.Click += this.ChangeBrush;
                    colorPickerButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Application.Current.Resources[id].ToString());

                    Grid.SetRow(colorPickerButton, row);
                    Grid.SetColumn(colorPickerButton, col + 2);
                    grid.Children.Add(colorPickerButton);
                    break;
                case 3:  // Slider
                    Grid sliderGrid = new Grid();
                    sliderGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
                    sliderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70, GridUnitType.Star) });
                    sliderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Star) });

                    Slider slider = new Slider();
                    slider.Name = id;
                    slider.VerticalAlignment = VerticalAlignment.Center;
                    slider.IsSnapToTickEnabled = true;
                    slider.LargeChange = (sliderMax == 1) ? 0.1 : 1;
                    slider.Maximum = sliderMax;
                    slider.Minimum = (sliderMax == 1) ? 0 : 1;
                    slider.SmallChange = 0.1;
                    slider.Style = (Style)this.FindResource("StandardSliderStyle");
                    slider.TickFrequency = (sliderMax == 1) ? 0.1 : 1;
                    slider.Value = (float)Application.Current.Resources[id];
                    slider.ValueChanged += ChangeSlider;

                    TextBlock sliderText = new TextBlock();
                    sliderText.Margin = new Thickness(15, 0, 0, 0);
                    sliderText.VerticalAlignment = VerticalAlignment.Center;
                    sliderText.FontSize = 18;
                    sliderText.FontWeight = FontWeights.Bold;
                    sliderText.Foreground = (Brush)Application.Current.Resources["DARKPrimaryDarker"];

                    System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Value");
                    binding.Source = slider;
                    binding.StringFormat = (sliderMax == 1) ? "0.0" : "0";
                    sliderText.SetBinding(TextBlock.TextProperty, binding);

                    Grid.SetRow(slider, 1);
                    Grid.SetColumn(slider, 0);
                    Grid.SetRow(sliderText, 1);
                    Grid.SetColumn(sliderText, 1);
                    sliderGrid.Children.Add(slider);
                    sliderGrid.Children.Add(sliderText);

                    Grid.SetRow(sliderGrid, row);
                    Grid.SetColumn(sliderGrid, col + 2);
                    grid.Children.Add(sliderGrid);
                    break;
                case 4:  // Switch Visibility
                case 5:  // Switch Bold Text
                    Switch toogle = new Switch();
                    toogle.Name = id;
                    toogle.ToggleSize = 14;
                    toogle.ToggleShadowColor = null;
                    toogle.Background = Brushes.Transparent;
                    toogle.ToggleBrush = (Brush)Application.Current.Resources["PrimaryDark"];
                    toogle.CheckedToggleBrush = (Brush)Application.Current.Resources["DARKPrimaryText"];
                    toogle.CheckedBackground = (Brush)Application.Current.Resources["Primary"];
                    toogle.CheckedBorderBrush = (Brush)Application.Current.Resources["Primary"];
                    toogle.BorderBrush = (Brush)Application.Current.Resources["PrimaryDark"];
                    toogle.BoxWidth = 35;
                    toogle.BoxHeight = 18;
                    toogle.Click += (type == 4) ? ChangeSwitchVisibility : ChangeSwitchBold;
                    toogle.IsChecked = (type == 4)
                        ? ((Visibility)Application.Current.Resources[id] == Visibility.Visible ? true : false)
                        : ((FontWeight)Application.Current.Resources[id] == FontWeights.Bold ? true : false);
                    Grid.SetRow(toogle, row);
                    Grid.SetColumn(toogle, col + 2);
                    grid.Children.Add(toogle);
                    break;
                case 6:  // Say Test Message
                    Button button = new Button();
                    button.Content = "TEST".Translate();
                    button.Width = 50;
                    button.Height = 20;
                    button.FontSize = 12;
                    button.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    button.Style = (Style)this.FindResource("ThemeButton");
                    button.Click += SayTestMessage;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col + 2);
                    grid.Children.Add(button);
                    break;
                case 7:  // Thickness and Border
                    Grid thicknessGrid = new Grid();
                    thicknessGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

                    WrapPanel wrapPanel = new WrapPanel();
                    wrapPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    wrapPanel.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(wrapPanel, 0);
                    Grid.SetColumn(wrapPanel, 4);

                    for (int i = 0; i < 4; i++)
                    {
                        thicknessGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                        Slider thicknessSlider = new Slider();
                        thicknessSlider.Name = $"{id}_{i + 1}";
                        thicknessSlider.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        thicknessSlider.VerticalAlignment = VerticalAlignment.Center;
                        thicknessSlider.IsSnapToTickEnabled = true;
                        thicknessSlider.LargeChange = 1;
                        thicknessSlider.Maximum = 250;
                        thicknessSlider.Minimum = 0;
                        thicknessSlider.SmallChange = 1;
                        thicknessSlider.Style = (Style)this.FindResource("StandardSliderStyle");
                        thicknessSlider.TickFrequency = 1;
                        thicknessSlider.Orientation = Orientation.Vertical;
                        thicknessSlider.ValueChanged += ChangeThickness;
                        thicknessSlider.Height = 35;
                        if (id.Contains("cr"))
                            thicknessSlider.Value = (i == 0) ? (float)((CornerRadius)Application.Current.Resources[id]).TopLeft
                                : (i == 1) ? (float)((CornerRadius)Application.Current.Resources[id]).TopRight
                                : (i == 2) ? (float)((CornerRadius)Application.Current.Resources[id]).BottomRight
                                : (float)((CornerRadius)Application.Current.Resources[id]).BottomLeft;
                        else
                            thicknessSlider.Value = (i == 0) ? (float)((Thickness)Application.Current.Resources[id]).Left
                                : (i == 1) ? (float)((Thickness)Application.Current.Resources[id]).Top
                                : (i == 2) ? (float)((Thickness)Application.Current.Resources[id]).Right
                                : (float)((Thickness)Application.Current.Resources[id]).Bottom;

                        Grid.SetRow(thicknessSlider, 0);
                        Grid.SetColumn(thicknessSlider, i);
                        thicknessGrid.Children.Add(thicknessSlider);

                        TextBlock thicknessText2 = new TextBlock();
                        thicknessText2.FontSize = 9;
                        thicknessText2.FontWeight = FontWeights.Bold;
                        thicknessText2.Text = ",";

                        TextBlock thicknessText = new TextBlock();
                        thicknessText.Background = null;
                        thicknessText.FontSize = 9;
                        thicknessText.FontWeight = FontWeights.Bold;
                        thicknessText.Foreground = (Brush)Application.Current.Resources["DARKPrimaryDarker"];

                        System.Windows.Data.Binding thicknessBinding = new System.Windows.Data.Binding("Value");
                        thicknessBinding.Source = thicknessSlider;
                        thicknessBinding.StringFormat = "0";
                        thicknessText.SetBinding(TextBlock.TextProperty, thicknessBinding);
                        this.RegisterName(thicknessSlider.Name, thicknessSlider);

                        wrapPanel.Children.Add(thicknessText);
                        if (i != 3)
                            wrapPanel.Children.Add(thicknessText2);
                    }
                    thicknessGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3.5, GridUnitType.Star) });

                    Grid.SetRow(wrapPanel, 0);
                    Grid.SetColumn(wrapPanel, 4);
                    thicknessGrid.Children.Add(wrapPanel);
                    Grid.SetRow(thicknessGrid, row);
                    Grid.SetColumn(thicknessGrid, col + 2);
                    grid.Children.Add(thicknessGrid);
                    break;
                case 8:  // Image button
                    WrapPanel removeImageWrapPanel = new WrapPanel();
                    removeImageWrapPanel.VerticalAlignment = VerticalAlignment.Center;

                    Button removeImageButton = new Button();
                    removeImageButton.Background = Brushes.Transparent;
                    removeImageButton.Height = 20;
                    removeImageButton.Margin = new Thickness(10, 0, 0, 0);
                    removeImageButton.Click += RemoveImage;
                    removeImageButton.Tag = id;

                    Image removeImageImage = new Image();
                    removeImageImage.ToolTip = "Remove image".Translate();
                    removeImageImage.Width = 12;
                    removeImageImage.Height = 12;
                    removeImageImage.Source = new BitmapImage(new Uri("/VPet.Plugin.ThemeCreator;component/svg/red-trash-solid.png", UriKind.RelativeOrAbsolute));
                    removeImageButton.Content = removeImageImage;

                    Button fileButton = new Button();
                    fileButton.Name = id;
                    fileButton.Content = "Open file".Translate();
                    fileButton.Height = 20;
                    fileButton.FontSize = 12;
                    fileButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    fileButton.Style = (Style)this.FindResource("ThemeButton");
                    fileButton.Click += SelectFile;
                    removeImageWrapPanel.Children.Add(fileButton);
                    removeImageWrapPanel.Children.Add(removeImageButton);

                    Grid.SetRow(removeImageWrapPanel, row);
                    Grid.SetColumn(removeImageWrapPanel, col + 2);
                    grid.Children.Add(removeImageWrapPanel);
                    break;
            }
        }

        private void RemoveImage(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            System.Windows.Application.Current.Resources[button.Tag.ToString()] = null;
            this.main.ApplyResources(button.Tag.ToString());
            this.hasChange = true;
        }

        private void ChangeTheme(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border border = sender as Border;
            this.main.activeTheme = border.Tag.ToString().Substring(10);
            File.WriteAllText($"{this.path}//config.lps", this.main.activeTheme);
            this.main.ChangeTheme(this.main.activeTheme);
            this.main.ApplyResources("All");
            this.main.themeSettings = null;
            this.Close();
        }

        private void SelectFile(object sender, RoutedEventArgs e)
        {
            if (this.disableTriggers) return;
            Button fb = sender as Button;
            Microsoft.Win32.OpenFileDialog fileDiaglog = new Microsoft.Win32.OpenFileDialog();
            fileDiaglog.DefaultExt = ".png";
            fileDiaglog.Filter = $"{"PNG Files (*.png)".Translate()}|*.png";
            Nullable<bool> result = fileDiaglog.ShowDialog();
            if (result == true)
            {
                string filename = fileDiaglog.FileName;
                string base64 = this.main.ConvertImagePathToBase64(filename);
                ImageSource image = this.main.ConvertBase64ToImage(base64);
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = image;

                System.Windows.Application.Current.Resources[fb.Name] = imageBrush;
                this.main.ApplyResources(fb.Name);
                this.hasChange = true;
            }
        }

        private void HoverThemeEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Opacity -= 0.2f;
        }

        private void HoverThemeLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Opacity += 0.2f;
        }

        private void CheckThemeEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Button button = sender as Button;
            this.main.ChangeTheme(button.Tag.ToString().Substring(10));
            this.main.ApplyResources("All");
        }

        private void CheckThemeLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.main.ChangeTheme(this.main.activeTheme);
            this.main.ApplyResources("All");
        }

        private void WindowX_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.mousePosition = e.GetPosition(this);
        }

        private void HideCancel(object sender, SelectionChangedEventArgs e)
        {
            if (this.mainTabControl.SelectedIndex == 0)
                this.cancelColorPicker.Visibility = Visibility.Collapsed;
            else if (this.showCancel)
                this.cancelColorPicker.Visibility = Visibility.Visible;
        }
    }
}
