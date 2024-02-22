using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte
{
    /// <summary>
    /// ClassicByte项目中所有异常的基类
    /// </summary>
    public class ClassicByteException:Exception 
    { 

        public ClassicByteException() 
        { 
            
        }
        public ClassicByteException(string message):base(message)
        {

        }
    }
    /// <summary>
    /// 当没有找到工作区时引发的异常
    /// </summary>
    public class WorkspaceNotFound : ClassicByteException { 
        public WorkspaceNotFound():base("未找到工作区") { }
        public WorkspaceNotFound(string message):base(message) { }
    }
}
