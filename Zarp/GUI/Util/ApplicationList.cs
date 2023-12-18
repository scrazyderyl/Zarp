using System;
using System.Collections;
using System.Collections.Generic;
using Zarp.Common.Cache;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Util
{
    public class ApplicationList : IListCache<ApplicationInfo>
    {
        private static readonly string CommonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs";
        private static readonly string UserStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs";

        private IListCache<ApplicationInfo>[] _ApplicationLists;
        private Dictionary<string, ApplicationInfo> _Applications;

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

        public ApplicationList()
        {
            _ApplicationLists = new IListCache<ApplicationInfo>[] {
                new StartMenuCache(CommonStartMenuPath),
                new StartMenuCache(UserStartMenuPath)
            };
            _Applications = new Dictionary<string, ApplicationInfo>();

            foreach (IListCache<ApplicationInfo> list in _ApplicationLists)
            {
                foreach (ApplicationInfo application in list)
                {
                    _Applications.TryAdd(application.ExecutablePath, application);
                }
            }
        }

        public IEnumerator<ApplicationInfo> GetEnumerator()
        {
            if (!Updated)
            {
                _Applications = new Dictionary<string, ApplicationInfo>();

                foreach (IListCache<ApplicationInfo> list in _ApplicationLists)
                {
                    foreach (ApplicationInfo application in list)
                    {
                        _Applications.TryAdd(application.ExecutablePath, application);
                    }
                }
            }

            return _Applications.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
