﻿namespace Fooble.Presentation

open Fooble.Core
open System.Web.Mvc

type FoobleModelBinder() =
    inherit DefaultModelBinder()

    override this.BindModel(controllerContext, bindingContext) =
        match bindingContext.ModelType with

        | x when x = typeof<ISelfServiceRegisterViewModel> ->
            let form = controllerContext.HttpContext.Request.Form

            let username = form.Get("username")
            let email = form.Get("email")
            let nickname = form.Get("nickname")

            let modelState = bindingContext.ModelState

            match Member.validateUsername(username) with
            | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
            | _ -> ()

            match Member.validateEmail(email) with
            | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
            | _ -> ()

            match Member.validateNickname(nickname) with
            | y when y.IsInvalid -> modelState.AddModelError(y.ParamName, y.Message)
            | _ -> ()

            box (SelfServiceRegisterViewModel.make username email nickname)

        | _ -> base.BindModel(controllerContext, bindingContext)