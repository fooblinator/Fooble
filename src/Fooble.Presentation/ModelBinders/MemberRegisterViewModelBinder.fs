namespace Fooble.Presentation

open Autofac.Integration.Mvc
open Fooble.Common
open System.Web.Mvc

/// Provides model binding for member register view models.
[<ModelBinderType(typeof<IMemberRegisterViewModel>)>]
type MemberRegisterViewModelBinder =
    inherit DefaultModelBinder

    val private ViewModelFactory:MemberRegisterViewModelFactory

    /// <summary>
    /// Validates and constructs view models based on the data submitted via HTTP request form variables, query string
    /// variables, and route data. Adds validation errors to model state.
    /// </summary>
    /// <param name="viewModelFactory">The member register view model factory to use.</param>
    new(viewModelFactory:MemberRegisterViewModelFactory) =
        ensureWith (validateRequired viewModelFactory "viewModelFactory" "Member register view model factory")
        { ViewModelFactory = viewModelFactory }

    override this.BindModel(controllerContext, bindingContext) =
#if DEBUG
        assertWith (validateRequired controllerContext "controllerContext" "Controller context")
        assertWith (validateRequired bindingContext "bindingContext" "Binding context")
#endif

        let form = controllerContext.HttpContext.Request.Form

        match bindingContext.ModelType with

        | x when x = typeof<IMemberRegisterViewModel> ->
              let username = form.Get("username")
              let password = form.Get("password")
              let confirmPassword = form.Get("confirmPassword")
              let email = form.Get("email")
              let nickname = form.Get("nickname")
              let avatarData = form.Get("avatarData")

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

              match validateMemberAvatarData avatarData with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              this.ViewModelFactory.Invoke(username, password, confirmPassword, email, nickname,
                  avatarData)
              |> box

        | _ -> base.BindModel(controllerContext, bindingContext)
