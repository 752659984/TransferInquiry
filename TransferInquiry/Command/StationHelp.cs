using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TransferInquiry.DataBase;
using TransferInquiry.Entitys;
using TransferInquiry.Server;

namespace TransferInquiry.Command
{
    public class StationHelp
    {
        public static string stationNameFilePath = "StationName.txt";

        public static bool IsContinue = false;

        /// <summary>
        /// 从文件中加载所有的站点
        /// </summary>
        /// <returns></returns>
        public static StationEntity[] LoadStationNames()
        {
            StationEntity[] result = null;
            if (!File.Exists(stationNameFilePath))
            {
                var url = "https://kyfw.12306.cn/otn/leftTicket/init";
                var html = HtmlHelp.GetHtmlString(url, Encoding.UTF8);
                if (html == "")
                {
                    throw new Exception("获取12306主页失败！");
                }
                var regTxt = "<script type=\"text/javascript\" src=\"(?'Url'/otn/resources/js/framework/station_name.js\\?station_version=[\\.0-9]+)\"\\s*xml:space=\"preserve\"></script>";
                var reg = new Regex(regTxt, RegexOptions.Singleline);
                var m = reg.Match(html);
                if (m == null || m.Groups["Url"].Value == "")
                {
                    throw new Exception("获取站点文件网址失败！");
                }

                var nameUrl = m.Groups["Url"].Value;
                //nameUrl = HtmlHelp.NeedHost(url, nameUrl);
                nameUrl = "https://kyfw.12306.cn" + nameUrl;
                var str = HtmlHelp.GetHtmlString(nameUrl, Encoding.UTF8);
                if (str == "")
                {
                    throw new Exception("获取站点文件失败！");
                }

                result = GetStationNames(str);
                if (result.Length <= 0)
                {
                    throw new Exception("站点数为零！");
                }

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                HtmlHelp.SaveTxtFile(stationNameFilePath, json, false);
            }
            else
            {
                if (HtmlHelp.cookieString == "")
                {
                    var url = "https://kyfw.12306.cn/otn/leftTicket/init";
                    HtmlHelp.GetHtmlString(url, Encoding.UTF8);
                }

                var str = HtmlHelp.LoadTxtFile(stationNameFilePath);
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<StationEntity[]>(str);
            }

            return result;
        }

        /// <summary>
        /// 根据字符串加载成实体
        /// </summary>
        /// <param name="stationStr"></param>
        /// <returns></returns>
        private static StationEntity[] GetStationNames(string stationStr)
        {
            var ss = stationStr.Split('@');
            var result = new StationEntity[ss.Length - 1];
            for (var i = 1; i < ss.Length; ++i)
            {
                var name = ss[i];
                var names = name.Split('|');
                result[i - 1] = new StationEntity()
                {
                    OP = names[0],
                    FN = names[1],
                    SC = names[2],
                    FP = names[3],
                    JP = names[4]
                };
                //result[i - 1] = new StationEntity()
                //{
                //    StationData = new string[4] 
                //    { names[0], names[1], names[2], names[3] }
                //};
            }

            return result;
        }

        /// <summary>
        /// 保存站点名称到DB
        /// </summary>
        /// <param name="stations"></param>
        public static void SaveStationNames(StationEntity[] stations)
        {
            StationEntityServer.InsertStationNames(stations);
        }





        /// <summary>
        /// 查询指定地点到目的地的所有符合的列车
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="time"></param>
        public static QueryTrainResultEntity QueryTrain(string startCode, string endCode, string time)
        {
            var url = "https://kyfw.12306.cn/otn/leftTicket/queryZ?leftTicketDTO.train_date={0}&leftTicketDTO.from_station={1}&leftTicketDTO.to_station={2}&purpose_codes=ADULT";
            url = string.Format(url, time, startCode, endCode);

            var html = HtmlHelp.GetHtmlString(url, Encoding.UTF8);
            if (html == "")
            {
                throw new Exception("查询列车失败！");
            }

            try
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryTrainResultEntity>(html);
                return result;
            }
            catch(Exception e)
            {
                throw new Exception("查询列车时反序列化失败！");
            }
        }

        /// <summary>
        /// 根据查询火车返回的字符串返回实体
        /// </summary>
        /// <param name="resultStr"></param>
        /// <returns></returns>
        public static QueryTrainResult GetQueryTrainResult(string resultStr)
        {
            var ss = resultStr.Split('|');
            var result = new QueryTrainResult()
            {
                TrainNo = ss[2],
                TrainNumber = ss[3],
                StartCode = ss[4],
                EndCode = ss[5],
                QueryStartCode = ss[6],
                QueryEndCode = ss[7],
                GoTime = ss[8],
                ComeTime = ss[9],
                TimeSpan = ss[10]
            };

            return result;
        }


        /// <summary>
        /// 查询列车的路由
        /// </summary>
        /// <param name="trainNo"></param>
        /// <param name="startCode"></param>
        /// <param name="endCode"></param>
        /// <param name="time"></param>
        public static QueryTrainRouteResult QueryTrainRoute(string trainNo, string startCode, string endCode, string time)
        {
            var url = "https://kyfw.12306.cn/otn/czxx/queryByTrainNo?train_no={0}&from_station_telecode={1}&to_station_telecode={2}&depart_date={3}";
            url = string.Format(url, trainNo, startCode, endCode, time);

            var html = HtmlHelp.GetHtmlString(url, Encoding.UTF8);
            if (html == "")
            {
                throw new Exception("查询列车的路由失败");
            }

            try
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<QueryTrainRouteResult>(html);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("查询列车路由时反序列化失败！");
            }
        }

        
        /// <summary>
        /// 从DB中获取所有列车的车次
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllTrains()
        {
            var result = (from q in TrainEntityServer.GetAllTrainNames().AsEnumerable()
                          select q.Field<string>("TrainNumber")).ToList();

            return result;
        }

        /// <summary>
        /// 加载所有的列车
        /// </summary>
        /// <returns></returns>
        public static bool LoadTheAllTrain(string date, Action<int, int> action, Action<string> msgAction)
        {
            try
            {
                //从文件中加载，可以加载所有的站点
                //var sns = LoadStationNames();
                //从DB中加载，仅加载大部分城市
                var sns = StationEntityServer.GetStations().ToArray();

                List<string> trainNos = GetAllTrains();
                var random = new Random();
                var total = sns.Length * sns.Length;

                for (var i = 0; i < sns.Length; ++i)
                {
                    for (var j = 0; j < sns.Length; ++j)
                    {
                        if (!IsContinue)
                        {
                            return true;
                        }

                        action(i * sns.Length + j, total);
                        if (sns[i].SC != sns[j].SC)
                        {
                            msgAction(string.Format("开始获取{0}——{1}", sns[i].SC, sns[j].SC));
                            try
                            {
                                var b = LoadTrain(sns[i].SC, sns[j].SC, date, trainNos);
                                if (b)
                                {
                                    msgAction(string.Format("获取{0}——{1}成功！！！", sns[i].SC, sns[j].SC));
                                }
                            }
                            catch (Exception e)
                            {
                                msgAction(string.Format("获取{0}——{1}失败【{2}】", sns[i].SC, sns[j].SC, e.Message));
                                //HtmlHelp.SaveTxtFile("Error.txt", e.Message, true);
                                //return false;
                            }
                            Thread.Sleep(random.Next(2, 5) * 1000);
                        }
                        else
                        {
                            msgAction(string.Format("跳过{0}——{1}", sns[i].SC, sns[j].SC));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                msgAction(ex.Message + "\r\n" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 加载指定地方的列车
        /// </summary>
        /// <param name="startCode"></param>
        /// <param name="endCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool LoadTrain(string startCode, string endCode, string date, List<string> trainNos)
        {
            if (trainNos == null)
            {
                trainNos = GetAllTrains();
            }

            //这里只需要按城市获取就可以，因为上海-贵港，跟上海南-贵港是一样的
            var qs = QueryTrain(startCode, endCode, date);
            foreach (var q in qs.Data.Result)
            {
                var qtr = GetQueryTrainResult(q);
                if (QueryTrainResultServer.ExistsQueryTrainResult(qtr.TrainNumber, qtr.QueryStartCode, qtr.QueryEndCode))
                {
                    continue;
                }
                //保存到数据库
                QueryTrainResultServer.InsertQueryTrainResult(qtr);

                //如果该车次还没有获取过则获取车次信息和路由
                //因为通过获取的路由和车次的信息都是完整的，所以不需要重新获取
                //比如获取一个上海-贵港，T81就获取完毕了，没必要在获取上海-南宁时再插入数据
                if (!trainNos.Contains(qtr.TrainNumber))
                {
                    trainNos.Add(qtr.TrainNumber);
                    var routeResult = QueryTrainRoute(qtr.TrainNo, qtr.StartCode, qtr.EndCode, date);
                    //保存到数据库
                    foreach (var r in routeResult.Data.Data)
                    {
                        r.TrainNumber = qtr.TrainNumber;
                        TrainRouteServer.InsertTrainRoute(r);
                    }

                    //列车也保存到数据库
                    var train = new TrainEntity()
                    {
                        TrainNumber = qtr.TrainNumber,
                        StartStation = qtr.StartCode,
                        EndStation = qtr.EndCode
                    };
                    if (routeResult.Data.Data.Length > 0)
                    {
                        train.GoTime = routeResult.Data.Data[0].StartTime;
                        train.ComeTime = routeResult.Data.Data[routeResult.Data.Data.Length - 1].ArriveTime;
                    }
                    TrainEntityServer.InsertTrain(train);
                }
            }

            return true;
        }





        public static void TransferQeury(string startName, string endName, int rideCount)
        {
            if (rideCount <= 0)
            {
                return;
            }

            List<string> train = new List<string>();

            ////获取所有的站点查询集合
            //var queryTrains = DBServer.QueryDataTable<QueryTrainResult>("QueryTrainResult");

            var n = rideCount;
            while (rideCount >= 1)
            {
                //Start
                if (n == 1)
                {

                }
            }
        }


        public static List<string> TransferOne(string startName, string endName)
        {
            List<string> result = new List<string>();

            var dt = QueryTrainResultServer.StartsWithName(startName, endName);
            result.AddRange(dt.DataTableToList<QueryTrainResult>().Select(p => p.TrainNumber));

            return result;
        }

        public static List<string> TransferTwo(string startName, string endName)
        {
            List<string> result = new List<string>();

            var startStations = QueryTrainResultServer.StartsWithNameByStart(startName).DataTableToList<QueryTrainResult>();
            var endStations = QueryTrainResultServer.StartsWithNameByEnd(endName).DataTableToList<QueryTrainResult>();

            foreach (var s in startStations)
            {
                foreach (var e in endStations)
                {
                    if (s.QueryEndCode == e.QueryStartCode)
                    {
                        result.Add(string.Format("{0}({1},{2})——>{3}({4},{5})", s.TrainNumber, s.QueryStartCode, s.QueryEndCode, e.TrainNumber, e.QueryStartCode, e.QueryEndCode));
                    }
                }
            }

            return result;
        }

        public static List<string> TransferThree(string startName, string endName, Action<string> action)
        {
            List<string> result = new List<string>();

            ////方法一：
            //var dt = StationEntityServer.GetTransferThree(startName, endName);

            ////方法二：
            //var startStations = QueryTrainResultServer.StartsWithNameByStart(startName).DataTableToList<QueryTrainResult>();
            //var endStations = QueryTrainResultServer.StartsWithNameByEnd(endName).DataTableToList<QueryTrainResult>();
            //action(startStations.Count + "," + endStations.Count);
            //foreach (var s in startStations)
            //{
            //    foreach (var e in endStations)
            //    {
            //        var query = QueryTrainResultServer.StartsWithCode(s.QueryEndCode, e.QueryStartCode).DataTableToList<QueryTrainResult>();
            //        if (query != null)
            //        {
            //            foreach (var q in query)
            //            {
            //                var str = string.Format("{0}({1},{2})——>{3}({4},{5})——>{6}({7},{8})"
            //                    , s.TrainNumber, s.QueryStartCode, s.QueryEndCode
            //                    , q.TrainNumber, q.QueryStartCode, q.QueryEndCode
            //                    , e.TrainNumber, e.QueryStartCode, e.QueryEndCode);

            //                result.Add(str);
            //                action(str);
            //            }
            //        }
            //        Thread.Sleep(100);
            //    }
            //}

            return result;
        }
    }
}
