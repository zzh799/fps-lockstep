using System.Net;
using UnityEngine;


	public static class NetworkHelper
	{
		public static IPEndPoint ToIPEndPoint(string host, int port)
		{
			return new IPEndPoint(IPAddress.Parse(host), port);
		}

		public static IPEndPoint ToIPEndPoint(string address)
		{
			int index = address.LastIndexOf(':');
			string host = address.Substring(0, index);
			string p = address.Substring(index + 1);
			int port = int.Parse(p);
			return ToIPEndPoint(host, port);
		}
		
		/// <summary>
		/// 网络可用
		/// </summary>
		public static bool NetAvailable
		{
			get { return Application.internetReachability != NetworkReachability.NotReachable; }
		}

		/// <summary>
		/// 是否是无线
		/// </summary>
		public static bool IsWifi
		{
			get { return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork; }
		}
	}

