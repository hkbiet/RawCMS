using RawCMS.Library.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RawCMS.Library
{
    public abstract class FactoryLambda : Lambda
    {
        public override string Name => "FactoryLambda";

        public override string Description => "This is an override";


        public abstract string PluginName { get; }
        public abstract Type OriginalType  {  get;}
        public abstract Type ReplacedWith { get; }

    }
}
