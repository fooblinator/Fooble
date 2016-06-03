namespace Fooble.IntegrationTest

open Fooble.Common
open Fooble.Core
open FSharp.Configuration
open Fooble.Persistence
open Fooble.Presentation
open Moq
open Moq.FSharp.Extensions
open System.Collections.Specialized
open System.Web
open System.Web.Mvc

type internal Settings = AppSettings<"App.config">

[<AutoOpen>]
module internal IntegrationTestHelpers =

    let bindModel<'T> formValues =
        let modelType = typeof<'T>

        let formValues' = NameValueCollection()
        Map.iter (fun k v -> formValues'.Add(k, v)) formValues

        let valueProvider = NameValueCollectionValueProvider(formValues', null)

        let modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, modelType)

        let bindingContext =
            ModelBindingContext(ModelName = modelType.Name, ValueProvider = valueProvider,
                ModelMetadata = modelMetadata)

        let httpRequestMock = Mock<HttpRequestBase>()
        httpRequestMock.SetupFunc(fun x -> x.Form).Returns(formValues').End

        let httpContextMock = Mock<HttpContextBase>()
        httpContextMock.SetupFunc(fun x -> x.Request).Returns(httpRequestMock.Object).End

        let controllerContext = ControllerContext()
        controllerContext.HttpContext <- httpContextMock.Object

        let binder = FoobleModelBinder()

        (binder.BindModel(controllerContext, bindingContext) :?> 'T, bindingContext.ModelState)

    let makeTestKeyGenerator key =
        { new IKeyGenerator with
            member this.GenerateKey() =
                match key with
                | Some x -> x
                | None -> Guid.random () }

    let makeTestMemberData id username password email nickname =
        assertMemberId id
        assertMemberUsername username
        assertMemberPassword password
        assertMemberEmail email
        assertMemberNickname nickname

        let idRef = ref id
        let usernameRef = ref username
        let passwordRef = ref password
        let emailRef = ref email
        let nicknameRef = ref nickname

        { new IMemberData with

              member this.Id
                  with get () = !idRef
                  and set (v) = idRef := v

              member this.Username
                  with get () = !usernameRef
                  and set (v) = usernameRef := v

              member this.Password
                  with get () = !passwordRef
                  and set (v) = passwordRef := v

              member this.Email
                  with get () = !emailRef
                  and set (v) = emailRef := v

              member this.Nickname
                  with get () = !nicknameRef
                  and set (v) = nicknameRef := v }
