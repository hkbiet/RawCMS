﻿//******************************************************************************
// <copyright file="license.md" company="RawCMS project  (https://github.com/arduosoft/RawCMS)">
// Copyright (c) 2019 RawCMS project  (https://github.com/arduosoft/RawCMS)
// RawCMS project is released under GPL3 terms, see LICENSE file on repository root at  https://github.com/arduosoft/RawCMS .
// </copyright>
// <author>Daniele Fontani, Emanuele Bucarelli, Francesco Min�</author>
// <autogenerated>true</autogenerated>
//******************************************************************************
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RawCMS.Library.Core.Attributes;

namespace RawCMS.Plugins.Core.Controllers.Controllers.admin
{
    [AllowAnonymous]
    [RawAuthentication]
    [Route("system/[controller]")]
    public class ConfigController : Controller
    {
    }
}