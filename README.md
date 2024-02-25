# RogueRPG

### 介绍
这是一个使用Hsenl框架实现的demo

### 框架简介
+ Hsenl框架是一款高性能的框架, 0GC, 由于核心底层使用了Bitlist, 所以组件的判存, 获取, 非常快, 且支持多态. 有丰富的API, 满足需求.
+ 框架不依赖于unity, 即便脱离unity, 核心功能以及大多数功能依然可以正常使用, 方便在服务端上使用框架. (这里说的大部分功能能用, 是指物理系统和网格导航是使用的unity的, 无法单独在服务器使用, 但接入第三方也方便, 后续写完联网部分再接这些第三方库)
+ 框架和用户代码分离, 方便更新框架
+ 系统事件执顺序是根据unity的规则来的, 记住一套规则就行了, 不需要再记一套
+ 工具丰富满足日常开发需求
+ 框架处于开发中, 还有些必要的模块正在开发中, 但如果是只做单机的话, 可以用了. 如果说作为一个成熟框架来说, 许多细节还未完善, 比如文档, 安装, 教程, 还有一些操作应该设计的更简单易用一些, 都需要整理完善, 慢慢来吧.
+ 目标: 让Hsenl成为一款高速、成熟、安全、强大、易用、灵活的框架

### 特色模块

> 特色模块不是指其本身有多复杂, 多必不可少, 而是框架特色模块, 带有强烈的个人风格, 市面上少有同质化的模块, 好用, 我肯定更想多说说了. 推荐使用, 你也可以不用, 如果你有你自己的解决方案的话.

##### 1、组合器系统(Combin)(强烈推荐)

可以实现组件之间的完全解耦合, 假如两个组件需要交互, 我们可以定义一个组合器, 当两个组件满足条件, 比如出现在同一个实体上时, 或父子实体上时, 就会触发组合, 然后我们把交互逻辑写在组合器中, 使用委托的形式, 当任意一个组件离开, 或者父子关系被改变时, 自动断开组合, 实现交互逻辑的热插拔, 同时自动检测委托是否正确移除, 防止内存泄露. 高度优化匹配逻辑, 组合与解组合速度非常快, 也支持自定义组合条件. 当然你也可以把组合器只当成依赖注入去使用, 也没有额外损耗.

意义: 传统对于解耦合, 主要是把功能模块化, 但对于组件与组件的交互, 我们往往还是一套逻辑写在组件里, 虽然解耦了, 但逻辑上却只有一套交互逻辑, 如果需要多套逻辑, 我们就得在组件里实现针对多套不同组件的交互逻辑, 但这又增加了耦合度, 所以我们大概会使用接口, 但那又增加了代码的复杂度, 同时把逻辑定死了, 要改逻辑, 所有的实现都要改. 现在使用组合器后, 我们把交互逻辑单独抽离出来, 单独管理, 用则有, 不用则无, 废弃的代码可以写一份新的交互逻辑覆盖掉原本的交互逻辑, 或者直接ctrl+a注释掉, 如果以后需要可以再随时注释回来. 没有继承, 没有接口, 你可以写很多套组件的交互逻辑, 你可以随意更改某个组合的逻辑, 却不用担心彼此之间的冲突耦合问题.

缺点: 因为要用到底层的高效率判存, 所以和框架本身绑定比较深, 无法单独拿出来使用.

##### 2、有形体系统(Bodied)(一般推荐)

在Combin基础上实现的Bodied系统, 定义了有形体(Bodied)和无形体(Unbodied)的概念, 其实无论是游戏制作中, 还是其他地方, 任何组件都可以分成有形体和无形体, 简单说, 有形体定义了这个实体是什么, 无形体则给他提供能力. 当你把一个组件定义成有形体的时候, 那意为着他就可以代表他挂载的实体, 这也代表一个实体只能有一个有形体, 如此, 在父子关系中, 实际上是一个个的有形体之间的父子关系, 而不是实体之间的.

意义: 他就是帮我们让所属关系更清晰的, 本身并不复杂.

缺点: 存在感不如其他的模块强, 和框架绑定同样颇深. 不想用, 可以无视他.

##### 3、影子函数系统(ShadowFunction)(推荐)

简单说, 在数据程序集里写一个源函数, 然后在另一个逻辑程序集里, 写一个一模一样的影子函数, 这两个函数就连通了, 当你调用源函数的时候, 他会执行到影子函数中去.

 支持async, 支持泛型类, 支持显示接口. 用起来更方便. 速度相比事件系统更快(因为是预先计算好了hashcode key, 所以整个过程只有一次强转). 能快速的定位源函数, 方便查看逻辑. 影子函数连接失败时, 会报错, 提升了安全性. 且该模块不强依赖于框架本身.

意义: 以一种更符合我们直觉的形式, 来实现数据与逻辑的分离, 如果你是要做热重载的话, 那相比传统方案, 没理由不用这个系统.

缺点: 虽然也支持类似广播的功能, 但不如传统事件系统灵活. 这主要与影子系统的设计理念有关, 他就是为了做热重载的, "广播功能"本就不该存在.

##### 4、优先级系统(Prioritizer)(非常推荐)

很久之前写的模块, 所以也维护很久了, 非常好用. 

意义: 主要用于制作非常复杂的技能、状态、动作系统. 技能前摇后摇, 点地板取消平A后摇, LOL吸血鬼W时可以放火箭腰带, 却不能放闪现, 石头人大可以放闪现却不能放火箭腰带, 武器大师摇花手时也可以正常释放其他技能, 吸血鬼放W时却不能放其他技能, 但如果是处于E技能释放状态, 则不会取消E技能, 净化能解引燃, 却不能解蚂蚱的大, 水银能解蚂蚱大, 却不能解引燃, 等等等等这些复杂的逻辑判断, 归根结底都是对优先级的判断, 一般来说, 游戏逻辑中的战斗系统复杂就复杂在各种状态的相互判断, 但有了这个系统, 一切都那么简单, 只需要在配置里改改参数, 就能实现各种复杂的逻辑判断.

针对其原理, 你甚至可以在UI逻辑上使用他, 比如打开了某个UI, 就需要关闭某些UI, 一般, 我们可能会把UI分组, 或者分流程, 但总归可能出现一个不安分的UI, 跟你唱反调, 他们组都关闭的时候, 他偏偏需要留下来, 或者如果有更复杂的上下层关系, 那你也可以使用优先级系统去处理他.

缺点: 没什么缺点, 就是好用.

##### 5、流水线系统(ProcedureLine)(推荐)

流水线系统(ProcedureLine), 该系统相当于Event系统的变种, 增加了热插拔功能, 本身并不复杂, 但带来的效果确实非常实用, 流水线有三个关键字, handle、item、worker. item代表了这条流水线是做什么的, handle是这条流水线上具体要做哪些事, worker则是决定了某个handle是否工作.

意义: 比如现在有条伤害逻辑, 固定的流程有, 计算人物属性 -> 计算对方属性 -> 交给伤害仲裁庭计算伤害 -> 执行掉血 -> 显示掉血信息, 但现在我加了一个增加伤害的BUFF, 这个BUFF可以在伤害时, 附加10%的攻击力, 那我就给他做一个worker + handle, 并把他的顺序排在计算人物属性与计算对方属性的中间, 当我获得这个BUFF的时候, 就把这个worker添加到流水线中, 这样再伤害时, 就会把人物攻击力增加10%再去仲裁庭, 同理, 如果有多种增加伤害的BUFF, 你也可以给他们排个序, 先增加谁, 再增加谁, 定好后, 就不用管了.

缺点: 写起来不友好, 要创建的东西多, 属于先受苦再享受的类型, 所以专门给他做了个代码生成Editor

### 基础模块

> 基础模块多是一些必要的模块, 标配模块, 这些模块并不稀奇

###### 行为树系统

根据知名的行为树插件实现的一个版本, 并在此基础上, 实现了AI行为树、剧本行为树、时间线行为树、极大的增加了代码的复用. 

但有上手门槛, 主要是内置事件的执行时机比较复杂. 但如果你想实现在"Excel里写逻辑"的话, 那就必须用他.

###### 控制器系统

方便用户定义自己的控制指令, 并与Input的输入形成映射关系, 方便做改键系统, 手柄与键盘无缝切换等功能. 依赖于unity的InputSystem包.

系统分为两层, InputContorller, 和Control, 前者是在view层面, 而后者是逻辑层面, 用户输入的信息, 会转换为我们游戏内的ControlCode, 这个过程是必须的.

###### 数值系统

人物属性, 装备数值, 等需要用到数值的地方, 分为Numeric和Node两块, Node相当于是你装备的属性, Numeric相当于你的属性栏, 穿上装备时, 就把Node的数值加到Numeric重新计算出新的最终属性, 并且可以查找属性的来源, 提供了多少属性, 自己的源属性等功能.

###### HTask

单线程的Task, 本想直接用UniTask第三方, 但奈何他和Unity高度绑定, 只能自己写了, Task这个系统的核心功能其实就那些, 所以暂时没有做很多的拓展.

支持多线程异步, 利用了线程池, 所以避免了线程的开启结束带来的损耗. 不建议把长久的逻辑写在里面, 建议把一些高耗时的计算放到多线程去执行. 对于客户端来说, 完全的够用, 不要盲目的追求多线程, 为了那点性能, 可能出现难以预测的隐患, 并不划算.

模块中自带了一个TaskLine, 可以让多线程以非并发的形式执行, 避免的多线程的访问冲突问题, 根据需要选择性使用.

针对需要持久运行的逻辑, 后续会专门搞个多线程模块.

###### 协程系统(Coroutine)

可以脱离unity使用的一个协程, 最大的特点是支持子协程的嵌套, 并自动管理子协程生命周期. 但说实话, 有了await\async后, 我已经很少使用协程了. 但确实有些时候, 协程更适合, 比如需要灵活管理生命周期的时候, 或是处理一个不要求代码连贯性的独立逻辑的时候, 更多的是在处理游戏逻辑的时候, 用他, 且你可以清楚的从inspector中查看当前有多少协程在运行.

###### 流程系统(Procedure)(注意这个和前面的ProcedureLine没什么关系, 只是名字比较像)

状态机FSM, 借鉴了GF框架中的Procedure系统, 当初第一次接触的时候, 对这个印象很深, 所以总是会带着它, 它最大的好处就是让你更清晰的查看你游戏运行的逻辑链, 适合客户端使用, 服务器别用, 浪费资源

###### 物理系统

写了一小部分, 碰撞检测, 射线检测, 不能作为解决方案使用, 但有兴趣的伙伴可以参考学习, 写了很多注释. 框架中确实有一些类似测试的代码, 一方面是作为我的笔记, 方便我需要时温习, 另一方面也是为了给伙伴们参考, 可能会不定时是删除这些笔记.

###### UI系统

一套简单的UI管理系统, 真就非常简单, 我不是太喜欢写UI

###### 事件系统(EventSystem)

框架标配

###### 网络模块

持续开发中...
实现了Tcp网络核心功能, 粘包拆包, 零拷贝, 暴力测试没问题.
明天测试下延迟, 和网络波动, 继续完善代码, 继续实现数据加密, 断线重连等, 封装成易用组件. 再后面实现Kcp, 分布式服务器, 以及寻路、物理系统、东西还真不少...
最后的最后, 再手搓个网游~.~, 是不是就齐活了, 到时候, 再做教程吧

### 插件

Luban配置文件插件

HybridCLR热更插件

YooAsset资源管理插件

MemoryPack序列化库

Protobuf-net序列化库

InputSystem(unity的插件)

FMath(一套符合unity左右手的顶点数学库)

ZString(0GC字符串库)

### 安装教程

1.  下载完完整项目

#### 使用说明

1、直接打开


#### 参与贡献

dcze；

#### 特技

