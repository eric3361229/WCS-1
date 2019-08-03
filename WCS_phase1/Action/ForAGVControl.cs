﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS_phase1.Models;
using System.Data;
using WCS_phase1.Functions;
using WCS_phase1.Devices;
using WCS_phase1.LOG;
using System.Configuration;
using System.Threading;

namespace WCS_phase1.Action
{
    /// <summary>
    /// AGV运输货物任务
    /// </summary>
    class ForAGVControl
    {
        Log log = new Log("AddTaskList");

        #region AGV 派车任务

        /// <summary>
        /// 调度AGV装货卸货
        /// </summary>
        public void Run_DispatchAGV()
        {
            try
            {
                // 获取空闲包装线固定辊台
                DataTable dt = DataControl._mMySql.SelectAll("select * from wcs_config_device where LEFT(AREA,1) = 'A' and FLAG = 'Y'");
                if (DataControl._mStools.IsNoData(dt))
                {
                    return;
                }
                List<WCS_CONFIG_DEVICE> frtList = dt.ToDataList<WCS_CONFIG_DEVICE>();
                // 遍历执行派车任务
                foreach (WCS_CONFIG_DEVICE frt in frtList)
                {
                    SendAGV(frt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 发送调度AGV指令
        /// </summary>
        /// <param name="frt"></param>
        public void SendAGV(WCS_CONFIG_DEVICE frt)
        {
            try
            {
                // IKey：新定义生成 WCS-AGV 唯一识别码
                string IKey = GetNewIKey().ToString();
                // 装货点：当前包装线固定辊台对接点
                string PickStation = frt.DEVICE;
                // 卸货点：初始化虚拟点
                string DropStation = ConfigurationManager.AppSettings["AGVDropStation"];

                // 发送 NDC
                //DataControl._mNDCControl.DoStartOrder("", IKey, PickStation, DropStation);

                // 数据库新增AGV任务资讯
                String sql = String.Format(@"insert into wcs_agv_info(IKEY,PICKSTATION,DROPSTATION) values('{0}','{1}','{2}')", IKey, PickStation, DropStation);
                DataControl._mMySql.ExcuteSql(sql);
            }
            catch (Exception ex)
            {
                // LOG
                log.LOG(String.Format(@"包装线辊台[{0}]请求AGV发送指令异常：{1}", frt.DEVICE, ex.ToString()));
            }
        }

        /// <summary>
        /// 获取一个新的 IKEY 值
        /// </summary>
        /// <returns></returns>
        public int GetNewIKey()
        {
            try
            {
                DataTable dt = DataControl._mMySql.SelectAll("select MAX(IKEY) IKEY from wcs_agv_info");
                if (DataControl._mStools.IsNoData(dt))
                {
                    return 1;
                }
                int ikey = Convert.ToInt32(dt.Rows[0]["IKEY"].ToString());
                // ikey 值定义 1~99 
                return ikey < 100 ? ikey + 1 : 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region AGV相关方法

        // AGV 抵达站点
        public void SubmitAGVIsReach(string id)
        {
            try
            {
                // 装货点
                // 更新数据库内对应AGV任务资讯
                // 当存在已扫码分配货位的货物时，发送指令启动AGV辊台

                // 卸货点
                // 先启动库存区的固定辊台，再发送指令启动AGV辊台
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // AGV 辊台启动
        public void SubmitAGVIsRun(string id)
        {
            try
            {
                // 装货点
                // 启动包装线辊台
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // AGV 离开站点
        public void SubmitAGVIsLeave()
        {
            try
            {
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // 更新AGV卸货点
        public void UpdateAGVStation()
        {
            try
            {
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // 获取AGV当前货物对应卸货点

        #endregion
    }
}