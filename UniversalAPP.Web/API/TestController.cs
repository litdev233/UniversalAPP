﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace UniversalAPP.Web.API
{
    /// <summary>
    /// 未指定版本的接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersionNeutral()]
    public class PublicController : BaseAPIController
    {

        private Models.SiteBasicConfig _config_basic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appkeys"></param>
        public PublicController(IOptionsSnapshot<Web.Models.SiteBasicConfig> appkeys)
        {
            _config_basic = appkeys.Value;

        }

        /// <summary>
        /// Demo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("doing")]
        public UnifiedResultEntity<string> doing()
        {
            return ResultBasicString(1, "OK", "接口已通");
        }


    }
}