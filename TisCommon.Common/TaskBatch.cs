using System;
using System.Collections;

namespace TiS.Core.TisCommon
{
	public interface ITask
	{
		void Perform();
	}
	
	[Serializable]
	public class TaskBatch: AutoHashedObject
	{
		private ArrayList m_oTasks = new ArrayList();

		//
		//	Public methods
		//

		public void AddTask(ITask oTask)
		{
			m_oTasks.Add(oTask);
		}

		public void Clear()
		{
			m_oTasks.Clear();
		}

		public void Perform()
		{
			foreach(ITask oTask in m_oTasks)
			{
				oTask.Perform();
			}
		}

	}
}
