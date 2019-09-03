﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading;
using TaskManager.Models;
using TaskManager.Functions;
using TaskManager.Devices;

namespace TaskManager
{
    /// <summary>
    /// AGV运输货物任务
    /// </summary>
    public class ForAGVControl
    {
        #region 线程

        Thread _thread;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ForAGVControl()
        {
            _thread = new Thread(ThreadFunc)
            {
                Name = "AGV任务逻辑处理线程",
                IsBackground = true
            };

            _thread.Start();
        }

        /// <summary>
        /// 事务线程
        /// </summary>
        private void ThreadFunc()
        {
            while (true)
            {
                Thread.Sleep(5000);
                if (!DataControl.IsRunSendAGV)
                {
                    continue;
                }
                try
                {
                    Run_DispatchAGV();
                    Run_Roller();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region AGV 派车任务

        /// <summary>
        /// 执行AGV装货卸货的调度
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
                // ID：获取当前时间生成 WCS-AGV 唯一识别码
                int ID = Convert.ToInt32(System.DateTime.Now.ToString("ddHHmmss"));
                // 装货点：当前包装线固定辊台对接点
                string PickStation = frt.DEVICE;
                // 卸货点：初始化站点
                string DropStation = DataControl._mStools.GetValueByKey("AGVDropStation");

                // 发送 NDC
                if (!DataControl._mNDCControl.AddNDCTask(ID, PickStation, DropStation, out string result))
                {
                    throw new Exception(result);
                }

                // 数据库新增AGV任务资讯
                String sql = String.Format(@"insert into wcs_agv_info(ID,PICKSTATION,DROPSTATION) values('{0}','{1}','{2}')", ID, PickStation, DropStation);
                DataControl._mMySql.ExcuteSql(sql);
            }
            catch (Exception ex)
            {
                // LOG
                DataControl._mTaskTools.RecordTaskErrLog("SendAGV()", "调度AGV装货卸货[固定辊台设备号]", frt.DEVICE, "", ex.ToString());
            }
        }

        #endregion

        #region 相关辊台控制

        /// <summary>
        /// 执行对应AGV任务的辊台设备
        /// </summary>
        public void Run_Roller()
        {
            try
            {
                //获取满足启动辊台条件的AGV任务---位于装货卸货点状态
                DataTable dt = DataControl._mMySql.SelectAll("select * from wcs_agv_info where MAGIC in(4,8) and AGV is not null order by CREATION_TIME");
                if (DataControl._mStools.IsNoData(dt))
                {
                    return;
                }
                List<WCS_AGV_INFO> agvList = dt.ToDataList<WCS_AGV_INFO>();
                // 遍历执行辊台
                foreach (WCS_AGV_INFO agv in agvList)
                {
                    CreatOrderTask(agv);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 生成对应辊台任务指令
        /// </summary>
        /// <param name="agv"></param>
        public void CreatOrderTask(WCS_AGV_INFO agv)
        {
            try
            {
                // 按任务当前状态处理
                switch (Convert.ToInt32(agv.MAGIC))
                {
                    case AGVMagic.到达装货点:
                        // 获取对应包装线固定辊台资讯
                        FRT frt = new FRT(agv.PICKSTATION);
                        // 是否作业中
                        if (frt.CurrentStatus() != FRT.RollerStop)
                        {
                            return;
                        }
                        // 是否存在货物
                        //if (frt.GoodsStatus() == FRT.GoodsYesAll)
                        if (frt.GoodsStatus() == FRT.GoodsYesAll || DataControl.IsIgnoreFRT)  //add调试判断
                        {
                            // 分配 WMS TASK
                            if (String.IsNullOrEmpty(agv.TASK_UID.Trim()))
                            {
                                // 获取WMS TASK ID
                                String sql = String.Format(@"select TASK_UID from wcs_task_info where TASK_TYPE = '{0}' and W_S_LOC = '{1}' and TASK_UID not in 
(select DISTINCT TASK_UID from wcs_agv_info where TASK_UID is not null)", TaskType.AGV搬运, DataControl._mTaskTools.GetArea(agv.PICKSTATION));
                                DataTable dt = DataControl._mMySql.SelectAll(sql);
                                if (DataControl._mStools.IsNoData(dt))
                                {
                                    throw new Exception("无对应 WMS Task！");
                                }
                                // 更新AGV任务资讯-- WMS TASK ID
                                agv.TASK_UID = dt.Rows[0]["TASK_UID"].ToString();
                                sql = String.Format(@"update wcs_agv_info set TASK_UID = '{1}' where ID = '{0}'", agv.ID, agv.TASK_UID);
                                DataControl._mMySql.ExcuteSql(sql);
                            }

                            // 分配卸货点
                            if (agv.UPDATE_TIME == null)
                            {
                                // 锁定已满足2次AGV运输的卸货点
                                String sqllock = @"update wcs_config_device set FLAG = 'L' where DEVICE in (
                                    select distinct DROPSTATION from wcs_agv_info GROUP BY DROPSTATION HAVING count(DROPSTATION) = 2)";
                                DataControl._mMySql.ExcuteSql(sqllock);
                                // 获取 WMS 任务目标点
                                String sqlloc = String.Format(@"select distinct DEVICE from wcs_config_device where FLAG = 'Y' and TYPE = 'FRT' and AREA in (
                                    select W_D_LOC from wcs_task_info where TASK_UID = '{0}') order by CREATION_TIME", agv.TASK_UID);
                                DataTable dtloc = DataControl._mMySql.SelectAll(sqlloc);
                                if (DataControl._mStools.IsNoData(dtloc))
                                {
                                    throw new Exception("无对应 WMS Task 目标位置！");
                                }
                                // 更新AGV任务资讯-- 卸货点
                                agv.DROPSTATION = dtloc.Rows[0]["DEVICE"].ToString();
                                sqlloc = String.Format(@"update wcs_agv_info set UPDATE_TIME = NOW(), DROPSTATION = '{1}' where ID = '{0}'", agv.ID, agv.DROPSTATION);
                                DataControl._mMySql.ExcuteSql(sqlloc);
                            }

                            // 发指令请求AGV启动辊台装货
                            if (!DataControl._mNDCControl.DoLoad(agv.ID, Convert.ToInt32(agv.AGV), out string result))
                            {
                                throw new Exception(result);
                            }
                        }

                        break;
                    case AGVMagic.到达卸货点:
                        // 获取对应包装线固定辊台资讯
                        FRT frtdrop = new FRT(agv.DROPSTATION);
                        // 是否作业中
                        if (frtdrop.CurrentStatus() != FRT.RollerStop)
                        {
                            // 当已启动辊台
                            if (frtdrop.CurrentTask() == FRT.TaskTake && (frtdrop.CurrentStatus() == FRT.RollerRun1 || frtdrop.CurrentStatus() == FRT.RollerRunAll))
                            {
                                // 发指令请求AGV启动辊台装货
                                if (!DataControl._mNDCControl.DoUnLoad(agv.ID, Convert.ToInt32(agv.AGV), out string result))
                                {
                                    throw new Exception(result);
                                }
                            }
                            return;
                        }
                        else // 未启动辊台
                        {
                            byte[] order = null;
                            // 当辊台都无货
                            //if (frtdrop.GoodsStatus() == FRT.GoodsNoAll)
                            if (frtdrop.GoodsStatus() == FRT.GoodsNoAll || DataControl.IsIgnoreFRT) //add调试判断
                            {
                                // 获取指令-- 启动所有辊台 正向接货
                                order = FRT._RollerControl(frtdrop.FRTNum(), FRT.RollerRunAll, FRT.RunFront, FRT.GoodsReceive, FRT.GoodsQty1);
                            }
                            // 当仅2#辊台有货
                            else if (frtdrop.GoodsStatus() == FRT.GoodsYes2)
                            {
                                // 获取指令-- 只启动1#辊台 正向接货
                                order = FRT._RollerControl(frtdrop.FRTNum(), FRT.RollerRun1, FRT.RunFront, FRT.GoodsReceive, FRT.GoodsQty1);
                            }
                            // 加入任务作业链表
                            WCS_TASK_ITEM item = new WCS_TASK_ITEM()
                            {
                                ITEM_ID = "库区接货",
                                WCS_NO = agv.TASK_UID,
                                ID = agv.ID,
                                DEVICE = agv.DROPSTATION,
                                LOC_FROM = agv.AGV
                            };
                            DataControl._mTaskControler.StartTask(new AGVFRTTack(item, DeviceType.固定辊台, order));
                        }

                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                // LOG
                DataControl._mTaskTools.RecordTaskErrLog("CreatOrderTask()", "AGV辊台任务[AGV任务ID]", agv.ID.ToString(), "", ex.ToString());
            }
        }

        #endregion

        #region AGV Method

        /// <summary>
        /// 更新AGV卸货点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="station"></param>
        public void UpdateAGVStation(int id, string station)
        {
            try
            {
                // 发送 NDC
                if (!DataControl._mNDCControl.DoReDerect(id, station, out string result))
                {
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                // LOG
                DataControl._mTaskTools.RecordTaskErrLog("UpdateAGVStation()", "更新AGV卸货点[AGV任务ID]", id.ToString(), "", ex.ToString());
            }
        }

        #endregion


        #region AGV Function [对外]

        /// <summary>
        /// 更新AGV当前任务状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="agv"></param>
        /// <param name="magic"></param>
        public void SubmitAgvMagic(int id, string agv, int magic)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(String.Format(@"update wcs_agv_info set MAGIC = '{1}' where ID = '{0}'", id, magic));

                if (magic == AGVMagic.到达装货点)
                {
                    // 更新数据库内对应AGV任务资讯---获得对应AGV设备号
                    sql.Append(String.Format(@";update wcs_agv_info set AGV = '{1}' where ID = '{0}'", id, agv));
                }

                if (magic == AGVMagic.装货完成)
                {
                    // 更新对应包装线辊台状态，允许新任务派车前往
                    sql.Append(String.Format(@";update wcs_config_device set FLAG = 'Y' where DEVICE in (select distinct PICKSTATION from wcs_agv_info where ID = '{0}')", id));
                }

                // 更新AGV任务资讯
                DataControl._mMySql.ExcuteSql(sql.ToString());

                if (magic == AGVMagic.重新定位卸货)
                {
                    string station;
                    // 获取任务ID对应的卸货点
                    String sqlsta = String.Format(@"select DISTINCT DROPSTATION From wcs_agv_info where ID = '{0}'", id);
                    DataTable dt = DataControl._mMySql.SelectAll(sqlsta);
                    if (DataControl._mStools.IsNoData(dt))
                    {
                        // 定位初始值
                        station = DataControl._mStools.GetValueByKey("AGVDropStation");
                    }

                    station = dt.Rows[0]["DROPSTATION"].ToString();
                    // 发送 NDC 更新点位
                    UpdateAGVStation(id, station);
                }

                if (magic == AGVMagic.任务完成)
                {
                    // 备份并删除
                    String sqlfinish = String.Format(@"");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// AGV 装货中
        /// </summary>
        /// <param name="id"></param>
        /// <param name="agv"></param>
        public void SubmitAgvLoading(int id, string agv)
        {
            try
            {
                // 获取对应AGV任务资讯
                String sql = String.Format(@"select * from wcs_agv_info where MAGIC = '{0}' and ID = '{1}' and AGV = '{2}'", AGVMagic.到达装货点, id.ToString(), agv);
                DataTable dt = DataControl._mMySql.SelectAll(sql);
                if (DataControl._mStools.IsNoData(dt))
                {
                    throw new Exception("找不到对应AGV任务资讯！");
                }
                WCS_AGV_INFO info = dt.ToDataEntity<WCS_AGV_INFO>();
                // 生成包装线固定辊台指令
                FRT frt = new FRT(info.PICKSTATION);
                // 获取指令-- 反向送货
                byte[] order = FRT._RollerControl(frt.FRTNum(), FRT.RollerRunAll, FRT.RunObverse, FRT.GoodsDeliver, FRT.GoodsQty1);
                // 加入任务作业链表
                WCS_TASK_ITEM item = new WCS_TASK_ITEM()
                {
                    ITEM_ID = "包装线送货",
                    WCS_NO = info.TASK_UID,
                    ID = info.ID,
                    DEVICE = info.PICKSTATION,
                    LOC_TO = info.AGV
                };
                DataControl._mTaskControler.StartTask(new AGVFRTTack(item, DeviceType.固定辊台, order));
            }
            catch (Exception ex)
            {
                // LOG
                DataControl._mTaskTools.RecordTaskErrLog("SubmitNDCPlcLoading()", "AGV装货中[AGV任务ID，AGV设备号]", id.ToString(), agv, ex.ToString());
            }
        }

        #endregion
    }

}
