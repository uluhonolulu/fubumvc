using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;


namespace FubuMVC.Core.Ajax
{
    public class AjaxContinuationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph
                .Behaviors
                .Where(IsAjaxContinuation)
                .Each(chain =>
                          {
                              if(chain.OfType<AjaxContinuationNode>().Any()) return;

                              var call = chain.LastCall(); // won't be null after our filter
                              graph.Observer.RecordCallStatus(call, "Adding {0} directly after action call".ToFormat(typeof(AjaxContinuationNode).Name));
                              chain.Calls.Last().AddAfter(new AjaxContinuationNode());
                          });
        }

        public static bool IsAjaxContinuation(BehaviorChain chain)
        {
            var outputType = chain.ActionOutputType();
            return outputType != null && outputType.CanBeCastTo<AjaxContinuation>();
        }
    }
}