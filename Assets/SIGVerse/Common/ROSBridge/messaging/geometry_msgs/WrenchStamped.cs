// Generated by gencs from geometry_msgs/WrenchStamped.msg
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.ROSBridge;
using UnityEngine;

using SIGVerse.ROSBridge.std_msgs;
using SIGVerse.ROSBridge.geometry_msgs;

namespace SIGVerse.ROSBridge 
{
	namespace geometry_msgs 
	{
		[System.Serializable]
		public class WrenchStamped : ROSMessage
		{
			public std_msgs.Header header;
			public geometry_msgs.Wrench wrench;


			public WrenchStamped()
			{
				this.header = new std_msgs.Header();
				this.wrench = new geometry_msgs.Wrench();
			}

			public WrenchStamped(std_msgs.Header header, geometry_msgs.Wrench wrench)
			{
				this.header = header;
				this.wrench = wrench;
			}

			new public static string GetMessageType()
			{
				return "geometry_msgs/WrenchStamped";
			}

			new public static string GetMD5Hash()
			{
				return "d78d3cb249ce23087ade7e7d0c40cfa7";
			}
		} // class WrenchStamped
	} // namespace geometry_msgs
} // namespace SIGVerse.ROSBridge

