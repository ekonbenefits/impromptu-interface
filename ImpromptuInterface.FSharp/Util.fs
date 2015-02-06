namespace ImpromptuInterface

module FSharpUtil=
    open System
    open ImpromptuInterface
    open System.Dynamic
    
      
    ///Wrap object to call the c# equivalent += dynamically when using f# dynamic set operator
    [<Obsolete("ImpromptuInterface.FSharp is deprecated, plese use FSharp.Dynamic http://goo.gl/X0QK4")>]
    type PropertySetCallsAddAssign(target:obj)=
      inherit DynamicObject()
        
      override this.TrySetMember(binder:SetMemberBinder, value:obj) =
        Impromptu.InvokeAddAssign(target,binder.Name,value)
        true

    ///Wrap object to call the c# equivalent -= dynamically when using f# dynamic set operator
    [<Obsolete("ImpromptuInterface.FSharp is deprecated, plese use FSharp.Dynamic http://goo.gl/X0QK4")>]
    type PropertySetCallsSubtractAssign(target:obj)=
      inherit DynamicObject()

      override this.TrySetMember(binder:SetMemberBinder, value:obj) =
        Impromptu.InvokeSubtractAssign(target,binder.Name,value)
        true

    ///Wrap object to use get operator to attach argument name for dynamic invocation
    [<Obsolete("ImpromptuInterface.FSharp is deprecated, plese use FSharp.Dynamic http://goo.gl/X0QK4")>]
    type PropertyGetCallsNamedArgument(target:obj)=
      inherit DynamicObject()

      override this.TryGetMember(binder:GetMemberBinder,  result: obj byref) =
        result <- InvokeArg(binder.Name,target) 
        true