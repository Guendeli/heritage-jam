using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Oryx.SaveLoad.Utilities
{
    public static class SerializeUtility
    {
        public static byte[] Serialize(object obj)
        {
            string serializeObject = JsonConvert.SerializeObject(obj);
            byte[] data = Encoding.ASCII.GetBytes(serializeObject);
            return data;;
        }
        

        public static object Deserialize<T>(byte[] data)
        {
            try
            {
                string deserializedObject = Encoding.ASCII.GetString(data);
                object o = JsonConvert.DeserializeObject<T>(deserializedObject);
                return o;
            }
            catch (Exception e)
            {
                Debug.LogError($"Cannot Deserialize the data , Error : {e.Message}");
               
                return null;
            }
        }
    }
}