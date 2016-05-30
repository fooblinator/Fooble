namespace Fooble.Persistence

open FSharp.Data.TypeProviders

type internal EntityConnection = SqlEntityConnection<ConnectionStringName = "FoobleContext">
type internal FoobleContext = EntityConnection.ServiceTypes.EntityContainer
type internal MemberData = EntityConnection.ServiceTypes.MemberData

type internal IExposeWrapped<'T> =
    abstract Inner:'T with get
