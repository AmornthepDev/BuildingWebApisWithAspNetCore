using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MyBGList.Attributes
{
    // any action methods that have this ManualValidationFilter as attribute will have to manual implement the handle of ModelState Validation
    public class ManualValidationFilterAttribute : Attribute, IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            for (int i = 0; i < action.Filters.Count; i++)
            {
                if (action.Filters[i] is ModelStateInvalidFilter || action.Filters[i].GetType().Name == "ModelStateInvalidFilterFactory")
                {
                    action.Filters.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
