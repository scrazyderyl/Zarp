using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using Zarp.Common;
using System.Windows.Markup;

namespace Zarp.GUI.Util
{
    public class ApplicationIconCache : FileCache<BitmapSource>
    {
        public ApplicationIconCache() { }

        protected override bool TryFetch(string path, out BitmapSource? data)
        {
            Icon? icon = Icon.ExtractAssociatedIcon(path);
            data = icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return true;
        }
    }
}
