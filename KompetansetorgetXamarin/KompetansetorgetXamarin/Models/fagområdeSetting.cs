using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Models
{
    class fagområdeSetting
    {
        public event EventHandler<EventArgs> OnToggled = delegate { };
        public string Name { get; set; }
        public string id { get; set; }
        public bool IsSelected
        {
            get { return _isSelected.Value; }
            set
            {
                if (_isSelected.HasValue == true && _isSelected.Value != value)
                {
                    _isSelected = value;
                    OnToggled(this, new EventArgs());
                }
                else
                {
                    _isSelected = value;
                }
                	System.Diagnostics.Debug.WriteLine ("IsSelected for {0} updated to {1}", Name, value);
            }
        }
        private bool? _isSelected = null;

        public fagområdeSetting(string name, bool isSelected, string id)
        {
            this.Name = name;
            this.IsSelected = isSelected;
            this.id = id;
        }


        //    public fagområdeSetting(bool isOn, string name)
        //    {
        //        this._isOn = isOn;
        //        this._name = name;
        //    }

        //    private string _name { get; set; }
        //    private bool _isOn { get; set; }
    };
}
