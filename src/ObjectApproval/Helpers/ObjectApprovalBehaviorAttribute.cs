using System;

namespace ObjectApproval
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ObjectApprovalBehaviorAttribute : Attribute
    {
        public ObjectApprovalBehaviorAttribute() { }
        public SerializerThreadingMode SerializerThreadingMode { get; set; } = SerializerThreadingMode.SingleThreaded;
    }
}
