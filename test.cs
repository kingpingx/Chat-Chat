using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WpfDemo
{
    class test : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _incomingData = "Client << ";

        private string _sendingData;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public string IncomingData
        {
            get
            {
                return _incomingData;
            }
            set
            {
                if (_incomingData != value)
                {
                    _incomingData += value;
                    OnPropertyChanged("_incomingData");
                }
            }
        }
        public string SendingData
        {
            get
            {
                return _sendingData;
            }
            set
            {
                if (_sendingData != value)
                {
                    _sendingData = value;
                    OnPropertyChanged("_sendingData");
                }
            }
        }
    }
}
