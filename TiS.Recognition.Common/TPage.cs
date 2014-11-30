using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Storage.ObjectStorage;
using TiS.Core.TisCommon.Storage;
using System.Text.RegularExpressions;


namespace TiS.Recognition.Common
{
    /// <summary>
    /// TPage class
    /// </summary>
    [DataContract]
    [Serializable]
    public class TPage : IOCRData//, ISerializationSurrogate, ISerializable
    {
         // All lines in the page sorted top to bottom
        [DataMember]
        List<TLine> m_oLines;
        // All lines area
        [DataMember]
        TOCRRect m_oRect;
        // The line deskwe angle. 
        // I.e Sin( Angle ) ~= m_iDeskewY / m_iDeskewX  (Where m_iDeskewY is very small related to m_iDeskewX)
        //     & Cos( Angle ) ~= 1
        // so newY = Y - (X * m_iDeskewY) / m_iDeskewX
        [DataMember]
        internal int m_iDeskewX;
        [DataMember]
        internal int m_iDeskewY;
        // The page resolution in DPI
        [DataMember]
        internal int m_iResolution;
        // Extra info...
        // Original Image info 
        // Image width [pixels]
        [DataMember]
        internal int m_iImageWidth;
        // Image height [pixels]
        [DataMember]
        internal int m_iImageHeight;

        //
        // Public methods
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="TPage"/> class.
        /// </summary>
        public TPage()
        {
            m_oLines = new List<TLine>();
            m_oRect = new TOCRRect();
            // Default values...
            m_iDeskewY = 0;
            m_iDeskewX = 4000;
            m_iResolution = 1;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public TPage(SerializationInfo info, StreamingContext context)
        {
            byte[] pageBuffer = info.GetValue("PageBuffer", typeof(byte[])) as byte[];

            // Read the page data from the BinaryReader.
            using (MemoryStream pageMemoryStream = new MemoryStream(pageBuffer))
            using (BinaryReader pageBinaryReader = new BinaryReader(pageMemoryStream, Encoding.Unicode))
            {
                ReadData(pageBinaryReader);

                // Get the lines from the SerializationInfo.
                m_oLines = info.GetValue("Lines", typeof(List<TLine>)) as List<TLine>;
            }
        }
        /// <summary>
        /// Rotate the rectangle to be horizontal
        /// </summary>
        /// <param name="oRect">The o rect.</param>
        /// <returns></returns>
        public TOCRRect Deskew(TOCRRect oRect)
        {
            return new TOCRRect(
                    oRect.Left + (oRect.Top * m_iDeskewY) / m_iDeskewX,
                    oRect.Top - (oRect.Left * m_iDeskewY) / m_iDeskewX,
                    oRect.Width,
                    oRect.Height);
        }

        /// <summary>
        /// Return the distance between X and Y  => Angle = ArcTng(Y/X)
        /// </summary>
        public Size ImageDeskew
        {
            get { return new Size(m_iDeskewX, m_iDeskewY); }
        }

        /// <summary>
        /// Gets the image resolution.
        /// </summary>
        /// <value>
        /// The image resolution.
        /// </value>
        public int ImageResolution
        {
            get { return m_iResolution; }
        }

        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        /// <value>
        /// The size of the image.
        /// </value>
        public Size ImageSize
        {
            get { return new Size(m_iImageWidth, m_iImageHeight); }
        }

        /// <summary>
        /// Sets the words.
        /// </summary>
        /// <param name="oWords">The o words.</param>
        /// <param name="iDeskewX">The i deskew x.</param>
        /// <param name="iDeskewY">The i deskew y.</param>
        /// <param name="iResolution">The i resolution.</param>
        /// <param name="iImageWidth">Width of the i image.</param>
        /// <param name="iImageHeight">Height of the i image.</param>
        public void SetWords(List<TWord> oWords, int iDeskewX, int iDeskewY, int iResolution, int iImageWidth, int iImageHeight)
        {
            m_iDeskewX = iDeskewX;
            m_iDeskewY = iDeskewY;
            m_iResolution = iResolution;
            m_iImageWidth = iImageWidth;
            m_iImageHeight = iImageHeight;
            m_oLines.Clear();
            foreach (TWord oNewWord in oWords)
            {
                AddWord(oNewWord);
            }
            if (Log.MinSeverity <= Log.Severity.DETAILED_DEBUG)
            {
                int iLineIndx = 0;
                foreach (TLine oLine in m_oLines)
                {
                    iLineIndx++;
                    Log.Write(Log.Severity.DETAILED_DEBUG, MethodInfo.GetCurrentMethod(), "Line{0}) {1}", iLineIndx, oLine);
                }
            }
            foreach (TLine line in m_oLines)
            {
                NormalizeHebLines(line);
            }
        }

        /// <summary>
        /// Saves to PRD.
        /// </summary>
        /// <param name="prdPage">The PRD page.</param>
        /// <param name="prdFilePath">The PRD file path.</param>
        static public void SaveToPRD(TPage prdPage, string prdFilePath)
        {
            DataContractSerializer pageSerializer = new DataContractSerializer(typeof(TPage), null, int.MaxValue, false, true, null, null);

            MemoryStream prdMemory = new MemoryStream();
            pageSerializer.WriteObject(prdMemory, prdPage);

            using (FileStream prdFileStream = new FileStream(prdFilePath, FileMode.Create, FileAccess.Write))
            {
                prdMemory.Position = 0;
                byte [] zipData = ZipUtil.ZipArchive(prdMemory, Compression.Medium);
                prdFileStream.Write(zipData, 0, zipData.Length);
            }
        }

        

        private static void ReplaceLineWithNewWords(IList<TWord> iWordList, TChar[] finalTCharString)
        {
            var words = iWordList.Reverse().ToArray();

            TWord[] finalTWords = new TWord[iWordList.Count];
            finalTWords[0] = new TWord();
            finalTWords[0].AddStyle(words[0].Style);
            finalTWords[0].Confidance = words[0].Confidance;
            for (int i = 0, j = 0; i < finalTCharString.Length; i++)
            {
                if (finalTCharString[i].CharData == ' ')
                {
                    iWordList[j] = finalTWords[j];
                    j++;
                    finalTWords[j] = new TWord();
                    finalTWords[j].AddStyle(words[j].Style);
                    finalTWords[j].Confidance = words[j].Confidance;
                }
                else
                {
                    finalTWords[j].AddChar(finalTCharString[i]);
                }
            }
            iWordList[iWordList.Count - 1] = finalTWords[iWordList.Count - 1];

           
        }

        /// <summary>
        /// Normalizes the heb lines.
        /// </summary>
        /// <param name="line">The line.</param>
        public static void NormalizeHebLines(TLine line)
        {
            string lineData = line.ToString();
            if (Regex.IsMatch(lineData, "\\p{IsHebrew}"))
            {
                int[] indexes, lengths, originalIndexes;
                string nbidi = NBidi.NBidi.LogicalToVisual(lineData, out indexes, out lengths);

                originalIndexes = new int[indexes.Length];
                for (int i = 0; i < indexes.Length; i++)
                {
                    originalIndexes[indexes[i]] = i;
                }
                TChar[] finalTCharString = CreateHebTCharString(line, originalIndexes, lineData);
                ReplaceLineWithNewWords(line.Words, finalTCharString);
            }
        }


        /// <summary>
        /// Creates the hebrew character string.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="newIndexes">The new indexes.</param>
        /// <param name="lineData">The line data.</param>
        /// <returns></returns>
        private static TChar[] CreateHebTCharString(TLine line, int[] newIndexes, string lineData)
        {
            TChar[] tempTCharString = new TChar[lineData.Length];

            List<TChar> chrlist = new List<TChar>();
            chrlist.AddRange(line.Words[0].Chars);
            for (int i = 1; i < line.Words.Count; i++)
            {
                chrlist.Add(new TChar(' ', 100, new TOCRRect()));
                chrlist.AddRange(line.Words[i].Chars);
            }

            for (int i = 0; i < chrlist.Count; i++)
            {
                tempTCharString[newIndexes[i]] = chrlist[i];
            }

            return tempTCharString;
        }


        /// <summary>
        /// Loads from PRD.
        /// </summary>
        /// <param name="prdFilePath">The PRD file path.</param>
        /// <returns></returns>
        static public TPage LoadFromPRD(string prdFilePath)
        {
            MemoryStream prdUnzipStream;
            using (FileStream prdFileStream = new FileStream(prdFilePath, FileMode.Open, FileAccess.Read))
            {
                prdUnzipStream = new MemoryStream(ZipUtil.ZipUnarchive(prdFileStream));
            }

            DataContractSerializer pageSerializer = new DataContractSerializer(typeof(TPage), null, int.MaxValue, false, true, null, null);

            TPage prdPage = pageSerializer.ReadObject(prdUnzipStream) as TPage;

            return prdPage;
        }

        //static public TPage LoadFromPRD(string prdFilePath)
        //{
        //    TPage prdPage;

        //    // Get byte array from the prd file.
        //    byte[] prdBytes = File.ReadAllBytes(prdFilePath);

        //    using (MemoryStream memoryStream = new MemoryStream())
        //    using (BinaryReader binaryReader = new BinaryReader(memoryStream))
        //    {
        //        memoryStream.Write(prdBytes, 0, prdBytes.Length);
        //        memoryStream.Seek(0, SeekOrigin.Begin);

        //        // If these bytes are of old prd, Perform the old code.
        //        PrdVersions prdVersion = GetPrdVersion(prdBytes, binaryReader);
        //        if (prdVersion == PrdVersions.Old)
        //        {
        //            string prdFileName = System.IO.Path.GetFileName(prdFilePath);
        //            string prdDirectoryName = System.IO.Path.GetDirectoryName(prdFilePath);

        //            SmartFormatter smartFormatter = new SmartFormatter(true, true);

        //            smartFormatter.Surrogates.SetTypeSurrogate(typeof(TWord), new TWord.WordSerializationSurrogate());
        //            smartFormatter.Surrogates.SetTypeSurrogate(typeof(TLine), new TLine.LineSerializationSurrogate());
        //            smartFormatter.Surrogates.SetTypeSurrogate(typeof(TPage), new TPage());
        //            memoryStream.Seek(0, SeekOrigin.Begin);
        //            prdPage = smartFormatter.Deserialize(memoryStream) as TPage;
        //            //ObjectStorageServices objectStorageServices = new ObjectStorageServices(smartFormatter, Compression.None);

        //            //BLOBStorageImplFS oldBlobStorage = new BLOBStorageImplFS(prdDirectoryName);
        //            //prdPage = objectStorageServices.LoadObject(oldBlobStorage, prdFileName) as TPage;
        //        }
        //        else
        //        {
        //            // If we got here it means we are in new prd. and the memory stream positioned
        //            // after the VersionBytes because the IsOldPrd progressed it.

        //            PRDFormatter binaryFormatter = new PRDFormatter();
        //            SetSurrogates(binaryFormatter.Formatter, prdVersion);

        //            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        //            prdPage = binaryFormatter.Deserialize(memoryStream);
        //            //AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

        //        }
        //    }

        //    return prdPage;
        //}

        /// <summary>
        /// Handles the AssemblyResolve event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName name = new AssemblyName(args.Name);
            name.Version = null;
            return Assembly.Load(name);
        }


        /// <summary>
        /// Loads from raw data.
        /// </summary>
        /// <param name="oRawData">The o raw data.</param>
        /// <returns></returns>
        static public TPage LoadFromRawData(byte[] oRawData)
        {
            string sPRDFileName = System.IO.Path.GetTempFileName();
            string sPRDName = System.IO.Path.GetFileName(sPRDFileName);
            string sPath = System.IO.Path.GetDirectoryName(sPRDFileName);

            // Create PRD file...			
            //			BLOBStorageImplFS oSavePRD = new BLOBStorageImplFS( sPath ) ;
            //			oSavePRD.WriteBLOB( sPath, sPRDName, oRawData ) ;
            System.IO.FileStream oFS = new FileStream(sPRDFileName, FileMode.Create);
            System.IO.BinaryWriter oBW = new BinaryWriter(oFS);
            oBW.Write(oRawData);
            oBW.Close();
            oFS.Close();

            TPage oPage = LoadFromPRD(sPRDFileName);

            System.IO.File.Delete(sPRDFileName);

            return oPage;
        }

        /// <summary>
        /// Ms the mto pixel.
        /// </summary>
        /// <param name="iMMvalue">The i m mvalue.</param>
        /// <returns></returns>
        public int MMtoPixel(int iMMvalue)
        {
            return (m_iResolution * iMMvalue * 10) / 254;
        }

        /// <summary>
        /// Words the specified i line.
        /// </summary>
        /// <param name="iLine">The i line.</param>
        /// <param name="iWord">The i word.</param>
        /// <returns></returns>
        public TWord Word(int iLine, int iWord)
        {
            return ((TLine)m_oLines[iLine]).Word(iWord);
        }



        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="oLine">The o line.</param>
        public void AddLine(TLine oLine)
        {
            m_oLines.Add(oLine);
            m_oLines.Sort(new CompareTLines(this));
            m_oRect.Add(oLine.Rect);
        }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public IList<TLine> Lines
        {
            get
            {
                return m_oLines;
            }
        }

        /// <summary>
        /// Gets the no of lines.
        /// </summary>
        /// <value>
        /// The no of lines.
        /// </value>
        public int NoOfLines
        {
            get
            {
                return m_oLines.Count;
            }
        }

        /// <summary>
        /// Gets the no of words.
        /// </summary>
        /// <value>
        /// The no of words.
        /// </value>
        public int NoOfWords
        {
            get
            {
                int NoOfWords = 0;
                foreach (TLine line in m_oLines) NoOfWords += line.Words.Count;
                return NoOfWords;
            }
        }

        //
        // Private methods
        //

        private class CompareTLines : IComparer<TLine>
        {
            // The page that hold the lines
            TPage m_oPage;
            public CompareTLines(TPage oPage)
            {
                m_oPage = oPage;
            }




            public int Compare(TLine x, TLine y)
            {
                TLine oLine1 = x;
                TLine oLine2 = y;
                if ((oLine1.Words.Count > 0) && (oLine2.Words.Count > 0))
                {
                    TOCRRect oRect1 = m_oPage.Deskew(oLine1.Word(0).Rect);
                    TOCRRect oRect2 = m_oPage.Deskew(oLine2.Word(0).Rect);
                    return oRect1.Top - oRect2.Top;
                }
                else
                {
                    TOCRRect oRect1 = m_oPage.Deskew(oLine1.Rect);
                    TOCRRect oRect2 = m_oPage.Deskew(oLine2.Rect);
                    return oRect1.Top - oRect2.Top;
                }
            }

        }

        private void AddWord(TWord oNewWord)
        {

            // Use the Deskew method to use the same Y-Axis
            TOCRRect oNewWordRect = Deskew(oNewWord.Rect);

            // Build candidate lines
            List<TLine> oCandidateLines = new List<TLine>();
            foreach (TLine oLine in m_oLines)
            {
                if (Deskew(oLine.Rect).Bottom + MMtoPixel(20) < oNewWordRect.Top)
                    continue;

                if (Deskew(oLine.Rect).Top - MMtoPixel(20) > oNewWordRect.Bottom)
                    break;

                oCandidateLines.Add(oLine);
            }

            // Try 1: Find closer matching on top
            foreach (TLine oLine in oCandidateLines)
            {
                foreach (TWord oWord in oLine)
                {
                    TOCRRect oWordRect = Deskew(oWord.Rect);
                    // If the word in 
                    if (oWordRect.IsEqualTop(oNewWordRect))
                    {
                        oLine.AddWord(oNewWord);
                        return;
                    }
                }
            }

            // Now we remove lines that one word is above or beneath the new word
            List<TLine> oCandidateLines2 = new List<TLine>();
            foreach (TLine oLine in oCandidateLines)
            {
                bool bLineOk = true;
                foreach (TWord oWord in oLine)
                {
                    TOCRRect oWordRect = Deskew(oWord.Rect);

                    // Word is above or beneath the new word
                    if ((oNewWordRect.Top > oWordRect.Bottom) || (oNewWordRect.Bottom < oWordRect.Top))
                    {
                        bLineOk = false;
                        break;
                    }
                }
                if (bLineOk)
                    oCandidateLines2.Add(oLine);
            }


            // Try 2: Find closer matching on bottom
            foreach (TLine oLine in oCandidateLines2)
            {
                foreach (TWord oWord in oLine)
                {
                    TOCRRect oWordRect = Deskew(oWord.Rect);
                    // If the word in 
                    if (oWordRect.IsEqualBottom(oNewWordRect))
                    {
                        oLine.AddWord(oNewWord);
                        return;
                    }
                }
            }

            // Try 3: Find far matching from top
            foreach (TLine oLine in oCandidateLines2)
            {
                foreach (TWord oWord in oLine)
                {
                    TOCRRect oWordRect = Deskew(oWord.Rect);
                    if (Math.Abs(oNewWordRect.Top - oWordRect.Top) <=
                            Math.Min(MMtoPixel(2), oWordRect.Height / 2))
                    {
                        oLine.AddWord(oNewWord);
                        return;
                    }
                }
            }
            // Try 4: Find far matching from bottom
            foreach (TLine oLine in oCandidateLines2)
            {
                foreach (TWord oWord in oLine)
                {
                    TOCRRect oWordRect = Deskew(oWord.Rect);

                    if (Math.Abs(oNewWordRect.Bottom - oWordRect.Bottom) <=
                            Math.Min(MMtoPixel(2), oWordRect.Height / 2))
                    {
                        oLine.AddWord(oNewWord);
                        return;
                    }
                }
            }

            // We do not found any line that can take the word, so we create a new line
            TLine oNewLine = new TLine();
            oNewLine.AddWord(oNewWord);
            AddLine(oNewLine);
        }

        /// <summary>
        /// Recieves prd in bytes and MemoryStream and checks by the first bytes is the prd is old or not.
        /// Notice: The Memory stream will be positioned after the version bytes.
        /// </summary>
        private static bool IsOldPrd(byte[] prdBytes, BinaryReader binaryReader)
        {
            byte byte1 = binaryReader.ReadByte();
            byte byte2 = binaryReader.ReadByte();
            byte byte3 = binaryReader.ReadByte();
            byte byte4 = binaryReader.ReadByte();

            if (byte1 == 1 && byte2 == 0 && byte3 == 165 && byte4 >= 4)
            {
                return false;
            }

            return true;
        }

      
        #region IOCRData Members

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data
        {
            get
            {
                StringBuilder data = new StringBuilder();
                foreach (IOCRData line in m_oLines)
                {
                    data.Append(line.Data);
                }
                return data.ToString();
            }
        }

        /// <summary>
        /// Gets the confidence.
        /// </summary>
        /// <value>
        /// The confidence.
        /// </value>
        public int Confidance
        {
            get
            {
                return 100;
            }
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>
        /// The rectangle.
        /// </value>
        public System.Drawing.Rectangle Rectangle
        {
            get { return m_oRect; }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Returns wether the backwords compitability function of SetObjectData is required.
        /// </summary>
        private bool IsOldSetObjectDataRequired(SerializationInfo info)
        {
            if (info.MemberCount != 2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Recieves BinaryWriter, and writes the page data into the binary writer.
        /// </summary>
        private void WriteData(System.IO.BinaryWriter pageBinaryWriter)
        {
            pageBinaryWriter.Write(m_iDeskewY);
            pageBinaryWriter.Write(m_iDeskewX);
            pageBinaryWriter.Write(m_iResolution);
            pageBinaryWriter.Write(m_iImageWidth);
            pageBinaryWriter.Write(m_iImageHeight);

            // Write Rectangle data
            pageBinaryWriter.Write(m_oRect.Top);
            pageBinaryWriter.Write(m_oRect.Height);
            pageBinaryWriter.Write(m_oRect.Left);
            pageBinaryWriter.Write(m_oRect.Width);
        }

        /// <summary>
        /// Recieves BinaryReader and page, and initialize the page from the reader.
        /// </summary>
        public void ReadData(System.IO.BinaryReader binaryReader)
        {
            m_iDeskewY = binaryReader.ReadInt32();
            m_iDeskewX = binaryReader.ReadInt32();
            m_iResolution = binaryReader.ReadInt32();
            m_iImageWidth = binaryReader.ReadInt32();
            m_iImageHeight = binaryReader.ReadInt32();

            // Read Rectangle data
            int rectangleTop = binaryReader.ReadInt32();
            int rectangleHeight = binaryReader.ReadInt32();
            int rectangleLeft = binaryReader.ReadInt32();
            int rectangleWidth = binaryReader.ReadInt32();
            m_oRect = new TOCRRect(rectangleLeft, rectangleTop, rectangleWidth, rectangleHeight);
        }

        #endregion

    }

}
