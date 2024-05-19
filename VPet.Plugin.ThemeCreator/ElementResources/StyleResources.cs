using System.Windows.Media;
using System.Windows;
using Panuon.WPF.UI;
using System.Windows.Controls;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.ThemeCreator.ElementResources;
internal class StyleResources
{
    public void UpdateResources()
    {
        Style sliderStyle = (Style)Application.Current.Resources["StandardSliderStyle"];
        Style newSliderStyle = new Style { BasedOn = sliderStyle, TargetType = typeof(Slider) };
        foreach (SetterBase setter in sliderStyle.Setters)
            newSliderStyle.Setters.Add(setter);
        newSliderStyle.Setters.Add(new Setter(SliderHelper.ThumbBackgroundProperty, (Brush)Application.Current.Resources["DARKPrimaryText"]));
        Application.Current.Resources["StandardSliderStyle"] = newSliderStyle;

        Style comboBoxStyle = (Style)Application.Current.Resources["StandardComboBoxStyle"];
        Style newComboBoxStyle = new Style { BasedOn = comboBoxStyle, TargetType = typeof(ComboBox) };
        foreach (SetterBase setter in comboBoxStyle.Setters)
            newComboBoxStyle.Setters.Add(setter);
        newComboBoxStyle.Setters.Add(new Setter(ComboBox.BackgroundProperty, Brushes.Transparent));
        newComboBoxStyle.Setters.Add(new Setter(ComboBox.ForegroundProperty, (Brush)Application.Current.Resources["PrimaryText"]));
        Application.Current.Resources["StandardComboBoxStyle"] = newComboBoxStyle;

        Style radioButtonStyle = (Style)Application.Current.Resources["StandardRadioButtonStyle"];
        Style newRadioButtonStyle = new Style { BasedOn = radioButtonStyle, TargetType = typeof(RadioButton) };
        foreach (SetterBase setter in radioButtonStyle.Setters)
            newRadioButtonStyle.Setters.Add(setter);
        newRadioButtonStyle.Setters.Add(new Setter(RadioButton.ForegroundProperty, (Brush)Application.Current.Resources["PrimaryText"]));
        Application.Current.Resources["StandardRadioButtonStyle"] = newRadioButtonStyle;

        Style textBoxStyle = (Style)Application.Current.Resources["StandardTextBoxStyle"];
        Style newTextBoxStyle = new Style { BasedOn = textBoxStyle, TargetType = typeof(TextBox) };
        foreach (SetterBase setter in textBoxStyle.Setters)
            newTextBoxStyle.Setters.Add(setter);
        newTextBoxStyle.Setters.Add(new Setter(TextBox.BackgroundProperty, Brushes.Transparent));
        newTextBoxStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, (Brush)Application.Current.Resources["PrimaryText"]));
        Application.Current.Resources["StandardTextBoxStyle"] = newTextBoxStyle;
    }
}
