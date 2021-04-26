# LanguageFrameworkForUnity
> 基于Unity2020.2.2f1c1开发的多语言框架，支持实时动态修改Image、Text。

>支持异步加载(宏:UseAsync)
>
>资源读取方式有Resource、AssetBundle、Addressable(使用该方式必须使用宏UseAsync;UseAD，同时也只能使用异步)
>
>该框架使用了DynamicJson.dll、UniTask、Addressable(可选)，同时也使用dynamic动态对象，所以必须用.Net4(或修改Json序列化方式)

> 包含组件有:

>>   * LanguagePanel
>>     			- 根据配置路径修改指定资源
>>   * LanguageImage
>>        * 通过多语言框架获取相应语言的指定资源，并且切换语言时也会重新获取资源
>>   * LanguageLabel
>>     				* 通过多语言框架获取相应语言的指定文本，并且切换语言时也会重新获取文本

----

## LanguagePanel组件
* 作用：通过节点路径给相应的组件动态修改相应的内容，一般用作一次修改。

* 使用需求：

       1. Json配置
       2. language.json配置
       3. 给相应的节点挂上LanguagePanel脚本并赋予json文件路径
* 使用方法：首先配置好UI和Json文件,然后在任意start生命周期中LanguageManager._Instance.InitPanel一下即可(注意优先级得比LanguageManager.Init低)，可通过LanguageManager._Instance.SwitchLanguage来动态切换语言。



----

## LanguagePanel配置规则

例子： {

​       	 	"UICompontents": [

​						{

​							"path":"this",

​               			 "uiType": "Image",

​							*"bundleName"*: "Resources",

​                			"resName": "test"

​						},

​						{

​							"path”:"bg/image1+bg/image2",

​               			 "uiType": "Image",

​							"bundleName”:"Resources"

​                			"resName": "test"

​						}

​				]

​			}



2. UICompontents为必选项，是一个数组
2. path:节点路径,this为挂载LanguagePanel组件的节点
3. path有多个节点使用到相同资源时可用+号
4. uiType类型有Image、Font(可自定，然后在LanguageFramework.InitComponent中添加相应处理)
5. bundleName为资源所在包名,"Resources"为通过Resources读取，其他为AssetBundle或Addressable读取
6. resName为资源路径(如果有文件夹的话)+名称

----

## language.json配置说明

* 作用：配置各个语言所用的文本，可通过id获得相应的文本。

* 每个对象对应一个id(这里的id是给UICompontents的resName项用的)，之后的语言是可选的。

* 只能存在一个language.json，默认放在Resources根目录下，也可以自己到LanguageManager修改路径。

* 可用LanguageLabel组件或LanguageManager._Instance.GetLanguageLabel(id)来获取文本。

* 示例：

  {

  ​    "languages":[

  ​        {

  ​            "id":"test",

  ​            "cn": "哈哈哈哈哈哈哈哈",

  ​            "en": "hahahahahahaha"

  ​        },

  ​		{
  
  ​			 "id":"test2",
  
  ​			 "cn": "中文",
  
  ​             "en": "英文"
  
  ​        }
  
  ​	]
  
  }

