﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DfmHttpService
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.DocAsCode.Build.Engine;
    using Microsoft.DocAsCode.Plugins;

    internal class DfmTokenTreeHandler : IHttpHandler
    {
        private readonly DfmServiceProvider _provider = new DfmServiceProvider();

        public bool CanHandle(ServiceContext context)
        {
            return context.Message.Name == CommandName.GenerateTokenTree;
        }

        public Task HandleAsync(ServiceContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    var tokenTree = GenerateTokenTree(context.Message.Documentation, context.Message.FilePath, context.Message.WorkspacePath);
                    Utility.ReplySuccessfulResponse(context.HttpContext, tokenTree, ContentType.Json);
                }
                catch (Exception ex)
                {
                    Utility.ReplyServerErrorResponse(context.HttpContext, ex.Message);
                }
            });
        }

        private string GenerateTokenTree(string documentation, string filePath, string workspacePath = null)
        {
            var service = _provider.CreateMarkdownService(new MarkdownServiceParameters { BasePath = workspacePath });

            return service.Markup(documentation, filePath).Html;
        }
    }
}