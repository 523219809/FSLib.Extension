using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.IO;

namespace System
{
	
}

namespace FSLib.IO.SerializeHelper
{
	/// <summary>
	/// ���������л�������
	/// </summary>
	public static class BinarySerializeHelper
	{

		/// <summary>
		/// ���ļ��з����л�����
		/// </summary>
		/// <param name="FileName">�ļ���</param>
		/// <returns>ԭ����</returns>
		public static object DeserializeFromFile(string FileName)
		{
			using (FileStream stream = new FileStream(FileName, FileMode.Open))
			{
				object res = stream.DeserializeFromStream();
				stream.Close();
				return res;
			}
		}

		/// <summary>
		/// ���ֽ������з����л�
		/// </summary>
		/// <param name="array">�ֽ�����</param>
		/// <returns>���л����</returns>
		public static object DeserialzieFromBytes(byte[] array)
		{
			object result = null;
			if (array == null || array.Length == 0)
				return result;

			using (var ms=new System.IO.MemoryStream())
			{
				ms.Write(array, 0, array.Length);
				ms.Seek(0, SeekOrigin.Begin);

				result = ms.DeserializeFromStream();
				ms.Close();
			}

			return result;
		}
	}
}
