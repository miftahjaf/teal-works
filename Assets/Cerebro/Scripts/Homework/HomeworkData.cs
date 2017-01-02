using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Cerebro {

	public class HomeworkData : Data<HomeworkDataCell> 
	{
		public HomeworkData() : base("HomeworkDataJSON")
		{
		}

		public void UpdateTestData()
		{
			string[] allStatus = new string[] { "open", "late", "closed" };
			for (int i = 0; i < 100; i++) {				
				string stat = allStatus [Random.Range (0, allStatus.Length)];
				HomeworkDataCell dataCell = new HomeworkDataCell ("1", "1", new TeacherData(), HomeworkDataCell.QuestionType.wc, new System.DateTime(), new System.DateTime(), HomeworkDataCell.SubmissionStatus.open, "chemistry", "url");
				dataCell.wcData = new WritersCornerData ("1", "Image", 250, "Answer the question", "", "", null);
				dataList.Add (dataCell);
			}
		}

		public void FillData(JSONNode jsonNode)
		{
			Debug.Log (jsonNode.ToString());
			int cnt = jsonNode ["feed_data"] ["feed"].Count;
			for (int i = 0; i < cnt; i++) {
				string id = jsonNode ["feed_data"] ["feed"] [i] ["fk_hw_id"].Value;
				JSONNode homeworkNode = jsonNode ["feed_data"] ["homeworks"] [id];
				if (homeworkNode != null) {
					HomeworkDataCell dataCell = new HomeworkDataCell ();
					dataCell.FillFeedData (homeworkNode);
					string createdById = jsonNode ["feed_data"] ["homeworks"] [id] ["created_by"].Value;
					if (jsonNode ["feed_data"] ["teacher_data"] [createdById] != null) {
						dataCell.createdBy = new TeacherData ();
						dataCell.createdBy.id = createdById;
						dataCell.createdBy.firstName = jsonNode ["feed_data"] ["teacher_data"] [createdById] ["first_name"].Value;
						dataCell.createdBy.lastName = jsonNode ["feed_data"] ["teacher_data"] [createdById] ["last_name"].Value;
						dataCell.createdBy.profilePicUrl = jsonNode ["feed_data"] ["teacher_data"] [createdById] ["profile_image"].Value;
					}
					dataCell.status = jsonNode ["feed_data"] ["hw_contexts"] [dataCell.contextId] ["status"].Value == "assigned" ? HomeworkDataCell.SubmissionStatus.open : HomeworkDataCell.SubmissionStatus.closed;
					if (dataCell.status == HomeworkDataCell.SubmissionStatus.open) {
						if (dataCell.dueTime.Subtract (System.DateTime.Now).TotalSeconds < 0) {
							dataCell.status = HomeworkDataCell.SubmissionStatus.late;
						}
					}

					string queType = homeworkNode ["ques_type"].Value;
					string queId = homeworkNode ["qid"].Value;
					if (queType == "wc") {
						JSONNode wcNode = jsonNode ["feed_data"] ["ques_types"] ["wc"] [queId];
						if (wcNode != null) {
							WritersCornerData wcData = new WritersCornerData ();
							wcData.FillQuestionData (wcNode);
							dataCell.wcData = wcData;
						}
					} else if (queType == "announcement") {
						JSONNode annNode = jsonNode ["feed_data"] ["ques_types"] ["announcement"] [queId];
						if (annNode != null) {
							AnnouncementData annData = new AnnouncementData ();
							annData.FillAnnouncementData (annNode);
							dataCell.announcementData = annData;
						}
					}
					dataList.Add (dataCell);
				}
			}
		}
	}

	public class HomeworkCell
	{
		
	}

	public class HomeworkTitleCell : HomeworkCell
	{
		public string cellTitle;
		public HomeworkTitleCell()
		{
			cellTitle = "";
		}

		public HomeworkTitleCell(string _cellTitle)
		{
			cellTitle = _cellTitle;
		}
	}

	public class HomeworkDataCell : HomeworkCell
	{
		public string id;
		public string contextId;
		public TeacherData createdBy;
		public QuestionType questionType;
		public System.DateTime dueTime;
		public System.DateTime createdTime;
		public SubmissionStatus status;
		public string subject;
		public string thumbnailUrl;
		public Sprite thumbnailSprite;
		public bool userSubmitted;
		public WritersCornerData wcData;
		public AnnouncementData announcementData;
		public bool isLateSubmission;
		public List<HomeworkComment> comments;
		public Dictionary<string, TeacherData> teacherData;
		public GradedStatus currGradedStatus;
		public List<HomeworkGrade> currGradedScore;
		public List<ResponseFeed> responseFeed;

		public enum GradedStatus
		{
			assigned,
			submited,
			graded
		}

		public enum SubmissionStatus
		{
			open,
			late,
			closed
		}

		public enum QuestionType
		{
			wc,
			announcement
		}

		public HomeworkDataCell()
		{
			id = "";
			contextId = "";
			createdBy = new TeacherData();
			questionType = QuestionType.wc;
			dueTime = new System.DateTime ();
			createdTime = new System.DateTime();
			status = SubmissionStatus.open;
			subject = "";
			thumbnailUrl = "";
			thumbnailSprite = null;
			userSubmitted = false;
			wcData = null;
			announcementData = null;
			isLateSubmission = true;
			comments = new List<HomeworkComment> ();
			teacherData = new Dictionary<string, TeacherData> ();
			currGradedStatus = GradedStatus.assigned;
			currGradedScore = new List<HomeworkGrade> ();
			responseFeed = new List<ResponseFeed> ();
		}

		public HomeworkDataCell(string _id, string _contextId, TeacherData _createdBy, QuestionType _questionType, System.DateTime _dueTime, System.DateTime _createdTime, SubmissionStatus _status, string _subject, string _thumbnailUrl)
		{
			id = _id;
			contextId = _contextId;
			createdBy = _createdBy;
			questionType = _questionType;
			dueTime = _dueTime;
			createdTime = _createdTime;
			status = _status;
			subject = _subject;
			thumbnailUrl = _thumbnailUrl;
			thumbnailSprite = null;
			userSubmitted = false;
			wcData = null;
			announcementData = null;
			isLateSubmission = true;
			comments = new List<HomeworkComment> ();
			teacherData = new Dictionary<string, TeacherData> ();
			currGradedStatus = GradedStatus.assigned;
			currGradedScore = new List<HomeworkGrade> ();
			responseFeed = new List<ResponseFeed> ();
		}

		public void FillFeedData(JSONNode jsonNode)
		{
			if (jsonNode == null) {
				return;
			}
			id = jsonNode ["hw_id"].Value;
			if (jsonNode ["contexts"].Count > 0) {
				contextId = jsonNode ["contexts"] [0].Value;
			}
			if (jsonNode ["ques_type"].Value == "wc") {
				questionType = QuestionType.wc;
			} else if (jsonNode ["ques_type"].Value == "announcement") {
				questionType = QuestionType.announcement;
			}
			string time = jsonNode ["due_time"].Value;
			if (time.Length > 18) {
				time = time.Substring (0, 19);
				dueTime = LaunchList.instance.ConvertStringToStandardDate (time);
				dueTime.Add (new System.TimeSpan (5, 30, 0));
			} else {
				dueTime = new System.DateTime();
			}
			time = jsonNode ["created_at"].Value;
			if (time.Length > 18) {
				time = time.Substring (0, 19);
				createdTime = LaunchList.instance.ConvertStringToStandardDate (time);
				createdTime.Add (new System.TimeSpan (5, 30, 0));
			} else {
				createdTime = new System.DateTime();
			}
			subject = jsonNode["subject"].Value == null ? "" : jsonNode["subject"].Value;
			thumbnailUrl = jsonNode["thumb_url"].Value;
		}

		public void FillResponseData(JSONNode jsonNode)
		{
			if (jsonNode == null) {
				return;
			}
			if (jsonNode ["context"] ["hwc_id"].Value != this.contextId) {
				return;
			}

			if (jsonNode ["context"] ["status"] == "assigned") {
				currGradedStatus = GradedStatus.assigned;
			} else if (jsonNode ["context"] ["status"] == "submited") {
				currGradedStatus = GradedStatus.submited;
			} else if (jsonNode ["context"] ["status"] == "graded") {
				currGradedStatus = GradedStatus.graded;
			}
			int cnt = jsonNode ["assessment_response"] ["data"].Count;
			currGradedScore = new List<HomeworkGrade> ();
			for(int i = 0; i < cnt; i++)
			{
				currGradedScore.Add(new HomeworkGrade (jsonNode ["assessment_response"] ["data"] [i] ["rubric_id"].Value, jsonNode ["assessment_response"] ["data"] [i] ["rate"].AsInt));
			}

			responseFeed = new List<ResponseFeed> ();
			comments = new List<HomeworkComment> ();
			wcData.userResponses = new List<WCResponse> ();
			cnt = jsonNode ["context_feed"] ["feed"].Count;
			for (int i = 0; i < cnt; i++) 
			{
				ResponseFeed feed = new ResponseFeed ();
				feed.dataId = jsonNode ["context_feed"] ["feed"] [i] ["fk_item_id"].Value;
				if (jsonNode ["context_feed"] ["feed"] [i] ["type"].Value == "hw_comment") {
					feed.responseType = ResponseFeed.ResponseType.Comment;
					HomeworkComment cmt = new HomeworkComment ();
					cmt.FillFeed (jsonNode ["context_feed"] ["comments"] [feed.dataId]);
					comments.Add (cmt);

					if (cmt.fromType == HomeworkComment.CommentFrom.Teacher) {
						if (!teacherData.ContainsKey(cmt.fromId)) {
							TeacherData data = new TeacherData ();
							data.id = cmt.fromId;
							data.firstName = jsonNode ["context_feed"] ["teacher_data"] [cmt.fromId] ["first_name"].Value;
							data.lastName = jsonNode ["context_feed"] ["teacher_data"] [cmt.fromId] ["last_name"].Value;
							data.profilePicUrl = jsonNode ["context_feed"] ["teacher_data"] [cmt.fromId] ["profile_image"].Value;
							teacherData.Add (cmt.fromId, data);
						}
					}
				} else {
					feed.responseType = ResponseFeed.ResponseType.Submission;
					WCResponse res = new WCResponse ();
					res.FillData (jsonNode ["context_feed"] ["responses"] [feed.dataId]);
					wcData.userResponses.Add (res);
				}
				feed.isFromLocal = false;
				responseFeed.Add (feed);
			}

			for(int i = 0; i < wcData.userResponses.Count; i++)
			{
				if (wcData.userResponses [i].createdAt.Subtract (dueTime).TotalSeconds < 0) {
					isLateSubmission = false;
				}
			}
		}

		public JSONNode ConvertFeedToJson()
		{
			JSONNode N = JSONSimple.Parse ("{\"Data\"}");
			N ["Data"] ["id"] = id;
			N ["Data"] ["contextId"] = contextId;
			N ["Data"] ["createdBy"] ["id"] = createdBy.id;
			N ["Data"] ["createdBy"] ["firstName"] = createdBy.firstName;
			N ["Data"] ["createdBy"] ["lastName"] = createdBy.lastName;
			N ["Data"] ["createdBy"] ["profilePicUrl"] = createdBy.profilePicUrl;
			N ["Data"] ["questionType"] = questionType.ToString();
			N ["Data"] ["dueTime"] = LaunchList.instance.ConvertDateToStandardString(dueTime);
			N ["Data"] ["createdTime"] = LaunchList.instance.ConvertDateToStandardString(createdTime);
			N ["Data"] ["status"] = status.ToString();
			N ["Data"] ["subject"] = subject;
			N ["Data"] ["thumbnailUrl"] = thumbnailUrl;
			N ["Data"] ["userSubmitted"] = userSubmitted.ToString();
			if (questionType == QuestionType.wc) {
				if (wcData != null) {
					N ["Data"] ["wcData"] = wcData.ConvertFeedToJson ();
				} else {
					N ["Data"] ["wcData"] = "";
				}
				N ["Data"] ["announcementData"] = "";
			} else if (questionType == QuestionType.announcement) {
				N ["Data"] ["wcData"] = "";
				if (announcementData != null) {
					N ["Data"] ["announcementData"] = announcementData.ConvertFeedToJson ();
				} else {
					N ["Data"] ["announcementData"] = "";
				}
			}

			return N ["Data"];
		}

		public JSONNode ConvertResponseToJson()
		{
			JSONNode N = JSONSimple.Parse ("{\"Data\"}");
			N ["Data"] ["isLateSubmission"] = isLateSubmission.ToString();
			int cnt = comments.Count;
			for(int i = 0; i < cnt; i++)
			{
				N ["Data"] ["comments"] [comments[i].id] ["id"] = comments[i].id;
				N ["Data"] ["comments"] [comments[i].id] ["comment"] = comments[i].comment;
				N ["Data"] ["comments"] [comments[i].id] ["createdAt"] = LaunchList.instance.ConvertDateToStandardString(comments[i].createdAt);
				N ["Data"] ["comments"] [comments[i].id] ["fromId"] = comments[i].fromId;
				N ["Data"] ["comments"] [comments[i].id] ["fromType"] = comments[i].fromType.ToString();
			}
			if (wcData.userResponses.Count > 0) {
				N ["Data"] ["userResponses"] = wcData.ConvertResponseToJson ();
			}
			cnt = 0;
			foreach(string key in teacherData.Keys)
			{
				N ["Data"] ["teacherData"] [key] ["id"] = key;
				N ["Data"] ["teacherData"] [key] ["firstName"] = teacherData[key].firstName;
				N ["Data"] ["teacherData"] [key] ["lastName"] = teacherData[key].lastName;
				N ["Data"] ["teacherData"] [key] ["profilePicUrl"] = teacherData[key].profilePicUrl;
				cnt++;
			}
			N ["Data"] ["currGradedStatus"] = currGradedStatus.ToString();
			cnt = currGradedScore.Count;
			for(int i = 0; i < cnt; i++)
			{
				N ["Data"] ["currGradedScore"] [i] ["criteria"] = currGradedScore[i].criteria;
				N ["Data"] ["currGradedScore"] [i] ["gradedScore"] = currGradedScore[i].gradedScore.ToString();
				N ["Data"] ["currGradedScore"] [i] ["maxScore"] = currGradedScore[i].maxScore.ToString();
			}
			cnt = responseFeed.Count;
			for(int i = 0; i < cnt; i++)
			{
				N ["Data"] ["responseFeed"] [i] ["dataId"] = responseFeed[i].dataId;
				N ["Data"] ["responseFeed"] [i] ["responseType"] = responseFeed[i].responseType.ToString();
			}

			return N ["Data"];
		}

		public void FillFeedDataFromLocal(JSONNode jsonNode)
		{
			if (jsonNode == null) {
				return;
			}
			id = jsonNode ["id"].Value;
			contextId = jsonNode ["contextId"].Value;
			createdBy = new TeacherData ();
			createdBy.id = jsonNode ["createdBy"] ["id"].Value;
			createdBy.firstName = jsonNode ["createdBy"] ["firstName"].Value;
			createdBy.lastName = jsonNode ["createdBy"] ["lastName"].Value;
			createdBy.profilePicUrl = jsonNode ["createdBy"] ["profilePicUrl"].Value;
			questionType = jsonNode ["questionType"].Value.ToEnum<QuestionType>();
			dueTime = LaunchList.instance.ConvertStringToStandardDate(jsonNode ["dueTime"].Value);
			createdTime = LaunchList.instance.ConvertStringToStandardDate(jsonNode ["createdTime"].Value);
			status = jsonNode ["status"].Value.ToEnum<SubmissionStatus> ();
			subject = jsonNode ["subject"].Value;
			thumbnailUrl = jsonNode ["thumbnailUrl"].Value;
			userSubmitted = jsonNode ["userSubmitted"].AsBool;
			if (questionType == QuestionType.wc) {
				wcData = new WritersCornerData ();
				wcData.FillFeedDataFromLocal (jsonNode ["wcData"]);
				announcementData = new AnnouncementData ();
			} else if (questionType == QuestionType.announcement) {
				wcData = new WritersCornerData ();
				announcementData = new AnnouncementData ();
				announcementData.FeelFeedDataFromLocal (jsonNode ["announcementData"]);
			}
		}

		public void FillResponseDataFromLocal(JSONNode jsonNode)
		{
			if (jsonNode == null) {
				return;
			}
			isLateSubmission = jsonNode ["isLateSubmission"].AsBool;
			comments = new List<HomeworkComment> ();
			int cnt = jsonNode ["comments"].Count;
			for(int i = 0; i < cnt; i++)
			{
				comments[i].id = jsonNode ["comments"] [i] ["id"].Value;
				comments[i].comment = jsonNode ["comments"] [i] ["comment"].Value;
				comments[i].createdAt = LaunchList.instance.ConvertStringToStandardDate(jsonNode ["comments"] [i] ["createdAt"].Value);
				comments[i].fromId = jsonNode ["comments"] [i] ["fromId"].Value;
				comments[i].fromType = jsonNode ["comments"] [i] ["fromType"].Value.ToEnum<HomeworkComment.CommentFrom>();
			}
			if (wcData != null) {
				wcData.FillResponseDataFromLocal (jsonNode ["userResponses"]);
			}
			cnt = jsonNode ["teacherData"].Count;
			teacherData = new Dictionary<string, TeacherData> ();
			for (int i = 0; i < cnt; i++) {
				TeacherData data = new TeacherData ();
				data.id = jsonNode ["teacherData"] [i] ["id"].Value;
				data.firstName = jsonNode ["teacherData"] [i] ["firstName"].Value;
				data.lastName = jsonNode ["teacherData"] [i] ["lastName"].Value;
				data.profilePicUrl = jsonNode ["teacherData"] [i] ["lastName"].Value;
				teacherData.Add (data.id, data);
			}
			currGradedStatus = jsonNode ["currGradedStatus"].Value.ToEnum<GradedStatus>();
			cnt = jsonNode ["currGradedScore"].Count;
			for(int i = 0; i < cnt; i++)
			{
				HomeworkGrade grade = new HomeworkGrade ();
				grade.criteria = jsonNode ["currGradedScore"] [i] ["criteria"].Value;
				grade.gradedScore = jsonNode ["currGradedScore"] [i] ["gradedScore"].AsInt;
				grade.maxScore = jsonNode ["currGradedScore"] [i] ["maxScore"].AsInt;
				currGradedScore.Add (grade);
			}
			cnt = jsonNode ["responseFeed"].Count;
			for(int i = 0; i < cnt; i++)
			{
				ResponseFeed feed = new ResponseFeed ();
				feed.dataId = jsonNode ["responseFeed"] [i] ["dataId"].Value;
				feed.responseType = jsonNode ["responseFeed"] [i] ["responseType"].Value.ToEnum<ResponseFeed.ResponseType>();
				feed.isFromLocal = true;
				responseFeed.Add (feed);
			}
		}
	}

	public class TeacherData
	{
		public string id;
		public string firstName;
		public string lastName;
		public string profilePicUrl;

		public TeacherData()
		{
			id = "";
			firstName = "";
			lastName = "";
			profilePicUrl = "";
		}

		public TeacherData(string _id, string _firstName, string _lastName, string _profilePicUrl)
		{
			id = _id;
			firstName = _firstName;
			lastName = _lastName;
			profilePicUrl = _profilePicUrl;
		}
	}

	public class ResponseFeed
	{
		public string dataId;
		public bool isFromLocal;
		public ResponseType responseType;

		public enum ResponseType
		{
			Submission,
			Comment
		}

		public ResponseFeed()
		{
			dataId = "";
			isFromLocal = false;
			responseType = ResponseType.Submission;
		}

		public ResponseFeed(string _dataId, bool _isFromLocal, ResponseType _responseType)
		{
			dataId = _dataId;
			isFromLocal = _isFromLocal;
			responseType = _responseType;
		}
	}

	public class HomeworkComment
	{
		public string id;
		public string comment;
		public System.DateTime createdAt;
		public string fromId;
		public CommentFrom fromType;

		public enum CommentFrom
		{
			Teacher,
			Me
		}

		public HomeworkComment()
		{
			id = "";
			comment = "";
			createdAt = System.DateTime.Now;
			fromId = "";
			fromType = CommentFrom.Me;
		}

		public HomeworkComment(string _id, string _comment, System.DateTime _createdAt, string _fromId, CommentFrom _fromType)
		{
			id = _id;
			comment = _comment;
			createdAt = _createdAt;
			fromId = _fromId;
			fromType = _fromType;
		}

		public void FillFeed(JSONNode jsonNode)
		{
			id = jsonNode ["item_id"].Value;
			string date = jsonNode ["created_at"].Value;
			if (date.Length > 18) {
				date = date.Substring (0, 19);
				createdAt = LaunchList.instance.ConvertStringToStandardDate (date);
				createdAt.Add (new System.TimeSpan (5, 30, 0));
			} else {
				createdAt = new System.DateTime();
			}
			comment = jsonNode ["data"] ["comment"].Value;
			fromId = jsonNode ["from"].Value;
			if (jsonNode ["from"].Value == LaunchList.instance.mCurrentStudent.StudentID) {
				fromType = CommentFrom.Me;
			} else {
				fromType = CommentFrom.Teacher;
			}
		}
	}

	public class HomeworkGrade
	{
		public string criteria;
		public int maxScore;
		public int gradedScore;

		public HomeworkGrade()
		{
			criteria = "";
			maxScore = 6;
			gradedScore = 0;
		}

		public HomeworkGrade(string _criteria, int _gradedScore, int _maxScore = 6)
		{
			criteria = _criteria;
			maxScore = _maxScore;
			gradedScore = _gradedScore;
		}
	}

	public class WritersCornerData
	{
		public string id;
		public string mediaType;
		public int wordLimit;
		public string promptText;
		public string mediaUrl;
		public string thumbnailUrl;
		public List<WCResponse> userResponses;

		public WritersCornerData()
		{
			id = "";
			mediaType = "";
			wordLimit = 0;
			promptText = "";
			mediaUrl = "";
			thumbnailUrl = "";
			userResponses = new List<WCResponse> ();
		}

		public WritersCornerData(string _id, string _mediaType, int _wordLimit, string _promptText, string _mediaUrl, string _thumbnailUrl, List<WCResponse> _userResponse)
		{
			id = _id;
			mediaType = _mediaType;
			wordLimit = _wordLimit;
			promptText = _promptText;
			mediaUrl = _mediaUrl;
			thumbnailUrl = _thumbnailUrl;
			userResponses = _userResponse;
		}

		public void FillQuestionData(JSONNode jsonNode)
		{
			id = jsonNode ["id"].Value;
			mediaType = jsonNode ["media_type"].Value;
			wordLimit = jsonNode ["word_limit"].AsInt;
			promptText = jsonNode ["prompt_text"].Value;
			mediaUrl = jsonNode ["media_url"].Value;
			thumbnailUrl = jsonNode ["thumb_url"].Value;
			userResponses = new List<WCResponse> ();
		}

		public JSONNode ConvertFeedToJson()
		{
			JSONNode N = JSONSimple.Parse ("{\"Data\"}");
			N ["Data"] ["id"] = id;
			N ["Data"] ["mediaType"] = mediaType;
			N ["Data"] ["wordLimit"] = wordLimit.ToString();
			N ["Data"] ["promptText"] = promptText;
			N ["Data"] ["mediaUrl"] = mediaUrl;
			N ["Data"] ["thumbnailUrl"] = thumbnailUrl;
			return N ["Data"];
		}

		public JSONNode ConvertResponseToJson()
		{
			JSONNode N = JSONSimple.Parse ("{\"Data\"}");
			for(int i = 0; i < userResponses.Count; i++)
			{
				N ["Data"] [userResponses[i].id] ["id"] = userResponses[i].id;
				N ["Data"] [userResponses[i].id] ["userResponse"] = userResponses[i].userResponse;
				N ["Data"] [userResponses[i].id] ["createdAt"] = LaunchList.instance.ConvertDateToStandardString (userResponses [i].createdAt);
			}
			return N ["Data"];
		}

		public void FillFeedDataFromLocal(JSONNode jsonNode)
		{
			id = jsonNode ["id"].Value;
			mediaType = jsonNode ["mediaType"].Value;
			wordLimit = jsonNode ["wordLimit"].AsInt;
			promptText = jsonNode ["promptText"].Value;
			mediaUrl = jsonNode ["mediaUrl"].Value;
			thumbnailUrl = jsonNode ["thumbnailUrl"].Value;
		}

		public void FillResponseDataFromLocal(JSONNode jsonNode)
		{
			userResponses = new List<WCResponse> ();
			int cnt = jsonNode.Count;
			for (int i = 0; i < cnt; i++) {
				WCResponse res = new WCResponse ();
				res.id = jsonNode [i] ["id"].Value;
				res.userResponse = jsonNode [i] ["userResponse"].Value;
				res.createdAt = LaunchList.instance.ConvertStringToStandardDate(jsonNode [i] ["createdAt"].Value);
				userResponses.Add (res);
			}
		}
	}

	public class WCResponse
	{
		public string id;
		public string userResponse;
		public System.DateTime createdAt;

		public WCResponse()
		{
			id = "";
			userResponse = "";
			createdAt = System.DateTime.Now;
		}

		public WCResponse(string _id, string _userResponse, System.DateTime _createdAt)
		{
			id = _id;
			userResponse = _userResponse;
			createdAt = _createdAt;
		}

		public void FillData(JSONNode jsonNode)
		{
			id = jsonNode ["item_id"].Value;
			userResponse = jsonNode ["data"] ["response"].Value;
			string date = jsonNode ["created_at"].Value;
			if (date.Length > 18) {
				date = date.Substring (0, 19);
				createdAt = LaunchList.instance.ConvertStringToStandardDate (date);
				createdAt.Add (new System.TimeSpan (5, 30, 0));
			} else {
				createdAt = new System.DateTime();
			}
		}
	}

	public class AnnouncementData
	{
		public string id;
		public string announcementText;
		public string thumbnailUrl;
		public bool isRead;

		public AnnouncementData()
		{
			id = "";
			announcementText = "";
			thumbnailUrl = "";
			isRead = false;
		}

		public AnnouncementData(string _id, string _announcementText, string _thumbnailUrl, bool _isRead)
		{
			id = _id;
			announcementText = _announcementText;
			thumbnailUrl = _thumbnailUrl;
			isRead = _isRead;
		}

		public void FillAnnouncementData(JSONNode jsonNode)
		{
			id = jsonNode ["id"].Value;
			announcementText = jsonNode ["announcement_text"].Value;
			thumbnailUrl = jsonNode ["thumb_url"].Value;
		}

		public JSONNode ConvertFeedToJson()
		{
			JSONNode N = JSONSimple.Parse ("{\"Data\"}");
			N ["Data"] ["id"] = id;
			N ["Data"] ["announcementText"] = announcementText;
			N ["Data"] ["thumbnailUrl"] = thumbnailUrl;
			return N ["Data"];
		}

		public JSONNode ConvertResponseToJson()
		{
			JSONNode N = JSONSimple.Parse ("{\"Data\"}");
			N ["Data"] ["id"] = id;
			N ["Data"] ["isRead"] = isRead.ToString();
			return N ["Data"];
		}

		public void FeelFeedDataFromLocal(JSONNode jsonNode)
		{
			id = jsonNode ["id"].Value;
			announcementText = jsonNode ["announcementText"].Value;
			thumbnailUrl = jsonNode ["thumbnailUrl"].Value;
		}

		public void FeelResponseDataFromLocal(JSONNode jsonNode)
		{
			isRead = jsonNode ["isRead"].AsBool;
		}
	}

}
