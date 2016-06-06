namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Web.Mvc

type FoobleModelBinder() =
    inherit DefaultModelBinder()

    override this.BindModel(controllerContext, bindingContext) =
        assert (isNotNull controllerContext)
        assert (isNotNull bindingContext)

        match bindingContext.ModelType with

        | x when x = typeof<IMemberChangePasswordViewModel> ->
              let form = controllerContext.HttpContext.Request.Form

              let mutable currentPassword = form.Get("currentPassword")
              let mutable newPassword = form.Get("newPassword")
              let mutable confirmPassword = form.Get("confirmPassword")

              let modelState = bindingContext.ModelState

              match Member.validatePassword currentPassword with
              | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
              | _ -> ()

              match Member.validatePasswords newPassword confirmPassword with
              | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
              | _ -> ()
              
              box (MemberChangePasswordViewModel.make currentPassword newPassword confirmPassword)

        | x when x = typeof<IMemberRegisterViewModel> ->
              let form = controllerContext.HttpContext.Request.Form

              let username = form.Get("username")
              let mutable password = form.Get("password")
              let mutable confirmPassword = form.Get("confirmPassword")
              let email = form.Get("email")
              let nickname = form.Get("nickname")

              let modelState = bindingContext.ModelState

              match Member.validateUsername username with
              | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
              | _ -> ()

              match Member.validatePasswords password confirmPassword with
              | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
              | _ -> ()

              match Member.validateEmail email with
              | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
              | _ -> ()

              match Member.validateNickname nickname with
              | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
              | _ -> ()

              box (MemberRegisterViewModel.make username password confirmPassword email nickname)

        | _ -> base.BindModel(controllerContext, bindingContext)
