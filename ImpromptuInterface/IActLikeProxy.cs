namespace ImpromptuInterface
{
    /// <summary>
    /// This interface can be used to access the original content of your emitted type;
    /// </summary>
    public interface IActLikeProxy
    {
        ///<summary>
        /// Returns the proxied object
        ///</summary>
        dynamic Original { get; }
        
    }
}