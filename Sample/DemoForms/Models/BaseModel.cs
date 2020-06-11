using System;
namespace DemoForms.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public abstract class BaseModel
    {
        public BaseModel()
        {
        }
    }
}
