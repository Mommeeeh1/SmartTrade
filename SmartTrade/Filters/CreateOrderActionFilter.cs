using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartTrade.Controllers;
using SmartTrade.Core.Domain.Entities;

namespace SmartTrade.Infrastructure.Filters;

public class CreateOrderActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // STEP 1: Check if this filter is applied to TradeController
        if (context.Controller is TradeController tradeController)
        {
            // STEP 2: Try to get the "orderRequest" parameter
            if (context.ActionArguments.ContainsKey("buyOrderRequest"))
            {
                var orderRequest = context.ActionArguments["buyOrderRequest"];
                
                // STEP 3: Check if there are model validation errors
                if (context.ModelState.IsValid == false)
                {
                    // STEP 4: Extract errors from ModelState
                    List<string> errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    // STEP 5: Create a StockTrade object (empty data for redisplay)
                    StockTrade stockTrade = new StockTrade() { StockSymbol = "Sample" };
                    
                    // STEP 6: Store errors in ViewBag so the view can display them
                    tradeController.ViewBag.Errors = errors;
                    
                    // STEP 7: Return to Index view instead of continuing to action
                    context.Result = new ViewResult()
                    {
                        ViewName = "Index",
                        ViewData = tradeController.ViewData
                    };
                    
                    // DON'T call next() - we're stopping here!
                    return;
                }
            }
            else if (context.ActionArguments.ContainsKey("sellOrderRequest"))
            {
                var orderRequest = context.ActionArguments["sellOrderRequest"];
                
                if (context.ModelState.IsValid == false)
                {
                    List<string> errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    StockTrade stockTrade = new StockTrade() { StockSymbol = "Sample" };
                    
                    tradeController.ViewBag.Errors = errors;
                    
                    context.Result = new ViewResult()
                    {
                        ViewName = "Index",
                        ViewData = tradeController.ViewData
                    };
                    
                    return;
                }
            }
        }
        
        // STEP 8: No errors? Continue to the controller action!
        await next();
    }
}


