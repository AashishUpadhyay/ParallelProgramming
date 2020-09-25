using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelProgramming
{
	class Program
	{
		private static readonly object _lock = new object();
		static void Main(string[] args)
		{
			Synchronous();
			ParallelUsingTaskStartNew();
			ParallelUsingTaskRun();
			ParallelUsingTasks();
			ParallelUsingParallelFor();
			ParallelUsingParallelForEach();
			Console.Read();
		}

		private static void ParallelUsingParallelForEach()
		{
			string total = string.Empty;
			int batchSize = 500000;
			Parallel.ForEach(Enumerable.Range(0,2), (index) =>
			{
				string sum = string.Empty;
				int start = index * batchSize + 1;
				int end = start + batchSize - 1;
				for (int i = start; i <= end; i++)
					lock (_lock)
						sum = Sum(sum, i.ToString());

				total = Sum(total, sum);
			});
			Console.WriteLine(total);
		}

		private static void ParallelUsingParallelFor()
		{
			string total = string.Empty;
			int batchSize = 500000;
			Parallel.For(0, 2, (index) =>
			{
				string sum = string.Empty;
				int start = index * batchSize + 1;
				int end = start + batchSize - 1;
				for (int i = start; i <= end; i++)
					lock (_lock)
						sum = Sum(sum, i.ToString());

				total = Sum(total, sum);
			});
			Console.WriteLine(total);
		}

		private static void ParallelUsingTasks()
		{
			string sumTask = string.Empty;
			var t5 = new Task(() =>
			{
				for (int i = 1; i <= 500000; i++)
					lock (_lock)
						sumTask = Sum(sumTask, i.ToString());
			});

			var t6 = new Task(() =>
			{
				for (int i = 500001; i <= 1000000; i++)
					lock (_lock)
						sumTask = Sum(sumTask, i.ToString());
			});

			t5.Start();
			t6.Start();

			Task.WaitAll(t5, t6);
			Console.WriteLine(sumTask);
		}

		private static void ParallelUsingTaskRun()
		{
			string sumRun = string.Empty;
			var t3 = Task.Run(() =>
			{
				for (int i = 1; i <= 500000; i++)
					lock (_lock)
						sumRun = Sum(sumRun, i.ToString());
			});

			var t4 = Task.Run(() =>
			{
				for (int i = 500001; i <= 1000000; i++)
					lock (_lock)
						sumRun = Sum(sumRun, i.ToString());
			});

			Task.WaitAll(t3, t4);
			Console.WriteLine(sumRun);
		}

		private static void ParallelUsingTaskStartNew()
		{
			string sumStartNew = string.Empty;
			var t1 = Task.Factory.StartNew(() =>
			{
				for (int i = 1; i <= 500000; i++)
					lock (_lock)
						sumStartNew = Sum(sumStartNew, i.ToString());
			});

			var t2 = Task.Factory.StartNew(() =>
			{
				for (int i = 500001; i <= 1000000; i++)
					lock (_lock)
						sumStartNew = Sum(sumStartNew, i.ToString());
			});

			Task.WaitAll(t1, t2);
			Console.WriteLine(sumStartNew);
		}

		private static void Synchronous()
		{
			string sum = string.Empty;
			for (int i = 1; i <= 500000; i++)
				sum = Sum(sum, i.ToString());

			for (int i = 500001; i <= 1000000; i++)
				sum = Sum(sum, i.ToString());

			Console.WriteLine(sum);
		}

		private static string Sum(string int1, string int2)
		{
			StringBuilder sb = new StringBuilder();
			int i = int1.Length - 1;
			int j = int2.Length - 1;
			int carry = 0;
			while (i >= 0 || j >= 0)
			{
				int first = 0;
				int second = 0;

				if (i >= 0)
				{
					first = Convert.ToInt16(int1[i].ToString());
					i--;
				}

				if (j >= 0)
				{
					second = Convert.ToInt16(int2[j].ToString());
					j--;
				}

				int sum = carry + first + second;
				sb.Insert(0, sum % 10);
				carry = sum / 10;
			}

			if (carry > 0)
				sb.Insert(0, carry);
			return sb.ToString();
		}
	}
}
