// Generated by gencs from std_msgs/Int8MultiArray.msg
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
		public class Int8MultiArray : ROSMessage
		{
			public std_msgs.MultiArrayLayout layout;
			public System.Collections.Generic.List<sbyte>  data;


			public Int8MultiArray()
			{
				this.layout = new std_msgs.MultiArrayLayout();
				this.data = new System.Collections.Generic.List<sbyte>();
			}

			public Int8MultiArray(std_msgs.MultiArrayLayout layout, System.Collections.Generic.List<sbyte>  data)
			{
				this.layout = layout;
				this.data = data;
			}

			new public static string GetMessageType()
			{
				return "std_msgs/Int8MultiArray";
			}

			new public static string GetMD5Hash()
			{
				return "d7c1af35a1b4781bbe79e03dd94b7c13";
			}
		} // class Int8MultiArray
	} // namespace std_msgs
} // namespace SIGVerse.ROSBridge

