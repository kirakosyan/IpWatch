using System;
using System.Collections.Generic;
using System.Text;

namespace WatcherApp.ViewModels
{
    public class AddHostWindowViewModel
    {
        public string Host { get; set; }

        public AddHostWindowViewModel()
        {
            Host = "localhost";
        }
    }
}
