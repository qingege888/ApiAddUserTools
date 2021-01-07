using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;


namespace noNetApiAdd
{
    class Program
    {
        private static readonly string PATH = "WinNT://" + Environment.MachineName;

        static void Main(string[] args)
        {
            var argments = CommandLineArgumentParser.Parse(args);
            instruct_getopt(args, argments);
        }

        //提示信息
        public static void helpMe()
        {
            Console.WriteLine("参数错误！正确格式: ");
            Console.WriteLine("添加用户：xxx.exe -u hacker -p sbhacker123");
            Console.WriteLine("添加用户到组：xxx.exe -u hacker -g administrators");
            Console.WriteLine("添加用户并把用户添加到组：xxx.exe -u hacker -p sbhacker123 -g administrators");
        }

        //参数解析
        public static void instruct_getopt(string[] args, CommandLineArgumentParser argments)
        {
            if (args.Length == 0 || !(argments.Has("-u") || argments.Has("-p") || argments.Has("-g")))
            {
                helpMe();
                return;
            }

            try
            {
                if (argments.Has("-u") && argments.Has("-p") && argments.Has("-g"))
                {
                    //Console.WriteLine("添加用户和组参数为:" + argments.Get("-u").Next + "," + argments.Get("-p").Next + "," + argments.Get("-g").Next);
                    AddAccountAndGroup(argments.Get("-u").Next, argments.Get("-p").Next, argments.Get("-g").Next);
                    Console.WriteLine("添加用户:" + argments.Get("-u").Next + "，密码为:" + argments.Get("-p").Next + "到" + argments.Get("-g").Next + "组成功！");
                    return;
                }

                if (argments.Has("-u") && argments.Has("-p"))
                {

                    //Console.WriteLine("添加用户参数为:" + argments.Get("-u").Next + "," + argments.Get("-p").Next);
                    AddAccount(argments.Get("-u").Next, argments.Get("-p").Next);
                    Console.WriteLine("添加用户:" + argments.Get("-u").Next + ",密码:" + argments.Get("-p").Next + "成功！");
                    return;
                }


                if (argments.Has("-u") && argments.Has("-g"))
                {

                    //Console.WriteLine("添加用户组参数为:" + argments.Get("-u").Next + "," + argments.Get("-g").Next);
                    AddGroup(argments.Get("-u").Next, argments.Get("-g").Next);
                    Console.WriteLine("把用户:" + argments.Get("-u").Next + "添加到" + argments.Get("-g").Next + "组成功！");
                    return;
                }
                helpMe();
                return;
            }
            catch (Exception err)
            {
                Console.WriteLine("参数异常！"+err.Message);
                //throw new Exception("参数异常！");
            }
                

            


        }



        //添加用户并加入指定组
        public static void AddAccountAndGroup(string username, string password, string group)
        {
            try
            {
                using (DirectoryEntry dir = new DirectoryEntry(PATH))
                {
                    //增加用户名
                    using (DirectoryEntry user = dir.Children.Add(username, "User")) 
                    {
                        //用户全称
                        user.Properties["FullName"].Add(username);
                        //用户密码
                        user.Invoke("SetPassword", password);
                        //用户详细描述
                        user.Invoke("Put", "Description", "Internet 信息服务使用的内置用户");
                        //用户下次登录需更改密码
                        //user.Invoke("Put","PasswordExpired",1); 
                        //密码永不过期
                        user.Invoke("Put", "UserFlags", 66049);
                        //用户不能更改密码
                        //user.Invoke("Put", "UserFlags", 0x0040);
                        //保存用户
                        user.CommitChanges();
                        //将用户添加到指定组
                        using (DirectoryEntry grp = dir.Children.Find(group, "group"))
                        {
                            if (grp.Name != "")
                            {
                                grp.Invoke("Add", user.Path.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            
        }

        //添加用户
        public static void AddAccount(string username, string password)
        {
            try
            {
                using (DirectoryEntry dir = new DirectoryEntry(PATH))
                {
                    using (DirectoryEntry user = dir.Children.Add(username, "User")) //增加用户名
                    {
                        user.Properties["FullName"].Add(username); //用户全称
                        user.Invoke("SetPassword", password); //用户密码
                        user.Invoke("Put", "Description", "Internet 信息服务使用的内置用户");//用户详细描述
                        //user.Invoke("Put","PasswordExpired",1); //用户下次登录需更改密码
                        user.Invoke("Put", "UserFlags", 66049); //密码永不过期
                        //user.Invoke("Put", "UserFlags", 0x0040);//用户不能更改密码s
                        user.CommitChanges();//保存用户
                    }
                }
            }
            catch (Exception err)
            {

                throw new Exception(err.Message);
            }
           
        }


        //把用户加入指定组
        public static void AddGroup(string username,string group)
        {
            try
            {
                using (DirectoryEntry dir = new DirectoryEntry(PATH))
                {
                    using (DirectoryEntry user = dir.Children.Find(username, "User"))
                    {
                        using (DirectoryEntry grp = dir.Children.Find(group, "group"))
                        {
                            if (grp.Name != "")
                            {
                                grp.Invoke("Add", user.Path.ToString());//将用户添加到某组
                                //grp.Invoke("Add",new object[]{user.Path});
                                grp.CommitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {

                throw new Exception(err.Message);
            }
          

        }



    }
}
