﻿using DataGridManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridManager
{
    public class AbcDataGrid
    {
        private ObservableCollection<ABCDeviceModel> _mDeviceList = new ObservableCollection<ABCDeviceModel>();
        private ObservableCollection<string> _mDeviceNameList = new ObservableCollection<string>();

        public ObservableCollection<ABCDeviceModel> DeviceList
        {
            set { _mDeviceList = value; }
            get
            {
                return _mDeviceList;
            }
        }

        public void UpdateDeviceList(ABCDeviceModel abc)
        {
            ABCDeviceModel m = _mDeviceList.FirstOrDefault(c => { return c.DeviceID == abc.DeviceID; });
            if(m == null)
            {
                _mDeviceList.Add(abc);
            }
            else
            {
                m.Update(abc);
            }
        }

        public ObservableCollection<string> DeviceNameList
        {
            set { _mDeviceNameList = value; }
            get
            {
                return _mDeviceNameList;
            }
        }

        public void UpdateDeviceNameList(string abc)
        {
            if (!_mDeviceNameList.Contains(abc))
            {
                _mDeviceNameList.Add(abc);
            }
        }


    }
}
