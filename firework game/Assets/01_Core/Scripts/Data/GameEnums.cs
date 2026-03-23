using UnityEngine;
using System.Collections.Generic; // 新增：用于映射工具类的List

/// <summary>
/// 游戏所有固定类型的枚举定义（统一数据规范）
/// </summary>
public enum FireworkComponent
{
    // 🔥 火药类型
    NormalPowder,    // 普通火药（基础组件）
    PushPowder,      // 推进火药（Day2解锁）
    SmokePowder,     // 烟雾火药（Day3解锁）

    // 🧵 引线类型
    ShortFuse,       // 短引线（基础组件）
    LongFuse,        // 长引线（Day2解锁）

    // 🌈 彩珠颜色
    RedBead,         // 红彩珠（基础组件）
    BlueBead,        // 蓝彩珠（基础组件）
    GreenBead,       // 绿彩珠（基础组件）
    PurpleBead,      // 紫彩珠（Day2解锁）
    GoldBead,        // 金彩珠（基础组件）
    SilverBead,      // 银白珠（Day2解锁，顾文才需求）

    // 📦 外壳形状
    CylinderShell,   // 圆柱外壳（基础组件）
    PyramidShell,    // 圆锥外壳（基础组件）
    RoundShell,      // 圆形外壳（基础组件）
    HeartShell,      // 心形外壳（Day4解锁）
    EllipseShell,    // 椭圆外壳（Day4解锁）

    // 🧩 其他组件
    Partition,       // 隔板（Day3解锁）
    RedPaper,        // 红色彩纸（基础组件）
    YellowPaper,     // 黄色彩纸（基础组件）
    GreenPaper,       // 绿色彩纸（基础组件）
    Clay             //粘土
}

/// <summary>
/// 烟花组件类别（用于“某类别选一个即可”的判定）
/// </summary>
public enum ComponentCategory
{
    None,               // 无类别（仅用于兜底）
    Powder,             // 火药类（普通/推进/烟雾火药）
    Fuse,               // 引线类（短/长引线）
    Bead,               // 彩珠类（红/蓝/绿/紫/金/银）
    Shell,              // 外壳类（圆柱/圆锥/圆形/心形/椭圆）
    Paper,              // 彩纸类（红/黄/绿）
    Partition           // 隔板类（仅隔板）
}

/// <summary>
/// 烟花制作判定结果（对应策划的顾客评价标准）
/// </summary>
public enum JudgeResult
{
    Perfect,   // 完美：核心+次要全满足
    Good,      // 良好：核心满足，次要部分满足
    Fail,      // 失败：核心不满足
    TimeOut    // 超时：1分30秒未完成
}

/// <summary>
/// 游戏天数（对应策划的五天安排）
/// </summary>
public enum GameDay
{
    Day1,      // 第一天（新手引导，3个普通客人）
    Day2,      // 第二天（解锁推进火药/长引线，5个普通客人+顾文才）
    Day3,      // 第三天（解锁烟雾火药/隔板，5个普通客人+英娥&松哥儿）
    Day4,      // 第四天（解锁心形/椭圆外壳，5个普通客人+甘昭）
    Day5       // 第五天（结局，4个普通客人+山魈+布姥姥）
}

/// <summary>
/// 组件-类别映射工具类（统一管理组件所属类别，放在枚举文件中更易维护）
/// </summary>
public static class ComponentCategoryHelper
{
    /// <summary>
    /// 获取组件所属的类别
    /// </summary>
    public static ComponentCategory GetCategory(FireworkComponent component)
    {
        return component switch
        {
            // 火药类
            FireworkComponent.NormalPowder or FireworkComponent.PushPowder or FireworkComponent.SmokePowder => ComponentCategory.Powder,
            // 引线类
            FireworkComponent.ShortFuse or FireworkComponent.LongFuse => ComponentCategory.Fuse,
            // 彩珠类
            FireworkComponent.RedBead or FireworkComponent.BlueBead or FireworkComponent.GreenBead or
            FireworkComponent.PurpleBead or FireworkComponent.GoldBead or FireworkComponent.SilverBead => ComponentCategory.Bead,
            // 外壳类
            FireworkComponent.CylinderShell or FireworkComponent.PyramidShell or FireworkComponent.RoundShell or
            FireworkComponent.HeartShell or FireworkComponent.EllipseShell => ComponentCategory.Shell,
            // 彩纸类
            FireworkComponent.RedPaper or FireworkComponent.YellowPaper or FireworkComponent.GreenPaper => ComponentCategory.Paper,
            // 隔板类
            FireworkComponent.Partition => ComponentCategory.Partition,
            // 默认无类别（防止漏配组件）
            _ => ComponentCategory.None
        };
    }

    /// <summary>
    /// 获取某类别下的所有组件
    /// </summary>
    public static List<FireworkComponent> GetComponentsInCategory(ComponentCategory category)
    {
        List<FireworkComponent> components = new List<FireworkComponent>();
        // 遍历所有组件，筛选出对应类别的组件
        foreach (FireworkComponent comp in System.Enum.GetValues(typeof(FireworkComponent)))
        {
            if (GetCategory(comp) == category)
            {
                components.Add(comp);
            }
        }
        return components;
    }
}