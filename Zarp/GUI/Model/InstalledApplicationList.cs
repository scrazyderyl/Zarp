using System;
using System.Collections;
using System.Collections.Generic;
using Zarp.Common.Cache;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Model
{
    internal class InstalledApplicationList : IListCache<ApplicationInfo>
    {
        private static string CommonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";
        private static string UserStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs";

        private IListCache<ApplicationInfo>[] _ApplicationLists;
        private HashSet<ApplicationInfo> _Applications;

        public int Count => _ApplicationLists.Length;
        public bool Updated
        {
            get
            {
                foreach (IListCache<ApplicationInfo> list in _ApplicationLists)
                {
                    if (!list.Updated)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public InstalledApplicationList()
        {
            _ApplicationLists = new IListCache<ApplicationInfo>[] {
                new StartMenuCache(CommonStartMenuPath),
                new StartMenuCache(UserStartMenuPath)
            };
            _Applications = new HashSet<ApplicationInfo>();

            foreach (IListCache<ApplicationInfo> list in _ApplicationLists)
            {
                foreach (ApplicationInfo application in list)
                {
                    _Applications.Add(application);
                }
            }
        }

        public IEnumerator<ApplicationInfo> GetEnumerator()
        {
            if (!Updated)
            {
                _Applications = new HashSet<ApplicationInfo>();

                foreach (IListCache<ApplicationInfo> list in _ApplicationLists)
                {
                    foreach (ApplicationInfo application in list)
                    {
                        _Applications.Add(application);
                    }
                }
            }

            return _Applications.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
