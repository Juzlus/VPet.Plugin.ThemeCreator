using LinePutScript.Localization.WPF;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using VPet_Simulator.Windows.Interface;
using System.Windows.Controls;
using System.Xml;
using VPet.Plugin.ThemeCreator.ElementResources;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using System.Threading.Tasks;
using Panuon.WPF.UI;
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace VPet.Plugin.ThemeCreator
{
    public class ThemeCreator : MainPlugin
    {
        private bool _initialized = false;

        private string path;
        public string activeTheme;
        public string version = "1.0.1";

        public winSettings themeSettings;
        private BrushConverter brushConverter = new BrushConverter();

        private StyleResources styleResources = new StyleResources();
        private BetterBuyResources betterBuyResources = new BetterBuyResources();
        private SettingsResources settingsResources = new SettingsResources();
        private MessageBarResources messageBarResources = new MessageBarResources();
        private ToolBarResources toolBarResources = new ToolBarResources();

        public override string PluginName => nameof(ThemeCreator);

        public ThemeCreator(IMainWindow mainwin)
          : base(mainwin)
        {
        }

        public override async void LoadPlugin()
        {
            this.path = Directory.GetParent(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
            this.messageBarResources.CreateResources(this.MW);
            this.toolBarResources.CreateResources(this.MW);

            this.activeTheme = this.GetDefaultTheme();
            this.CreateSettingsMenu();
            this.ChangeTheme(this.activeTheme);
            this.ApplyResources("All");

            this.SetUpVPetBetterBuy();
        }

        private async void SetUpVPetBetterBuy()
        {
            if (Application.Current.Windows.Count < 2)
            {
                await Task.Delay(1000);
                this.SetUpVPetBetterBuy();
                return;
            }

            if (Application.Current.Resources["Bb_bg1"] == null)
            {
                this.betterBuyResources.CreateResources(this.MW);
                this.settingsResources.CreateResources(this);
                this.SetUpVPetSettings();
            }

            Window VPetBetterBuy = Application.Current.Windows[2];
            VPetBetterBuy.Background = (Brush)Application.Current.Resources["Bb_bg1"];

            VPetBetterBuy.SetValue(WindowXCaption.BackgroundProperty, (Brush)Application.Current.Resources["Bb_bg2"]);
            VPetBetterBuy.SetValue(WindowXCaption.ForegroundProperty, (Brush)Application.Current.Resources["Bb_fg1"]);
            VPetBetterBuy.SetValue(WindowXCaption.ShadowColorProperty, (Color)Application.Current.Resources["Bb_sh1"]);
            VPetBetterBuy.BorderBrush = (Brush)Application.Current.Resources["Bb_bg3"];
            VPetBetterBuy.BorderThickness = (Thickness)Application.Current.Resources["Bb_bt1"];
            VPetBetterBuy.Opacity = (float)Application.Current.Resources["Bb_o1"];

            Grid firstGrid = FindChild<Grid>(VPetBetterBuy);
            if (firstGrid != null)
            {
                firstGrid.Background = (ImageBrush)Application.Current.Resources["Bb_ib1"];
                Border firstBorder = FindChild<Border>(firstGrid);
                if (firstBorder != null)
                    firstBorder.Background = (Brush)Application.Current.Resources["Bb_bg19"];
            }

            ListBox lsbCategory = (ListBox)VPetBetterBuy.FindName("LsbCategory");
            lsbCategory.Background = (Brush)Application.Current.Resources["Bb_bg4"];
            lsbCategory.Foreground = (Brush)Application.Current.Resources["Bb_bg18"];
            lsbCategory.Margin = (Thickness)Application.Current.Resources["Bb_m1"];
            lsbCategory.SetValue(ListBoxHelper.ItemsCornerRadiusProperty, (CornerRadius)Application.Current.Resources["Bb_cr1"]);
            lsbCategory.SetValue(ListBoxHelper.ItemsSelectedBackgroundProperty, (Brush)Application.Current.Resources["Bb_bg5"]);
            lsbCategory.SetValue(ListBoxHelper.ItemsSelectedForegroundProperty, (Brush)Application.Current.Resources["Bb_bg6"]);
            lsbCategory.SetValue(ListBoxHelper.ItemsHoverBackgroundProperty, (Brush)Application.Current.Resources["Bb_bg15"]);

            ListBox lsbSortRule = (ListBox)VPetBetterBuy.FindName("LsbSortRule");
            lsbSortRule.Background = (Brush)Application.Current.Resources["Bb_bg7"];
            lsbSortRule.Foreground = (Brush)Application.Current.Resources["Bb_bg18"];
            lsbSortRule.SetValue(ListBoxHelper.ItemsCornerRadiusProperty, (CornerRadius)Application.Current.Resources["Bb_cr2"]);
            lsbSortRule.SetValue(ListBoxHelper.ItemsSelectedBackgroundProperty, (Brush)Application.Current.Resources["Bb_bg8"]);
            lsbSortRule.SetValue(ListBoxHelper.ItemsSelectedForegroundProperty, (Brush)Application.Current.Resources["Bb_bg9"]);
            lsbSortRule.SetValue(ListBoxHelper.ItemsHoverBackgroundProperty, (Brush)Application.Current.Resources["Bb_bg16"]);

            ListBox lsbSortAsc = (ListBox)VPetBetterBuy.FindName("LsbSortAsc");
            lsbSortAsc.Background = (Brush)Application.Current.Resources["Bb_bg10"];
            lsbSortAsc.Foreground = (Brush)Application.Current.Resources["Bb_bg18"];
            lsbSortAsc.SetValue(ListBoxHelper.ItemsCornerRadiusProperty, (CornerRadius)Application.Current.Resources["Bb_cr3"]);
            lsbSortAsc.SetValue(ListBoxHelper.ItemsSelectedBackgroundProperty, (Brush)Application.Current.Resources["Bb_bg11"]);
            lsbSortAsc.SetValue(ListBoxHelper.ItemsSelectedForegroundProperty, (Brush)Application.Current.Resources["Bb_bg12"]);
            lsbSortAsc.SetValue(ListBoxHelper.ItemsHoverBackgroundProperty, (Brush)Application.Current.Resources["Bb_bg17"]);

            ItemsControl icCommodity = (ItemsControl)VPetBetterBuy.FindName("IcCommodity");
            icCommodity.Background = (Brush)Application.Current.Resources["Bb_bg13"];
            icCommodity.Visibility = Visibility.Hidden;

            TextBox tbPage = (TextBox)VPetBetterBuy.FindName("TbPage");
            tbPage.Background = (Brush)Application.Current.Resources["Bb_bg4"];
            tbPage.Foreground = (Brush)Application.Current.Resources["Bb_bg18"];

            if (this._initialized) return;
            MenuItem settingsButton = (MenuItem)this.MW.Main.ToolBar.MenuSetting.Items[this.MW.Main.ToolBar.MenuSetting.Items.Count - 1];
            settingsButton.Click += RefreshVPetSettings;

            icCommodity.ItemContainerGenerator.ItemsChanged += async(sender, args) =>
            {
                Grid firstGrid = FindChild<Grid>(VPetBetterBuy);
                if (firstGrid != null)
                {
                    firstGrid.Background = (ImageBrush)Application.Current.Resources["Bb_ib1"];
                    Border firstBorder = FindChild<Border>(firstGrid);
                    if (firstBorder != null)
                        firstBorder.Background = (Brush)Application.Current.Resources["Bb_bg19"];
                }

                icCommodity.Visibility = Visibility.Hidden;
                ItemContainerGenerator itemsControl = (sender as ItemContainerGenerator);
                await Task.Delay(100);
                foreach (var item in itemsControl.Items)
                {
                    ContentPresenter contentPresenter = itemsControl.ContainerFromItem(item) as ContentPresenter;
                    if (contentPresenter == null) continue;
                    Border border = FindChild<Border>(contentPresenter);
                    if (border == null) continue;
                    border.Background = (SolidColorBrush)Application.Current.TryFindResource("Bb_bg14");
                    border.CornerRadius = (CornerRadius)Application.Current.TryFindResource("Bb_cr4");

                    TextBlock textBlock = FindChild<TextBlock>(contentPresenter);
                    if (textBlock != null)
                        textBlock.Foreground = (SolidColorBrush)Application.Current.TryFindResource("SecondaryText");

                    icCommodity.Visibility = Visibility.Visible;
                }
            };
            this._initialized = true;
        }

        private void RefreshVPetSettings(object sender, RoutedEventArgs e)
        {
            MenuItem btn = sender as MenuItem;
            btn.Click -= RefreshVPetSettings;
            this.SetUpVPetSettings();
        }
        
        private void SetUpVPetSettings()
        {
            if (Application.Current.Windows.Count < 2)
                return;

            Window VPetSettings = Application.Current.Windows[1];
            VPetSettings.Background = (Brush)Application.Current.Resources["St_bg1"];

            VPetSettings.SetValue(WindowXCaption.BackgroundProperty, (Brush)Application.Current.Resources["St_bg2"]);
            VPetSettings.SetValue(WindowXCaption.ForegroundProperty, (Brush)Application.Current.Resources["Bb_fg1"]);
            VPetSettings.SetValue(WindowXCaption.ShadowColorProperty, (Color)Application.Current.Resources["St_sh1"]);
            VPetSettings.BorderBrush = (Brush)Application.Current.Resources["St_bg3"];
            VPetSettings.BorderThickness = (Thickness)Application.Current.Resources["St_bt1"];
            VPetSettings.Opacity = (float)Application.Current.Resources["St_o1"];

            ListBox listMenu = (ListBox)VPetSettings.FindName("ListMenu");
            listMenu.Background = (Brush)Application.Current.Resources["St_bg4"];
            listMenu.SetValue(ListBoxHelper.ItemsHoverBackgroundProperty, (Brush)Application.Current.Resources["St_bg5"]);
            listMenu.Foreground = (Brush)Application.Current.Resources["St_fg1"];

            ListBox LBHave = (ListBox)VPetSettings.FindName("LBHave");
            LBHave.Background = Brushes.Transparent;
            LBHave.Foreground = (Brush)Application.Current.Resources["St_fg1"];

            NumberInput numBackupSaveMaxNum = (NumberInput)VPetSettings.FindName("numBackupSaveMaxNum");
            numBackupSaveMaxNum.Background = Brushes.Transparent;
            numBackupSaveMaxNum.BorderBrush = (Brush)Application.Current.Resources["DARKPrimary"];

            DataTemplate dataTemplate = new DataTemplate();
            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.MarginProperty, new Thickness(5, 5, 0, 10));
            borderFactory.SetValue(Border.BackgroundProperty, (Brush)Application.Current.Resources["St_bg6"]);
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(15));
            FrameworkElementFactory contentControlFactory = new FrameworkElementFactory(typeof(ContentControl));
            contentControlFactory.SetValue(ContentControl.MarginProperty, new Thickness(10, 5, 10, 5));
            contentControlFactory.SetBinding(ContentControl.ContentProperty, new Binding("."));
            borderFactory.AppendChild(contentControlFactory);
            dataTemplate.VisualTree = borderFactory;

            TabControl mainTab = (TabControl)VPetSettings.FindName("MainTab");
            mainTab.ContentTemplate = dataTemplate;

            Grid firstGrid = FindChild<Grid>(VPetSettings);
            if (firstGrid != null)
                firstGrid.Background = (ImageBrush)Application.Current.Resources["St_ib1"];
        }

        private void RefreshVPetSettings(object sender)
        {
            this.SetUpVPetSettings();
        }

        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public void ApplyResources(string name)
        {
            switch (name.Substring(0, 2))
            {
                case "Tb":
                    this.toolBarResources.ApplyResources(this.MW);
                    break;
                case "Mb":
                    this.messageBarResources.ApplyResources(this.MW);
                    break;
                case "St":
                    this.SetUpVPetSettings();
                    break;
                case "Bb":
                    this.SetUpVPetBetterBuy();
                    break;
                case "Al":
                    this.toolBarResources.ApplyResources(this.MW);
                    this.messageBarResources.ApplyResources(this.MW);
                    this.SetUpVPetBetterBuy();
                    this.SetUpVPetSettings();
                    break;
            }
            this.styleResources.UpdateResources();
        }

        private string GetDefaultTheme()
        {
            string path = $"{this.path}\\config.lps";
            string line = null;
            try
            {
                line = File.ReadAllText(path);
            } catch { }
            if(line == null) return "Orginal";
            return line;
        }

        private void CreateSettingsMenu()
        {
            MenuItem menuModConfig = this.MW.Main.ToolBar.MenuMODConfig;
            menuModConfig.Visibility = Visibility.Visible;

            MenuItem settingsButton = new MenuItem()
            {
                Header = "Theme Creator".Translate(),
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            settingsButton.Click += (RoutedEventHandler)((s, e) => {
                try
                {
                    themeSettings.Show();
                }
                catch
                {
                    themeSettings = new winSettings(this, this.path);
                    themeSettings.Show();
                }
            });

            menuModConfig.Items.Add(settingsButton);
        }

        public void ChangeTheme(string themeName)
        {
            string configPath = themeName.StartsWith(this.path) ? themeName : $"{this.path}//theme//{themeName}.cfg";
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(configPath))
                if (!File.Exists($"{this.path}//theme//Orginal.cfg"))
                    return;
                else
                    configPath = $"{this.path}//theme//Orginal.cfg";
            xmlDoc.Load(configPath);
            XmlNodeList nodes = xmlDoc.DocumentElement.ChildNodes;
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "Name" || node.Name == "Desc" || node.Name == "ModVersion")
                    continue;
                else if (node.Name == "ShadowColor" || node.Name.Contains("_sh"))
                    Application.Current.Resources[node.Name] = this.GenerateColor(node.InnerText);
                else if (node.Name.Contains("_bt") || node.Name.Contains("_m") || node.Name.Contains("_bbt") || node.Name.Contains("cr"))
                {
                    string[] args = node.InnerText.Split(',');
                    Application.Current.Resources[node.Name] = node.Name.Contains("cr")
                        ? new CornerRadius(double.Parse(args[0]), double.Parse(args[1]), double.Parse(args[2]), double.Parse(args[3]))
                        : new Thickness(double.Parse(args[0]), double.Parse(args[1]), double.Parse(args[2]), double.Parse(args[3]));
                }
                else if (node.Name.Contains("_fs") || node.Name == "Mb_o" || node.Name == "Tb_h1" || node.Name == "Tb_o1" || node.Name == "St_o1" || node.Name == "Bb_o1")
                    Application.Current.Resources[node.Name] = float.Parse(node.InnerText);
                else if (node.Name.Contains("_sv"))
                    Application.Current.Resources[node.Name] = bool.Parse(node.InnerText) ? Visibility.Visible : Visibility.Collapsed;
                else if (node.Name.Contains("_fw"))
                    Application.Current.Resources[node.Name] = bool.Parse(node.InnerText) ? FontWeights.Bold : FontWeights.Normal;
                else if (node.Name.Contains("_ib"))
                {
                    if (string.IsNullOrEmpty(node.InnerText))
                    {
                        Application.Current.Resources[node.Name] = null;
                        continue;
                    }
                    ImageSource image = this.ConvertBase64ToImage(node.InnerText);
                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = image;
                    Application.Current.Resources[node.Name] = imageBrush;
                }
                else
                    Application.Current.Resources[node.Name] = this.GenerateBrush(node.InnerText);
            }
        }

        public void SaveToConfig(string key, int type = -1, string value = null)
        {
            string configPath = $"{this.path}//theme//{this.activeTheme}.cfg";

            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists(configPath))
                xmlDoc.Load(configPath);
            else
            {
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(xmlDeclaration);

                XmlElement root = xmlDoc.CreateElement($"Root");
                xmlDoc.AppendChild(root);
            }

            XmlElement node = xmlDoc.SelectSingleNode($"//{key}") as XmlElement;
            if (node is null)
            {
                node = xmlDoc.CreateElement(key);
                xmlDoc.DocumentElement?.AppendChild(node);
            }
            if (value != null)
                node.InnerText = value;
            else if (type == 0)
                node.InnerText = (Application.Current.Resources[key] as SolidColorBrush).Color.ToString();
            else if (type == 1)
                node.InnerText = (Application.Current.Resources[key]).ToString();
            else if (type == 2)
                node.InnerText = (Visibility)Application.Current.Resources[key] == Visibility.Visible ? "true" : "false";
            else if (type == 3)
                node.InnerText = (FontWeight)Application.Current.Resources[key] == FontWeights.Bold ? "true" : "false";
            else if (type == 4)
            {
                Thickness thickness = (Thickness)Application.Current.Resources[key];
                node.InnerText = $"{thickness.Left},{thickness.Top},{thickness.Right},{thickness.Bottom}";
            }
            else if (type == 5)
            {
                CornerRadius thickness = (CornerRadius)Application.Current.Resources[key];
                node.InnerText = $"{thickness.TopLeft},{thickness.TopRight},{thickness.BottomRight},{thickness.BottomLeft}";
            }
            else if (type == 6)
                node.InnerText = ConvertBitmapSourceToBase64((ImageBrush)Application.Current.Resources[key]);
            try
            {
                xmlDoc.Save(configPath);
            }
            catch { }
        }

        public string GetFromConfig(string themeName, string key)
        {
            string configPath = themeName.StartsWith(this.path) ? themeName : $"{this.path}//theme//{themeName}.cfg";

            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(configPath)) return null;
            xmlDoc.Load(configPath);
            
            XmlElement node = xmlDoc.SelectSingleNode($"//{key}") as XmlElement;
            return (node is null) ? null : node.InnerText;
        }

        private Brush GenerateBrush(string hexColor)
        {
            return (Brush) brushConverter.ConvertFromString(hexColor);
        }

        private Color GenerateColor(string hexColor)
        {
            return (Color) ColorConverter.ConvertFromString(hexColor);
        }

        public ImageSource ConvertBase64ToImage(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }

        public string ConvertImagePathToBase64(string imagePath)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
            catch { return null; }
        }
        
        private string ConvertBitmapSourceToBase64(ImageBrush imageBrush)
        {
            if (imageBrush == null) return "";
            BitmapSource bitmapSource = (BitmapSource)imageBrush.ImageSource;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}