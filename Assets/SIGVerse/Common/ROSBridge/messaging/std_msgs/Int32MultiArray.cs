// Generated by gencs from std_msgs/Int32MultiArray.msg
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.ROSBridge;
using UnityEngine;

using SIGVerse.ROSBridge.std_msgs;

namespace SIGVerse.ROSBridge 
{
	namespace std_msgs 
	{
		[System.Serializable]
		public class Int32MultiArray : ROSMessage
		{
			public std_msgs.MultiArrayLayout layout;
			public System.Collections.Generic.List<System.Int32>  data;


			public Int32MultiArray()
			{
				this.layout = new std_msgs.MultiArrayLayout();
				this.data = new System.Collections.Generic.List<System.Int32>();
			}

			public Int32MultiArray(std_msgs.MultiArrayLayout layout, System.Collections.Generic.List<System.Int32>  data)
			{
				this.layout = layout;
				this.data = data;
			}

			new public static string GetMessageType()
			{
				return "std_msgs/Int32MultiArray";
			}

			new public static string GetMD5Hash()
			{
				return "1d99f79f8b325b44fee908053e9c945b";
			}
		} // class Int32MultiArray
	} // namespace std_msgs
} // namespace SIGVerse.ROSBridge

