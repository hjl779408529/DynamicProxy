using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Abp.Domain.Uow;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;

namespace DynamicProxy
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface ICat
    {
        void Eat();
        void Run();
    }
    /// <summary>
    /// 类
    /// </summary>
    [Intercept(typeof(CatInterceptor))]
    public class Cat : ICat
    {
        public void Eat()
        {
            Console.WriteLine("猫在吃东西！");
        }
        void ICat.Run()
        {
            Console.WriteLine("猫在跑");
        }
        public void Claw()
        {
            Console.WriteLine("猫在挠人");
        }
    }
    /// <summary>
    /// 代理类
    /// </summary>
    public class ProxyCat : ICat
    {
        private ICat _cat;
        public ProxyCat(ICat cat)
        {
            _cat = cat;
        }
        public void Eat()
        {
            Console.WriteLine("猫吃东西之前");
            _cat.Eat();
            Console.WriteLine("猫吃东西之后");
        }
        void ICat.Run()
        {
            Console.WriteLine("猫在跑");
        }
    }
    /// <summary>
    /// 定义拦截器
    /// </summary>
    
    public class CatInterceptor: IInterceptor
    {
        private ICat _cat;
        public CatInterceptor(ICat cat)
        {
            _cat = cat;
        }
        public void Intercept(IInvocation invocation)
        {
            //常规用法
            //Console.WriteLine("猫吃东西之前");
            //invocation.Proceed();
            //Console.WriteLine("猫吃东西之后");

            //高级用法 
            //Console.WriteLine("喂猫吃东西");
            //invocation.Method.Invoke(_cat,invocation.Arguments);//调用Cat的指定方法

            //高级用法 对方法特性识别，拦截到方法级别 测试失败
            Console.WriteLine("喂猫吃东西");
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }
            UnitOfWorkAttribute transaction = methodInfo.GetCustomAttributes<UnitOfWorkAttribute>(true).FirstOrDefault();
            if (transaction != null)
            {
                invocation.Proceed();
            }
        }
    }
    /// <summary>
    /// 猫主人
    /// </summary>
    public class CatOwner
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
            //静态代理
            //ICat cat = new Cat();
            //var proxycat = new ProxyCat(cat);
            //proxycat.Eat();

            //动态代理
            //var builder = new ContainerBuilder();
            //builder.RegisterType<CatInterceptor>();//注册拦截器
            //builder.RegisterType<Cat>().As<ICat>().InterceptedBy(typeof(CatInterceptor)).EnableInterfaceInterceptors();//注册Cat并未其添加拦截器
            //var container = builder.Build();
            //var cat = container.Resolve<ICat>();
            //cat.Eat();
            //cat.Run();

            //动态代理高级用法  为代理类添加接口
            //var builder = new ContainerBuilder();
            //builder.RegisterType<CatInterceptor>();//注册拦截器
            //builder.RegisterType<Cat>().As<ICat>();//注册Cat
            //builder.RegisterType<CatOwner>().InterceptedBy(typeof(CatInterceptor)).EnableClassInterceptors(ProxyGenerationOptions.Default, additionalInterfaces: typeof(ICat));//注册CatOwner并为其添加拦截器和接口
            //var container = builder.Build();
            //var cat = container.Resolve<CatOwner>();//获取CatOwner代理类
            //cat.GetType().GetMethod("Eat").Invoke(cat, null);//因为为代理类添加了ICat接口，所以通过反射获取代理类方法执行

            Thread td1 = new Thread(() => Console.WriteLine("哈哈哒！"));
            td1.Start();


            Console.Read();
        }
    }
}
