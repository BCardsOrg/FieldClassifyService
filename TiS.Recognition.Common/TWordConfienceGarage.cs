using System;
using System.Collections.Generic;
using System.Text;

namespace TiS.Recognition.Common
{

    /// <summary>
    /// 
    /// </summary>
    public class TWordConfienceGarage
    {
        // The absolute factor to set the current word confidence
        /// <summary>
        /// The m_i word confi factor
        /// </summary>
        int m_iWordConfiFactor;
        // The absolute factor to set the current char confidence
        /// <summary>
        /// The m_i character confi factor
        /// </summary>
        int m_iCharConfiFactor;
        // The word we are working now
        /// <summary>
        /// The m_o working word
        /// </summary>
        TWord m_oWorkingWord;

        /// <summary>
        /// Initializes a new instance of the <see cref="TWordConfienceGarage"/> class.
        /// </summary>
        /// <param name="iWordConfiFactor">The i word confi factor.</param>
        /// <param name="iCharConfiFactor">The i character confi factor.</param>
        public TWordConfienceGarage(int iWordConfiFactor, int iCharConfiFactor)
        {
            m_iWordConfiFactor = iWordConfiFactor;
            m_iCharConfiFactor = iCharConfiFactor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TWordConfienceGarage"/> class.
        /// </summary>
        public TWordConfienceGarage()
            : this(100, 100)
        { }

        /// <summary>
        /// Sets the factor.
        /// </summary>
        /// <param name="iWordConfiFactor">The i word confi factor.</param>
        /// <param name="iCharConfiFactor">The i character confi factor.</param>
        public void SetFactor(int iWordConfiFactor, int iCharConfiFactor)
        {
            m_iWordConfiFactor = iWordConfiFactor;
            m_iCharConfiFactor = iCharConfiFactor;
        }

        /// <summary>
        /// Sets the word.
        /// </summary>
        /// <param name="oWord">The o word.</param>
        public void SetWord(TWord oWord)
        {
            m_oWorkingWord = oWord;
        }

        /// <summary>
        /// Sets the word confidence.
        /// </summary>
        /// <param name="iWordConfi">The i word confi.</param>
        public void SetWordConfidence(int iWordConfi)
        {
            m_oWorkingWord.m_iConfidance = (short)( ( iWordConfi * m_iWordConfiFactor ) / 100 );
        }

        /// <summary>
        /// Adds the word confidence.
        /// </summary>
        /// <param name="iWordConfi">The i word confi.</param>
        public void AddWordConfidence(int iWordConfi)
        {
            m_oWorkingWord.m_iConfidance += (short)( ( iWordConfi * m_iWordConfiFactor ) / 100 );
        }

        /// <summary>
        /// Sets the character confidence.
        /// </summary>
        /// <param name="iCharIndex">Index of the i character.</param>
        /// <param name="iCandidateIndex">Index of the i candidate.</param>
        /// <param name="iCharConfi">The i character confi.</param>
        /// <param name="bWordOrder">if set to <c>true</c> [b word order].</param>
        public void SetCharConfidence(int iCharIndex, int iCandidateIndex, int iCharConfi, bool bWordOrder)
        {
            TBasicChar oCharCandidate = ( m_oWorkingWord.Chars[iCharIndex] as TChar ).m_oCharCandidate[iCandidateIndex] as TBasicChar;
            oCharCandidate.m_iConfidance = ( iCharConfi * m_iCharConfiFactor ) / 100;
            if ( bWordOrder )
                FixWordOrder();
        }

        /// <summary>
        /// Adds the character confidence.
        /// </summary>
        /// <param name="iCharIndex">Index of the i character.</param>
        /// <param name="iCandidateIndex">Index of the i candidate.</param>
        /// <param name="iCharConfi">The i character confi.</param>
        /// <param name="bWordOrder">if set to <c>true</c> [b word order].</param>
        public void AddCharConfidence(int iCharIndex, int iCandidateIndex, int iCharConfi, bool bWordOrder)
        {
            TBasicChar oCharCandidate = ( m_oWorkingWord.Chars[iCharIndex] as TChar ).m_oCharCandidate[iCandidateIndex] as TBasicChar;
            oCharCandidate.m_iConfidance += ( iCharConfi * m_iCharConfiFactor ) / 100;
            if ( bWordOrder )
                FixWordOrder();
        }

        /// <summary>
        /// Fixes the word order.
        /// </summary>
        public void FixWordOrder()
        {
            m_oWorkingWord.SortCandidates();
        }

    }
}
