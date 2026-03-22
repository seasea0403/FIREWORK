using UnityEngine;

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
    GreenPaper       // 绿色彩纸（基础组件）
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