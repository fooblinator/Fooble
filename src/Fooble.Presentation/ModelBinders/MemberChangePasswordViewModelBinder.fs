namespace Fooble.Presentation

open Autofac.Integration.Mvc
open Fooble.Common
open System
open System.Web.Mvc

/// Provides model binding for member change password view models.
[<ModelBinderType(typeof<IMemberChangePasswordViewModel>)>]
type MemberChangePasswordViewModelBinder =
    inherit DefaultModelBinder

    val private ViewModelFactory:MemberChangePasswordViewModelFactory

    /// <summary>
    /// Validates and constructs view models based on the data submitted via HTTP request form variables, query string
    /// variables, and route data. Adds validation errors to model state.
    /// </summary>
    /// <param name="viewModelFactory">The member change password view model factory to use.</param>
    new(viewModelFactory:MemberChangePasswordViewModelFactory) =
        ensureWith (validateRequired viewModelFactory "viewModelFactory" "Member change password view model factory")
        { ViewModelFactory = viewModelFactory }

    override this.BindModel(controllerContext, bindingContext) =
#if DEBUG
        assertWith (validateRequired controllerContext "controllerContext" "Controller context")
        assertWith (validateRequired bindingContext "bindingContext" "Binding context")
#endif

        let routeData = controllerContext.RouteData
        let form = controllerContext.HttpContext.Request.Form

        match bindingContext.ModelType with

        | x when x = typeof<IMemberChangePasswordViewModel> ->
              let id = routeData.GetRequiredString("id")
              let currentPassword = form.Get("currentPassword")
              let password = form.Get("password")
              let confirmPassword = form.Get("confirmPassword")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.Parse(id)

              let modelState = bindingContext.ModelState

              match validateMemberPasswords password (Some(confirmPassword)) with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              this.ViewModelFactory.Invoke(id, currentPassword, password, confirmPassword)
              |> box

        | _ -> base.BindModel(controllerContext, bindingContext)
