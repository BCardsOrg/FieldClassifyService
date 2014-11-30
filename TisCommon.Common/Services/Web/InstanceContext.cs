// © 2008 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.ServiceModel;

namespace TiS.Core.TisCommon.Services.Web
{
   public class InstanceContext<T> 
   {
      public InstanceContext Context
      {get;private set;}

      public InstanceContext(T implementation)
      {
         Context = new InstanceContext(implementation);
      }
      public void ReleaseServiceInstance()
      {
         Context.ReleaseServiceInstance();
      }
      public T ServiceInstance
      {
         get
         {
            return (T)Context.GetServiceInstance();
         }
      }
   }
}
