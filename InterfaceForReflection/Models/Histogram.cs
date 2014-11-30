using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
    /// <summary>
    /// Histogram data create & holder
    /// </summary>
    public class Histograms
    {
        int[] m_values;
        double m_maxValue;
        readonly int Threshold = 2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxElemnts" No of elemnts used for X axis></param>
        /// <param name="maxValue" Max value we can recive for each item></param>
        public Histograms(int maxElemnts, double maxValue)
        {
            m_maxValue = maxValue;
            m_values = new int[maxElemnts];
            for (int i = 0; i < m_values.Length; i++)
            {
                m_values[i] = 0;
            }
        }

        /// <summary>
        ///  Add Item
        /// </summary>
        /// <param name="elemntValue"></param>
        public void Add(double elemntValue)
        {
            int i = (int)Math.Truncate((m_values.Length) * elemntValue / m_maxValue);
            i = Math.Max(0, i);
            i = Math.Min(m_values.Length - 1, i);
            m_values[i] += 1;
        }

        // Get the X where Y is the higher
        public double GetMaxIndex()
        {
            int maxIndex = -1;
            double maxTotal = -1;
            for (int index = 1; index < m_values.Length - 1; index++)
            {
                double total = m_values[index - 1] + m_values[index] * 2 + m_values[index + 1];
                if (total > maxTotal)
                {
                    maxIndex = index;
                    maxTotal = total;
                }
            }
            return (maxIndex * m_maxValue) / (m_values.Length - 1);
        }

        /// <summary>
        /// Return the Histogram graph (value,count)
        /// </summary>
        /// <returns></returns>
        public IDictionary<double, int> GetHistogramGraph()
        {
            var result = new Dictionary<double, int>(m_values.Length);

            for (int i = 0; i < m_values.Length; i++)
            {
                var key = (((double)i + 0.5) * m_maxValue) / m_values.Length;
                if (result.ContainsKey(key) == true)
                {
                    result[key] += m_values[i];
                }
                else
                {
                    result.Add(key, m_values[i]);
                }
            }
            return result;
        }

        public IDictionary<double, int> GetHistogramGraphNormalized()
        {
            var result = new Dictionary<double, int>(m_values.Length);
            double sumValus = (double)m_values.Sum()  / 100;

            for (int i = 0; i < m_values.Length; i++)
            {
                var key = (((double)i + 0.5) * m_maxValue) / m_values.Length;
                if (result.ContainsKey(key) == true)
                {
                    result[key] += m_values[i];
                }
                else
                {
                    result.Add(key, (int)((double)m_values[i] / sumValus) );
                }
            }
          
            return result;
        }

        public double  RemoveEdgeMax()
        {
            IDictionary<double, int> histogramresult = GetHistogramGraphNormalized();
            double NewMax = m_maxValue;

            if (histogramresult.ElementAt(histogramresult.Count - 1).Value > Threshold) return NewMax;
            if (histogramresult.Count() < 2) return NewMax;
           for(int i = histogramresult.Count - 2;i > 0 ;i--)
           {
             
               if (histogramresult.ElementAt(i).Value < Threshold)
               {
                   NewMax = (histogramresult.ElementAt(i).Key + histogramresult.ElementAt(i-1).Key) / 2;
               }
               else
               {
                   break;
               }
            }

           return NewMax;
        }

        internal void Clear()
        {
            for (int index = 0; index < m_values.Length; index++)
            {
                m_values[index] = 0;
            }
        }
    }

}
