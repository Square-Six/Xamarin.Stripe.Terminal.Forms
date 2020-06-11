using System;
namespace DemoForms.Models
{
    public class StatusModel : BaseModel
    {
        public string Text { get; private set; }
        public string IconSource { get; private set; }

        public StatusModel(string textValue)
        {
            Text = textValue;
            IconSource = "payment.png";
        }
    }
}
