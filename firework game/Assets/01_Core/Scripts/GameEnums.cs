using UnityEngine;

//存放所有固定类型的枚举
// 判定结果枚举
public enum JudgeResult
{
    Perfect,   // 完美（核心+次要全满足）
    Good,      // 良好（核心满足，次要部分满足）
    Fail,      // 失败（核心不满足）
    TimeOut    // 超时
}

// 烟花组件枚举（对应策划中的材料）
public enum FireworkComponent
{
    // 火药类型
    ExplodePowder,    // 普通火药
    ThrustPowder,      // 推进火药
    SmokePowder,     // 烟雾火药
    // 引线类型
    ShortFuse,       // 短引线
    LongFuse,        // 长引线
    // 彩珠颜色
    RedBead, BlueBead, GreenBead, PurpleBead, GoldBead, LightBlueBead,
    // 外壳形状
    CylinderShell, PyramidShell, RoundShell,
    // 彩纸/隔板
    BluePaper, YellowPaper, GreenPaper
}

// 游戏天数枚举
public enum GameDay
{
    Day1, Day2, Day3, Day4, Day5
}