﻿using ModuleManager.NDC;
using NdcManager;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdcManager.DataGrid.Models
{
    [Serializable]
    public class NdcTaskModel : BaseDataGrid
    {
        private int taskid;
        private int ikey;
        private int order;
        private int agvname;
        private string loadsite;
        private string unloadsite;
        private string redirectsite;
        private bool hasload;
        private bool hasunload;

        [DisplayName("ID")]
        public int TaskID
        {
            get
            {
                return taskid;
            }
            set
            {
                taskid = value;
                OnPropertyChanged("TaskID");
            }
        }

        [DisplayName("IKey")]
        public int IKey
        {
            get
            {
                return ikey;
            }
            set
            {
                ikey = value;
                OnPropertyChanged("IKey");
            }
        }

        [DisplayName("Index")]
       
        public int Order {
            get {
                return order;
            }
            set
            {
                order = value;
                OnPropertyChanged("Order");
            }
        }


        [DisplayName("AGV")]
        public int AgvName
        {
            get
            {
                return agvname;
            }
            set
            {
                agvname = value;
                OnPropertyChanged("AgvName");
            }
        }

        [DisplayName("接货点")]
        public string LoadSite
        {
            get
            {
                return loadsite;
            }
            set
            {
                loadsite = value;
                OnPropertyChanged("LoadSite");
            }
        }

        [DisplayName("卸货点")]
        public string UnLoadSite
        {
            get
            {
                return unloadsite;
            }
            set
            {
                unloadsite = value;
                OnPropertyChanged("UnLoadSite");
            }
        }

        [DisplayName("重定向")]
        public string RedirectSite
        {
            get
            {
                return redirectsite;
            }
            set
            {
                redirectsite = value;
                OnPropertyChanged("RedirectSite");
            }
        }

        [DisplayName("接货")]
        public bool HasLoad
        {
            get
            {
                return hasload;
            }
            set
            {
                hasload = value;
                OnPropertyChanged("HasLoad");
            }
        }

        [DisplayName("接货")]
        public bool HasUnLoad
        {
            get
            {
                return hasunload;
            }
            set
            {
                hasunload = value;
                OnPropertyChanged("HasUnLoad");
            }
        }

        public void Update(NDCItem item)
        {
            if (taskid != item._mTask.TASKID)
            {
                TaskID = item._mTask.TASKID;
            }
            if (ikey != item._mTask.IKEY)
            {
                IKey = item._mTask.IKEY;
            }
            if (order != item._mTask.ORDERINDEX)
            {
                Order = item._mTask.ORDERINDEX;
            }
            if (agvname != item.CarrierId )
            {
                AgvName = item.CarrierId;
            }
            if (loadsite != item._mTask.LOADSITE)
            {
                LoadSite = item._mTask.LOADSITE;
            }
            if (unloadsite != item._mTask.UNLOADSITE)
            {
                UnLoadSite = item._mTask.UNLOADSITE;
            }
            if (redirectsite != item._mTask.REDIRECTSITE)
            {
                RedirectSite = item._mTask.REDIRECTSITE;
            }
            if (hasload = item._mTask.HADLOAD)
            {
                HasLoad = item._mTask.HADLOAD;
            }
            if (hasunload = item._mTask.HADUNLOAD)
            {
                HasUnLoad = item._mTask.HADUNLOAD;
            }
        }

        public NdcTaskModel(TempItem item)
        {
            IKey = item.IKey;
            TaskID = item.TaskID;
            LoadSite = item.LoadSite;
            UnLoadSite = item.UnloadSite;
            RedirectSite = item.RedirectSite;
        }

        public NdcTaskModel(NDCItem item)
        {
            IKey = item._mTask.IKEY;
            TaskID = item._mTask.TASKID;
            Order = item._mTask.ORDERINDEX;
            agvname = item.CarrierId;
            LoadSite = item._mTask.LOADSITE;
            UnLoadSite = item._mTask.UNLOADSITE;
            RedirectSite = item._mTask.REDIRECTSITE;
            HasLoad = item._mTask.HADLOAD;
            HasUnLoad = item._mTask.HADUNLOAD;
        }
    }
}
