using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptExtends {
    /// <summary>
    /// 将类 转化为 枚举中的值
    /// </summary>
    /// <typeparam name="E">枚举</typeparam>
    /// <typeparam name="Type">类</typeparam>
    /// <param name="deleteStr">类中要删去的结尾字符串</param>
    /// <returns></returns>
    public static E TypeToEnum<E,Type>(string deleteStr) {
        int mi = typeof(Type).Name.LastIndexOf(deleteStr);
        string name = typeof(Type).Name.Substring(0, mi);
        return (E)Enum.Parse(typeof(E), name);
    }


}
