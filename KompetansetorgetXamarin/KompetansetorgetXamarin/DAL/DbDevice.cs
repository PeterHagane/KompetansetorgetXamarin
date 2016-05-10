using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.DAL
{
    public class DbDevice : DbBase
    {
        /// <summary>
        /// Deletes all devices from the local database.
        /// </summary>
        public void DeleteAllDevices()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteAllDevices: Before delete.");
                Db.Execute("delete from " + "Device");
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteAllDevices: After delete.");
            }
        }


        /// <summary>
        /// Inserts the Device
        /// into the database.
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Returns true if the Device was inserted, returns false if a Device with the same 
        ///  primary key already exists in the table.</returns>
        public bool InsertDevice(Device device)
        {
            if (CheckIfDeviceExist(device.id))
            {
                return false;
            }

            lock (DbContext.locker)
            {
                Db.Insert(device);
            }
            return true;
        }

        /// <summary>
        /// Checks if there already is an entry of that Device primary key
        /// In the database.
        /// Returns true if exist, false if it doesnt exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns true if exist, false if it doesnt exist.</returns>
        public bool CheckIfDeviceExist(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Device>(id);
                }
                System.Diagnostics.Debug.WriteLine("DbDevice - CheckIfDeviceExist: Device Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbDevice - CheckIfDeviceExist: Device Already exists");
                System.Diagnostics.Debug.WriteLine("DbDevice - CheckIfDeviceExist: Exception msg: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates a Device, but if it doesnt already exist a new entry will be inserted into the db.
        /// </summary>
        /// <param name="device"></param>
        public void UpdateDevice(Device device)
        {
            if (CheckIfDeviceExist(device.id))
            {
                lock (DbContext.locker)
                {
                    Db.Update(device);
                }
            }

            else
            {
                lock (DbContext.locker)
                {
                    Db.Insert(device);
                }
            }
        }

        /// <summary>
        /// Gets the Device with the spesific id. 
        /// If no matching Device is found it returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Device GetDevice(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    return Db.Get<Device>(id);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(string id): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(string id): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(string id): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Returns all devices stored in the database. 
        /// If no Devices is found it returns null.
        /// </summary>
        /// <returns></returns>
        public List<Device> GetDevices()
        {
            try
            {
                List<Device> results;
                lock (DbContext.locker)
                {
                    results = Db.Query<Device>("Select * from Device");
                }
                return results;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Returns all devices stored in the database. 
        /// If no Devices is found it returns null.
        /// </summary>
        /// <returns></returns>
        public Device GetDevice()
        {
            try
            {
                List<Device> results;
                lock (DbContext.locker)
                {
                    results = Db.Query<Device>("Select * from Device");
                    try
                    {
                        return results[0];
                    }
                    catch
                    {
                        return null;
                    }
                }

            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbDevice - GetDevice(): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Deletes the Device with the spesific id. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void DeleteDevice(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    Db.Delete<Device>(id);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteDevice(string id): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteDevice(string id): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteDevice(string id): End Of Stack Trace");
            }
        }

        /// <summary>
        /// Deletes the Device from the database
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public void DeleteDevice(Device device)
        {
            try
            {
                lock (DbContext.locker)
                {
                    Db.Delete<Device>(device.id);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteDevice(Device device): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteDevice(Device device): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbDevice - DeleteDevice(Device device): End Of Stack Trace");
            }
        }

        /// <summary>
        /// Sets the foreign key in Device: device.username
        /// </summary>
        /// <returns></returns>
        public List<Device> FixStudentForeignKey(string username)
        {
            List<Device> list = GetDevices();
            foreach (var device in list)
            {
                device.username = username;
                lock (DbContext.locker)
                {
                    Db.Update(device);
                }
            }
            return list;
        }
    }
}
