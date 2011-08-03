namespace ImpromptuInterface

module FSharpUtil=
    open ImpromptuInterface
    open System.Dynamic

    type PropertySetCallsAddAssign(target:obj)=
      inherit DynamicObject()
        
      override this.TrySetMember(binder:SetMemberBinder, value:obj) =
        Impromptu.InvokeAddAssign(target,binder.Name,value)
        true


    type PropertySetCallsSubtractAssign(target:obj)=
      inherit DynamicObject()

      override this.TrySetMember(binder:SetMemberBinder, value:obj) =
        Impromptu.InvokeSubtractAssign(target,binder.Name,value)
        true


    type PropertyGetCallsNamedArgument(target:obj)=
      inherit DynamicObject()

      override this.TryGetMember(binder:GetMemberBinder,  result: obj byref) =
        result <- InvokeArg(binder.Name,target) :>obj
        true