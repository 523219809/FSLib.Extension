namespace System
{
	using IO;

	using Runtime.Serialization.Formatters.Binary;

	/// <summary>
	/// XML���л�֧����
	/// </summary>
	public static class FSLib_BinarySerializeExtension
	{

		/// <summary>
		/// ���л������ļ�
		/// </summary>
		/// <param name="ObjectToSerilize">Ҫ���л��Ķ���</param>
		/// <param name="FileName">���浽���ļ�·��</param>
		public static void SerializeToFile(this object ObjectToSerialize, string FileName)
		{
			if (ObjectToSerialize == null || String.IsNullOrEmpty(FileName))
				return;

			using (FileStream stream = new FileStream(FileName, FileMode.Create))
			{
				SerializeToStream(ObjectToSerialize, stream);
				stream.Close();
			}
		}

		/// <summary>
		/// ���л������ֽ�����
		/// </summary>
		/// <param name="objectToSerialize">Ҫ���л��Ķ���</param>
		/// <returns>���ش�������ֽ�����</returns>
		public static byte[] SerializeToBytes(this object objectToSerialize)
		{
			byte[] result = null;
			if (objectToSerialize == null)
				return result;

			using (var ms = new MemoryStream())
			{
				objectToSerialize.SerializeToStream(ms);
				ms.Close();
				result = ms.ToArray();
			}

			return result;
		}

		/// <summary>
		/// ���л�������
		/// </summary>
		/// <param name="objectToSerialize">Ҫ���л��Ķ���</param>
		/// <param name="stream">���������Ϣ����</param>
		public static void SerializeToStream(this object objectToSerialize, Stream stream)
		{
			if (objectToSerialize == null || stream == null)
				return;

			BinaryFormatter xso = new BinaryFormatter();
			xso.Serialize(stream, objectToSerialize);
		}

		/// <summary>
		/// �����з����л�����
		/// </summary>
		/// <param name="stream">��</param>
		/// <returns>�����л��Ķ���</returns>
		public static object DeserializeFromStream(this Stream stream)
		{
			object result = null;
			if (stream == null)
				return result;

			BinaryFormatter xso = new BinaryFormatter();
			result = xso.Deserialize(stream);

			return result;
		}
	}
}