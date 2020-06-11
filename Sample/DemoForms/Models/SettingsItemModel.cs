using System;
using DemoForms.Enums;
using Xamarin.Forms;

namespace DemoForms.Models
{
    public class SettingsItemModel : BaseModel
    {
        public string Text { get; private set; }
        public string TitleText { get; private set; }
        public Color DetailTextColor { get; private set; }
        public string IconSource { get; private set; }
        public bool IsIconVisible { get; private set; }
        public SettingsType SettingsType { get; private set; }

        public SettingsItemModel(string textValue, string title, Color detailColor, SettingsType settingsType, bool isIconVisible = true)
        {
            Text = textValue;
            TitleText = title;
            DetailTextColor = detailColor;
            IsIconVisible = isIconVisible;
            SettingsType = settingsType;

            IconSource = "forward.png";        }
    }
}
