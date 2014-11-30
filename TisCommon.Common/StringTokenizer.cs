using System;
using System.Collections;

namespace TiS.Core.TisCommon
{
	// Used to perform advanced string tokenizing
	// Allows to define DelimTokens parameter - a list of chars
	// that act as delimiters but will be included in the tokens list,
	// For example, for expression "A>2 AND B<3", space can be delimiter char,
	// but '<' and '>' can be delimiter tokens
	public class StringTokenizer
	{
		// Setup data
		private char[] m_Delims;
		private char[] m_DelimTokens;

		// Original string
		private string m_sVal;

		// Chars of tokenized string
		private char[] m_ValChars;

		// Parsing state
		int			m_nTokenParseStartPos = 0;
		int         m_nCurrPos		      = 0;

		public StringTokenizer(
			char[] Delims, 
			char[] DelimTokens, 
			string sVal)		
		{
			// Keep settings
			m_Delims	  = Delims;
			m_DelimTokens = DelimTokens;
			m_sVal		  = sVal;
			
			// Convert provided string to a char array
			m_ValChars = sVal.ToCharArray();

			// Skip delimiters that are not tokens
			SkipDelims();

            SetTokenParseStartPos(m_nCurrPos);
        }

		public string[] GetAllTokens()
		{
			// Declare a temporary collection, since we
			// don't know the number of tokens yet
			ArrayList Tokens = new ArrayList();
			
			// Fill the collection with tokens
			while(HasMoreTokens())
			{
				Tokens.Add(GetNextToken());
			}
			
			// Create an array
			string[] TokensArray = new string[Tokens.Count];
			
			// Fill the array from collection
			Tokens.CopyTo(TokensArray);
			
			// Return the array
			return TokensArray;
		}
		
		public bool HasMoreTokens()
		{
			return HasMoreChars();
		}

		public string GetNextToken()
		{
			string sToken = null;

			// Check that we didn't reach the end already
			if(HasMoreTokens() == false)
			{
				throw new TisException("No more tokens");
			}

			// In case of delimiter that is token
			if(IsDelimToken(m_ValChars[m_nCurrPos]))
			{
				// Advance current position by 1
				m_nCurrPos++;
				
				// Extract token
				sToken = ExtractToken();

				SetTokenParseStartPos(m_nCurrPos);
			}
			else
			{
				// Scan the string untill Delim/DelimToken is found
				while(HasMoreChars() &&
					!IsDelim(m_ValChars[m_nCurrPos]) &&
					!IsDelimToken(m_ValChars[m_nCurrPos]))
				{
					m_nCurrPos++;
				}

				// Extract the token
				sToken = ExtractToken();
					
				// Place the m_nCurrPos to the beginning of next token
				SkipDelims();

				// Set m_nTokenParseStartPos to the current position
				SetTokenParseStartPos(m_nCurrPos);
			}

			return sToken;
		}

		//
		// Private 
		//

		private bool HasMoreChars()
		{
			return (m_nCurrPos < m_ValChars.Length);
		}

		private string ExtractToken()
		{
			string sToken = new string(
				m_ValChars, 
				m_nTokenParseStartPos, 
				m_nCurrPos-m_nTokenParseStartPos);

			return sToken;
		}

		private void SkipDelims()
		{
			// While current char is delimiter and end is not reached
			while( (m_nCurrPos < m_ValChars.Length) &&
				   IsDelim(m_ValChars[m_nCurrPos]) )
			{
				// Advance current position
				m_nCurrPos++;
			}
		}

		private void SetTokenParseStartPos(int nPos)
		{
			m_nTokenParseStartPos = nPos;
		}

		private bool IsDelim(char Ch)
		{
			return (System.Array.IndexOf(m_Delims, Ch) != -1);
		}

		private bool IsDelimToken(char Ch)
		{
			return (System.Array.IndexOf(m_DelimTokens, Ch) != -1);
		}
	}
}
