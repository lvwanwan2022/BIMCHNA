using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    ///<summary>

    ///所有geometry对象定义的基类。提供统一的哈希、类型提取和序列化。

    ///动态属性命名约定：

    ///👉 属性开头的“_u”意味着它将被忽略，无论是对于哈希还是序列化（例如“_ignoreMe”）

    ///👉 属性名称开头的“@”表示它将被分离（使用传输序列化时）（例如（（动态）obj）[“@meshEquivalent”]=…）

    ///</summary>
    [Serializable]
    public class Base 
    {
        /// <summary>
        /// 基于其属性的唯一哈希.注意：除非对象是从源反序列化的，否则此字段将为空。使用函数<see cref="GetId(bool)"/>获取它
        /// </summary>
        protected string id;
        public string ID => id;

        private string __type;
        public virtual string SwhiType
        {
            get
            {
                if (__type == null)
                {
                    List<string> bases = new List<string>();
                    Type myType = this.GetType();

                    while (myType.Name != nameof(Base))
                    {
                        bases.Add(myType.FullName);
                        myType = myType.BaseType;
                    }

                    if (bases.Count == 0)
                    {
                        __type = nameof(Base);
                    }
                    else
                    {
                        bases.Reverse();
                        __type = string.Join(":", bases);
                    }
                }
                return __type;
            }
        }
        /// <summary>
        /// 获取此对象的id（唯一哈希）。
        /// </summary>
        /// <returns></returns>
        protected virtual void GenerateId()
        {
             id = this.GetType().Name + ":"+this.GetHashCode().ToString();
        }
        public bool IsPublic
        {
            get
            {
                if (user_name == "public")
                {
                    return true;
                }
                else
                { return false; }
            }
        }
        private string user_name= "public";
        private string pass_word = "public";
        public string UserName => user_name;
        /// <summary>
        /// 返回结果：
        /// -1=>用户名不能为：public；
        ///  0=>已经存在用户名，不能重复设置；如需修改用户名和密码，请调用ChangeUserInfo()；如需删除用户名密码请调用UserInfoToDefeat
        ///  1=>设置成功；
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int SetUserInfo(string username,string password="123456")
        {
            if (username == "public")
            {
                throw new Exception("用户名不能为public");
            }
            if(user_name== "public" && pass_word == "public")
            {
                user_name = username;
                pass_word = EncryptPassword(password);
                return 1;
            }
            else
            {
                throw new Exception("已经存在用户名，不能重复设置");
            }
        }
        /// <summary>
        /// 返回结果：
        /// -1=>用户名不能为：public；
        ///  1=>更改成功；
        ///  -2=>用户名密码错误；
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int ChangeUserInfo(string oldusername,string oldpassword,string username,string password="123456")
        {
            if (user_name== oldusername && pass_word == EncryptPassword(oldpassword))
            {
                if (username == "public")
                {
                    throw new Exception("用户名不能为public");
                }
                else
                {
                    user_name = username;
                    pass_word = EncryptPassword(password);
                    return 1;
                }
            }
            else
            {
                throw new Exception("用户名密码错误");
            }

        }
        /// <summary>
        /// 返回结果：
        /// -1=>该实例已经是默认状态，无须重新设为默认；
        ///  1=>设为默认状态成功；
        ///  -2=>用户名密码错误；
        /// </summary>
        /// <param name="oldusername"></param>
        /// <param name="oldpassword"></param>
        /// <returns></returns>
        public int UserInfoToDefeat(string username, string password)
        {
            if (user_name == "public")
            {
                throw new Exception("已经是默认状态");
            }
            else
            {
                if (user_name == username && pass_word == EncryptPassword(password))
                {
                    user_name = "public";
                    pass_word = "public";
                    return 1;
                }
                else
                {
                    throw new Exception("用户名密码错误");
                }
            }
            
        }

        public bool IsAuthorized(string username, string password)
        {
            if (user_name == "public")
            {
                return true;
            }
            else
            {
                if(user_name== username && pass_word == EncryptPassword(password))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 对字符串加密
        /// </summary>
        /// <param name="passwordAndSalt"></param>
        /// <returns></returns>
        private  static string EncryptPassword(string passwordAndSalt)
        {
            MD5 md5 = MD5.Create();
            byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(passwordAndSalt);
            byte[] hs = md5.ComputeHash(passwordAndSaltBytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        
        public int IntLabel { get; set; }=0;
        public string StrLabel { get; set; }="";
        private Dictionary<string, object> Labels { get; set; }
        public void AddLabel(string key,object obj)
        {
            Labels.Add(key, obj);
        }
        public void DeleteLabel(string key)
        {
            Labels.Remove(key);
        }
    }
}
