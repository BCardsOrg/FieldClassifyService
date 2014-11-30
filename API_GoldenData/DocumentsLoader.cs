using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading.Tasks.Dataflow;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Xaml;
using TiS.Recognition.Common;


namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    public static class DocumentsLoader
    { 
        public static void GetAllDocuemntsNew(string path, ICollection<DocumentData> listDocuments)
        {
            listDocuments.Clear();
            var CreateFileList = new TransformBlock<string, string[]>(text =>
            {
                Trace.WriteLine("Creating file list...");

                // Remove common punctuation by replacing all non-letter characters  
                // with a space character to. 
                return Directory.GetFiles(path, "*.tif");
            });

            var CreateDocList = new TransformBlock<string[], DocumentData[]>(files =>
            {
                var Documents = new ConcurrentQueue<DocumentData>();
                //for (int i = 0; i < files.Length; i++)
                //{
                //    var file = files[i];

                //if (file != "E:\\Files\\Projects\\FieldClassifier\\Boaz\\UK\\300_invoices_150Vendors\\6E4AE6F0-5359-4115-82A8-2B6EE261AD2C.TIF")
                //        continue;
                
                Parallel.ForEach(files, file =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var dirName = Path.GetDirectoryName(file);

                    //  List<DocumentData> lodedDocs = new List<DocumentData>();
                    int page = 0;

                    BitmapDecoder decoder = BitmapDecoder.Create(File.OpenRead(file), BitmapCreateOptions.DelayCreation | BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);

                    foreach (var dataFile in Directory.GetFiles(dirName, fileName + "_P*.fdGldData"))
                    {
                        long DocIndex = 0;
                        var doc = ParseDataFile(dataFile, DocIndex++);

                        doc.ImageSource = file;
                        if (decoder.Frames.Count() <= page) continue;
                        doc.ImageSize = new Size(decoder.Frames[page].PixelWidth, decoder.Frames[page].PixelHeight);
                        doc.DocumentName = fileName;

                        LoadOcrData(doc, Path.ChangeExtension(dataFile, ".PRD"), true);
                        doc.Clusters = fieldClusterModel.ClusterListFromDoc(doc);
                        setMapping(doc.Words, doc.Lines);
                        //LoadOcrData(doc.Words, doc.Lines, Path.ChangeExtension(dataFile, ".PRD2Text"));


                        doc.PageNumber = page;
                        page++;
                        if (doc.ImageSize.Width > 0 && doc.ImageSize.Height > 0)
                            Documents.Enqueue(doc);
                    }

                });
                //}
                return Documents.ToArray();

            });


            var AfterCreateDocList = new ActionBlock<DocumentData[]>(results =>
            {
                Array.ForEach(results, a => listDocuments.Add(a));
            });

            CreateFileList.Completion.ContinueWith(t =>
            {
                if (t.IsFaulted) ((IDataflowBlock)CreateDocList).Fault(t.Exception);
                else CreateDocList.Complete();
            });

            CreateDocList.Completion.ContinueWith(t =>
            {
                if (t.IsFaulted) ((IDataflowBlock)AfterCreateDocList).Fault(t.Exception);
                else AfterCreateDocList.Complete();
            });


         

            CreateFileList.LinkTo(CreateDocList);
            CreateDocList.LinkTo(AfterCreateDocList);



            CreateFileList.Post(path);

            // Mark the head of the pipeline as complete. The continuation tasks  
            // propagate completion through the pipeline as each part of the  
            // pipeline finishes.
            CreateFileList.Complete();

            // Wait for the last block in the pipeline to process all messages.
            AfterCreateDocList.Completion.Wait();



       
        }
       /* public static void GetAllDocuemnts(string path,ObservableCollection<DocumentData> listDocuments)
        {
          
            Task.Run(async () =>
            {


                await DocumentsLoader.LoadDocumentsAsync(path, listDocuments);

            }).Wait();
        }*/

        public static void GetAllDocuemntPRDTIFOnly(string tiffilename,out DocumentData doc)
        {
          doc = new DocumentData();

        
                //for (int i = 0; i < files.Length; i++)
                //{
                //    var file = files[i];

                //if (file != "E:\\Files\\Projects\\FieldClassifier\\Boaz\\UK\\300_invoices_150Vendors\\6E4AE6F0-5359-4115-82A8-2B6EE261AD2C.TIF")
                //        continue;


                    var fileName = Path.GetFileNameWithoutExtension(tiffilename); 
                    var dirName = Path.GetDirectoryName(tiffilename);

                    string prdFileName = Path.Combine(dirName, fileName +"_P0001.PRD");

                    //  List<DocumentData> lodedDocs = new List<DocumentData>();
                    int page = 0;

                    BitmapDecoder decoder = BitmapDecoder.Create(File.OpenRead(tiffilename), BitmapCreateOptions.DelayCreation | BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);

                 
                        long DocIndex = 0;


                        doc.ImageSource = tiffilename;

                        doc.ImageSize = new Size(decoder.Frames[page].PixelWidth, decoder.Frames[page].PixelHeight);
                        doc.DocumentName = fileName;

                        LoadOcrData(doc, prdFileName, true);
                        doc.Clusters = fieldClusterModel.ClusterListFromDoc(doc);
                        setMapping(doc.Words, doc.Lines);
                        //LoadOcrData(doc.Words, doc.Lines, Path.ChangeExtension(dataFile, ".PRD2Text"));


                        doc.PageNumber = page;
                      
        }

        public static async Task LoadDocumentsAsync(string path, ObservableCollection<DocumentData> listDocuments)
        {

            List<Task<DocumentData>> lodedDocs = new List<Task<DocumentData>>();
            foreach (var file in Directory.GetFiles(path, "*.tif"))
            {
                try
                {
                    var res = await LoadTifData(file);
                    foreach (var item in res)
                    {
                        listDocuments.Add(item);
                        //  progress.Report(item);
                    }
                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
              

            }

        }

        public static async Task<List<DocumentData>> LoadTifData(string file)
        {

            var fileName = Path.GetFileNameWithoutExtension(file);
            var dirName = Path.GetDirectoryName(file);
            return await Task.Factory.StartNew(() =>
            {
                List<DocumentData> lodedDocs = new List<DocumentData>();
                int page = 0;
                BitmapDecoder decoder = BitmapDecoder.Create(File.OpenRead(file), BitmapCreateOptions.DelayCreation | BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
                foreach (var dataFile in Directory.GetFiles(dirName, fileName + "_P*.fdGldData"))
                {
                    long DocIndex = 0;
                    var doc = ParseDataFile(dataFile, DocIndex++);
                 //   LoadOcrData(doc.Words, doc.Lines, Path.ChangeExtension(dataFile, ".PRD2Text"));
                    LoadOcrData(doc, Path.ChangeExtension(dataFile, ".PRD"),true);
                    doc.DocumentName = fileName;
                    doc.ImageSource = file;
                    doc.ImageSize = new Size(decoder.Frames[page].PixelWidth, decoder.Frames[page].PixelHeight);
                    
                    doc.PageNumber = page;
                    page++;
                    lodedDocs.Add(doc);
                }
                return lodedDocs;
            });

        }

        private static void LoadOcrData(DocumentData doc, string ocrFile, bool deskew)
        {
            var page = TPage.LoadFromPRD(ocrFile); //*.prd
          //  var pageNavigation = new TPageNavigation(page);
            var data = doc.Words;
            var lines = doc.Lines;
         
          
           /* if (deskew)
            {
                foreach(FieldData field in doc.Fields)
                {
                    field.OriginRectangle = field.Rectangle;
                    TOCRRect origin = new TOCRRect((int)field.Rectangle.Left,(int)field.Rectangle.Top,(int)field.Rectangle.Width,(int)field.Rectangle.Height);
                    var originSkew = page.Deskew( origin);
                    field.Rectangle = new Rect(originSkew.Left, originSkew.Top, originSkew.Width, originSkew.Height);
                }
            }*/

            List<TOCRRect> wordsRects = new List<TOCRRect>();
            foreach (TLine line in page.Lines)
            {
               
                OcrLine ocrLine = new OcrLine();
               
                ocrLine.Rectangle = new Rect(line.Rect.Left, line.Rect.Top, line.Rect.Width, line.Rect.Height);
                

                foreach (TWord word in line.Words)
                {
                    if ( wordsRects.Any( x => x.Left == word.Rect.Left &&
                                         x.Top == word.Rect.Top &&
                                         x.Right == word.Rect.Right &&
                                         x.Bottom == word.Rect.Bottom) == true)
                    {
                        continue;
                    }
                    else
                    {
                        wordsRects.Add(word.Rect);
                    }
                    // Split word
                    IList<MappedWord> spltWords = SplitWord(word)
                                                    .Select(x =>
                                                        {
                                                            var mapWord = new MappedWord()
                                                            {
                                                                Confidence = x.Confidance,
                                                                Contents = string.Concat(x.Chars.Select(a => a.CharData).ToArray())
                                                            };

                                                            TOCRRect deskewedRect = x.Rect;

                                                            if (deskew)
                                                                deskewedRect = page.Deskew(x.Rect);

                                                            mapWord.Rectangle = new Rect(deskewedRect.Left, deskewedRect.Top, deskewedRect.Width, deskewedRect.Height);
                                                            mapWord.OriginRectangle = new Rect(x.Rect.Left, x.Rect.Top, x.Rect.Width, x.Rect.Height);

                                                            return mapWord;
                                                        })
                                                    .ToList();

                    // Add splited each words to the doc
                    for (int i = 0; i < spltWords.Count; i++)
                    {
                        MappedWord mapWord = spltWords[i];

                        if ( i > 0 )
                        {
                            spltWords[i - 1].SplitRight = mapWord;
                        }
                        if (i + 1 < spltWords.Count)
                        {
                            spltWords[i + 1].SplitLeft = mapWord;
                        }


                        doc.Words.Add(mapWord);
                        mapWord.Line = ocrLine;
                        ocrLine.Words.Add(mapWord);
                    }
                }
                if (ocrLine.Words.Any())
                {
                    lines.Add(ocrLine);
                }

            }

            int indexWord = 0;
            int indexLine = 0;

            foreach (var ocrLine in lines.OrderBy(a => a.Rectangle.Top).ToList())
            {
                ocrLine.ID = indexLine++;
            }
            data = data.OrderBy(a => a.Line.ID).ThenBy(a => a.Rectangle.X).ToList();
            foreach (OcrWord ocrWord in data) ocrWord.ID = indexWord++;
         

        }

        // Split word into 2 words (if necessary)
        private static IEnumerable<TWord> SplitWord(TWord word)
        {
            var colonIdx = word.Data.IndexOf(':');
            if (colonIdx > 0 && colonIdx + 1 < word.Chars.Count)
            {
                yield return CopyFromWord(word, 0, colonIdx + 1);
                yield return CopyFromWord(word, colonIdx + 1, word.Chars.Count);
            }
            else
            {
                yield return word;
            }

        }

        // Copy part of word - return the new word
        private static TWord CopyFromWord(TWord word, int strtIdx, int len)
        {
            var subWord = new TWord();

            for (int i = strtIdx; i < len; i++)
            {
                subWord.AddChar(word.Chars[i].Clone() as TChar);
            }
            subWord.Confidance = word.Confidance;
            subWord.AddStyle( word.Style );

            return subWord;
        }

         static void setMapping (ICollection<MappedWord> data, ICollection<OcrLine> lines)
        {

            foreach (MappedWord word in data)
            {
                word.ClosestToRight = word.Clusterline.Fields.Where(a => a.Rectangle.X > word.Rectangle.X).FirstOrDefault();
                word.ClosestToLeft = word.Clusterline.Fields.Where(a => a.Rectangle.X < word.Rectangle.X).FirstOrDefault();
                FieldClusterLine Downline =word.Cluster.lines.Where(a => a.ID == word.Clusterline.ID + 1).FirstOrDefault();
                if (Downline != null)
               {
                   word.ClosestToBottom = Downline.Fields.Where(a => ((a.Rectangle.X > word.Rectangle.X - word.Rectangle.Width / 2) && (a.Rectangle.Right >  word.Rectangle.X))).FirstOrDefault();
               }

                FieldClusterLine Upline = word.Cluster.lines.Where(a => a.ID == word.Clusterline.ID - 1).FirstOrDefault();
                if (Upline != null)
                {
                    word.ClosestToTop = Upline.Fields.Where(a => (a.Center.X > word.Rectangle.Left && a.Center.X < word.Rectangle.Right)).FirstOrDefault();
                }

            }

        }

        private static void LoadOcrDataOriginal(ICollection<OcrWord> data, ICollection<OcrLine> lines, string ocrFile)
        {

            int skipChars = 0;
            var ocrLine = new OcrLine();

            foreach (var line in File.ReadLines(ocrFile))
            {
                if (skipChars-- > 0)
                    continue;
                var wordLine = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (wordLine.Length < 3)
                {
                    if (line.Contains("Line") == true)
                    {
                        if (ocrLine.Words.Count > 0)
                        {
                            lines.Add(ocrLine);
                            var x = from c in ocrLine.Words
                                    select c.Rectangle;
                            ocrLine.Rectangle = x.Aggregate((accumlator, tested) =>
                            {
                                if (accumlator.IsEmpty == true)
                                {
                                    accumlator = tested;

                                }
                                else
                                    accumlator.Union(tested);
                                return accumlator;
                            });
                        }
                        ocrLine = new OcrLine();
                    }
                    continue;
                }
                var wrd = new OcrWord() { Contents = wordLine[0], Rectangle = GetRect(wordLine[1]), Confidence = int.Parse(wordLine[2], NumberStyles.Integer) };
                data.Add(wrd);
             //   ocrLine.Words.Add(wrd);
                wrd.Line = ocrLine;
                skipChars = wrd.Contents.Length;
            }

        }
        private static DocumentData ParseDataFile(string dataFile,long docIndex)
        {
            var doc = new DocumentData(docIndex);
            var parser = new IniParser(dataFile);
            try
            {
                foreach (var fiels in parser.EnumSection("FieldsData").Where( x => x.Contains('$') == false ))
                {
                    var val = parser.GetSetting("FieldsData", fiels);
                    var fieldData = new FieldData();
                    fieldData.Name = fiels;
                    var valuse = val.Split('|');
                    fieldData.Contents = valuse[0];
                    string rectString, okString;
                    Rect x;
                    if (val.Count(f => f == '|') == 2)
                    {
                        rectString = valuse[1];
                        okString = valuse[2];
                        x = GetRectNew(rectString);
                    }
                    else
                    {
                        if (valuse.Length == 4)
                        {
                            rectString = valuse[1];
                            okString = valuse[2];
                        }
                        else
                        {
                            rectString = valuse[2];
                            okString = valuse[3];
                        }
                        x = GetRect(rectString);
                    }

                    fieldData.Rectangle = x;
                    doc.Fields.Add(fieldData);
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return doc;
        }

        private static Rect GetRect(string valuse)
        {
            Rect x = new Rect();
            if (string.IsNullOrWhiteSpace(valuse) == true)
                return x;

            var rectSring = valuse.Split(',');

            x.X = double.Parse(rectSring[0], CultureInfo.InvariantCulture);
            x.Y = double.Parse(rectSring[1], CultureInfo.InvariantCulture);
            x.Width = double.Parse(rectSring[2], CultureInfo.InvariantCulture) - x.X;
            x.Height = double.Parse(rectSring[3], CultureInfo.InvariantCulture) - x.Y;

            return x;
        }

        private static Rect GetRectNew(string valuse)
        {
            Rect x = new Rect();
            if (string.IsNullOrWhiteSpace(valuse) == true)
                return x;

            var rectSring = valuse.Split(',');

            x.X = double.Parse(rectSring[0], CultureInfo.InvariantCulture);
            x.Y = double.Parse(rectSring[1], CultureInfo.InvariantCulture);
            x.Width = double.Parse(rectSring[2], CultureInfo.InvariantCulture) ;
            x.Height = double.Parse(rectSring[3], CultureInfo.InvariantCulture) ;

            return x;
        }
    }
}
