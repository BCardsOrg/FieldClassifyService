using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.UserControls.TreeViewHelper
{
	// These 3 classes used for binding node view model to TreeView control

    public interface ITreeNode
    {
        string NodeName { get; set; }
		bool IsSelected { get; set; }
        ObservableCollection<ITreeNode> Children { get; }
    }

    public class TreeNode : NotifyPropertyChanged, ITreeNode
    {
        private bool m_IsSelected;
        private bool m_IsEdit;
        private string m_NodeName;
        private ObservableCollection<ITreeNode> m_Children = new ObservableCollection<ITreeNode>();

        public ObservableCollection<ITreeNode> Children
        {
            get
            {
                return m_Children;
            }
        }
        public string NodeName
        {
            get
            {
                return m_NodeName;
            }
            set
            {
				OnChange(ref m_NodeName, value, "NodeName");
            }
        }
        public bool IsEdit
        {
            get
            {
                return m_IsEdit;
            }
            set
            {
				OnChange(ref m_IsEdit, value, "IsEdit");
			}
        }
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
				OnChange(ref m_IsSelected, value, "IsSelected");
            }
        }

    }

    public class TreeNode<T> : TreeNode
    {
        public T Item { get; set; }
    }
}
