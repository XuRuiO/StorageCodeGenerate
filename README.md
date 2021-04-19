# StorageCodeGenerate

### 简介
    1、项目使用仓储模式设计的，需要根据表生成实体、仓储层、服务层的基本模板，以前都是通过T4模板生成，个人认为T4模板不太好用。
    2、现在通过SqlSugar（支持大佬的ORM框架，https://www.donet5.com/Home/Doc） 可以生成Model、IRepository、Repository、IService、Service 
       的基本模板，大佬的框架源码已经可以生成实体模型（Model）模板，我在源码的基础上加了仓储层、服务层基本模板的生成。
    3、新增新表，需要生成实体、仓储层、服务层的基本模板时，只需要将需要生成的表新增至数组，通过Where条件过滤即可，代码已说明。
    
### 给个星星⭐
    如果这个小工具帮到了小伙伴，请给个⭐鼓励一下❤
    
