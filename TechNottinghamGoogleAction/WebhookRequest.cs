namespace TechNottinghamGoogleAction
{

    public class WebhookRequest
    {
        public string responseId { get; set; }
        public Queryresult queryResult { get; set; }
        public Webhookstatus webhookStatus { get; set; }
    }

    public class Queryresult
    {
        public string queryText { get; set; }
        public Parameters parameters { get; set; }
        public bool allRequiredParamsPresent { get; set; }
        public Intent intent { get; set; }
        public int intentDetectionConfidence { get; set; }
        public Diagnosticinfo diagnosticInfo { get; set; }
        public string languageCode { get; set; }
    }

    public class Parameters
    {
        public string Event { get; set; }
    }

    public class Intent
    {
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class Diagnosticinfo
    {
        public int webhook_latency_ms { get; set; }
    }

    public class Webhookstatus
    {
        public int code { get; set; }
        public string message { get; set; }
    }

}
