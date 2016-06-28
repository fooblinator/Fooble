namespace Fooble.Presentation

open Autofac.Integration.Mvc
open Fooble.Common
open System
open System.Web.Mvc

/// Provides model binding for member deactivate view models.
[<ModelBinderType(typeof<IMemberDeactivateViewModel>)>]
type MemberDeactivateViewModelBinder =
    inherit DefaultModelBinder

    val private ViewModelFactory:MemberDeactivateViewModelFactory

    /// <summary>
    /// Validates and constructs view models based on the data submitted via HTTP request form variables, query string
    /// variables, and route data. Adds validation errors to model state.
    /// </summary>
    /// <param name="viewModelFactory">The member deactivate view model factory to use.</param>
    new(viewModelFactory:MemberDeactivateViewModelFactory) =
        ensureWith (validateRequired viewModelFactory "viewModelFactory" "Member deactivate view model factory")
        { ViewModelFactory = viewModelFactory }

    override this.BindModel(controllerContext, bindingContext) =
#if DEBUG
        assertWith (validateRequired controllerContext "controllerContext" "Controller context")
        assertWith (validateRequired bindingContext "bindingContext" "Binding context")
#endif

        let routeData = controllerContext.RouteData
        let form = controllerContext.HttpContext.Request.Form

        match bindingContext.ModelType with

        | x when x = typeof<IMemberDeactivateViewModel> ->
              let id = routeData.GetRequiredString("id")
              let currentPassword = form.Get("currentPassword")

#if DEBUG
              assertWith (validateMemberIdString id)
#endif
              let id = Guid.Parse(id)

              this.ViewModelFactory.Invoke(id, currentPassword)
              |> box

        | _ -> base.BindModel(controllerContext, bindingContext)
