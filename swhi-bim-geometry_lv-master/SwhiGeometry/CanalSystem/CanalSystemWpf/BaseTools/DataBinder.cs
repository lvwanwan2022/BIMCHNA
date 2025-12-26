using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CanalSystem.BaseTools
{
    public static class SerialzeDesObj
    {
        //1.测试1=>对象序列化和反序列化
        //对象序列化为字节数组

        public static byte[] ObjectSerialze(this object obj)

        {

            MemoryStream stream = new MemoryStream();

            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(stream, obj);

            byte[] newArray = new byte[stream.Length];

            stream.Position = 0;

            stream.Read(newArray, 0, (int)stream.Length);

            stream.Close();
            // string result=Convert.ToBase64String(newArray);
            return newArray;

        }


        //字节数组反序列化为对象

        public static Object ArrayDeserialize(this byte[] array)

        {
            //byte[] array = Convert.FromBase64String(str);
            MemoryStream stream = new MemoryStream(array);

            BinaryFormatter bf = new BinaryFormatter();

            Object obj = bf.Deserialize(stream);

            stream.Close();

            return obj;

        }
        //2.测试2=>对象属性绑定

        //3.测试3=>
    }

    /// <summary>
    /// SampleCsUserDataPlugIn plug-in class
    /// </summary>
    public class DictionaryDataBinder
    {

    }

}
