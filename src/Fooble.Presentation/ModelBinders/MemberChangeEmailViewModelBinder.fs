namespace Fooble.Presentation

open Autofac.Integration.Mvc
open Fooble.Common
open System
open System.Web.Mvc

/// Provides model binding for member change email view models.
[<ModelBinderType(typeof<IMemberChangeEmailViewModel>)>]
type MemberChangeEmailViewModelBinder =
    inherit DefaultModelBinder

    val private ViewModelFactory:MemberChangeEmailViewModelFactory

    /// <summary>
    /// Validates and constructs view models based on the data submitted via HTTP request form variables, query string
    /// variables, and route data. Adds validation errors to model state.
    /// </summary>
    /// <param name="viewModelFactory">The member change email view model factory to use.</param>
    new(viewModelFactory:MemberChangeEmailViewModelFactory) =
        ensureWith (validateRequired viewModelFactory "viewModelFactory" "Member change email view model factory")
        { ViewModelFactory = viewModelFactory }

    override this.BindModel(controllerContext, bindingContext) =
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
              let email = form.Get("email")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.Parse(id)

              let modelState = bindingContext.ModelState

              match validateMemberEmail email with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              box (this.ViewModelFactory.Invoke(id, currentPassword, email))

        | _ -> base.BindModel(controllerContext, bindingContext)
