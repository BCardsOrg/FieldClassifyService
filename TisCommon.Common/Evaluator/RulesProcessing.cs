using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class Rule
    {
        protected string m_RuleText;

        public Rule(string RuleText)
        {
            m_RuleText = RuleText;
            this.NormalizeRuleText();
        }

        protected void NormalizeRuleText()
        {
            if ((this.Text).Length == 0)
                return;

            String WhitespaceChars = " \t";

            m_RuleText = m_RuleText.Trim(WhitespaceChars.ToCharArray());
        }

        public string Text
        {
            get
            {
                return m_RuleText;
            }
        }

        public override int GetHashCode()
        {
            return m_RuleText.GetHashCode();
        }

        public override bool Equals(object ObjectToCompareTo)
        {

            // 'this' can not be null
            if (ObjectToCompareTo == null)
                return false;

            if (ObjectToCompareTo.GetType() != this.GetType())
                return false;

            Rule OtherObject = (Rule)ObjectToCompareTo;

            if (!((this.Text).Equals(OtherObject.Text)))
                return false;

            return true;
        }

        public override string ToString()
        {
            return this.Text;
        }

        public static bool operator ==(Rule o1, Rule o2)
        {
            return Object.Equals(o1, o2);
        }

        public static bool operator !=(Rule o1, Rule o2)
        {
            return !(o1 == o2);
        }

    }
}
