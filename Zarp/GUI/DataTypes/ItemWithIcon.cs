using System.Windows.Media;

namespace Zarp.GUI.DataTypes
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
