namespace Fooble.Presentation

open Autofac.Integration.Mvc
open Fooble.Common
open System
open System.Web.Mvc

/// Provides model binding for member change username view models.
[<ModelBinderType(typeof<IMemberChangeUsernameViewModel>)>]
type MemberChangeUsernameViewModelBinder =
    inherit DefaultModelBinder

    val private ViewModelFactory:MemberChangeUsernameViewModelFactory

    /// <summary>
    /// Validates and constructs view models based on the data submitted via HTTP request form variables, query string
    /// variables, and route data. Adds validation errors to model state.
    /// </summary>
    /// <param name="viewModelFactory">The member change username view model factory to use.</param>
    new(viewModelFactory:MemberChangeUsernameViewModelFactory) =
        ensureWith (validateRequired viewModelFactory "viewModelFactory" "Member change username view model factory")
        { ViewModelFactory = viewModelFactory }

    override this.BindModel(controllerContext, bindingContext) =
#if DEBUG
        assertWith (validateRequired controllerContext "controllerContext" "Controller context")
        assertWith (validateRequired bindingContext "bindingContext" "Binding context")
#endif

        let routeData = controllerContext.RouteData
        let form = controllerContext.HttpContext.Request.Form

        match bindingContext.ModelType with

        | x when x = typeof<IMemberChangeUsernameViewModel> ->
              let id = routeData.GetRequiredString("id")
              let currentPassword = form.Get("currentPassword")
              let username = form.Get("username")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.Parse(id)

              let modelState = bindingContext.ModelState

              match validateMemberUsername username with
              | Some(x, y) -> modelState.AddModelError(x, y)
              | _ -> ()

              this.ViewModelFactory.Invoke(id, currentPassword, username)
              |> box

        | _ -> base.BindModel(controllerContext, bindingContext)
