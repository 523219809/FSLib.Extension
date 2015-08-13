namespace System.FishLib
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;

	/// <summary>
	/// �ļ�����������
	/// </summary>
	/// <remarks>
	/// �������������������Ϊ�˼�һЩ���룬ʵ���Ͽ��ǵ��������������ʱ�����ܻ��������������
	/// �ض���File Directory FileInfo DirectoryInfo Path Drive DriveInfo���������ܸ���
	/// </remarks>
	public static class IOUtility
	{
		/// <summary>
		/// ���ָ��Ŀ¼�µ������ļ����˷������������޷����ʵ��ļ���
		/// </summary>
		/// <param name="path">Ҫ��õ�Ŀ¼</param>
		/// <returns>�����ļ���������</returns>
		public static string[] GetAllFiles(string path)
		{
			if (path == null || path.IsNullOrEmpty())
				throw new ArgumentNullException("path");
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException();

			string[] files, folders;
			try
			{
				files = Directory.GetFiles(path);
				folders = Directory.GetDirectories(path);
			}
			catch (Exception)
			{
				return new string[]
				{
				};
			}

			return files.Union(folders.SelectMany(GetAllFiles)).ToArray();
		}

		/// <summary>
		/// ����û��ǰ׺���ŵ���չ��
		/// </summary>
		/// <param name="path">·��</param>
		/// <returns>��չ��</returns>
		public static string GetExtensionWithoutDot(string path)
		{
			return Path.GetExtension(path).Trim('.');
		}

		/// <summary>
		/// ��������ȫд���ļ�����ָ��·��������ʱ�����Զ�����
		/// </summary>
		/// <param name="path">·��</param>
		/// <param name="data">����</param>
		public static void WriteAllBytes(string path, byte[] data)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllBytes(path, data);
		}

		/// <summary>
		/// ��������ȫд���ļ�����ָ��·��������ʱ�����Զ�����
		/// </summary>
		/// <param name="path">·��</param>
		public static void WriteAllText(string path, string content)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, content);
		}

		/// <summary>
		/// ��������ȫд���ļ�����ָ��·��������ʱ�����Զ�����
		/// </summary>
		/// <param name="path">·��</param>
		public static void WriteAllLines(string path, string[] lines)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllLines(path, lines);
		}

		/// <summary>
		/// ���ָ��Ŀ¼�µ��ļ���������ָ���Ĺ��˹�����й���
		/// </summary>
		/// <param name="path">Ҫ������·��</param>
		/// <param name="searchPattern">���˹���������ʽ��</param>
		/// <param name="option">����ѡ��</param>
		/// <returns><see cref="T:System.Array"/></returns>
		public static string[] GetFiles(string path, string searchPattern, SearchOption option)
		{
			var reg = new Regex(searchPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			return Directory.GetFiles(path, "*.*", option).Where(s => reg.IsMatch(s)).ToArray();
		}


		/// <summary>
		/// �ļ��Ƿ�ֻ��
		/// </summary>
		/// <param name="fullpath"></param>
		/// <returns></returns>
		public static bool FileIsReadOnly(string fullpath)
		{
			FileInfo file = new FileInfo(fullpath);
			if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// �����ļ��Ƿ�ֻ��
		/// </summary>
		/// <param name="fullpath"></param>
		/// <param name="flag">true��ʾֻ������֮</param>
		public static void SetFileReadonly(string fullpath, bool flag)
		{
			FileInfo file = new FileInfo(fullpath);

			if (flag)
			{
				// ���ֻ������
				file.Attributes |= FileAttributes.ReadOnly;
			}
			else
			{
				// �Ƴ�ֻ������
				file.Attributes &= ~FileAttributes.ReadOnly;
			}
		}



		/// <summary>
		/// ȡ�ļ����洢ʱ��
		/// </summary>
		/// <param name="fullpath"></param>
		/// <returns></returns>
		public static DateTime GetLastWriteTime(string fullpath)
		{
			FileInfo fi = new FileInfo(fullpath);
			return fi.LastWriteTime;
		}



		/// <summary>
		/// ����һ��Ŀ¼�Ĵ�С
		/// </summary>
		/// <param name="di">ָ��Ŀ¼</param>
		/// <param name="includeSubDir">�Ƿ������Ŀ¼</param>
		/// <returns></returns>
		public static long CalculateDirectorySize(DirectoryInfo di, bool includeSubDir)
		{
			long totalSize = 0;

			// ������У�ֱ�ӣ��������ļ�
			FileInfo[] files = di.GetFiles();
			foreach (FileInfo file in files)
			{
				totalSize += file.Length;
			}

			// ���������Ŀ¼�����includeSubDir����Ϊtrue
			if (includeSubDir)
			{
				DirectoryInfo[] dirs = di.GetDirectories();
				foreach (DirectoryInfo dir in dirs)
				{
					totalSize += CalculateDirectorySize(dir, includeSubDir);
				}
			}

			return totalSize;
		}



		/// <summary>
		/// ����Ŀ¼��Ŀ��Ŀ¼
		/// </summary>
		/// <param name="source">ԴĿ¼</param>
		/// <param name="destination">Ŀ��Ŀ¼</param>
		public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
		{
			// �������Ŀ¼��ͬ�������븴��
			if (destination.FullName.Equals(source.FullName))
			{
				return;
			}

			// ���Ŀ��Ŀ¼�����ڣ�������
			if (!destination.Exists)
			{
				destination.Create();
			}

			// ���������ļ�
			FileInfo[] files = source.GetFiles();
			foreach (FileInfo file in files)
			{
				// ���ļ����Ƶ�Ŀ��Ŀ¼
				file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
			}

			// ������Ŀ¼
			DirectoryInfo[] dirs = source.GetDirectories();
			foreach (DirectoryInfo dir in dirs)
			{
				string destinationDir = Path.Combine(destination.FullName, dir.Name);

				// �ݹ鴦����Ŀ¼
				CopyDirectory(dir, new DirectoryInfo(destinationDir));
			}
		}


		/// <summary>
		/// �ϲ�����·��
		/// </summary>
		/// <param name="path1">·��1</param>
		/// <param name="path2">·��2</param>
		/// <param name="sourcePathIncludeFileName">ָ��·��1�Ƿ�����ļ��������Ϊtrue����ô���Ƚ��ļ���ȥ����ȡ�ļ��У�</param>
		/// <returns>�ϲ���·��</returns>
		public static string CombinePath(string path1, string path2, bool sourcePathIncludeFileName = false)
		{
			var sep = Path.DirectorySeparatorChar;
			var isWinOsPath = path1[1] == ':';
			var newPath = "";

			if (sourcePathIncludeFileName)
				path1 = Path.GetDirectoryName(path1);

			if (path2.IsNullOrEmpty()) newPath = path1;
			else if (path1.IsNullOrEmpty()) newPath = path2;
			else if (path2[0] == sep)
			{
				if (isWinOsPath)
				{
					newPath = path1[0] + ":" + path2;
				}
				else
					newPath = path2;
			}
			else
			{
				if (path1.Last() == sep) newPath = path1 + path2;
				else newPath = path1 + sep + path2;
			}
			//��ʽ��·��
			var stack = new Stack<string>();
			var arrays = newPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in arrays)
			{
				if (item == ".") continue;
				if (item == "..")
				{
					if (stack.Count > 0) stack.Pop();
					continue;
				}
				stack.Push(item);
			}
			var newPathArray = stack.ToArray();
			Array.Reverse(newPathArray);

			return newPathArray.Join(sep.ToString());
		}

		/// <summary>
		/// �����Ե�ַ
		/// </summary>
		/// <param name="basePath">��ǰ��ַ</param>
		/// <param name="secondPath">Ҫת��Ϊ��Ե�ַ��·��</param>
		/// <returns>��Ե�ַ</returns>
		public static string GetRelativePath(string basePath, string secondPath)
		{
			if (string.IsNullOrEmpty(secondPath) || string.IsNullOrEmpty(basePath) || char.ToLower(basePath[0]) != char.ToLower(secondPath[0]))
				return secondPath;

			var ps1 = basePath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			var ps2 = secondPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

			var upperBound = Math.Min(ps1.Length, ps2.Length);
			var startIndex = Enumerable.Range(0, upperBound).FirstOrDefault(s => string.Compare(ps1[s], ps2[s], true) != 0);

			if (startIndex == 0)
				return secondPath;
			if (ps1.Length == startIndex && ps2.Length <= startIndex)
				return ".\\";

			return string.Join(Path.DirectorySeparatorChar.ToString(), Enumerable.Repeat("..", ps1.Length - startIndex).Concat(ps2.Skip(startIndex)).ToArray());
		}

		/// <summary>
		/// ���������ʾ�Ķ�·��
		/// </summary>
		/// <param name="src"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GetShortDisplayPath(string src, int length)
		{
			if (string.IsNullOrEmpty(src)) return "";

			var fileName = Path.DirectorySeparatorChar + Path.GetFileName(src);
			var directory = Path.GetDirectoryName(src);

			var fileNameLength = fileName.Sum(s => s > 255 ? 2 : 1);
			var restBytesCount = Math.Max(0, length - fileNameLength);
			if (restBytesCount > 0) return directory.GetSubString(restBytesCount, "...") + fileName;
			return fileName;
		}

		/// <summary>
		/// ����������ʽ�����ļ�
		/// </summary>
		/// <param name="path">����Դ·��</param>
		/// <param name="pattern">���˱��ʽ</param>
		/// <param name="includeSubDirectory">�Ƿ�������ļ���</param>
		/// <param name="applyFilerToPath">�Ƿ񽫹��˱��ʽӦ�õ�����·��</param>
		/// <returns></returns>
		public static string[] RegFindFile(string path, string pattern, bool includeSubDirectory = true, bool applyFilerToPath = false)
		{
			var files = System.IO.Directory.GetFiles(path, "*.*", includeSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			pattern = "^" + pattern + "$";

			var query = files.AsQueryable();
			if (applyFilerToPath)
			{
				query = query.Where(s => Regex.IsMatch(s, pattern));
			}
			else
			{
				query = query.Where(s => Regex.IsMatch(System.IO.Path.GetFileName(s), pattern));
			}

			return query.ToArray();
		}
	}
}
