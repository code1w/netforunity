using System;
using System.Security.Cryptography;
using System.Text;
namespace KaiGeX.Util
{
	public class PasswordUtil
	{
		public static string MD5Password(string pass)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			byte[] array = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(pass));
			for (int i = 0; i < array.Length; i++)
			{
				byte b = array[i];
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
