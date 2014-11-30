using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.DirectoryServices;
using ActiveDs;

namespace TiS.Core.TisCommon.Network
{
    #region BaseDirectorySearchSession

    internal class BaseDirectorySearchSession
    {
        private ICollection m_searchResults;
        private DirectoryEntry m_directorySearchRoot;
        private string m_directorySearchFilter;
        private IEnumerator m_lastResultEnumerator;

        public BaseDirectorySearchSession(DirectoryEntry searchRoot)
        {
            m_directorySearchRoot = searchRoot;
        }

        public virtual void Search(string searchFilter)
        {
            Filter = searchFilter;

            Search();
        }

        public virtual void Search()
        {
            if (m_searchResults != null)
            {
                m_lastResultEnumerator = null;
                m_searchResults = null;
            }
        }

        public string Filter
        {
            get
            {
                return m_directorySearchFilter;
            }
            set
            {
                SetSearchFilter(value);
            }
        }

        public virtual ICollection LastResults
        {
            get
            {
                return m_searchResults;
            }
        }

        public virtual object NextResult
        {
            get
            {
                if (ResultEnumerator.MoveNext())
                {
                    return ResultEnumerator.Current;
                }
                else
                {
                    ResultEnumerator.Reset();

                    return null;
                }
            }
        }

        protected DirectoryEntry SearchRoot
        {
            get
            {
                return m_directorySearchRoot;
            }
        }

        protected IEnumerator ResultEnumerator
        {
            get
            {
                if (m_lastResultEnumerator == null)
                {
                    m_lastResultEnumerator = LastResults.GetEnumerator();
                }

                return m_lastResultEnumerator;
            }
        }

        protected virtual void SetSearchFilter(string filter)
        {
            m_directorySearchFilter = filter;
        }

        protected virtual void SetSearchResults(ICollection searchResults)
        {
            m_searchResults = searchResults;
        }
    }

    #endregion

    #region DomainDirectorySearchSession

    internal class DomainDirectorySearchSession : BaseDirectorySearchSession
    {
        private DirectorySearcher m_directorySearcher;

        public DomainDirectorySearchSession(DirectoryEntry searchRoot)
            : base(searchRoot)
        {
            m_directorySearcher = new DirectorySearcher(
                SearchRoot, 
                String.Empty, 
                new string[] { DirectoryServicesUtils.DOMAIN_ACCOUNT_NAME_PROPERTY }, 
                SearchScope.Subtree);
        }

        public override void Search()
        {
            base.Search();

            SetSearchResults(m_directorySearcher.FindAll());
        }

        public new SearchResultCollection LastResults
        {
            get
            {
                return (SearchResultCollection)base.LastResults;
            }
        }

        public new SearchResult NextResult
        {
            get
            {
                return (SearchResult)base.NextResult;
            }
        }

        protected override void SetSearchFilter(string filter)
        {
            base.SetSearchFilter(filter);

            m_directorySearcher.Filter = filter;
        }
    }

    #endregion

    #region LocalDirectorySearchSession

    internal class LocalDirectorySearchSession : BaseDirectorySearchSession
    {
        private string m_schemaClassName;
        List<string> m_userNames = new List<string>();

        public LocalDirectorySearchSession(DirectoryEntry searchRoot)
            : base(searchRoot)
        {
        }

        public override void Search()
        {
            List<DirectoryEntry> searchResults = new List<DirectoryEntry>();

            base.Search();

            try
            {
                IADs foundObject;

                foreach (DirectoryEntry child in SearchRoot.Children)
                {
                    if (StringUtil.CompareIgnoreCase(child.SchemaClassName, m_schemaClassName))
                    {
                        foundObject = (IADs)child.NativeObject;

                        if (m_userNames.Count == 0 || 
                            m_userNames.Contains((string)foundObject.Get(DirectoryServicesUtils.LOCAL_ACCOUNT_NAME_PROPERTY)))
                        {
                            searchResults.Add(child);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to perform local directory search. Details : {0}", exc.Message);
            }

            SetSearchResults(searchResults);
        }

        public new List<DirectoryEntry> LastResults
        {
            get
            {
                return (List<DirectoryEntry>) base.LastResults;
            }
        }

        public new DirectoryEntry NextResult
        {
            get
            {
                return (DirectoryEntry)base.NextResult;
            }
        }

        protected override void SetSearchFilter(string filter)
        {
            base.SetSearchFilter(filter);

            ParseSearchFilter();
        }

        private void ParseSearchFilter()
        {
            m_schemaClassName = String.Empty;
            m_userNames.Clear();

            StringTokenizer filterTokenizer = new StringTokenizer(
                new char[] { ';', '=', ')', '(', '&', '|', '!' },
                new char[] { },
                Filter);

            while (filterTokenizer.HasMoreTokens())
            {
                string filterToken = filterTokenizer.GetNextToken();

                if (StringUtil.CompareIgnoreCase(filterToken, "objectclass"))
                {
                    m_schemaClassName = filterTokenizer.GetNextToken();
                }

                if (StringUtil.CompareIgnoreCase(filterToken, DirectoryServicesUtils.DOMAIN_ACCOUNT_NAME_PROPERTY))
                {
                    m_userNames.Add(filterTokenizer.GetNextToken());
                }
            }
        }
    }

    #endregion

}
