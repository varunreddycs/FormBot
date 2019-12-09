using System;
using System.Collections.Generic;
using System.Text;

namespace FormBot.Common.Constants
{
    public static class StringConstants
    {
        /// <summary>
        /// Defines the ReferenceIdPrefix
        /// </summary>
        public const string ReferenceIdPrefix = "FLQT";

        /// <summary>
        /// Defines the NoMatchAnswer
        /// </summary>
        public const string NoMatchAnswer = "There are no results in {0} for the given \"{1}\" .Please provide value again.";

        /// <summary>
        /// Defines the NotAuthorized
        /// </summary>
        public const string NotAuthorized = "NotAuthorized";

        /// <summary>
        /// Defines the SignInPlease
        /// </summary>
        public const string SignInPlease = "Sign In Please";

        /// <summary>
        /// Defines the DownloadCertificates
        /// </summary>
        public const string DownloadCertificates = "DownloadCertificates";

        /// <summary>
        /// Defines the ArasReferenceIdPrefix
        /// </summary>
        public const string ArasReferenceIdPrefix = "ARAS";

        /// <summary>
        /// Defines the ShowMore
        /// </summary>
        public const string ShowMore = "Show more";

        /// <summary>
        /// Defines the ComplyAdvantageCheckSuccessMessage
        /// </summary>
        public const string ComplyAdvantageCheckSuccessMessage = "An email for creation of New Client(ARAS) has been sent to the technicians";

        /// <summary>
        /// Defines the NotImplemented
        /// </summary>
        public const string NotImplemented = "The flow has not been implemented";



        public const string EndDialogs = "EndDialogs";

        /// <summary>
        /// Defines the referenceIdPrefix
        /// </summary>
        public const string referenceIdPrefix = "FLQT";

        /// <summary>
        /// Defines the GoodBye
        /// </summary>
        public const string GoodBye = "The flow has been aborted sucessfully";

        /// <summary>
        /// Defines the UserSelectionMessage
        /// </summary>
        public const string UserEnteredMessage = "*You have entered the value : '{0}'*";
        public const string UserSelectionMessage = "*You have selected the value : '{0}'*";

        /// <summary>
        /// Defines the ID
        /// </summary>
        public const string ID = "ID: ";



        public const string ArasReferenceIDPrefix = "ID: ARAS";

        /// <summary>
        /// Defines the AtTheRate
        /// </summary>
        public const string AtTheRate = "@";

        /// <summary>
        /// Defines the Sign_In_Please
        /// </summary>
        public const string Sign_In_Please = "Sign In Please";

        /// <summary>
        /// Defines the arasReferenceIdPrefix
        /// </summary>
        public const string ArasPrefix = "ARAS";



        public const string QuoteInitialquestion = "Sure, What type of Quote do you want to create?";

        /// <summary>
        /// Defines the No_match
        /// </summary>
        public const string ArasMailNotification = "An email for creation of New Client (ARAS) has been sent to the technicians.\n\n Vihitha.gogula@hyperiongrp.com";


        public const string ArasBodyHeading = "<p>Hi Team,<p><br/>The below referenced client is not currently approved for use by {0}<br/></p>";

        public const string ArasMailsSubject = "ChatBot:Action Required: ARAS Entry - Client Name: {0} - Broker Name: {1}";
        public const string ArasSharePointLinkMessage = "Please follow the below link and complete the necessary ARAS entry to have them approved:";
        public const string ArasSharePointLink = "http://intranet1.rkh.com/aras2/";



        /// <summary>
        /// Defines the No_match
        /// </summary>
        public const string No_match = "no_match";

        /// <summary>
        /// Defines the ComplyCheck_InitialResponse
        /// </summary>
        public const string ComplyCheck_InitialResponse = "Comply Advantage Check being performed…";

        /// <summary>
        /// Defines the ComplyAdvantage_Check_SuccessMessage
        /// </summary>
        public const string Aras_Check_SuccessMessage = "An email for creation of New Client(ARAS) has been sent to the technicians";
        public const string ArasFailedMessage = "ARAS Request failed.";
        public const string Perform_Another_ArasRequest = "Do you want add another New Client?";

        public const string ComplyAdvantageSuccessMessage = "A potential match was found. An email has been sent to the Vihitha.gogula@hyperiongrp.com for review. \n\n Please Click [Here](https://app.complyadvantage.com/#/case-management?page=1&perPage=10&sort_by=created_at&sort_order=DESC) for more details ";

        /// <summary>
        /// Defines the Perform_Another_ComplyCheck
        /// </summary>
        public const string Perform_Another_ComplyCheck = "Do you want to perform Comply Advantage check for another Assured?";
        public const string SanctionsCheckID = "ID: CA";
        public const string NoMatchMessage = "No matches found, the certificate has been downloaded to the location";

        /// <summary>
        /// Defines the Login
        /// </summary>
        public const string Login = "Login";

        /// <summary>
        /// Defines the Profile
        /// </summary>
        public const string Profile = "Profile";

        /// <summary>
        /// Defines the DateFormat
        /// </summary>
        public const string DateFormat = "ddMMyyyyhhmmssfff";

        /// <summary>
        /// Defines the Modification
        /// </summary>
        public const string Modification = "Modification";
        public const string Modify = "Modify";

        /// <summary>
        /// Defines the hi
        /// </summary>
        public const string hi = "hi";

        /// <summary>
        /// Defines the Yes
        /// </summary>
        public const string Yes = "Yes";

        /// <summary>
        /// Defines the No
        /// </summary>
        public const string No = "No";

        /// <summary>
        /// Defines the Submit
        /// </summary>
        public const string Submit = "Submit";

        /// <summary>
        /// Defines the hi_response_1
        /// </summary>

        /// <summary>
        /// Defines the hi_response_2
        /// </summary>
        public const string hi_response_2 = "*Let's get started!*";
        public const string hi_response_3 = "*What can I do for you today?*";


        public const string SummaryMessage = "The Summary of details provided by you are:";


        public const string ReStartFlow = "ReStartFlow";


        public const string AddNewClient = "Add New Client";


        public const string Cancel = "Cancel";
        public const string Direct = "Direct";
        public const string Faqfile = "Faq.json";
        public const string NoneIntentResponse = "I'm still learning... Sorry, I do not know how to help you with that.";
        public const string SubmitQuestion = "Would you like to Submit the details?";
        public const string TrueStringValue = "true";
        public const string FalseStringValue = "false";
        public const string HyperionMailFooter = "<p>---------------------------------------------------------------------</p><p>Hyperion Insurance Group Hyperion Insurance Group Limited is registered in England and Wales under company registration number 02937398.Registered office: One Creechurch Place, London, EC3A 5AF.Tel: +44(0)20 7398 4888.Fax: +44(0)20 7623 3807.Website:< a href = 'www.hyperiongrp.com'> www.hyperiongrp.com </ a > The information contained in this communication is intended for the named recipients only.It may contain legally privileged and confidential information and if you are not the intended recipient, you must not copy, distribute or take any action in reliance on it.If you have received this communication in error, please notify us immediately by telephone on British Isles + 44(0)20 7623 3806 or return the original to the sender by email. </ p ><p>---------------------------------------------------------------------</p> ";

    }
}
