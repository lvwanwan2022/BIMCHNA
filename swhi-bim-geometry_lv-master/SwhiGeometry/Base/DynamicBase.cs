using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Lv.BIM.Geometry
{

    /// <summary>
    /// 基类实现了一系列非常不错的动态对象方法，比如动态添加和删除属性。让c#感觉就像json
    /// <para>改编自 Rick Strahl 🤘</para>
    /// <para>https://weblog.west-wind.com/posts/2012/feb/08/creating-a-dynamic-extensible-c-expando-object</para>
    /// </summary>
     [Serializable]
    public class DynamicBase : DynamicObject, IDynamicMetaObjectProvider
  {
    /// <summary>
    /// 用来动态添加属性存储属性的属性字典
    /// </summary>
    private Dictionary<string, object> properties = new Dictionary<string, object>();

    public DynamicBase()
    {

    }

    /// <summary>
    /// 通过点语法获取属性
    /// <para><pre>((dynamic)myObject).superProperty;</pre></para>
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      return (properties.TryGetValue(binder.Name, out result));
    }

    /// <summary>
    /// 通过点语法设置属性
    /// <para><pre>((dynamic)myObject).superProperty = something;</pre></para>
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      var valid = IsPropNameValid(binder.Name, out _);
      if (valid)
        properties[binder.Name] = value;
      return valid;
    }

        /// <summary>
        /// 判断属性名是否可用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
    public bool IsPropNameValid(string name, out string reason)
    {
      // 语法规则
      // 匹配@开头字符，2次以上.
      var manyLeadingAtChars = new Regex(@"^@{2,}");
      // 不可用字符串（.和/）.
      var invalidChars = new Regex(@"[\.\/]");
      // Existing members
      var members = GetInstanceMembersNames();

      // TODO: 检查分离或不分离 detached/non-detached的重复名字? 例如: '@something' vs 'something'
      // TODO: 实例成员不会被重写, 注意可能引起的问题.
      var checks = new List<(bool, string)>
      {
        (!(string.IsNullOrEmpty(name) || name == "@"), "属性名为空"),
        // Checks for multiple leading @
        (!manyLeadingAtChars.IsMatch(name), "仅允许使用以 '@' 开头的字符串， 这表示应该分离属性值."),
        // Checks for invalid chars
        (!invalidChars.IsMatch(name), $"属性名 '{name}' 包含不可用字符串，以下字符串不可用: ./"),
        // Checks if you are trying to change a member property
        //(!members.Contains(name), "Modifying the value of instance member properties is not allowed.")
      };

      var r = "";
      // Prop name is valid if none of the checks are true
      var isValid = checks.TrueForAll(v =>
      {
        if (!v.Item1) r = v.Item2;
        return v.Item1;
      });

      reason = r;
      return isValid;
    }

    /// <summary>
    /// 使用key形式设置或获取属性值. 例如:
    /// <para><pre>((dynamic)myObject)["superProperty"] = 42;</pre></para>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object this[string key]
    {
      get
      {
        if (properties.ContainsKey(key))
          return properties[key];

        var prop = GetType().GetProperty(key);

        if (prop == null)
          return null;

        return prop.GetValue(this);
      }
      set
      {
        if (!IsPropNameValid(key, out string reason)) throw new Exception("Invalid prop name: " + reason);

        if (properties.ContainsKey(key))
        {
          properties[key] = value;
          return;
        }

        var prop = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.Name == key);

        if (prop == null)
        {
          properties[key] = value;
          return;
        }
        try
        {
          prop.SetValue(this, value);
        }
        catch (Exception ex)
        {
          throw new Exception(ex.Message, ex);
        }
      }
    }

        /// <summary>
        /// 获取类的所有属性名, 包含动态的dynamic和强类型的typed.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
    {
      var names = new List<string>();
      foreach (var kvp in properties) names.Add(kvp.Key);

      var pinfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      foreach (var pinfo in pinfos) names.Add(pinfo.Name);

      names.Remove("Item"); // TODO: investigate why we get Item out?
      return names;
    }

        /// <summary>
        /// 获取实例类自定义的属性名（强类型的typed）结果为IEnumerable<string>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetInstanceMembersNames()
    {
      var names = new List<string>();
      var pinfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      foreach (var pinfo in pinfos) names.Add(pinfo.Name);

      names.Remove("Item"); // TODO: investigate why we get Item out?
      return names;
    }

        /// <summary>
        /// 获取实例类自定义的属性（强类型的typed），结果为IEnumerable<PropertyInfo>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> GetInstanceMembers()
    {
      var names = new List<PropertyInfo>();
      var pinfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

      foreach (var pinfo in pinfos)
        if (pinfo.Name != "Item") names.Add(pinfo);

      return names;
    }

        /// <summary>
        /// 获取所有 没有 [SchemaIgnore] 特性标签标记的属性名（typed and dynamic），返回类型为IEnumerable<string>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetMemberNames()
    {
      var names = new List<string>();
      foreach (var kvp in properties) names.Add(kvp.Key);

      var pinfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetCustomAttribute(typeof(SchemaIgnore)) == null && x.Name != "Item");
      foreach (var pinfo in pinfos) names.Add(pinfo.Name);

      return names;
    }

        /// <summary>
        ///  获取所有 没有 [SchemaIgnore] 特性标签标记的属性名（typed and dynamic），返回类型为Dictionary<string, object>
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetMembers()
    {
      //typed members
      var dic = new Dictionary<string, object>();
      var pinfos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetCustomAttribute(typeof(SchemaIgnore)) == null && x.Name != "Item");
      foreach (var pi in pinfos)
        dic.Add(pi.Name, pi.GetValue(this));
      //dynamic members
      foreach (var kvp in properties)
        dic.Add(kvp.Key, kvp.Value);
      return dic;
    }

    /// <summary>
    /// 仅获取动态属性名（dynamic）
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetDynamicMembers()
    {
      foreach (var kvp in properties)
        yield return kvp.Key;
    }

        /// <summary>
        /// 目前，我们假设只有1个属性意味着我们自动将其包装在DynamicBase中，因此是一个包装器。这在未来可能会改变。
        /// </summary>
        public bool IsWrapper() => properties.Count == 1;

  }

}
