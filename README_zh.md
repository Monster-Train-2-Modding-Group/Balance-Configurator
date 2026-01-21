# 平衡性数值编辑

[![GitHub Release](https://img.shields.io/github/v/release/Monster-Train-2-Modding-Group/Balance-Configurator?color=4CAF50&label=latest)](https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/releases)
[![Trainworks Reloaded](https://img.shields.io/badge/framework-Trainworks--Reloaded-blue?logo=github)](https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded)
[![License](https://img.shields.io/github/license/Monster-Train-2-Modding-Group/Balance-Configurator?color=lightgrey)](https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/blob/main/LICENSE)
[![Donate](https://img.shields.io/badge/Ko--Fi-brandonandzeus-F16061?color=F16061&logo=ko-fi&style=flat&labelColor=?color=4E4E4E&logoColor=FFFFFF)](https://ko-fi.com/brandonandzeus)

支持的语言: [English](https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/blob/main/README.md) | 简体中文

![icon](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/icon.png)

本非装扮类模组允许玩家修改一些平衡性相关的数据，是一个使用 Trainworks Reloaded 实现用户可编辑配置的示例模组。

![starting](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/starting.png)

![relic_draft](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/relic_reward.png)
<sub>跳过自选神器时获得的金钱由25修改到75</sub>

![champion_upgrade](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/champion_upgrades.png)
<sub>三个勇者升级都出现了！(UI适配结果不太好)</sub>

![deployment](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/deployment.png)
<sub>可以修改余烬数量和部署余烬数量</sub>

![battle](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/battle.png)
<sub>可以修改抽牌数量和手牌数量上限。这里每回合抽 10 张牌，获得 5 点余烬</sub>

![card_draft](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/card_draft.png)
<sub>也可以修改跳过自选卡牌时获得的金钱</sub>

## 功能

可以修改下方这些在开始一轮游戏时的初始情况或数值：

* 卡牌、商店升级、神器的各稀有度的出现权重
  * 示例: 假设普通升级石的出现权重为 70，高级升级石的出现权重为 30，那么就有 $`\frac{70}{70 + 30}`$ 的概率刷新普通升级石，$`\frac{30}{70 + 30}`$ 的概率刷新高级升级石。
* 初始金钱
* 手牌数量上限
* 每回合获得的初始余烬数量
* 初始部署余烬数量
* 余烬数量上限
* 龙族宝藏的初始数量上限
* 龙族宝藏的最大数量上限
* 每回合的初始抽牌数量
* 单位的初始升级栏位数量
* 法术的初始升级栏位数量
* 装备的初始升级栏位数量 *(没有用)*
* 每层每方势力的单位数量上限
* 每层的初始容量值
* 转变器数量上限
* 光之锻炉出现的勇者升级数量
* 主氏族获得经验的倍率
* 盟友氏族获得经验的倍率
* 备选勇者的解锁等级
* 跳过奖励时获得的金钱
* (1.2.0 版本新增) 契约前哨站可选剧情对话的出现概率
* (1.2.0 版本新增) 对话的速度倍率
* (1.2.0 版本新增) 商店各物品各稀有度的价格区间 (卡牌、升级石、神器)
* (1.3.0 版本新增) 历史记录的条目数量上限
* (1.4.0 版本新增) 各个事件的出现权重（对于每个事件对应的英文名称，请参考[wiki](https://monstertrain2.miraheze.org/wiki/Celestial_Alcove)）
* (1.4.0 版本新增) 破碎光环是否也对自选战旗生效
* (1.4.0 版本新增) 自选战旗出现的单位数量
* (1.4.0 版本新增) 从3层开始是否会出现普通卡牌
* (1.4.0 版本新增) 武器商人是否会售卖普通稀有度的装备或房间卡
* (1.4.0 版本新增) 位面挑战、自定义游戏和社区挑战是否也会精通卡牌
* (2.2.0 版本新增) 默认采用的/游戏本体“默认”选项对应的卡组排序方式
* (2.3.0 版本新增) 是否使用可部署卡牌在前的卡组排序方式

## 使用方法

**强烈**建议使用一个模组管理器（Thunderstore Mod Manager、Gale Mod Manager或r2modman）来安装这个模组。如果手动安装，则需要同时安装[Trainworks Reloaded](https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded)。

使用模组管理器安装或者手动安装之后，将通过编辑 `BalanceConfigurator.Plugin.cfg` 文件来修改游戏内数值，默认数值是游戏原版所使用的数值。

* 如果使用模组管理器安装，可以在 config editor 里编辑该文件。

* 如果手动安装，该文件正常会出现在游戏根目录的 BepInEx 文件夹内，一般该文件的位置在类似于 `C:\Program Files (x86)\Steam\steamapps\common\Monster Train 2\BepInEx\config\BalanceConfigurator.Plugin.cfg` 的地方

## 警告

对于所有的非装扮类模组，**不应该**在社区挑战或者每日挑战内使用模组。通过模组在上述挑战中刷得高分会导致您被封禁：[Steam forums post - Don't anger ShinySteve!](https://steamcommunity.com/app/2742830/discussions/0/599653789035669752/)

在上述挑战中，您应当使用模组的默认数值，也就是游戏原版所使用的数值来进行挑战。

***有可能可以修改*** 的数值包括以下这些，因为它们不影响在线挑战：

- 主氏族获得经验的倍率
- 盟友氏族获得经验的倍率
- 备选勇者的解锁等级
- 契约前哨站可选剧情对话的出现概率
- 对话的速度倍率
- 历史记录的条目数量上限
- 位面挑战、自定义游戏和社区挑战是否也会精通卡牌
- 默认采用的/游戏本体“默认”选项对应的卡组排序方式

### **重要内容**

当启用本模组时，按 F8 进行 bug 汇报的功能将无法使用 (仍然可以打开界面，但是提交内容不会发送给游戏开发者)，以防止对游戏开发组 Shiny Shoe 产生困扰。如果你在使用模组时发现了游戏本体的 bug，请通过其他途径汇报，比如 [Steam 社区](https://steamcommunity.com/app/2742830/discussions/1/)或者 discord。

## 免责声明

我们对使用本模组导致的非预期行为或者文件损坏等概不负责，请慎重使用。
