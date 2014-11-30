using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Security;

namespace TiS.Core.TisCommon.Security
{
	#region TisReadOnlyPermissions Members

	[ComVisible(false)]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisReadOnlyPermissions : ITisReadOnlyPermissions
	{
		[IgnoreDataMember]
		private string[] m_Permissions;
 
		public TisReadOnlyPermissions ()
		{
			m_Permissions = EmptyArrays.StringArray;
		}

		public TisReadOnlyPermissions (string[] Permissions)
		{
			m_Permissions = Permissions;
		}

		#region ITisReadOnlyPermissions Members

		public virtual string[] Names
		{
			get
			{
				return m_Permissions;
			}
		}

		public virtual int Count 
		{
			get
			{
				return Names.Length;
			}
		}

		public virtual bool Contains (string sPermission)
		{
			return Array.IndexOf (m_Permissions, sPermission) > -1;
		}

		internal void AddPermissions (string[] Permissions)
		{
			m_Permissions = Permissions;
		}

		#endregion
	}

	#endregion

	#region TisWritablePermissions Members

	[ComVisible(false)]
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisWritablePermissions : TisReadOnlyPermissions, 
		ITisWritablePermissions, IDeserializationCallback
	{
        [DataMember]
        private ListWithEvents<string> m_oPermissions;

		public TisWritablePermissions ()
		{
            m_oPermissions = new ListWithEvents<string>();

            m_oPermissions.ItemRemoved += new EventHandler<EventArgs>(OnItemRemoved);
		}

		#region ITisWritablePermissions Members

		public new void AddPermissions (string[] Permissions)
		{
			m_oPermissions.AddRange (Permissions);
		}

		public void AddPermission (string sPermission)
		{
			m_oPermissions.Add (sPermission);
		}

		public void SetPermissions (string[] Permissions)
		{
			m_oPermissions.Clear ();

			AddPermissions (Permissions);
		}

		public void SetPermission (string sPermission)
		{
			SetPermissions (new string[] {sPermission});
		}

		public void RemovePermissions (string[] Permissions)
		{
			foreach (string sPermission in Permissions)
			{
				m_oPermissions.Remove (sPermission);
			}
		}

		public void RemovePermission (string sPermission)
		{
			RemovePermissions (new string[] {sPermission});
		}

		public void Clear ()
		{
			m_oPermissions.Clear ();
		}

		#endregion

		#region ITisReadOnlyPermissions Members

		public override string[] Names
		{
			get
			{
				return m_oPermissions.ToArray();
			}
		}

		public override bool Contains(string sPermission)
		{
			return m_oPermissions.Contains (sPermission);
		}

		#endregion

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
            m_oPermissions.ItemRemoved += new EventHandler<EventArgs>(OnItemRemoved);
		}

		#endregion

        internal event EventHandler<EventArgs> OnPermissionRemoved;

        private void OnItemRemoved(object sender, EventArgs e)
		{
			if (OnPermissionRemoved != null)
			{
				OnPermissionRemoved (sender, e);
			}
		}
	}

	#endregion
}
