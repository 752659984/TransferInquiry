using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferInquiry.Entitys
{
    public class TrainEntity
    {
        /// <summary>
        /// 车次
        /// </summary>
        public string TrainNumber { get; set; }

        /// <summary>
        /// 起点
        /// </summary>
        public string StartStation { get; set; }

        /// <summary>
        /// 终点
        /// </summary>
        public string EndStation { get; set; }

        /// <summary>
        /// 出发时间
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
