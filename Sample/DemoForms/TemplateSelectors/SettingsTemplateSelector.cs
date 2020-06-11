using System;
using DemoForms.Models;
using Xamarin.Forms;

namespace DemoForms.TemplateSelectors
{
    public class SettingsTemplateSelector :  DataTemplateSelector
    {
        public DataTemplate StatusTemplate { get; set; }
		public DataTemplate ItemTemplate { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item is StatusModel)
            {
				return StatusTemplate;
			}

			if (item is SettingsItemModel)
            {
				return ItemTemplate;
            }

			throw new Exception($"No template found for this item: {item}. {GetType().Name}");
		}
	}
}
