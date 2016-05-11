namespace Fooble.Core

open System.Data.Entity

[<CLIMutable>]
type internal MemberData = 
    { Id : string
      Name : string }

[<AllowNullLiteral>]
type internal IDataContext = 
    abstract Members : IDbSet<MemberData>

type internal DataContext() = 
    inherit DbContext("name=FoobleContext")
    
    interface IDataContext with
        member this.Members = base.Set<MemberData>() :> IDbSet<MemberData>
    
    member this.Members = (this :> IDataContext).Members
