# 每日结算页面实现指南

## 概述
本项目已实现每日结算功能，当玩家完成一天的游戏后，会显示结算页面统计当天的表现。

## 已完成的代码修改

### 1. SettlementManager.cs (新建)
- 管理结算页面的显示和隐藏
- 统计当天收入和好评数
- 显示游戏状态信息

### 2. GameManager.cs 修改
- `NextDay()` 方法现在显示结算页面而不是直接切换天数
- 新增 `ContinueToNextDay()` 方法处理从结算页面继续游戏
- `AddGoodReview()` 方法现在同时更新当天统计

### 3. GuestManager.cs 修改
- `LoadDayGuests()` 支持加载任意天数的客人数据
- `NextGuest()` 支持多天数据切换
- 获得报酬时同时更新结算管理器的当天统计

## 需要在Unity编辑器中完成的设置

### 创建结算UI预制件

1. **创建Canvas**
   - 在场景中创建 `UI > Canvas`
   - 命名为 "SettlementCanvas"

2. **创建结算面板**
   - 在Canvas下创建 `UI > Panel`
   - 命名为 "SettlementPanel"
   - 设置合适的尺寸和位置

3. **添加UI元素**
   在SettlementPanel下添加以下TextMeshPro文本元素：

   - DayText: 显示"第X天结算"
   - TotalMoneyText: 显示总金钱
   - DayEarningsText: 显示当天收入
   - TotalGoodReviewsText: 显示总好评数
   - DayGoodReviewsText: 显示当天好评
   - FragmentCountText: 显示碎片数量

4. **添加继续按钮**
   - 创建 `UI > Button`
   - 命名为 "ContinueButton"
   - 添加文本"继续"

5. **创建预制件**
   - 将SettlementCanvas拖到Assets/04_UI/Prefabs/文件夹中创建预制件

### 设置SettlementManager

1. **创建空对象**
   - 在场景中创建空GameObject
   - 命名为 "SettlementManager"
   - 添加SettlementManager脚本

2. **绑定UI组件**
   - 将预制件中的UI元素拖到SettlementManager的对应字段：
     - Settlement Panel
     - Day Text
     - Total Money Text
     - Day Earnings Text
     - Total Good Reviews Text
     - Day Good Reviews Text
     - Fragment Count Text
     - Continue Button

3. **确保单例设置**
   - SettlementManager继承自Singleton，确保场景中只有一个实例

## 游戏流程

1. 玩家完成当天所有客人后，自动调用 `GameManager.Instance.NextDay()`
2. NextDay() 显示结算页面
3. 玩家查看当天统计信息
4. 点击"继续"按钮调用 `ContinueToNextDay()`
5. 切换到下一天，重置统计数据，加载新客人

## 注意事项

- 确保所有UI元素正确绑定到SettlementManager
- 结算页面默认隐藏，只在NextDay()时显示
- 当天统计数据在ContinueToNextDay()时重置
- 支持5天游戏循环