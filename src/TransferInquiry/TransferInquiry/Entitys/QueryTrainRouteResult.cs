using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferInquiry.Entitys
{
    public class QueryTrainRouteResult
    {
        [JsonProperty("data")]
        public QueryTrainRouteData Data { get; set; }

        [JsonProperty("httpstatus")]
        public int HttpStatus { get; set; }

        [JsonProperty("messages")]
        public object[] Messages { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("validateMessages")]
        public object ValidateMessages { get; set; }

        [JsonProperty("validateMessagesShowId")]
        public string ValidateMessagesShowId { get; set; }
    }

    public class QueryTrainRouteData
    {
        [JsonProperty("data")]
        public TrainRoute[] Data { get; set; }
    }

    public class TrainRoute
    {
        [JsonIgnore]
        public string TrainNumber { get; set; }

        [JsonProperty("start_station_name")]
        public string StartStationName { get; set; }

        [JsonProperty("arrive_time")]
        public string ArriveTime { get; set; }

        [JsonProperty("station_train_code")]
        public string StationTrainCode { get; set; }

        [JsonProperty("station_name")]
        public string StationName { get; set; }

        [JsonProperty("train_class_name")]
        public string TrainClassName { get; set; }

        [JsonProperty("service_type")]
        public string ServiceType { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("stopover_time")]
        public string StopoverTime { get; set; }

        [JsonProperty("end_station_name")]
        public string EndStationName { get; set; }

        [JsonProperty("station_no")]
        public string StationNo { get; set; }

        [JsonProperty("isEnabled")]
        public bool? IsEnabled { get; set; }
    }
}
