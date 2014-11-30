using System;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon
{
    #region TisException
    // Exception that allows providing the message as format + variable-parameters
	[Serializable]
	public class TisException: ApplicationException
	{
		public TisException(
			SerializationInfo info , 
			StreamingContext  context)
			:base(info, context)
		{
			
		}

		public TisException(string sFormat, params object[] Args)
			:base(String.Format(sFormat, Args))
		{
			WriteToLog();
		}

		public TisException(
			Exception oInnerException, 
			string	  sFormat, 
			params	  object[] Args)
			:base(String.Format(sFormat, Args), oInnerException)
		{
			WriteToLog();
		}

		public override void GetObjectData(
			SerializationInfo info , 
			StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public override int GetHashCode()
		{
			return Message.GetHashCode ();
		}

        public static bool IsWFConnectivityException(Exception exc)
        {
            return
                exc is SocketException ||
                exc is System.ServiceModel.FaultException ||
                exc is System.ServiceModel.ServerTooBusyException ||
                exc is System.Data.EntityException ||
                exc is SqlException ||
                (exc.InnerException != null && exc.InnerException is SqlException && (exc.InnerException as SqlException).Number != 2601);
        }
		//
		//	Protected
		//

		protected virtual void WriteToLog()
		{
			Log.WriteException(this);
		}
	}

    #endregion

    #region TisWFIllegalSession

    [Serializable]
    public class TisWFIllegalSession : ApplicationException
    {
        public TisWFIllegalSession()
        {
        }

        public TisWFIllegalSession(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    #endregion
}
