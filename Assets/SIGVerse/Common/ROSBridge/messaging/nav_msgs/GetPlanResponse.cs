// Generated by gencs from nav_msgs/GetPlan.srv
// DO NOT EDIT THIS FILE BY HAND!

using System;
using System.Collections;
using System.Collections.Generic;
using SIGVerse.ROSBridge;
using UnityEngine;

using SIGVerse.ROSBridge.nav_msgs;

namespace SIGVerse.ROSBridge 
{
	namespace nav_msgs 
	{
		[System.Serializable]
		public class GetPlanResponse : ServiceResponse
		{
			public nav_msgs.Path plan;


			public GetPlanResponse()
			{
				this.plan = new nav_msgs.Path();
			}

			public GetPlanResponse(nav_msgs.Path plan)
			{
				this.plan = plan;
			}

			new public static string GetMessageType()
			{
				return "nav_msgs/GetPlanResponse";
			}

			new public static string GetMD5Hash()
			{
				return "0002bc113c0259d71f6cf8cbc9430e18";
			}
		} // class GetPlanResponse
	} // namespace nav_msgs
} // namespace SIGVerse.ROSBridge

