using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 判定管理器：负责对玩家制作的烟花组件与当前客人需求进行匹配判断。
/// 包含最终判定逻辑和各类需求校验函数。
/// </summary>
public class JudgeManager : Singleton<JudgeManager>
{
    /// <summary>
    /// 对当前客人需求和选中组件进行判定，返回最终评价结果。
    /// </summary>
    /// <param name="selectedComponents">玩家当前选中的组件列表。</param>
    /// <param name="guestDemand">当前客人的需求数据对象。</param>
    /// <returns>判定结果，可能为 Perfect、Good 或 Fail。</returns>
    public JudgeResult JudgeFirework(List<FireworkComponent> selectedComponents, GuestDemandSO guestDemand)
    {
        // 0. 参数校验：如果组件或需求为空，则直接判定失败。
        if (selectedComponents == null || selectedComponents.Count == 0 || guestDemand == null)
        {
            Debug.LogError("判定失败：组件或客人需求为空。");
            return JudgeResult.Fail;
        }

        // 1. 检查是否存在禁止组件，禁止组件直接判定失败。
        if (HasForbiddenComponents(selectedComponents, guestDemand))
        {
            return JudgeResult.Fail;
        }

        // 2. 检查核心需求是否满足，核心需求不满足直接判定失败。
        bool corePass = CheckCoreRequirements(selectedComponents, guestDemand);
        if (!corePass)
        {
            return JudgeResult.Fail;
        }

        // 3. 如果该客人没有次要需求，则核心需求满足即为完美。
        bool hasSecondary = HasSecondaryRequirements(guestDemand);
        if (!hasSecondary)
        {
            if (guestDemand.isSpecialNPC)
            {
                GameManager.Instance.AddFragment();
            }
            return JudgeResult.Perfect;
        }

        // 4. 检查次要需求。
        bool secondaryPass = CheckSecondaryRequirements(selectedComponents, guestDemand);
        if (secondaryPass)
        {
            if (guestDemand.isSpecialNPC)
            {
                GameManager.Instance.AddFragment();
            }
            return JudgeResult.Perfect;
        }

        return JudgeResult.Good;
    }

    #region 判定子逻辑
    /// <summary>
    /// 检查玩家选中组件中是否包含客人需求中的禁止组件或禁止类别中的任何组件。
    /// </summary>
    private bool HasForbiddenComponents(List<FireworkComponent> selected, GuestDemandSO demand)
    {
        if (demand == null) return false;

        if (demand.forbiddenSpecificComponents != null && demand.forbiddenSpecificComponents.Count > 0)
        {
            foreach (var comp in demand.forbiddenSpecificComponents)
            {
                if (selected.Contains(comp))
                {
                    Debug.Log($"判定失败：包含禁止组件 {comp}。");
                    return true;
                }
            }
        }

        if (demand.forbiddenCategoryAnyOne != null && demand.forbiddenCategoryAnyOne.Count > 0)
        {
            foreach (var forbiddenCategory in demand.forbiddenCategoryAnyOne)
            {
                List<FireworkComponent> categoryComps = ComponentCategoryHelper.GetComponentsInCategory(forbiddenCategory);
                if (categoryComps == null || categoryComps.Count == 0) continue;

                foreach (var selectedComp in selected)
                {
                    if (categoryComps.Contains(selectedComp))
                    {
                        Debug.Log($"判定失败：包含禁止类别 {forbiddenCategory} 中的组件 {selectedComp}。");
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 检查玩家是否满足客人需求的核心组件要求。
    /// 包括指定组件和指定类别至少一个组件。
    /// </summary>
    private bool CheckCoreRequirements(List<FireworkComponent> selected, GuestDemandSO demand)
    {
        foreach (var comp in demand.coreSpecificComponents)
        {
            if (!selected.Contains(comp))
            {
                Debug.Log($"判定失败：缺少核心指定组件 {comp}。");
                return false;
            }
        }

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
                Debug.Log($"判定失败：缺少核心类别 {category} 的任意组件。");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 判断当前客人是否存在次要需求。
    /// </summary>
    private bool HasSecondaryRequirements(GuestDemandSO demand)
    {
        bool hasSpecific = demand.secondarySpecificComponents != null && demand.secondarySpecificComponents.Count > 0;
        bool hasCategory = demand.secondaryCategoryAnyOne != null && demand.secondaryCategoryAnyOne.Count > 0;
        return hasSpecific || hasCategory;
    }

    /// <summary>
    /// 检查玩家是否满足客人需求的次要组件要求。
    /// 次要要求可以是指定组件或指定类别中的任意组件。
    /// </summary>
    private bool CheckSecondaryRequirements(List<FireworkComponent> selected, GuestDemandSO demand)
    {
        if (demand.secondarySpecificComponents != null && demand.secondarySpecificComponents.Count > 0)
        {
            foreach (var comp in demand.secondarySpecificComponents)
            {
                if (!selected.Contains(comp))
                {
                    Debug.Log($"次要判定失败：缺少指定组件 {comp}。");
                    return false;
                }
            }
        }

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
                    Debug.Log($"次要判定失败：缺少类别 {category} 中的任意组件。");
                    return false;
                }
            }
        }

        return true;
    }
    #endregion
}