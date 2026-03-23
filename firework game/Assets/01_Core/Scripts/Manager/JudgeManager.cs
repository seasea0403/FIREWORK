using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 判定管理器（继承泛型单例，全局唯一）
/// </summary>
public class JudgeManager : Singleton<JudgeManager>
{
    /// <summary>
    /// 执行烟花判定（外部调用的核心方法）
    /// </summary>
    /// <param name="selectedComponents">玩家选中的组件列表</param>
    /// <param name="guestDemand">客人的需求（核心/次要判定）</param>
    /// <returns>判定结果</returns>
    public JudgeResult JudgeFirework(List<FireworkComponent> selectedComponents, GuestDemandSO guestDemand)
    {
        // 0. 前置检查：参数为空直接判定失败
        if (selectedComponents == null || selectedComponents.Count == 0 || guestDemand == null)
        {
            Debug.LogError("判定失败：参数为空！");
            return JudgeResult.Fail;
        }

        // 1. 检查禁止组件：包含则直接失败
        if (HasForbiddenComponents(selectedComponents, guestDemand))
        {
            return JudgeResult.Fail;
        }

        // 2. 检查核心判定：不满足则失败
        bool corePass = CheckCoreRequirements(selectedComponents, guestDemand);
        if (!corePass)
        {
            return JudgeResult.Fail;
        }

        // 3. 检查是否有次要判定：无则直接完美
        bool hasSecondary = HasSecondaryRequirements(guestDemand);
        if (!hasSecondary)
        {
            // 特殊NPC判定完美加碎片
            if (guestDemand.isSpecialNPC)
            {
                GameManager.Instance.AddFragment();
            }
            return JudgeResult.Perfect;
        }

        // 4. 有次要判定：检查次要判定是否全过
        bool secondaryPass = CheckSecondaryRequirements(selectedComponents, guestDemand);
        if (secondaryPass)
        {
            if (guestDemand.isSpecialNPC)
            {
                GameManager.Instance.AddFragment();
            }
            return JudgeResult.Perfect;
        }
        else
        {
            return JudgeResult.Good;
        }
    }

    #region 判定辅助方法（重点修改HasForbiddenComponents）
    /// <summary>
    /// 检查是否包含禁止组件（适配新的GuestDemandSO：禁止指定组件 + 禁止类别组件）
    /// </summary>
    private bool HasForbiddenComponents(List<FireworkComponent> selected, GuestDemandSO demand)
    {
        // 前置空值校验：避免配置为空时报错
        if (demand == null) return false;

        // 1. 检查禁止-指定单个组件（原逻辑适配字段名）
        if (demand.forbiddenSpecificComponents != null && demand.forbiddenSpecificComponents.Count > 0)
        {
            foreach (var comp in demand.forbiddenSpecificComponents)
            {
                if (selected.Contains(comp))
                {
                    Debug.Log($"判定失败：包含禁止组件【{comp}】！");
                    return true;
                }
            }
        }

        // 2. 新增：检查禁止-类别组件（该类别下的所有组件都不能包含）
        if (demand.forbiddenCategoryAnyOne != null && demand.forbiddenCategoryAnyOne.Count > 0)
        {
            foreach (var forbiddenCategory in demand.forbiddenCategoryAnyOne)
            {
                // 获取该禁止类别下的所有组件（比如引线类包含短引线、长引线）
                List<FireworkComponent> categoryComps = ComponentCategoryHelper.GetComponentsInCategory(forbiddenCategory);
                if (categoryComps == null || categoryComps.Count == 0) continue;

                // 检查玩家选中的组件是否包含该类别下的任意一个
                foreach (var selectedComp in selected)
                {
                    if (categoryComps.Contains(selectedComp))
                    {
                        Debug.Log($"判定失败：包含禁止类别【{forbiddenCategory}】下的组件【{selectedComp}】！");
                        return true;
                    }
                }
            }
        }

        // 无禁止组件
        return false;
    }

    /// <summary>
    /// 检查核心判定（指定单个+类别任选）
    /// </summary>
    private bool CheckCoreRequirements(List<FireworkComponent> selected, GuestDemandSO demand)
    {
        // 检查核心-指定单个：必须全部包含
        foreach (var comp in demand.coreSpecificComponents)
        {
            if (!selected.Contains(comp))
            {
                Debug.Log($"核心判定失败：缺少指定组件{comp}！");
                return false;
            }
        }

        // 检查核心-类别任选：每个类别至少包含一个
        foreach (var category in demand.coreCategoryAnyOne)
        {
            List<FireworkComponent> categoryComps = ComponentCategoryHelper.GetComponentsInCategory(category);
            bool hasAny = false;
            foreach (var comp in selected)
            {
                if (categoryComps.Contains(comp))
                {
                    hasAny = true;
                    break;
                }
            }
            if (!hasAny)
            {
                Debug.Log($"核心判定失败：缺少{category}类别组件！");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 检查是否有次要判定
    /// </summary>
    private bool HasSecondaryRequirements(GuestDemandSO demand)
    {
        bool hasSpecific = demand.secondarySpecificComponents != null && demand.secondarySpecificComponents.Count > 0;
        bool hasCategory = demand.secondaryCategoryAnyOne != null && demand.secondaryCategoryAnyOne.Count > 0;
        return hasSpecific || hasCategory;
    }

    /// <summary>
    /// 检查次要判定（指定单个+类别任选）
    /// </summary>
    private bool CheckSecondaryRequirements(List<FireworkComponent> selected, GuestDemandSO demand)
    {
        // 检查次要-指定单个：必须全部包含
        if (demand.secondarySpecificComponents != null && demand.secondarySpecificComponents.Count > 0)
        {
            foreach (var comp in demand.secondarySpecificComponents)
            {
                if (!selected.Contains(comp))
                {
                    Debug.Log($"次要判定失败：缺少指定组件{comp}！");
                    return false;
                }
            }
        }

        // 检查次要-类别任选：每个类别至少包含一个
        if (demand.secondaryCategoryAnyOne != null && demand.secondaryCategoryAnyOne.Count > 0)
        {
            foreach (var category in demand.secondaryCategoryAnyOne)
            {
                List<FireworkComponent> categoryComps = ComponentCategoryHelper.GetComponentsInCategory(category);
                bool hasAny = false;
                foreach (var comp in selected)
                {
                    if (categoryComps.Contains(comp))
                    {
                        hasAny = true;
                        break;
                    }
                }
                if (!hasAny)
                {
                    Debug.Log($"次要判定失败：缺少{category}类别组件！");
                    return false;
                }
            }
        }

        return true;
    }
    #endregion
}