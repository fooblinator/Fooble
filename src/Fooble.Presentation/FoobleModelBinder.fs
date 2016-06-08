namespace Fooble.Presentation

open Fooble.Common
open System.Web.Mvc

type FoobleModelBinder() =
    inherit DefaultModelBinder()

    override __.BindModel(controllerContext, bindingContext) =
#if DEBUG
        assertWith (validateRequired controllerContext "controllerContext" "Controller context")
        assertWith (validateRequired bindingContext "bindingContext" "Binding context")
#endif

        match bindingContext.ModelType with

        | x when x = typeof<IMemberChangePasswordViewModel> ->
              let routeData = controllerContext.RouteData
              let form = controllerContext.HttpContext.Request.Form

              let id = routeData.GetRequiredString("id")
              let currentPassword = form.Get("currentPassword")
              let newPassword = form.Get("newPassword")
              let confirmPassword = form.Get("confirmPassword")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.parse id

              let modelState = bindingContext.ModelState

              match validateMemberPasswordsWith newPassword (Some(confirmPassword)) "newPassword" "New password" with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              box (MemberChangePasswordViewModel.make id currentPassword newPassword confirmPassword)

        | x when x = typeof<IMemberRegisterViewModel> ->
              let form = controllerContext.HttpContext.Request.Form

              let username = form.Get("username")
              let password = form.Get("password")
              let confirmPassword = form.Get("confirmPassword")
              let email = form.Get("email")
              let nickname = form.Get("nickname")

              let modelState = bindingContext.ModelState

              match validateMemberUsername username with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              match validateMemberPasswords password (Some(confirmPassword)) with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              match validateMemberEmail email with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              match validateMemberNickname nickname with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              box (MemberRegisterViewModel.make username password confirmPassword email nickname)

        | _ -> base.BindModel(controllerContext, bindingContext)
