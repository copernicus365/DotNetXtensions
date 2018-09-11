using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;

namespace DotNetXtensions.Net
{
	/// <summary>
	/// Extension methods for Networking.
	/// </summary>
	public static class XNet
	{

		#region Send

		/// <summary>
		/// Sends MailMessages with the specified settings.
		/// </summary>
		/// <param name="email">A MailMessage instance.</param>
		/// <param name="host">The Smtp host.</param>
		/// <param name="port">The Smtp port.  The int was made a nullable type so 
		/// that null can (and must) be entered when the user did not indicate a port.</param>
		/// <param name="credentialOpts">A Credentials enumeration value
		/// indicating the user's Credentials selection.</param>
		/// <param name="useDefaultCredentials">This value is only checked if 
		/// Credentials.DefaultCredentials is set, in which case smtp.UseDefaultCredentials
		/// is set to this value.  Nonetheless, for clarity, enter false when another Credentials 
		/// enumeration is used.</param>
		/// <param name="networkCredentials">The NetworkCredentials instance param passed in,
		/// or null if none was passed in.</param>
		/// <param name="userName">The userName string value passed in.  This will only
		/// be checked / used if Credentials.UNamePswd is selected. Nonetheless, for clarity, 
		/// enter null when another Credentials enumeration is used.</param>
		/// <param name="psswd">The psswd string value passed in.  This will only
		/// be checked / used if Credentials.UNamePswd is selected. Nonetheless, for clarity, 
		/// enter null when another Credentials enumeration is used.</param>
		/// <param name="sendAsync">The value indicating if the message should be sent 
		/// asynchronously.</param>
		private static void __SendMessageSmtp(
  this MailMessage email, string host, int? port, Credentials credentialOpts, bool useDefaultCredentials,
  NetworkCredential networkCredentials, string userName, string psswd, bool sendAsync)
		{
			if (email == null) throw new ArgumentNullException("email");
			if (host == null) throw new ArgumentNullException("host");

			SmtpClient smtp = new SmtpClient();
			smtp.Host = host;

			if (port != null)
				smtp.Port = (int)port;

			// Smtp.Credentials can be set to null, so a NetworkCredential instance param can 
			// be set to null by the user; also, new NetworkCredential(null, null) is also valid.  
			// As such, we must allow such options rather than throwing ArgumentNullExceptions.
			// One result of this is that we cannot decide what Credential option the user chose by 
			// checking for null, which led us to the enumeration route (with the benefit of its pretty 
			// switch statement).
			switch (credentialOpts)
			{
				case Credentials.None:
					break;
				case Credentials.CredentialInstance:
					smtp.Credentials = networkCredentials;
					break;
				case Credentials.UNamePswd:
					smtp.Credentials = new NetworkCredential(userName, psswd);
					break;
				// The user could validly have entered useDefaultCredentials = false; so we 
				// still have to take useDefaultCredentials in as a param (the enum option only
				// tells us if this overloaded option was chosen) and set it to smtp.UseDefaultCredentials.
				case Credentials.DefaultCredentials:
					smtp.UseDefaultCredentials = useDefaultCredentials;
					break;
			}
			if (sendAsync)
				smtp.SendAsync(email, null);
			else
				smtp.Send(email);
		}

		/// <summary>
		/// Credential options for SMTP sent messages.
		/// </summary>
		private enum Credentials
		{
			/// <summary>
			/// No credentials were set.
			/// </summary>
			None,
			/// <summary>
			/// Set Credentials to a NetworkCredential instance param.
			/// </summary>
			CredentialInstance,
			/// <summary>
			/// Set Credentials to a new NetworkCredential with the given userNm and pswd.
			/// </summary>
			UNamePswd,
			/// <summary>
			/// Set Smtp.UseDefaultCredentials to the specified boolean param.
			/// </summary>
			DefaultCredentials
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send().
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		public static void Send(this MailMessage email, string host)
		{
			__SendMessageSmtp(
				 email, host, null, Credentials.None, false, null, null, null, false);
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send().
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		public static void Send(this MailMessage email, string host, int port)
		{
			__SendMessageSmtp(
				 email, host, port, Credentials.None, false, null, null, null, false);
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send() with 
		/// the specified network credentials.
		/// <example><code>
		/// <![CDATA[
		/// MailMessage email = new MailMessage("from@yahoo.com", "to@hotmail.com", "(subject)", "(body)");
		/// // set other wanted MailMessage settings (e.g. attachments, etc).
		/// 
		/// email.Send("plus.smtp.mail.yahoo.com", "from@yahoo.com", "pswd");
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="userName">The user name for the NetworkCredential to set for this transaction.</param>
		/// <param name="psswd">The password for the NetworkCredential to set for this transaction.</param>
		public static void Send(this MailMessage email, string host, string userName, string psswd)
		{
			__SendMessageSmtp(
				 email, host, null, Credentials.UNamePswd, false, null, userName, psswd, false);
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send() with 
		/// the specified network credentials.
		/// <example><code>
		/// <![CDATA[
		/// MailMessage email = new MailMessage("from@yahoo.com", "to@hotmail.com", "(subject)", "(body)");
		/// // set other wanted MailMessage settings (e.g. attachments, etc).
		/// 
		/// email.Send("plus.smtp.mail.yahoo.com", 88, "from@yahoo.com", "pswd");
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		/// <param name="userName">The user name for the NetworkCredential to set for this transaction.</param>
		/// <param name="psswd">The password for the NetworkCredential to set for this transaction.</param>
		public static void Send(this MailMessage email, string host, int port, string userName, string psswd)
		{
			__SendMessageSmtp(
				 email, host, port, Credentials.UNamePswd, false, null, userName, psswd, false);
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send() with 
		/// the specified network credentials.
		/// <example><code>
		/// <![CDATA[
		/// MailMessage email = new MailMessage("from@yahoo.com", "to@hotmail.com", "(subject)", "(body)");
		/// // set other wanted MailMessage settings (e.g. attachments, etc).
		/// 
		/// email.Send("plus.smtp.mail.yahoo.com", true);
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="useDefaultCredentials">Indicates whether the 
		/// System.Net.CredentialCache.DefaultCredentials are to be sent with this transaction.</param>
		public static void Send(this MailMessage email, string host, bool useDefaultCredentials)
		{
			__SendMessageSmtp(
 email, host, null, Credentials.DefaultCredentials, useDefaultCredentials, null, null, null, false);
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send() with 
		/// the specified network credentials.
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		/// <param name="useDefaultCredentials">Indicates whether the 
		/// System.Net.CredentialCache.DefaultCredentials are to be sent with this transaction.</param>
		public static void Send(this MailMessage email, string host, int port, bool useDefaultCredentials)
		{
			__SendMessageSmtp(
 email, host, port, Credentials.DefaultCredentials, useDefaultCredentials, null, null, null, false);
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send() with 
		/// the specified network credentials.
		/// <example><code>
		/// <![CDATA[
		/// MailMessage email = new MailMessage("from@yahoo.com", "to@hotmail.com", "(subject)", "(body)");
		/// // set other wanted MailMessage settings (e.g. attachments, etc).
		/// 
		/// NetworkCredential credentials = new NetworkCredential("from@yahoo.com", "pswd");
		/// 
		/// email.Send("plus.smtp.mail.yahoo.com", credentials);
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="networkCredentials">A System.Net.NetworkCredential 
		/// instance to use for authenticating this transaction.</param>
		public static void Send(this MailMessage email, string host, NetworkCredential networkCredentials)
		{
			__SendMessageSmtp(
email, host, null, Credentials.CredentialInstance, false, networkCredentials, null, null, false);
		}

		/// <summary>
		/// Sends this message through the specified host using SmtpClient.Send() with 
		/// the specified network credentials.
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		/// <param name="networkCredentials">A System.Net.NetworkCredential 
		/// instance to use for authenticating this transaction.</param>
		public static void Send(this MailMessage email, string host, int port, NetworkCredential networkCredentials)
		{
			__SendMessageSmtp(
email, host, port, Credentials.CredentialInstance, false, networkCredentials, null, null, false);
		}

		#endregion

		#region SendAsync

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync().
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		public static void SendAsync(this MailMessage email, string host)
		{
			__SendMessageSmtp(
				 email, host, null, Credentials.None, false, null, null, null, true);
		}

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync().
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		public static void SendAsync(this MailMessage email, string host, int port)
		{
			__SendMessageSmtp(
				 email, host, port, Credentials.None, false, null, null, null, true);
		}

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync() with 
		/// the specified network credentials.
		/// <example><code>
		/// <![CDATA[
		/// MailMessage email = new MailMessage("from@yahoo.com", "to@hotmail.com", "(subject)", "(body)");
		/// // set other wanted MailMessage settings (e.g. attachments, etc).
		/// 
		/// email.SendAsync("plus.smtp.mail.yahoo.com", "from@yahoo.com", "pswd");
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="userName">The user name for the NetworkCredential to set for this transaction.</param>
		/// <param name="psswd">The password for the NetworkCredential to set for this transaction.</param>
		public static void SendAsync(this MailMessage email, string host, string userName, string psswd)
		{
			__SendMessageSmtp(
				 email, host, null, Credentials.UNamePswd, false, null, userName, psswd, true);
		}

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync() with 
		/// the specified network credentials.
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		/// <param name="userName">The user name for the NetworkCredential to set for this transaction.</param>
		/// <param name="psswd">The password for the NetworkCredential to set for this transaction.</param>
		public static void SendAsync(this MailMessage email, string host, int port, string userName, string psswd)
		{
			__SendMessageSmtp(
				 email, host, port, Credentials.UNamePswd, false, null, userName, psswd, true);
		}

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync() with 
		/// the specified network credentials.
		/// <example><code>
		/// <![CDATA[
		/// MailMessage email = new MailMessage("from@yahoo.com", "to@hotmail.com", "(subject)", "(body)");
		/// // set other wanted MailMessage settings (e.g. attachments, etc).
		/// 
		/// email.SendAsync("plus.smtp.mail.yahoo.com", true);
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="useDefaultCredentials">Indicates whether the 
		/// System.Net.CredentialCache.DefaultCredentials are to be sent with this transaction.</param>
		public static void SendAsync(this MailMessage email, string host, bool useDefaultCredentials)
		{
			__SendMessageSmtp(
 email, host, null, Credentials.DefaultCredentials, useDefaultCredentials, null, null, null, true);
		}

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync() with 
		/// the specified network credentials.
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		/// <param name="useDefaultCredentials">Indicates whether the 
		/// System.Net.CredentialCache.DefaultCredentials are to be sent with this transaction.</param>
		public static void SendAsync(this MailMessage email, string host, int port, bool useDefaultCredentials)
		{
			__SendMessageSmtp(
 email, host, port, Credentials.DefaultCredentials, useDefaultCredentials, null, null, null, true);
		}

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync() with 
		/// the specified network credentials.
		/// <example><code>
		/// <![CDATA[
		/// MailMessage email = new MailMessage("from@yahoo.com", "to@hotmail.com", "(subject)", "(body)");
		/// // set other wanted MailMessage settings (e.g. attachments, etc).
		/// 
		/// NetworkCredential credentials = new NetworkCredential("from@yahoo.com", "pswd");
		/// 
		/// email.SendAsync("plus.smtp.mail.yahoo.com", credentials);
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="networkCredentials">A System.Net.NetworkCredential 
		/// instance to use for authenticating this transaction.</param>
		public static void SendAsync(this MailMessage email, string host, NetworkCredential networkCredentials)
		{
			__SendMessageSmtp(
email, host, null, Credentials.CredentialInstance, false, networkCredentials, null, null, true);
		}

		/// <summary>
		/// Asynchonously sends this message through the specified host using SmtpClient.SendAsync() with 
		/// the specified network credentials.
		/// </summary>
		/// <param name="email">This MailMessage.</param>
		/// <param name="host">The name or IP address of the host used for 
		/// sending this message as a SMTP transaction.</param>
		/// <param name="port">Set the port to be used for this SMTP transaction.
		/// Otherwise the default port (port 25) will be used.</param>
		/// <param name="networkCredentials">A System.Net.NetworkCredential 
		/// instance to use for authenticating this transaction.</param>
		public static void SendAsync(this MailMessage email, string host, int port, NetworkCredential networkCredentials)
		{
			__SendMessageSmtp(
email, host, port, Credentials.CredentialInstance, false, networkCredentials, null, null, true);
		}

		#endregion

		#region GetResponseData

		#region PrivateMembers

		/// <summary>
		/// Hub for GetResponseData that handles all variables.
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="proxy">The proxy to set.</param>
		/// <param name="setProxy">True if the proxy needs set.</param>
		/// <param name="timeout">The timeout if there is one to set.</param>
		/// <returns></returns>
		private static byte[] __GetResponseDataHub(
			 WebRequest webRequest, WebProxy proxy, bool setProxy, int? timeout)
		{
			if (webRequest == null) throw new ArgumentNullException("webRequest");

			if (setProxy)
				webRequest.Proxy = proxy;

			if (timeout != null)
				webRequest.Timeout = (int)timeout;

			return __GetResponseData(webRequest);
		}

		/// <summary>
		/// Gets the response to the WebRequest.
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <returns>A byte array containing the received data.</returns>
		private static byte[] __GetResponseData(this WebRequest webRequest)
		{
			using (WebResponse webResponse = webRequest.GetResponse())
			using (Stream s = webResponse.GetResponseStream())
			{
				return __ReadWebResponseStream(s);
			}
		}

		/// <summary>
		/// Gets the response to the WebRequest while setting the resposne CookieCollection 
		/// to <paramref name="receivedCookies"/>.
		/// <paramref name="webRequest"/> must be of type HttpWebRequest.
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="receivedCookies">A System.Net.CookieCollection object that will contain the 
		/// collection of cookies sent back from the WebResponse.</param>
		/// <returns>A byte array containing the received data.</returns>
		private static byte[] __GetResponseData(this HttpWebRequest webRequest, ref CookieCollection receivedCookies)
		{
			webRequest.CookieContainer = new CookieContainer();

			using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
			using (Stream s = webResponse.GetResponseStream())
			{
				receivedCookies = webResponse.Cookies;

				return __ReadWebResponseStream(s);
			}
		}

		/// <summary>
		/// Gets the response to the WebRequest; the work of reading the stream is done here
		/// which allows the cookie handling overload of __GetResponseData to share this code.
		/// </summary>
		/// <param name="webResponseStream">The WebRequest.</param>
		/// <returns>The response stream converted to a byte array.</returns>
		private static byte[] __ReadWebResponseStream(Stream webResponseStream)
		{
			int bufferLen = 4096;

			using (MemoryStream ms = new MemoryStream(bufferLen))
			{
				byte[] buffer = new byte[bufferLen];
				int bytesRead = 0;

				while (true)
				{
					bytesRead = webResponseStream.Read(buffer, 0, bufferLen);
					ms.Write(buffer, 0, bytesRead);

					// !! Big problem to avert: the response-stream seems to *read in spurts*,
					// often reading LESS than len even though it hasn't finished reading.  So make sure
					// to only stop reading when ZERO bytes are read, rather than, typically, LESS THAN len
					if (bytesRead == 0)
						break;
				}
				return ms.ToArray();
			}
		}

		#endregion

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream(); 
		/// The received data is returned as byte array.  The expected use of this method is when 
		/// WebClient.DownloadData() does not provide some WebRequest options that may be needed
		/// (such as the ability to set the timeout period or to set cookies).
		/// <example><code>
		/// <![CDATA[
		/// WebRequest req = WebRequest.Create("http://www.asp.net/");
		/// // set other request values
		/// 
		/// byte[] responseData = req.GetResponseData();
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <returns>A byte array containing the received data.</returns>
		static public byte[] GetResponseData(this WebRequest webRequest)
		{
			return __GetResponseDataHub(webRequest, null, false, null);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream(); 
		/// The received data is returned as byte array, while the collection of cookies sent back from 
		/// the (Http)WebResponse are set to <paramref name="receivedCookies"/>.  
		/// <paramref name="webRequest"/> must be of type HttpWebRequest.
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="receivedCookies">A System.Net.CookieCollection object that will contain the 
		/// collection of cookies sent back from the WebResponse.</param>
		/// <returns>A byte array containing the received data.</returns>
		static public byte[] GetResponseData(this WebRequest webRequest, ref CookieCollection receivedCookies)
		{
			if (webRequest == null) throw new ArgumentNullException("webRequest");

			return __GetResponseData((HttpWebRequest)webRequest, ref receivedCookies);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream(); 
		/// The received data is returned as byte array.  The expected use of this method is when 
		/// WebClient.DownloadData() does not provide some WebRequest options that may be needed
		/// (such as the ability to set the timeout period or to set cookies).
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="proxy">The webRequest proxy (null if none).</param>
		/// <returns>A byte array containing the received data.</returns>
		static public byte[] GetResponseData(this WebRequest webRequest, WebProxy proxy)
		{
			return __GetResponseDataHub(webRequest, proxy, true, null);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream(); 
		/// The received data is returned as byte array.  The expected use of this method is when 
		/// WebClient.DownloadData() does not provide some WebRequest options that may be needed
		/// (such as the ability to set the timeout period or to set cookies).
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
		/// <returns>A byte array containing the received data.</returns>
		static public byte[] GetResponseData(this WebRequest webRequest, int timeout)
		{
			return __GetResponseDataHub(webRequest, null, false, timeout);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream(); 
		/// The received data is returned as byte array.  The expected use of this method is when 
		/// WebClient.DownloadData() does not provide some WebRequest options that may be needed
		/// (such as the ability to set the timeout period or to set cookies).
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="proxy">The webRequest proxy (null if none).</param>
		/// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
		/// <returns>A byte array containing the received data.</returns>
		static public byte[] GetResponseData(this WebRequest webRequest, WebProxy proxy, int timeout)
		{
			return __GetResponseDataHub(webRequest, proxy, true, timeout);
		}

		#endregion

		#region GetResponseString

		#region PrivateMembers

		/// <summary>
		/// Hub for GetResponseString that handles all variables.
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="proxy">The proxy to set.</param>
		/// <param name="setProxy">True if the proxy needs set.</param>
		/// <param name="timeout">The timeout if there is one to set.</param>
		/// <returns></returns>
		private static string __GetResponseStringHub(WebRequest webRequest,
  WebProxy proxy, bool setProxy, int? timeout)
		{
			if (webRequest == null) throw new ArgumentNullException("webRequest");

			if (setProxy)
				webRequest.Proxy = proxy;

			if (timeout != null)
				webRequest.Timeout = (int)timeout;

			return __GetResponseString(webRequest);
		}

		/// <summary>
		/// Gets the response and converts it to a string.
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <returns>The data returned as a UTF8 encoded string.</returns>
		private static string __GetResponseString(WebRequest webRequest)
		{
			using (WebResponse webResponse = webRequest.GetResponse())
			using (Stream s = webResponse.GetResponseStream())
			using (StreamReader sr = new StreamReader(s, Encoding.UTF8))
			{
				return sr.ReadToEnd();
			}
		}

		#endregion

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream; 
		/// The received data (which should be of an underlying textual type) is 
		/// converted to a UTF8 encoded string.  The expected use of this method is when 
		/// WebClient.DownloadString() does not provide some needed WebRequest options
		/// (such as the ability to set the timeout period or to set cookies).
		/// <example><code>
		/// <![CDATA[
		/// WebRequest req = WebRequest.Create("http://www.asp.net/");
		/// // set other request values
		/// 
		/// string responseString = req.GetResponseString();
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <returns>The received data encoded into a UTF8 string.</returns>
		static public string GetResponseString(this WebRequest webRequest)
		{
			return __GetResponseStringHub(webRequest, null, false, null);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream; 
		/// The received data (which should be of an underlying textual type) is 
		/// converted to a UTF8 encoded string.  The expected use of this method is when 
		/// WebClient.DownloadString() does not provide some needed WebRequest options
		/// (such as the ability to set the timeout period or to set cookies).
		/// <example><code>
		/// <![CDATA[
		/// WebRequest req = WebRequest.Create("http://www.asp.net/");
		/// // set other request values
		/// 
		/// //set proxy here for one less line of code (if none, set to null)
		/// string responseString = req.GetResponseString(null); 
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="proxy">The webRequest proxy (null if none).</param>
		/// <returns>The received data encoded into a UTF8 string.</returns>
		static public string GetResponseString(this WebRequest webRequest, WebProxy proxy)
		{
			return __GetResponseStringHub(webRequest, proxy, true, null);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream; 
		/// The received data (which should be of an underlying textual type) is 
		/// converted to a UTF8 encoded string.  The expected use of this method is when 
		/// WebClient.DownloadString() does not provide some needed WebRequest options
		/// (such as the ability to set the timeout period or to set cookies).
		/// <example><code>
		/// <![CDATA[
		/// WebRequest req = WebRequest.Create("http://www.asp.net/");
		/// // set other request values
		/// 
		/// //set the proxy and timeout (below: to 10 seconds) here for a couple less lines of code 
		/// string responseString = req.GetResponseString(null, 10000); 
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
		/// <returns>The received data encoded into a UTF8 string.</returns>
		static public string GetResponseString(this WebRequest webRequest, int timeout)
		{
			return __GetResponseStringHub(webRequest, null, false, timeout);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream; 
		/// The received data (which should be of an underlying textual type) is 
		/// converted to a UTF8 encoded string.  The expected use of this method is when 
		/// WebClient.DownloadString() does not provide some needed WebRequest options
		/// (such as the ability to set the timeout period or to set cookies).
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="proxy">The webRequest proxy (null if none).</param>
		/// <param name="timeout">The length of time, in milliseconds, before the request times out.</param>
		/// <returns>The received data encoded into a UTF8 string.</returns>
		static public string GetResponseString(this WebRequest webRequest, WebProxy proxy, int timeout)
		{
			return __GetResponseStringHub(webRequest, proxy, true, timeout);
		}

		/// <summary>
		/// Gets the response to this WebRequest using WebResponse.GetResponseStream; 
		/// The received data (which should be of an underlying textual type) is 
		/// converted to a UTF8 encoded string, while the collection of cookies sent back from 
		/// the HttpWebResponse are set to <paramref name="receivedCookies"/>.  
		/// <paramref name="webRequest"/> must be of type HttpWebRequest.
		/// <example><code>
		/// <![CDATA[
		/// WebRequest req = WebRequest.Create("http://social.msdn.microsoft.com/Forums/en-US/categories");
		/// 
		/// CookieCollection cookieColl = new CookieCollection();
		/// 
		/// string responseString = req.GetResponseString(ref cookieColl);
		/// 
		/// 
		/// // === View Received Cookies ===
		/// int count = cookieColl.Count;
		/// 
		/// foreach (Cookie c in cookieColl)
		///     "Name: {0}  Value: {1}\nExpires: {2}\n\n-------\n".FormatX(c.Name, c.Value, c.Expires).Print();
		/// 
		/// // Result:
		/// // Name: .ASPXANONYMOUS  Value: CFOaytWMywEkAAAANTc5...
		/// // Expires: 11/25/2010 2:20:16 PM
		/// // -------
		/// // Name: msdn  Value: L=1033
		/// // Expires: 1/1/0001 12:00:00 AM
		/// // -------
		/// ]]></code></example>
		/// </summary>
		/// <param name="webRequest">The WebRequest.</param>
		/// <param name="receivedCookies">A System.Net.CookieCollection object that will contain the 
		/// collection of cookies sent back from the WebResponse.</param>
		/// <returns>The received data encoded into a UTF8 string.</returns>
		static public string GetResponseString(this WebRequest webRequest, ref CookieCollection receivedCookies)
		{
			// Note: this method works different from the others by going through the __GetResponseData
			// route, rather than using the StreamReader; this allows us to not double the Cookie handling 
			// code.  Ideally all of the GetResponseString methods could call __GetResponseData and then
			// Encode it as we do below, but since the StreamReader route is so much simpler, we'll keep it 
			if (webRequest == null) throw new ArgumentNullException("webRequest");

			return Encoding.UTF8.GetString(
				 __GetResponseData((HttpWebRequest)webRequest, ref receivedCookies));
		}

		#endregion

	}
}
