﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WCS_phase1.Models
{
    /// <summary>
    /// 设备资讯
    /// </summary>
    public class Device
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
        public String PORT { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public String FLAG { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CREATION_TIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UPDATE_TIME { get; set; }
    }
}
