namespace ObjectApproval
{
    /// <summary>
    /// <para>
    /// Can be set on an assembly level with <see cref="ObjectApprovalBehaviorAttribute"/>
    /// </para>
    /// <para>
    /// SingleThreaded mode means that all threds will share changes to <see cref="SerializerBuilder"/> 
    /// </para>
    /// <para>
    /// MultiThreaded mode means that each thread will have its own set of changes to <see cref="SerializerBuilder"/>
    /// </para>
    /// </summary>
    public enum SerializerThreadingMode
    {
        SingleThreaded,
        MultiThreaded
    }
}
