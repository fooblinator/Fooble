namespace Fooble.Presentation

open Fooble.Common
open System.Web.Mvc

/// Provides model binding for view models.
type FoobleModelBinder() =
    inherit DefaultModelBinder()

    override __.BindModel(controllerContext, bindingContext) =
#if DEBUG
        assertWith (validateRequired controllerContext "controllerContext" "Controller context")
        assertWith (validateRequired bindingContext "bindingContext" "Binding context")
#endif

        let routeData = controllerContext.RouteData
        let form = controllerContext.HttpContext.Request.Form

        match bindingContext.ModelType with

        | x when x = typeof<IMemberChangeEmailViewModel> ->
              let id = routeData.GetRequiredString("id")
              let currentPassword = form.Get("currentPassword")
              let newEmail = form.Get("newEmail")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.parse id

              let modelState = bindingContext.ModelState

              match validateMemberEmailWith newEmail "newEmail" "New email" with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              box (MemberChangeEmailViewModel.make id currentPassword newEmail)

        | x when x = typeof<IMemberChangeOtherViewModel> ->
              let id = routeData.GetRequiredString("id")
              let newNickname = form.Get("newNickname")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.parse id

              let modelState = bindingContext.ModelState

              match validateMemberNicknameWith newNickname "newNickname" "New nickname" with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              box (MemberChangeOtherViewModel.make id newNickname)

        | x when x = typeof<IMemberChangePasswordViewModel> ->
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

        | x when x = typeof<IMemberChangeUsernameViewModel> ->
              let id = routeData.GetRequiredString("id")
              let currentPassword = form.Get("currentPassword")
              let newUsername = form.Get("newUsername")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.parse id

              let modelState = bindingContext.ModelState

              match validateMemberUsernameWith newUsername "newUsername" "New username" with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              box (MemberChangeUsernameViewModel.make id currentPassword newUsername)

        | x when x = typeof<IMemberDeactivateViewModel> ->
              let id = routeData.GetRequiredString("id")
              let currentPassword = form.Get("currentPassword")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.parse id

              box (MemberDeactivateViewModel.make id currentPassword)

        | x when x = typeof<IMemberRegisterViewModel> ->
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
