// Generated by gencs from nav_msgs/GetMapActionFeedback.msg
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.ROSBridge;
using UnityEngine;

using SIGVerse.ROSBridge.std_msgs;
using SIGVerse.ROSBridge.actionlib_msgs;
using SIGVerse.ROSBridge.nav_msgs;

namespace SIGVerse.ROSBridge 
{
	namespace nav_msgs 
	{
		[System.Serializable]
		public class GetMapActionFeedback : ROSMessage
		{
			public std_msgs.Header header;
			public actionlib_msgs.GoalStatus status;
			public nav_msgs.GetMapFeedback feedback;


			public GetMapActionFeedback()
			{
				this.header = new std_msgs.Header();
				this.status = new actionlib_msgs.GoalStatus();
				this.feedback = new nav_msgs.GetMapFeedback();
			}

			public GetMapActionFeedback(std_msgs.Header header, actionlib_msgs.GoalStatus status, nav_msgs.GetMapFeedback feedback)
			{
				this.header = header;
				this.status = status;
				this.feedback = feedback;
			}

			new public static string GetMessageType()
			{
				return "nav_msgs/GetMapActionFeedback";
			}

			new public static string GetMD5Hash()
			{
				return "aae20e09065c3809e8a8e87c4c8953fd";
			}
		} // class GetMapActionFeedback
	} // namespace nav_msgs
} // namespace SIGVerse.ROSBridge

