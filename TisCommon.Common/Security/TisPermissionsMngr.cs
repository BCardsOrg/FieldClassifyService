using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Security
{
    #region TisPermissionsMngr

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisPermissionsMngr : IDeserializationCallback
    {
        [DataMember]
        private DictionaryWithEvents<string, TisPermissionsSet> m_oPermissionsSets;

        public TisPermissionsMngr()
        {
            m_oPermissionsSets = new DictionaryWithEvents<string, TisPermissionsSet>();

            m_oPermissionsSets.ItemsCleared += new EventHandler<EventArgs>(OnClearedHandler);
        }

        #region ITisPermissionsMngr Members

        public virtual List<TisPermissionsSet> PermissionsSets
        {
            get
            {
                return new List<TisPermissionsSet>(m_oPermissionsSets.Values);
            }
        }

        public virtual TisPermissionsSet AddPermissionsSet(object oSecuredEntity, string[] Permissions)
        {
            if (oSecuredEntity == null || Permissions.Length == 0)
            {
                return null;
            }

            string sPersistKey = GetPersistKey(oSecuredEntity);
            string sTypedPersistKey = GetTypedPersistKey(oSecuredEntity);

            if (sPersistKey == String.Empty || sTypedPersistKey == String.Empty)
            {
                throw new TisException("Failed to obtain persistence key for [{0}]", oSecuredEntity.ToString());
            }

            return CreatePermissionsSet(sPersistKey, sTypedPersistKey, Permissions);
        }

        public virtual void RemovePermissionsSets(object[] SecuredEntities)
        {
            foreach (object oSecuredEntity in SecuredEntities)
            {
                Remove(GetPersistKey(oSecuredEntity), GetTypedPersistKey(oSecuredEntity));
            }
        }

        public virtual void RemovePermissionsSets(TisPermissionsSet[] PermissionsSets)
        {
            foreach (TisPermissionsSet oPermissionsSet in PermissionsSets)
            {
                Remove(oPermissionsSet.PersistKey, oPermissionsSet.TypedPersistKey);
            }
        }

        public virtual bool ContainsPermissionsSet(object oSecuredEntity)
        {
            return ContainsPermissionsSet(GetPersistKey(oSecuredEntity));
        }

        public virtual bool ContainsPermissionsSet(TisPermissionsSet oPermission)
        {
            return m_oPermissionsSets.ContainsValue(oPermission);
        }

        public virtual bool ContainsPermissionsSet(string sPersistKey)
        {
            return m_oPermissionsSets.ContainsKey(sPersistKey);
        }

        public virtual TisPermissionsSet GetByPersistKey(object oEntity)
        {
            if (oEntity == null)
            {
                return null;
            }

            if (oEntity.GetType() == typeof(string))
            {
                return GetByPersistKey((string)oEntity);
            }
            else
            {
                return GetByPersistKey(GetPersistKey(oEntity));
            }
        }

        public virtual TisPermissionsSet GetByPersistKey(string sPersistKey)
        {
            if (sPersistKey == String.Empty)
            {
                return null;
            }

            return (TisPermissionsSet)m_oPermissionsSets[sPersistKey];
        }

        public virtual string GetPersistKey(object oSecuredEntity)
        {
            if (oSecuredEntity is IPersistKeyProvider)
            {
                return (oSecuredEntity as IPersistKeyProvider).FullPersistKey;
            }
            else
            {
                return String.Empty;
            }
        }

        public virtual string GetTypedPersistKey(object oSecuredEntity)
        {
            if (oSecuredEntity is IPersistKeyProvider)
            {
                return (oSecuredEntity as IPersistKeyProvider).TypedPersistKey;
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region IDeserializationCallback Members

        public virtual void OnDeserialization(object sender)
        {
            //			m_oPermissionsSets.OnDeserialization (sender);

            foreach (TisPermissionsSet oPermissionsSet in PermissionsSets)
            {
                oPermissionsSet.OnEmpty += new PermissionsSetDelegate(OnPermissionsSetEmptyHandler);
                oPermissionsSet.OnChanged += new PermissionsSetDelegate(OnPermissionsSetChangedHandler);
            }
        }

        #endregion

        public event PermissionsSetDelegate OnPermissionsSetChanged;
        public event PermissionsSetDelegate OnPermissionsSetRemoved;
        public event EventHandler<EventArgs> OnCleared;

        internal void ReplacePersistKey(string sOldPersistKey, string sNewPersistKey)
        {
            if (sOldPersistKey != sNewPersistKey && m_oPermissionsSets.ContainsKey(sOldPersistKey))
            {
                TisPermissionsSet oPermissionsSet = (TisPermissionsSet)m_oPermissionsSets[sOldPersistKey];

                oPermissionsSet.ChangePersistKey(sNewPersistKey);

                m_oPermissionsSets.Add(sNewPersistKey, oPermissionsSet);

                m_oPermissionsSets.Remove(sOldPersistKey);
            }
        }

        internal void Remove(string sPersistKey, string sTypedPersistKey)
        {
            m_oPermissionsSets.Remove(sPersistKey);

            if (OnPermissionsSetRemoved != null)
            {
                OnPermissionsSetRemoved(new PermissionsSetArgs(sPersistKey, sTypedPersistKey));
            }
        }

        internal void Clear()
        {
            m_oPermissionsSets.Clear();
        }

        protected virtual TisPermissionsSet CreatePermissionsSetInstance(string sPersistKey,
                                                                          string sTypedPersistKey)
        {
            return new TisPermissionsSet(sPersistKey, sTypedPersistKey);
        }

        protected virtual TisPermissionsSet CreatePermissionsSet(
            string sPersistKey,
            string sTypedPersistKey,
            string[] Permissions)
        {
            TisPermissionsSet oPermissionsSet = GetByPersistKey(sPersistKey);

            if (oPermissionsSet == null)
            {
                oPermissionsSet = CreatePermissionsSetInstance(
                    sPersistKey,
                    sTypedPersistKey);

                oPermissionsSet.OnEmpty += new PermissionsSetDelegate(OnPermissionsSetEmptyHandler);
                oPermissionsSet.OnChanged += new PermissionsSetDelegate(OnPermissionsSetChangedHandler);

                m_oPermissionsSets.Add(sPersistKey, oPermissionsSet);
            }

            oPermissionsSet.AddPermissions(Permissions);

            return oPermissionsSet;
        }

        private void OnPermissionsSetChangedHandler(PermissionsSetArgs e)
        {
            if (OnPermissionsSetChanged != null)
            {
                OnPermissionsSetChanged(e);
            }
        }

        private void OnPermissionsSetEmptyHandler(PermissionsSetArgs e)
        {
            Remove(e.PersistKey, e.TypedPersistKey);
        }

        void m_oPermissionsSets_ItemsCleared(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnClearedHandler(object sender, EventArgs e)
        {
            if (OnCleared != null)
            {
                OnCleared(sender, e);
            }
        }

        private bool PermissionsSetFilter(object oElement)
        {
            return oElement is TisPermissionsSet;
        }
    }

    #endregion

    #region TisDefinedPermissionsMngr

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisDefinedPermissionsMngr : TisPermissionsMngr, IDeserializationCallback
    {
        public TisDefinedPermissionsMngr()
        {
        }

        public new List<TisDefinedPermissionsSet> PermissionsSets
        {
            get
            {
                List<TisPermissionsSet> PermissionsSets = base.PermissionsSets;

                TisDefinedPermissionsSet[] DefinedPermissionsSets =
                    (TisDefinedPermissionsSet[])Array.CreateInstance(
                         typeof(TisDefinedPermissionsSet), PermissionsSets.Count);

                PermissionsSets.CopyTo(DefinedPermissionsSets, 0);

                return new List<TisDefinedPermissionsSet>(DefinedPermissionsSets);
            }
        }

        public new TisDefinedPermissionsSet GetByPersistKey(object oEntity)
        {
            return (TisDefinedPermissionsSet)base.GetByPersistKey(oEntity);
        }

        protected override TisPermissionsSet CreatePermissionsSetInstance(
            string sPersistKey,
            string sTypedPersistKey)
        {
            return new TisDefinedPermissionsSet(sPersistKey, sTypedPersistKey);
        }

        #region IDeserializationCallback Members

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
        }

        #endregion
    }

    #endregion

    #region TisSupportedPermissionsMngr

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisSupportedPermissionsMngr : TisPermissionsMngr, IDeserializationCallback
    {
        public TisSupportedPermissionsMngr()
        {
        }

        public override string GetPersistKey(object oSecuredEntity)
        {
            string sTypedPersistKey = GetTypedPersistKey(oSecuredEntity);

            TisSupportedPermissionsSet oSupportedPermissionsSet =
                (TisSupportedPermissionsSet)GetByPersistKey(sTypedPersistKey);

            if (oSupportedPermissionsSet == null || oSupportedPermissionsSet.BuiltInPermissions.Count == 0)
            {
                AttributeExplorer oAttributeExplorer = new AttributeExplorer();

                string[] BuiltInPermissions = AttributeExplorer.ObtainDeclaredPermissions(oSecuredEntity);

                if (BuiltInPermissions.Length > 0)
                {
                    if (oSupportedPermissionsSet == null)
                    {
                        oSupportedPermissionsSet = (TisSupportedPermissionsSet)CreatePermissionsSet(
                            sTypedPersistKey,
                            sTypedPersistKey,
                            EmptyArrays.StringArray);
                    }

                    if (oSupportedPermissionsSet != null)
                    {
                        oSupportedPermissionsSet.SetBuiltInPermissions(BuiltInPermissions);
                    }
                }
            }

            return sTypedPersistKey;
        }

        public new TisSupportedPermissionsSet GetByPersistKey(object oEntity)
        {
            return (TisSupportedPermissionsSet)base.GetByPersistKey(oEntity);
        }

        public new List<ITisSupportedPermissionsSet> PermissionsSets
        {
            get
            {
                List<TisPermissionsSet> PermissionsSets = base.PermissionsSets;

                TisSupportedPermissionsSet[] SupportedPermissionsSets =
                    (TisSupportedPermissionsSet[])Array.CreateInstance(
                           typeof(TisSupportedPermissionsSet), PermissionsSets.Count);

                PermissionsSets.CopyTo(SupportedPermissionsSets, 0);

                return new List<ITisSupportedPermissionsSet>(SupportedPermissionsSets);
            }
        }

        #region IDeserializationCallback Members

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
        }

        #endregion

        protected override TisPermissionsSet CreatePermissionsSetInstance(
            string sPersistKey,
            string sTypedPersistKey)
        {
            return new TisSupportedPermissionsSet(sPersistKey, sTypedPersistKey);
        }
    }

    #endregion

    #region PermissionsSetArgs

    [ComVisible(false)]
    public class PermissionsSetArgs : EventArgs
    {
        private string m_sPersistKey;
        private string m_sTypedPersistKey;

        public PermissionsSetArgs(string sPersistKey, string sTypedPersistKey)
        {
            m_sPersistKey = sPersistKey;
            m_sTypedPersistKey = sTypedPersistKey;
        }

        public string PersistKey
        {
            get
            {
                return m_sPersistKey;
            }
        }

        public string TypedPersistKey
        {
            get
            {
                return m_sTypedPersistKey;
            }
        }
    }

    #endregion
}
