using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;



namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
   public class CheckBoxItem : NotifyPropertyChanged
    {
       Color m_rectColor;
       public Color rectColor
       {
           get
           {
               return m_rectColor;
           }
           set
           {
               OnChange(ref m_rectColor, value, "rectColor");
           }
       }
       public CheckBoxItem(string namein,bool checkedin)
       {
           _name = namein;
           _isChecked = checkedin;
           _numApear = 0;
       }
       private string _name;

       private bool _isChecked;
       public bool IsChecked { 
           get { return _isChecked;}
           set { _isChecked = value; }
       }
       public string Name { get { return _name; } }

       public long _numApear;
       public long NumApear { get { return _numApear; } set { _numApear = value; }       }

       
    

    }
}
