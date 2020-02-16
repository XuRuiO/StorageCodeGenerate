﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace StorageCodeGenerate
{
    public class CodeGenerate
    {
        private SqlSugarClient Db;

        public void Init()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Server=.;Database=DBCRM;User ID=sa;Password=123456;",       //连接字符串
                DbType = DbType.SqlServer,      //数据库类型
                IsAutoCloseConnection = true,     //(默认false)是否自动释放数据库，设为true我们不需要close或者Using的操作，比较推荐
                InitKeyType = InitKeyType.SystemTable
            });

            foreach (var item in db.DbMaintenance.GetTableInfoList())
            {
                string entityName = item.Name.Replace("T_", "");
                db.MappingTables.Add(entityName, item.Name);
            }

            Db = db;
        }

        /// <summary>
        /// 生成Model模型
        /// </summary>
        /// <param name="db"></param>
        public void GenerateModelCode()
        {
            //第一次可以创建所有的表，以后加表的话可以通过集合，通过 Db.DbFirst.Where() 控制创建需要生成的实体表
            //后面的IRepository、Repository、IService、Service控制同理
            //var entityNameList = new List<string>()
            //{
            //    "T_User"
            //};

            Db.DbFirst
                //.Where(entityNameList.ToArray())
                .IsCreateAttribute()        //是否需要引入命名控件：   using SqlSugar;
                .SettingClassTemplate(old =>
                {
                    return "{using}\r\n" +
                           "namespace {Namespace}\r\n" +
                           "{\r\n" +
                           "{ClassDescription}{SugarTable}\r\n" +
                           DbFirstTemplate.ClassSpace + "public partial class {ClassName}\r\n" +
                           DbFirstTemplate.ClassSpace + "{\r\n" +
                           "{PropertyName}" +
                           DbFirstTemplate.ClassSpace + "}\r\n" +
                           "}\r\n";
                })
                .SettingNamespaceTemplate(old => { return old; })
                .SettingPropertyDescriptionTemplate(old =>
                {
                    return DbFirstTemplate.PropertySpace + "/// <summary>\r\n" +
                           DbFirstTemplate.PropertySpace + "/// {PropertyDescription}\r\n" +
                           DbFirstTemplate.PropertySpace + "/// </summary>\r\n";
                })
                .SettingPropertyTemplate(old =>
                {
                    return DbFirstTemplate.PropertySpace + "public {PropertyType} {PropertyName} { get; set; }\r\n";
                })
                .SettingConstructorTemplate(old => { return old; })
                .CreateClassFile(@"E:\SqlSugarGenerateCode\Models", "CRM.Model.Models");
        }

        /// <summary>
        /// 生成IRepository仓储接口层
        /// </summary>
        /// <param name="db"></param>
        public void GenerateIRepositoryCode()
        {
            //文件名称
            string fileName = "I{ClassName}Repository";

            Db.DbFirst
                .IsCreateAttribute(false)       //是否需要引入命名控件：   using SqlSugar; 
                .SettingClassTemplate(old =>
                {
                    return "{using}\r\n" +
                           "namespace {Namespace}\r\n" +
                           "{\r\n" +
                           DbFirstTemplate.ClassSpace + "public interface I{ClassName}Repository : IBaseRepository<{ClassName}>\r\n" +
                           DbFirstTemplate.ClassSpace + "{\r\n" +
                           "\r\n" +
                           DbFirstTemplate.ClassSpace + "}\r\n" +
                           "}\r\n";
                })
                .SettingNamespaceTemplate(old =>
                {
                    //自定义命名Using的内容
                    var UserDefinedTemp = "using CRM.IRepository.IBase;\r\n" +
                                          "using CRM.Model.Models;\r\n";

                    StringBuilder sb = new StringBuilder();
                    sb.Append(old).Append(UserDefinedTemp);

                    return sb.ToString();
                })
                .CreateIRepositoryIService(@"E:\SqlSugarGenerateCode\IRepositorys", fileName, "CRM.IRepository.IRepositorys");
        }

        /// <summary>
        /// 生成Repository仓储接口实现层
        /// </summary>
        /// <param name="db"></param>
        public void GenerateRepositoryCode()
        {
            //文件名称
            string fileName = "{ClassName}Repository";

            Db.DbFirst
                .IsCreateAttribute(false)       //是否需要引入命名控件：   using SqlSugar;
                .SettingClassTemplate(old =>
                {
                    return "{using}\r\n" +
                           "namespace {Namespace}\r\n" +
                           "{\r\n" +
                           DbFirstTemplate.ClassSpace + "public class {ClassName}Repository : BaseRepository<{ClassName}>, I{ClassName}Repository\r\n" +
                           DbFirstTemplate.ClassSpace + "{\r\n" +
                           "\r\n" +
                           DbFirstTemplate.ClassSpace + "}\r\n" +
                           "}\r\n";
                })
                .SettingNamespaceTemplate(old =>
                {
                    //自定义命名Using的内容
                    var UserDefinedTemp = "using CRM.IRepository.IRepositorys;\r\n" +
                                          "using CRM.Model.Models;\r\n" +
                                          "using CRM.Repository.Base;\r\n";

                    StringBuilder sb = new StringBuilder();
                    sb.Append(UserDefinedTemp).Append(old);

                    return sb.ToString();
                })
                .CreateIRepositoryIService(@"E:\SqlSugarGenerateCode\Repositorys", fileName, "CRM.Repository.Repositorys");
        }

        /// <summary>
        /// 生成IService服务接口层
        /// </summary>
        public void GenerateIServiceCode()
        {
            //文件名称
            string fileName = "I{ClassName}Service";

            Db.DbFirst
                .IsCreateAttribute(false)       //是否需要引入命名控件：   using SqlSugar; 
                .SettingClassTemplate(old =>
                {
                    return "{using}\r\n" +
                           "namespace {Namespace}\r\n" +
                           "{\r\n" +
                           DbFirstTemplate.ClassSpace + "public interface I{ClassName}Service : IBaseService<{ClassName}>\r\n" +
                           DbFirstTemplate.ClassSpace + "{\r\n" +
                           "\r\n" +
                           DbFirstTemplate.ClassSpace + "}\r\n" +
                           "}\r\n";
                })
                .SettingNamespaceTemplate(old =>
                {
                    //自定义命名Using的内容
                    var UserDefinedTemp = "using CRM.IService.IBase;\r\n" +
                                          "using CRM.Model.Models;\r\n";

                    StringBuilder sb = new StringBuilder();
                    sb.Append(old).Append(UserDefinedTemp);

                    return sb.ToString();
                })
                .CreateIRepositoryIService(@"E:\SqlSugarGenerateCode\IServices", fileName, "CRM.IService.IServices");
        }

        /// <summary>
        /// 生成Service服务接口实现层
        /// </summary>
        public void GenerateServiceCode()
        {
            //文件名称
            string fileName = "{ClassName}Service";

            Db.DbFirst
                .IsCreateAttribute(false)       //是否需要引入命名控件：   using SqlSugar;
                .SettingClassTemplate(old =>
                {
                    return "{using}\r\n" +
                           "namespace {Namespace}\r\n" +
                           "{\r\n" +
                           DbFirstTemplate.ClassSpace + "public class {ClassName}Service : BaseService<{ClassName}>, I{ClassName}Service\r\n" +
                           DbFirstTemplate.ClassSpace + "{\r\n" +
                           "\r\n" +
                           DbFirstTemplate.ClassSpace + "}\r\n" +
                           "}\r\n";
                })
                .SettingNamespaceTemplate(old =>
                {
                    //自定义命名Using的内容
                    var UserDefinedTemp = "using CRM.IService.IServices;\r\n" +
                                          "using CRM.Model.Models;\r\n" +
                                          "using CRM.Service.Base;\r\n";

                    StringBuilder sb = new StringBuilder();
                    sb.Append(UserDefinedTemp).Append(old);

                    return sb.ToString();
                })
                .CreateIRepositoryIService(@"E:\SqlSugarGenerateCode\Services", fileName, "CRM.Service.Services");
        }
    }
}
