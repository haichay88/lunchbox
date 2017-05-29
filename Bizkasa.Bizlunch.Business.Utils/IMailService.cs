using DDay.iCal;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public interface IMailService
    {
        string SendMeeting(MailMeetingEntity mailMeetingEntity);
        string SendMeeting(MailMeetingEntity mailMeetingEntity, Action<MailMessage> actionBeforeSendMeeting);
    }

    public class MailServiceICal : IMailService
    {
        #region Constructors

        public MailServiceICal()
        {

        }
        public MailServiceICal(MailConfigEntity mailConfigEntity)
        {
            this.MailConfig = mailConfigEntity;
        }

        #endregion

        #region Properties

        public MailConfigEntity MailConfig { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// Send mail ICalendar
        /// </summary>
        /// <param name="mailMeetingEntity">MailMeetingEntity</param>
        public string SendMeeting(MailMeetingEntity mailMeetingEntity)
        {
            return this.SendMeeting(mailMeetingEntity, null);
        }

        /// <summary>
        /// Send mail ICalendar
        /// </summary>
        /// <param name="mailMeetingEntity">MailMeetingEntity</param>
        /// <param name="actionBeforeSendMeeting">Action<MailMessage></param>
        public string SendMeeting(MailMeetingEntity mailMeetingEntity, Action<MailMessage> actionBeforeSendMeeting)
        {
            SmtpClient m_SmtpClient = new SmtpClient();

            if (this.MailConfig != null)
            {
                m_SmtpClient.UseDefaultCredentials = MailConfig.UseDefaultCredentials;
                if (!MailConfig.UseDefaultCredentials) //tạo mới Smtp Credentials
                    m_SmtpClient.Credentials = new NetworkCredential(MailConfig.Username, MailConfig.Password, MailConfig.Domain);
                m_SmtpClient.Port = MailConfig.Port;
                m_SmtpClient.Host = MailConfig.Host;
                m_SmtpClient.EnableSsl = MailConfig.EnableSsl;
            }

            MailMessage m_MailMessage = new MailMessage()
            {
                From = new MailAddress(mailMeetingEntity.From),
                Body = mailMeetingEntity.Body,
                Subject = mailMeetingEntity.Subject,
                IsBodyHtml = true,
            };

            //Parse MailMeetingEntity -> ICalendar Entity

            // Create a new iCalendar
            iCalendar m_iCalendar = new iCalendar()
            {
                Method = MailServiceICal.ICalendarMethod_Request, //PUBLISH THÌ KO ADD VÀO TRONG CALENDAR
                Version = MailServiceICal.ICalendar_Version,
                ProductID = MailServiceICal.ICalendar_ProductID,
            };

            // Create the event, and add it to the iCalendar
            Event m_Event = m_iCalendar.Create<Event>();

            // Set information about the event
            m_Event.UID = mailMeetingEntity.UID;
            m_Event.DTStamp = new iCalDateTime(mailMeetingEntity.Stamp);
            m_Event.Start = new iCalDateTime(mailMeetingEntity.Start);
            m_Event.End = new iCalDateTime(mailMeetingEntity.End);
            m_Event.Description = mailMeetingEntity.Description;
            m_Event.Location = mailMeetingEntity.Location;
            m_Event.Summary = mailMeetingEntity.Description;
            //m_event.Transparency = TransparencyType.Opaque;

            //CONFIG ALARM
            foreach (var m_AlarmEntity in mailMeetingEntity.Alarms)
            {
                AlarmAction m_AlarmAction = new AlarmAction();
                if (m_AlarmEntity.Trigger.Equals(MailServiceICal.Action_Audio))
                    m_AlarmAction = AlarmAction.Audio;
                else if (m_AlarmEntity.Trigger.Equals(MailServiceICal.Action_Display))
                    m_AlarmAction = AlarmAction.Display;
                else if (m_AlarmEntity.Trigger.Equals(MailServiceICal.Action_Email))
                    m_AlarmAction = AlarmAction.Email;
                else if (m_AlarmEntity.Trigger.Equals(MailServiceICal.Action_Procedure))
                    m_AlarmAction = AlarmAction.Procedure;
                m_Event.Alarms.Add(new Alarm
                {
                    Duration = m_AlarmEntity.Duration,
                    Trigger = new Trigger(m_AlarmEntity.Trigger),
                    Description = m_AlarmEntity.Description,
                    Repeat = m_AlarmEntity.RepeatTime,
                    Action = m_AlarmAction,
                });
            }

            //Add Attendees
            var m_Attendes = new List<IAttendee>();
            foreach (var m_AttendeesEntity in mailMeetingEntity.Attendees)
            {
                m_MailMessage.To.Add(new MailAddress(m_AttendeesEntity.Email));
                IAttendee m_Attendee = new DDay.iCal.Attendee(MailServiceICal.Attendee_MailTo + m_AttendeesEntity.Email);
                if (m_AttendeesEntity.IsOptional)
                    m_Attendee.Role = MailServiceICal.Role_Optional;
                else
                    m_Attendee.Role = MailServiceICal.Role_Request;
                m_Attendes.Add(m_Attendee);    
            }

            if (m_Attendes != null && m_Attendes.Count > 0)
                m_Event.Attendees = m_Attendes;

            //Check before send meeting
            if (actionBeforeSendMeeting != null)
                actionBeforeSendMeeting(m_MailMessage);

            DDay.iCal.Serialization.iCalendar.iCalendarSerializer m_Serializer = new DDay.iCal.Serialization.iCalendar.iCalendarSerializer();
            //Convert iCal to string
            string m_iCalendarData = m_Serializer.SerializeToString(m_iCalendar);
            System.Net.Mime.ContentType m_Contype = new System.Net.Mime.ContentType(MailServiceICal.ICalendar_ContentType);
            m_Contype.Parameters.Add(MailServiceICal.ICalendar_Method, MailServiceICal.ICalendarMethod_Request);
            m_Contype.Parameters.Add(MailServiceICal.ICalendar_Name, MailServiceICal.ICalendar_FileName);
            AlternateView m_AlternateView = AlternateView.CreateAlternateViewFromString(m_iCalendarData, m_Contype);
            m_MailMessage.AlternateViews.Add(m_AlternateView);

            m_SmtpClient.Send(m_MailMessage);
            return m_iCalendarData;
        }

        #endregion

        #region Constants

        public const string ICalendarMethod_Request = "REQUEST";
        public const string ICalendarMethod_Publish = "PUBLISH";
        public const string ICalendar_Version = "2.0";
        public const string ICalendar_ProductID = "-//Schedule a Meeting";

        public const string Role_Request = "REQ-PARTICIPANT";
        public const string Role_Optional = "OPT-PARTICIPANT";
        public const string Attendee_MailTo = "MAILTO:";
        public const string Action_Audio = "Audio";
        public const string Action_Display = "Display";
        public const string Action_Email = "Email";
        public const string Action_Procedure = "Procedure";
        public const string ICalendar_ContentType = "text/calendar";
        public const string ICalendar_Method = "method";
        public const string ICalendar_Name = "name";
        public const string ICalendar_FileName = "ScheduleMeeting.ics";

        #endregion

    }

    public class MailConfigEntity
    {
        #region Properties

        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        #endregion
    }

    public class MailMeetingEntity
    {
        #region Constructors

        public MailMeetingEntity()
        {
            this.UID = Guid.NewGuid().ToString();
            this.Attendees = new List<MailMeetingAttendeeEntity>();
            this.Alarms = new List<MailMeetingAlarmEntity>();
        }

        #endregion

        #region Properties

        //Auto Generate if not set
        public string UID { get; set; }
        public string From { get; set; }

        public string Subject { get; set; } //Summary
        public string Location { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }

        public DateTime Stamp { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public List<MailMeetingAttendeeEntity> Attendees { get; set; }
        public List<MailMeetingAlarmEntity> Alarms { get; set; }

        #endregion
    }

    public class MailMeetingAttendeeEntity
    {
        #region Constructors

        public MailMeetingAttendeeEntity()
        {
            this.IsOptional = false;
        }

        #endregion

        #region Properties

        public string Email { get; set; }
        public bool IsOptional { get; set; }

        #endregion
    }

    public class MailMeetingAlarmEntity
    {
        #region Constructors

        public MailMeetingAlarmEntity()
        {
            this.Trigger = new TimeSpan(0, 30, 0);
            this.Duration = new TimeSpan(0, 5, 0);
            this.RepeatTime = 3;
            this.Action = MailServiceICal.Action_Display;
        }

        #endregion

        #region Properties

        public TimeSpan Trigger { get; set; }
        public TimeSpan Duration { get; set; }
        public int RepeatTime { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }

        #endregion
    }
}