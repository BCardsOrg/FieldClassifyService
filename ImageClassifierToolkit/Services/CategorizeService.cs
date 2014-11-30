using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TiS.Recognition.FieldClassifyService.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services
{
    class CategorizeService
    {
        // Set for each page the ClassName
        // Using a selected field to be the classifier 
        internal static void SetPageClass(IEnumerable<PageData> pages, string fieldName)
        {
            foreach (var pagesSet in pages.GroupBy(x => 
                                {
                                    var field = x.DocData.Fields.FirstOrDefault(y => y.Name == fieldName);
                                    if (field == null || string.IsNullOrEmpty(field.Contents) == true)
                                    {
                                        return "NoClass";
                                    }
                                    else
                                    {
                                        return field.Contents;
                                    }
                                }))
            {
                foreach (var page in pagesSet)
                {
                    page.Setup.ClassName = pagesSet.Key;
                }
            }
        }

        // Split the data into 2 groups so in each group we will have ration% from each page class
        // If mixClasses == true, then we peek ratio pages from each class
        // If mixClasses == false, then we peek the whole class
        internal static IEnumerable<PageData> SplitPerClass(IEnumerable<PageData> pages, double ratio, int minSplitSize, bool mixClasses)
        {
            if (mixClasses == true)
            {
                foreach (var pagesSet in pages.GroupBy(x => x.Setup.ClassName))
                {
                    double size = pagesSet.Count();
                    int splitSize = Math.Max(minSplitSize, (int)(ratio * size));

                    foreach (var page in pagesSet.Take(splitSize))
                    {
                        yield return page;
                    }
                }
            }
            else
            {
                int maxSplitPgaes = Math.Max(minSplitSize, (int)((double)pages.Count() * ratio));
                
                int sizSplitPgaes = 0;

                var pagesSet = pages.GroupBy(x => x.Setup.ClassName)
                                         .OrderByDescending(x => x.Count())
                                         .TakeWhile(x => 
                                         {
                                            if ( sizSplitPgaes >= maxSplitPgaes )
                                            {
                                                return false;
                                            }
                                            else
                                            {
                                                sizSplitPgaes += x.Count();
                                                return true;
                                            }
                                         });

                foreach (var page in pagesSet.SelectMany( x => x ))
                {
                    yield return page;
                }
            }
        }
    }
}
