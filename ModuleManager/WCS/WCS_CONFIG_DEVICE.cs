﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ModuleManager.WCS
{
    /// <summary>
    /// 设备配置资讯    WCS_CONFIG_DEVICE
    /// </summary>
    public class WCS_CONFIG_DEVICE
    {
        /// <summary>
        /// 设备
        /// </summary>
        public String DEVICE { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public String IP { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int PORT { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public String TYPE { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public String AREA { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public String REMARK { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public String FLAG { get; set; }

        /// <summary>
        /// 锁定清单号
        /// </summary>
        public String LOCK_WCS_NO { get; set; }

        /// <summary>
        /// 职责
        /// </summary>
        public String DUTY { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CREATION_TIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UPDATE_TIME { get; set; }
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public class DeviceType
    {
        public const string 行车 = "ABC";
        public const string 运输车 = "RGV";
        public const string 摆渡车 = "ARF";
        public const string 固定辊台 = "FRT";
    }

    /// <summary>
    /// 设备状态
    /// </summary>
    public class DeviceFlag
    {
        public const string 失效 = "N";
        public const string 空闲 = "Y";
        public const string 占用 = "U";  // 已使用其中一个辊台
        public const string 锁定 = "L";
    }

    /// <summary>
    /// 设备职责
    /// </summary>
    public class DeviceDuty
    {
        public const string 负责全部 = "A";
        public const string 负责入库 = "I";
        public const string 负责出库 = "O";
    }
}
