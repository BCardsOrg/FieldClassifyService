using System;
using System.Collections;
using System.Runtime.Serialization;
using TiS.Core.TisCommon;

namespace TiS.Core.TisCommon.Security
{
    #region TisPermissionsSet Members

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    [KnownType(typeof(TisDefinedPermissionsSet))]
    [KnownType(typeof(TisSupportedPermissionsSet))]
    public class TisPermissionsSet
    {
        public TisPermissionsSet()
        {
        }

        public TisPermissionsSet(string sPersistKey, string sTypedPersistKey)
        {
            PersistKey = sPersistKey;
            TypedPersistKey = sTypedPersistKey;
        }

        #region ITisPermissionsSet Members

        [DataMember]
        public virtual string PersistKey { get; private set; }
        [DataMember]
        public virtual string TypedPersistKey { get; private set; }

        #endregion

        public event PermissionsSetDelegate OnChanged;
        public event PermissionsSetDelegate OnEmpty;

        public virtual void AddPermissions(string[] Permissions)
        {
        }

        internal void ChangePersistKey(string sNewPersistKey)
        {
            PersistKey = sNewPersistKey;
        }

        protected void FireOnEmptyEvent()
        {
            if (OnEmpty != null)
            {
                OnEmpty(new PermissionsSetArgs(PersistKey, TypedPersistKey));
            }
        }

        protected void FireOnChangedEvent()
        {
            if (OnChanged != null)
            {
                OnChanged(new PermissionsSetArgs(PersistKey, TypedPersistKey));
            }
        }
    }

    #endregion

    #region TisDefinedPermissionsSet Members

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisDefinedPermissionsSet : TisPermissionsSet,
        ITisDefinedPermissionsSet, IDeserializationCallback
    {
        [DataMember]
        private TisWritablePermissions m_oDefinedPermissions;

        public TisDefinedPermissionsSet()
        {
            m_oDefinedPermissions = new TisWritablePermissions();
        }

        public TisDefinedPermissionsSet(
            string sPersistKey,
            string sTypedPersistKey) :
            base(sPersistKey,
                 sTypedPersistKey)
        {
            m_oDefinedPermissions = new TisWritablePermissions();

            m_oDefinedPermissions.OnPermissionRemoved += new EventHandler<EventArgs>(OnPermissionRemovedHandler);
        }

        #region ITisDefinedPermissionsSet Members

        public ITisWritablePermissions DefinedPermissions
        {
            get
            {
                return m_oDefinedPermissions;
            }
        }

        public virtual bool ContainsPermission(string sPermission)
        {
            return m_oDefinedPermissions.Contains(sPermission);
        }

        #endregion

        public override void AddPermissions(string[] Permissions)
        {
            m_oDefinedPermissions.AddPermissions(Permissions);
        }

        #region IDeserializationCallback Members

        public virtual void OnDeserialization(object sender)
        {
            m_oDefinedPermissions.OnPermissionRemoved += new EventHandler<EventArgs>(OnPermissionRemovedHandler);
        }

        #endregion

        protected virtual void OnPermissionRemovedHandler(object sender, EventArgs e)
        {
            if (m_oDefinedPermissions.Names.Length == 0)
            {
                FireOnEmptyEvent();
            }
            else
            {
                FireOnChangedEvent();
            }
        }
    }

    #endregion

    #region TisSupportedPermissionsSet Members

    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisSupportedPermissionsSet : TisDefinedPermissionsSet,
        ITisSupportedPermissionsSet, IDeserializationCallback
    {
        [IgnoreDataMember]
        private TisReadOnlyPermissions m_oBuiltInPermissions;

        public TisSupportedPermissionsSet()
        {
        }

        public TisSupportedPermissionsSet(
            string sPersistKey,
            string sTypedPersistKey) :
            base(String.Empty, sTypedPersistKey)
        {
        }

        #region ITisSupportedPermissionsSet Members

        public ITisReadOnlyPermissions AllPermissions
        {
            get
            {
                return new TisReadOnlyPermissions(
                    (string[])ArrayBuilder.ArrayAddElements(
                    BuiltInPermissions.Names, DefinedPermissions.Names, typeof(string)));
            }
        }

        public ITisReadOnlyPermissions BuiltInPermissions
        {
            get
            {
                if (m_oBuiltInPermissions == null)
                {
                    m_oBuiltInPermissions = new TisReadOnlyPermissions();
                }

                return m_oBuiltInPermissions;
            }
        }

        public override bool ContainsPermission(string sPermission)
        {
            return DefinedPermissions.Contains(sPermission) ||
                   BuiltInPermissions.Contains(sPermission);
        }

        protected override void OnPermissionRemovedHandler(object sender, EventArgs e)
        {
            if (DefinedPermissions.Names.Length == 0 && BuiltInPermissions.Names.Length == 0)
            {
                FireOnEmptyEvent();
            }
            else
            {
                FireOnChangedEvent();
            }
        }

        #endregion

        #region IDeserializationCallback Members

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
        }

        #endregion

        public override void AddPermissions(string[] Permissions)
        {
            foreach (string sPermission in Permissions)
            {
                if (!BuiltInPermissions.Contains(sPermission))
                {
                    DefinedPermissions.AddPermission(sPermission);
                }
            }
        }

        public void SetBuiltInPermissions(string[] builtInPermissions)
        {
            ((TisReadOnlyPermissions)BuiltInPermissions).AddPermissions(builtInPermissions);
        }
    }

    #endregion
}
