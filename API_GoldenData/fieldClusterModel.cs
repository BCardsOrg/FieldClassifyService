using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiS.Recognition.FieldClassifyService.API_GoldenData;
using System.Windows;






namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [Serializable]
    public class fieldClusterModel
    {
        public fieldClusterModel()
        {
            fields = new List<MappedWord>();
            X = 0; Y = 0; Height = 0; Width = 0;
            NumberOfrepitions = 0;
        }

        public Rect Area { get; private set; }

        readonly double WidenessRatio = 0.7;
        double Wideness;
        readonly double MISMATCHChance = 0.17;

        public fieldClusterModel(MappedWord fieldIn)
        {
            fields = new List<MappedWord>();
            Area = Rect.Empty;
            X = 0; Y = 0; Height = 0; Width = 0;
            NumberOfrepitions = 0;
            AddWord(fieldIn);
        }

        private void AddWord(MappedWord fieldIn)
        {
            fields.Add(fieldIn);
            if (Area.IsEmpty == true)
            {
                Area = fieldIn.Rectangle; 
                Wideness = Math.Min( 40,  Area.Height * WidenessRatio );
            }
            else
            {
                var left = Math.Min(Area.Left, fieldIn.Rectangle.Left);
                var top = Math.Min(Area.Top, fieldIn.Rectangle.Top);
                var right = Math.Max(Area.Right, fieldIn.Rectangle.Right);
                var bottom = Math.Max(Area.Bottom, fieldIn.Rectangle.Bottom);
                //Area.Union(fieldIn.Rectangle);
                var rect1 = new Rect(
                    left,
                    top,
                    right - left,
                    bottom - top);
                Area = rect1;
            }
            X = Area.Left - Wideness;
            Y = Area.Top - Wideness;
            Width = Area.Width + 2 * Wideness;
            Height = Area.Height + 2 * Wideness;
        }

        public long ID { get; set; }
        List<MappedWord> fields { get; set; }
        public bool isEmpty { get { return fields.Count() == 0; } }

        double X;
        double Y;
        double Width;
        double Height;

        public List<MappedWord> Fields { get { return fields; } }
        public long NumberOfrepitions { get; set; }

        public void Clear()
        {
            fields.Clear();
        }

        public Rect GetRect
        {
            get
            {
                return new Rect(X, Y, Width, Height);
            }
        }

        private bool AddIfIntersect(fieldClusterModel ClusterCandidate)
        {
            if (Math.Abs(GetHeightAverage - ClusterCandidate.GetHeightAverage) / (GetHeightAverage + ClusterCandidate.GetHeightAverage) > MISMATCHChance) return false;
            if (GetRect.IntersectsWith(ClusterCandidate.GetRect))
            {
                ClusterCandidate.fields.ForEach(a => AddWord(a));
                return true;
            }
            return false;
        }


        private bool AddIfInside(fieldClusterModel ClusterCandidate)
        {
            var parentRect = GetRect;
            if ( parentRect.Contains(ClusterCandidate.Area) )
            {
                ClusterCandidate.fields.ForEach(a => AddWord(a));
                return true;
            }
            return false;
        }

        public bool IncreaseifMatch(fieldClusterModel clusterin)
        {
            if (isClusterMatch(clusterin))
            {
                NumberOfrepitions++;
                return true;
            }
            return false;
        }

        public bool isClusterMatch(fieldClusterModel clusterin)
        {
            return false;// Enumerable.SequenceEqual(Fields.OrderBy(a => a.Name).Select(a => a.Name), clusterin.fields.OrderBy(a => a.Name).Select(a => a.Name));
        }

        public double GetHeightAverage
        {
            get
            {
                return Fields.Select(a => a.Rectangle.Height).Average();
            }
        }


        public double GetMaxHeight
        {
            get
            {
                return Fields.Select(a => a.Rectangle.Height).OrderByDescending(a => a).First();
            }
        }

        public List<FieldClusterLine> lines { get; set; }

        public override string ToString()
        {
            if ( fields.Count == 0)
            {
                return "<empty>";
            }
            else
            {
                return string.Format("'{0}'  ({1})", fields.First().Contents, fields.Count);
            }
        }



        public static List<fieldClusterModel> ClusterListFromDoc(DocumentData doc)
        {
            List<fieldClusterModel> result = new List<fieldClusterModel>();



            doc.Words.ToList().ForEach(a => result.Add(new fieldClusterModel((MappedWord)a)));

            bool change = false;
            do
            {
                change = false;
                for (int i = 0; i < result.Count(); i++)
                {
                    var fieldCluste1 = result.ElementAt(i);
                    if (fieldCluste1.isEmpty == false)
                    {
                        for (int j = 0; j < result.Count(); j++)
                        {
                            if ( i == j )
                            {
                                continue;
                            }

                            var fieldCluste2 = result.ElementAt(j);
                            if (fieldCluste2.isEmpty == false)
                            {
                                if (fieldCluste2.AddIfIntersect(fieldCluste1))
                                {
                                    fieldCluste1.Clear();
                                    change = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                result.RemoveAll(a => a.isEmpty);
            } while (change == true);

            change = false;
            do
            {
                change = false;
                for (int i = 0; i < result.Count(); i++)
                {
                    var fieldCluste1 = result.ElementAt(i);
                    if (fieldCluste1.isEmpty == false)
                    {
                        for (int j = 0; j < result.Count(); j++)
                        {
                            if (i == j)
                            {
                                continue;
                            }

                            var fieldCluste2 = result.ElementAt(j);
                            if (fieldCluste2.isEmpty == false)
                            {
                                if (fieldCluste2.AddIfInside(fieldCluste1))
                                {
                                    fieldCluste1.Clear();
                                    change = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                result.RemoveAll(a => a.isEmpty);
            } while (change == true);

            int index = 0;
            result.ForEach(a => a.ID = index++);

            foreach (MappedWord word in doc.Words)
            {
                word.Cluster = result.Where(a => a.Fields.Contains(word)).FirstOrDefault();
            }

            result.ForEach(a => a.lines = LineEngine.CreateLines(doc, a));


            return result;
        }


    }



    [Serializable]
    public class FieldClusterLine
    {
        public List<MappedWord> Fields { get; set; }
        public int ID { get; set; }
    }
}
