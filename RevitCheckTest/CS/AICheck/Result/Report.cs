using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitLookup.AICheck.Result
{
	class Report
	{
		public static void exportFile(string path, string line)
		{
			FileStream fs = new FileStream(path, FileMode.Append);
			StreamWriter sw = new StreamWriter(fs);
			sw.WriteLine(line);
			sw.Flush();
			sw.Close();
			fs.Close();
		}

		public static void clearFile(string path)
		{
			FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			fs.Seek(0, SeekOrigin.Begin);
			fs.SetLength(0);				//clear file
			fs.Close();
		}

		public static void exportReport(string path)
		{
		}
	}
}
