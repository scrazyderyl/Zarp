using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Zarp.GUI.Util
{
    public struct ItemWithIcon<T>
    {
        public T Data { get; set; }
        public ImageSource? Icon { get; set; }

        public ItemWithIcon(T data, ImageSource? icon)
        {
            Data = data;
            Icon = icon;
        }
    }
}
