using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferInquiry.Entitys
{
    public class StationEntity
    {
        public string ID { get; set; }

        /// <summary>
        /// 全名
        /// </summary>
        public string FN { get; set; }

        /// <summary>
        /// 全拼
        /// </summary>
        public string FP { get; set; }

        /// <summary>
        /// 简拼
        /// </summary>
        public string JP { get; set; }

        /// <summary>
        /// 其他拼
        /// </summary>
        public string OP { get; set; }

        /// <summary>
        /// 站台代码
        /// </summary>
        public string SC { get; set; }

        /// <summary>
        /// 所属城市
        /// </summary>
        [JsonIgnore]
        public string City { get; set; }

        //public string[] StationData { get; set; }
    }
}
