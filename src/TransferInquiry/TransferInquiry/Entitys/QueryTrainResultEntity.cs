using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferInquiry.Entitys
{
    public class QueryTrainResultEntity
    {
        [JsonProperty("data")]
        public QueryTrainData Data { get; set; }

        [JsonProperty("httpstatus")]
        public int? HttpStatus { get; set; }

        [JsonProperty("messages")]
        public string Messages { get; set; }

        [JsonProperty("status")]
        public bool? Status { get; set; }
    }

    public class QueryTrainData
    {
        [JsonProperty("flag")]
        public string Flag { get; set; }

        [JsonProperty("map")]
        public Dictionary<string, string> Map { get; set; }

        [JsonProperty("result")]
        public string[] Result { get; set; }
    }

    public class QueryTrainResult
    {
        /// <summary>
        /// 查询用的列车号
        /// </summary>
        public string TrainNo { get; set; }

        /// <summary>
        /// 车次
        /// </summary>
        public string TrainNumber { get; set; }

        /// <summary>
        /// 车次的起始站
        /// </summary>
        public string StartCode { get; set; }

        /// <summary>
        /// 车次的终点站
        /// </summary>
        public string EndCode { get; set; }

        /// <summary>
        /// 查询的起始站
        /// </summary>
        public string QueryStartCode { get; set; }

        /// <summary>
        /// 查询的终点站
        /// </summary>
        public string QueryEndCode { get; set; }

        /// <summary>
        /// 发车时间
        /// </summary>
        public string GoTime { get; set; }

        /// <summary>
        /// 到达时间
        /// </summary>
        public string ComeTime { get; set; }

        /// <summary>
        /// 历时
        /// </summary>
        public string TimeSpan { get; set; }
    }
}
