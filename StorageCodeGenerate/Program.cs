using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageCodeGenerate
{
    class Program
    {
        static void Main(string[] args)
        {
            var generate = new CodeGenerate();
            generate.Init();

            //生成Model模型
            generate.GenerateModelCode();

            //生成IRepository仓储接口层
            generate.GenerateIRepositoryCode();
            //生成Repository仓储接口实现层
            generate.GenerateRepositoryCode();
            //生成IService服务接口层
            generate.GenerateIServiceCode();
            //生成Service服务接口实现层
            generate.GenerateServiceCode();

            Console.WriteLine("执行完成");
            Console.ReadLine();

        }
    }
}
