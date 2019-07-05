﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS_phase1.Models
{
    /// <summary>
    /// 总任务资讯
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// 任务UID
        /// </summary>
        public String TASK_UID { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public String TASK_TYPE { get; set; }

        /// <summary>
        /// 货物码
        /// </summary>
        public String BARCODE { get; set; }

        /// <summary>
        /// 来源货位
        /// </summary>
        public String W_S_LOC { get; set; }

        /// <summary>
        /// 目标货位
        /// </summary>
        public String W_D_LOC { get; set; }

        /// <summary>
        /// 站点
        /// </summary>
        public String SITE { get; set; }

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
