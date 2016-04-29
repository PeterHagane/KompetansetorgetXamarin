
using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using KompetansetorgetXamarin.Views;
using KompetansetorgetXamarin.Controls;

namespace KompetansetorgetXamarin.Models
{
    class ModelSwitchBindings : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        private string _comment;
        private Color _color;
        private bool _isOn;

        public string name      { get { return _name; }     set { OnPropertyChanged(); _name = value; } }
        public string comment   { get { return _comment; }  set { OnPropertyChanged(); _comment = value; } }
        public Color color      { get { return _color; }    set { OnPropertyChanged(); _color = value; } }
        public bool isOn        { get { return _isOn; }     set { OnPropertyChanged(); OnPropertyChanged("isNotOn"); _isOn = value; } }
        public bool isNotOn     { get { return !_isOn; }    }

        //set { OnPropertyChanged(); _name = "top kek"; }
        public void change()
        {
            System.Diagnostics.Debug.WriteLine("Changed to" + isOn + "!!!");
        }

    public ModelSwitchBindings()
        {
            this.isOn = false;
            this.name = "";
            this.color = Color.Blue;
            this.comment = "";
        }
        public ModelSwitchBindings(bool isOn, string name, Color color, string comment)
        {
            this.isOn = isOn;
            this.name = name;
            this.color = color;
            this.comment = comment;
        }
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
                System.Diagnostics.Debug.WriteLine(name + propertyName);
            }
            
        }
    }
}
