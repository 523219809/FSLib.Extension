namespace System
{
	using IO;

	using Xml.Serialization;

	/// <summary>
	/// XML���л�֧����
	/// </summary>
	public static class FSLib_XmlSerializeExtension
	{
		/// <summary>
		/// ���л������ļ�
		/// </summary>
		/// <param name="objectToSerialize">Ҫ���л��Ķ���</param>
		/// <param name="fileName">���浽��Ŀ���ļ�</param>
		public static void XmlSerilizeToFile(this object objectToSerialize, string fileName)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

			using (var stream = new FileStream(fileName, FileMode.Create))
			{
				objectToSerialize.XmlSerializeToStream(stream);
				stream.Close();
			}
		}

		/// <summary>
		/// ���л�����Ϊ�ı�
		/// </summary>
		/// <param name="objectToSerialize">Ҫ���л��Ķ���</param>
		/// <returns>������Ϣ�� <see cref="T:System.String"/></returns>
		public static string XmlSerializeToString(this object objectToSerialize)
		{
			if (objectToSerialize == null)
				return null;

			using (var ms = objectToSerialize.XmlSerializeToStream())
			{
				ms.Close();
				return Text.Encoding.UTF8.GetString(ms.ToArray());
			}
		}

		/// <summary>
		/// ���л�ָ������Ϊһ���ڴ���
		/// </summary>
		/// <param name="objectToSerialize">Ҫ���л��Ķ���</param>
		/// <returns>�������л���Ϣ�� <see cref="T:System.IO.MemoryStream"/></returns>
		public static MemoryStream XmlSerializeToStream(this object objectToSerialize)
		{
			MemoryStream result;
			if (objectToSerialize == null)
				return null;

			result = new MemoryStream();
			objectToSerialize.XmlSerializeToStream(result);

			return result;
		}

		/// <summary>
		/// ���л�ָ������ָ������
		/// </summary>
		/// <param name="objectToSerialize">Ҫ���л��Ķ���</param>
		/// <param name="stream">Ŀ����</param>
		public static void XmlSerializeToStream(this object objectToSerialize, Stream stream)
		{
			if (objectToSerialize == null || stream == null)
				return;

			var xso = new XmlSerializer(objectToSerialize.GetType());
			xso.Serialize(stream, objectToSerialize);
		}

		/// <summary>
		/// ��ָ�����ַ������ļ��з����л�����
		/// </summary>
		/// <param name="type">Ŀ������</param>
		/// <param name="content">�ļ�·����XML�ı�</param>
		/// <returns>�����л��Ľ��</returns>
		public static object XmlDeserialize(this Type type, string content)
		{
			content = content.Trim();

			if (String.IsNullOrEmpty(content))
				return null;
			if (content[0] == '<')
			{
				using (var ms = new MemoryStream())
				{
					byte[] buffer = Text.Encoding.Unicode.GetBytes(content);
					ms.Write(buffer, 0, buffer.Length);
					ms.Seek(0, SeekOrigin.Begin);

					return ms.XmlDeserialize(type);
				}
			}
			else
			{
				return XmlDeserializeFromFile(content, type);
			}
		}

		/// <summary>
		/// ���ļ��з����л�ָ�����͵Ķ���
		/// </summary>
		/// <param name="objType">�����л��Ķ�������</param>
		/// <param name="fileName">�ļ���</param>
		/// <returns>����</returns>
		public static object XmlDeserializeFromFile(string fileName, System.Type objType)
		{
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				object res = stream.XmlDeserialize(objType);
				stream.Close();
				return res;
			}
		}


		/// <summary>
		/// �����з����л���ָ���������͵Ķ���
		/// </summary>
		/// <param name="objType">��������</param>
		/// <param name="stream">������</param>
		/// <returns>�����н��</returns>
		public static object XmlDeserialize(this Stream stream, System.Type objType)
		{
			var xso = new XmlSerializer(objType);
			object res = xso.Deserialize(stream);

			return res;
		}

		/// <summary>
		/// �����з����л�����
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="stream">������</param>
		/// <returns>�����л����</returns>
		public static T XmlDeserialize<T>(this Stream stream) where T : class
		{
			T res = stream.XmlDeserialize(typeof(T)) as T;

			return res;
		}

		/// <summary>
		/// ���л��ı����ļ�Ϊ����
		/// </summary>
		/// <returns>������Ϣ�� <see cref="T:System.String"/></returns>
		public static T XmlDeserialize<T>(this string content) where T : class
		{
			return (T)typeof(T).XmlDeserialize(content);
		}
	}
}
