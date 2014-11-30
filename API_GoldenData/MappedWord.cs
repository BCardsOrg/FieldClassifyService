using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TiS.Recognition.FieldClassifyService.API_GoldenData
{
    [Serializable]
    public class MappedWord : OcrWord
    {
        public MappedWord ClosestToRight { get; set; }
        public MappedWord ClosestToTop { get; set; }
        public MappedWord ClosestToLeft { get; set; }
        public MappedWord ClosestToBottom { get; set; }

        public FieldClusterLine Clusterline { get; set; }


        public fieldClusterModel Cluster { get; set; }

        public bool IsSplit
        {
            get
            {
                return SplitLeft != null || SplitRight != null;
            }
        }

        // The left word we split from 
        public MappedWord SplitLeft { get; set; }

        // The right word we split from 
        public MappedWord SplitRight { get; set; }


        public Point Center
        {
            get
            {
                return new Point(Rectangle.X + Rectangle.Width / 2, Rectangle.Y + Rectangle.Height / 2);
            }
        }



    }
}
