# 使用

## 接入HybridCLR
* https://gitee.com/focus-creative-games/hybridclr_unity.git
* Window > Package Manager > + > add package from git url 
* HybridCLR > installer > 安装

## HybirdCLR 设置
* 关闭增量GC  File > BuildSettings > Player Settings > Using incremental GC
* IL2CPP 打包
* .Net 版本


## 程序入口
* AOT -> GameLoader
  * 加载热更dll
  * 补充元数据
  * 从AOT进入资源管理程序
* ManagerHotfix
  * 资源管理程序
  * Base
    * 各种基类
  * Item
    * 各种Item
  * Manager
    * 资源管理 ResourcesManager
    * 声音管理 AudioManager
    * 配置表管理 ConfigDataManager
    * 下载管理 DownLoadManager
    * 事件中心 EventCenter
    * Model管理 ModelManager
    * Module管理 ModuleManager
    * 提示管理 TipsManager
    * AB包管理 ABManager
  * Utils
    * 日志管理 DebugUtils
    * 工具类 Utils
  * Config 配置
* 游戏逻辑
  * HotFix -> GameManager
  

## 使用
### UI
UI使用Module-View 模式，即一个Module,对应一个VIew再对应一个UIPanel的路径。整个Module的开启销毁由ModuleManager进行管理。一般新建一个UI的流程如下：
* 拼图，保存为prefab
* 新建Module，继承BaseModule,核心代码如下：
  * GetView 中需要返回需要挂载的脚本 如下面中的xxxView
  ``` 
  public class xxxModule : BaseModule
    {
        public xxxModule()
        {
            PreFabs = "这是prefabs路径";
        }

        public override Type GetView()
        {
            return typeof(这是需要挂载的脚本);
        }
    }
  ```
* 新建View 继承BaseView ,核心代码如下：
  * BaseView 声明了几个抽象类
  * ShowView  在Awake时执行，用于打开脚本时传值，可直接替代awake 
  * AddEventListener 用于统一添加事件 awake时执行
  * RemoveEventListener 用于统一移除事件 destory时执行
  * OnBtnClickEvent  用于统一添加按钮 awake时执行
   ```
   public class xxxView : BaseView
    {
		public override void AddEventListener()
        {
        }

        public override void ShowView(object obj)
        {
        }

        public override void OnBtnClickEvent()
        {
        }

        public override void RemoveEventListener()
        {
        }

	}
   ```

做完前面两步，就可以使用ModuleManager对一个新的UI进行管理。
* 打开
  * 打开的Module
  * 打开后的回调
  * 打开时传递的参数，用来控制不同的效果
``` 
ModuleManager.GetInstance().OpenModule(typeof(xxxModule),()=> {},object);
```

* 销毁
```
ModuleManager.GetInstance().CloseModule<MainModule>();
```



## 数据
数据使用Model，用来与UI控制逻辑分离。代码如下：
* Init 用来初始化数据
* ResetModel 用来重置数据

```
public class xxxModel : BaseModel
    {
		public override void Init()
        {
        }

        public override void ResetModel()
        {
        }
	}
```

一般来说，一个功能对应一个Model，可以有多个Module。多个Module通过Model共享数据。并使用ModelManager来管理。获取数据一般可以这么写：
```
ModelManager.GetInstance().GetModelInstence<xxxModel>()
```


### 事件中心

事件中心定义前需要先声明事件，如:

```
# 事件生命可以在每个功能的 Model中声明，建议后面使用小写方便阅读
public static  string NEXT_DOWNLOAD_COROUTINE = "next_download_coroutine";
```

目前事件中心为了简单，只有3个方法：
1.添加监听
```
EventCenter.GetInstance().AddEventListener(NEXT_DOWNLOAD_COROUTINE, StartNextIE);
```
2.移除监听
```
EventCenter.GetInstance().RemoveEventListener(NEXT_DOWNLOAD_COROUTINE, StartNextIE);
```
3.发送触发事件
```
EventCenter.GetInstance().EventTrigger(DownLoadManager.NEXT_DOWNLOAD_COROUTINE);
```

添加和移除可以在View的AddEventListener 与 RemoveEventListener中写入


### 配置表管理
* excel格式
  * 数据类型展示支持int、string。
  * 表格第一列数据 是控制是否导出该配置
  * 表格第一行为数据类型。
  * 第二行为提示内容summary
  * 第三行为使用名称
  * 第四行开始是数据
 
  例如
  |bool| int | string  | string  | string  |
  |----|----|----|----|----|
  |是否导出|唯一id|名称|iconUrl|ModdelUrl|
  |1|id|name|icon|modul|
  |1|1|dada1|123456|456789|
  |0|2|dada2|123456|456789|
  |1|3|dada3|123456|456789|
  |0|4|dada4|123456|456789|

  导出后的表格为：
  |id|name|icon|modul|
  |----|----|----|----|
  |1|dada1|123456|456789|
  |3|dada3|123456|456789|



* 通过python脚本生成txt和CSharp文件
  * 通过传递下列参数执行
  ```
    --excelpath EXCELPATH  #excel保存路径
    --cspath CSPATH        #生成的Csharp文件保存路径
    --txtpath TXTPATH      #生成的txt文件保存路径
    --extension EXTENSION  #excel的后缀名称，默认为xls
  ```
* 编辑器工具
  * 使用编辑器工具是为了更加无缝地生成文件
  * 主要就是使用C#运行Cmd命令，从而运行python脚本。
  * 所以，需要在编辑器工具ExcelReaderTools中获取python脚本运行时需要的参数。
  * 然后进行组装。
  * 运行 Process.Start("CMD.exe", "/k " + cmdStr);
* 配置表管理工具
  * TableDataManager.cs
  * 使用反射,创建新的BaseTable对象,填充数据
  * 将数据保存到一个新的BaseTable的data中，并使用字典缓存起来
  * 读取表数据
  * 使用 
```
  TestData data = TableDataManager.GetInstance().GetTableDataByType<TestData>();
```


### 提示管理

提示管理用来生成游戏中的各类提示，通过配置表配置提示内容，一般直接使用配置表中的id即可。
```
TipsManager.GetInstance().ShowTipsByID(1001);
```
如果想获取对应提示的内容，还可以直接使用
```
TipsManager.GetInstance().GetTipStringByID(1001);
```
提示管理其实也是Module-View组合成的。


### 资源管理

游戏中所有的资源加载都使用ResourcesManager进行加载。
* 加载GameObject
  ```
  GameObject go = ResourcesManager.GetPrefab(path);
  ```
* 加载Sprite
  ```
  Sprite sp = ResourcesManager.GetSprite(path);
  ```

在这里分为打包模式与编辑器模式，打包模式是指使用AB包进行加载资源,编辑器模式是指使用AssetDatabase进行加载各种资源。打AB包以及打APK时注意切换为打包模式。

### 打包工具