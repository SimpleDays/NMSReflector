# NMSReflector
##简介：
  在我们遍历reader，excel表格，文件解析等其他需求时，难免要根据列名去给model赋值，为了简化操作，写了这个类库。考虑到性能，采用反射输出技术；考虑到易用性，本库对object类型做了扩展，以方便程序员使用。



##案例1：
```C#
    public class Student : INMSReflector
    {
        public string Name;
        public string Description { get; set; }

        public static string StaticField;
        public static string StaticProperty { get; set; }
    }
```  
###引用步骤:
```html
  Step1 :  引用类库.
  Step2 :  using NMSReflector.
  Step3 :  将你的类实现INMSReflector接口；(当然了，如果你嫌麻烦，可以改一下源码，在ModelOperator.cs中).
  Step4 :  用Create方法创建缓存. (会扫描搜索入口程序集的所有类)
``` 

###用法:
```C#
   ModelOperator.Create();
   Student t = new Student();

   //普通字段
   t.Name = "小明";
   t.EmitSet("Name", "小明胸前的红领巾更加鲜艳了！");
   Console.WriteLine(t.Name);
   Console.WriteLine(t.EmitGet("Name"));

   //普通属性
   t.EmitSet("Description", "他爱着小刚");
   Console.WriteLine(t.Description);
   Console.WriteLine(t.EmitGet("Description"));

   //静态字段
   t.EmitSet("StaticFiled", "是他挨着小刚");
   Console.WriteLine(Student.StaticField);
   Console.WriteLine(t.EmitGet("StaticField"));

   //静态属性
   t.EmitSet("StaticProperty", "刚才打错了");
   Console.WriteLine(Student.StaticProperty);
   Console.WriteLine(t.EmitGet("StaticProperty"));
  
``` 
###结果：
![](https://github.com/NMSLanX/ImageCache/blob/master/NMSReflector%E6%A1%88%E4%BE%8B1.PNG)

##案例2:
```C#
    public class Student : INMSReflector
    {
        public string Name;
        [Column("Note")]
        public string Description { get; set; }

        public static string StaticField;

        public static string StaticProperty { get; set; }
    }
``` 
###注意：
```HTML
  这里的标签是来自于System.ComponentModel.DataAnnotations.Schema;
  所以需要using System.ComponentModel.DataAnnotations.Schema;
```

###用法：
```C#
    ModelOperator.Create();
    Student t = new Student();

    t.EmitSet("Note", "设置标签");
    Console.WriteLine(t.Description);
    Console.WriteLine(t.EmitGet("Note"));
```
###结果：
![](https://github.com/NMSLanX/ImageCache/blob/master/NMSReflector%E6%A1%88%E4%BE%8B2.PNG)

##其他：

```C#
    //获取真实属性名
    ModelOperator.GetRealName<Student>("Note");
    //获取映射名
    ModelOperator.GetMapName<Student>("Description");
    //其他更多方法详见ModelOperator.cs 注释已经写好
``` 
##性能测试:
![](https://github.com/NMSLanX/ImageCache/blob/master/NMSReflector%E6%A1%88%E4%BE%8B3.PNG)

##优点：
```HTML 
   就是看着放心，用着舒心....
```
*Copyright 2016 Xin Hu (2765968624@qq.com)*
