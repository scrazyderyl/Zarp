using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Zarp.Common;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Util
{
    public class ApplicationList : ListCache<ItemWithIcon<ApplicationInfo>>
    {
        private static readonly string CommonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";
        private static readonly string UserStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs";

        public bool IsUpdated
        {
            get
            {
                foreach (ListCache<ApplicationInfo> list in _ApplicationLists)
                {
                    if (!list.IsUpdated)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        public int Count => _Count;

        private int _Count;

        private StartMenuCache _SystemStartMenu;
        private StartMenuCache _UserStartMenu;
        private ListCache<ApplicationInfo>[] _ApplicationLists;

        private ApplicationIconCache _IconCache;
        private List<ItemWithIcon<ApplicationInfo>> _Applications;
        private Dictionary<string, ApplicationInfo> _ApplicationLookup;

        public ApplicationList()
        {
            _SystemStartMenu = new StartMenuCache(CommonStartMenuPath);
            _UserStartMenu = new StartMenuCache(UserStartMenuPath);
            _ApplicationLists = new ListCache<ApplicationInfo>[] { _SystemStartMenu, _UserStartMenu };

            _IconCache = new ApplicationIconCache();
            _ApplicationLookup = new Dictionary<string, ApplicationInfo>();

            int count = 0;

            foreach (ListCache<ApplicationInfo> list in _ApplicationLists)
            {
                count += list.Count;
            }

            _Applications = new List<ItemWithIcon<ApplicationInfo>>(count);

            Populate();
        }

        public void Populate()
        {
            foreach (ListCache<ApplicationInfo> list in _ApplicationLists)
            {
                foreach (ApplicationInfo application in list)
                {
                    _IconCache.Get(application.ExecutablePath, out BitmapSource? icon);
                    _Applications.Add(new ItemWithIcon<ApplicationInfo>(application, icon));
                    _ApplicationLookup.TryAdd(application.ExecutablePath, application);
                }
            }
        }

        public void Update()
        {
            var newList = new List<ItemWithIcon<ApplicationInfo>>(_Applications.Count);
            _Count = 0;
            int originalIndex = 0;

            foreach (ListCache<ApplicationInfo> list in _ApplicationLists)
            {
                originalIndex += list.Count;

                if (list.IsUpdated)
                {
                    // Copy updated lists
                    newList.AddRange(_Applications.GetRange(originalIndex, list.Count));
                }
                else
                {
                    // For outdated lists
                    list.Update();

                    foreach (ApplicationInfo application in list)
                    {
                        _IconCache.Get(application.ExecutablePath, out BitmapSource? icon);
                        _Applications.Add(new ItemWithIcon<ApplicationInfo>(application, icon));
                        _ApplicationLookup.TryAdd(application.ExecutablePath, application);
                    }
                }

                _Count += list.Count;
            }

            _Applications = newList;
        }

        public ApplicationInfo? LookupApplication(string path)
        {
            if (_ApplicationLookup.TryGetValue(path, out ApplicationInfo applicationInfo))
            {
                return applicationInfo;
            }

            return null;
        }

        public IEnumerator<ItemWithIcon<ApplicationInfo>> GetEnumerator() => _Applications.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
